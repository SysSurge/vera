using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using VeraWAF.AzureTableStorage;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace VeraWAF.WebPages.Bll.Search
{
    public class LuceneClient {
        Searcher _searcher;
        StandardAnalyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

        public Searcher GetSearcher()
        {
            return _searcher;
        }

        public Lucene.Net.Documents.Document CreateDocument(string title, string ingress, string virtualPath, string text,
            DateTime publishedDate, string author, string rollupImage, string rollupText) {

            var doc = new Lucene.Net.Documents.Document();

            title = title ?? String.Empty;
            ingress = ingress ?? String.Empty;
            author = author ?? String.Empty;
            virtualPath = virtualPath ?? String.Empty;
            text = text ?? String.Empty;
            rollupImage = rollupImage ?? String.Empty;
            rollupText = rollupText ?? String.Empty;

            doc.Add(new Lucene.Net.Documents.Field(
                "text",
                new StringReader(text)));

            doc.Add(new Lucene.Net.Documents.Field(
                "title",
                title,
                Lucene.Net.Documents.Field.Store.YES,
                Lucene.Net.Documents.Field.Index.ANALYZED));

            doc.Add(new Lucene.Net.Documents.Field(
                "ingress",
                ingress,
                Lucene.Net.Documents.Field.Store.YES,
                Lucene.Net.Documents.Field.Index.ANALYZED));

            doc.Add(new Lucene.Net.Documents.Field(
                "author",
                author,
                Lucene.Net.Documents.Field.Store.YES,
                Lucene.Net.Documents.Field.Index.ANALYZED));

            doc.Add(new Lucene.Net.Documents.Field(
                "rollupImage",
                rollupImage,
                Lucene.Net.Documents.Field.Store.YES,
                Lucene.Net.Documents.Field.Index.NOT_ANALYZED));

            doc.Add(new Lucene.Net.Documents.Field(
                "rollupText",
                rollupText,
                Lucene.Net.Documents.Field.Store.YES,
                Lucene.Net.Documents.Field.Index.NOT_ANALYZED));     

            doc.Add(new Lucene.Net.Documents.Field(
                "virtualPath",
                virtualPath,
                Lucene.Net.Documents.Field.Store.YES,
                Lucene.Net.Documents.Field.Index.NOT_ANALYZED));

            doc.Add(new Lucene.Net.Documents.Field(
                "publishedDate",
                publishedDate.Ticks.ToString(CultureInfo.InvariantCulture),
                Lucene.Net.Documents.Field.Store.YES,
                Lucene.Net.Documents.Field.Index.NOT_ANALYZED));

            doc.Add(new Lucene.Net.Documents.Field(
                "mod_date",
                publishedDate.ToString("yyyyMMdd"),
                Lucene.Net.Documents.Field.Store.YES,
                Lucene.Net.Documents.Field.Index.ANALYZED));


            // Store the raw text for later use by the highlighter
            doc.Add(new Lucene.Net.Documents.Field(
                "raw_text",
                text,
                Lucene.Net.Documents.Field.Store.YES,
                Lucene.Net.Documents.Field.Index.NOT_ANALYZED));

            return doc;
        }

        Lucene.Net.Store.Directory GetIndexFilePath()
        {
            return Lucene.Net.Store.FSDirectory.Open(RoleEnvironment.GetLocalResource("LuceneIndex").RootPath);
        }

        public void DeleteFromIndex(PageEntity page)
        {
            try {
                if (_searcher != null) {
                    try {
                        _searcher.Close();
                    } catch (Exception e) {
                        throw new ApplicationException("Exception closing Lucene searcher:" + e.Message);
                    }
                    _searcher = null;
                }

                var modifier = new Lucene.Net.Index.IndexWriter(GetIndexFilePath(), analyzer, false, IndexWriter.MaxFieldLength.UNLIMITED);

                var virtualPath = page.VirtualPath;

                modifier.DeleteDocuments(new Term("virtualPath", virtualPath));

                modifier.Flush(true, true, true);
                modifier.Close();
            } catch (Exception e) {
                throw new ApplicationException("Exception deleting item from Lucene index: " + e.Message);
            }            
        }

        public void UpdateIndex(PageEntity page)
        {
            try
            {
                if (_searcher != null)
                {
                    try
                    {
                        _searcher.Close();
                    }
                    catch (Exception e)
                    {
                        throw new ApplicationException("Exception closing Lucene searcher:" + e.Message);
                    }
                    _searcher = null;
                }

                var modifier = new Lucene.Net.Index.IndexWriter(GetIndexFilePath(), analyzer,  false, IndexWriter.MaxFieldLength.UNLIMITED);

                // same as build, but uses "modifier" instead of write.
                // uses additional "where" clause for bugid

                var virtualPath = page.VirtualPath;

                modifier.DeleteDocuments(new Term("virtualPath", virtualPath));

                modifier.AddDocument(
                    CreateDocument(
                        page.Title,
                        page.Ingress,
                        page.VirtualPath,
                        page.MainContent,
                        page.Timestamp,
                        page.Author,
                        page.RollupImage,
                        page.RollupText)
                    );

                modifier.Flush(true, true, true);
                modifier.Close();
            }
            catch (Exception e)
            {
                throw new ApplicationException("Exception updating Lucene index: " + e.Message);
            }
        }

        public void BuildIndex(IEnumerable<PageEntity> pages)
        {
            try
            {
                var writer = new Lucene.Net.Index.IndexWriter(GetIndexFilePath(), analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);

                foreach (var page in pages.Where(article => 
                    !String.IsNullOrWhiteSpace(article.MainContent) 
                    && !String.IsNullOrWhiteSpace(article.VirtualPath)  
                    && article.IsPublished 
                    && String.IsNullOrWhiteSpace(article.RedirectUrl)
                    && String.IsNullOrWhiteSpace(article.ParentRowKey)
                    && article.Index
                    ))
                    writer.AddDocument(CreateDocument(
                        page.Title,
                        page.Ingress,
                        page.VirtualPath,
                        page.MainContent,
                        page.Timestamp,
                        page.Author, 
                        page.RollupImage,
                        page.RollupText));
            
                writer.Optimize();
                writer.Close();
            }
            catch (Exception e)
            {
                throw new ApplicationException("Exception building Lucene index: " + e.Message);
            }
        }

        string CleanUpQueryText(string queryText)
        {
            if (String.IsNullOrWhiteSpace(queryText)) return null;
            
            var queryTextWords = new List<string>(queryText.Trim().Split(' '));

            bool hasChanged;
            do {
                hasChanged = false;
                var lastWord = queryTextWords.Last();
                if (lastWord == "AND"
                    || lastWord == "OR"
                    || lastWord == "+"
                    || lastWord == "-"
                    || lastWord == "("
                    || lastWord == "NOT") {
                    queryTextWords.RemoveAt(queryTextWords.Count - 1);
                    hasChanged = true;
                }

                if (queryTextWords.Count == 0) return null;
            
            } while (hasChanged);


            return String.Join(" ", queryTextWords);            
        }

        public TopDocs Search(string queryText, out Query query)
        {
            // Clean up query, remove errors etc.
            var cleanQueryText = CleanUpQueryText(queryText);
            if (String.IsNullOrWhiteSpace(cleanQueryText))
            {
                query = null;
                return null;
            }

            TopDocs hits;
            var parser = new Lucene.Net.QueryParsers.QueryParser(Lucene.Net.Util.Version.LUCENE_30, "text", analyzer);

            try 
            {
                query = parser.Parse(cleanQueryText);

                if (_searcher == null) _searcher = new IndexSearcher(GetIndexFilePath());

                hits = _searcher.Search(query, 50);
            }
            catch (Exception e) 
            {
                throw new ApplicationException("Exception querying the Lucene index: " + e.Message);
            }

            return hits;
        }
    }
}