using Lucene.Net.Analysis.Standard;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using VeraWAF.WebPages.Bll.Resources;
using VeraWAF.WebPages.Dal;
using VeraWAF.WebPages.Dal.Interchange;

namespace VeraWAF.WebPages.Bll.Search
{
    /// <summary>
    /// Functionality for searhing the Vera content pages
    /// </summary>
    public class SearchQueryHelper
    {
        /// <summary>
        /// Maximum number of items per result
        /// </summary>
        readonly int MaxSearchResultLength = 50;

        /// <summary>
        /// Process the search query
        /// </summary>
        /// <returns>
        /// HTML markup with the search results
        /// </returns>
        public string ProcessQueryHtml(string baseUri, string queryRequest)
        {
            // Container for HTML markup with query results
            var markup = new StringBuilder();

            if (!String.IsNullOrWhiteSpace(queryRequest) && !queryRequest.StartsWith("*") && !queryRequest.StartsWith("?"))
            {
                Lucene.Net.Search.Query query;
                LuceneClient searcher = new LuceneClient();
                var hits = searcher.Search(queryRequest, out query);

                // Did the query yield any results?
                if (hits == null || hits.TotalHits == 0)
                    return Template.No_articles_where_found;    // No results

                // Highlight hits the HTML5 way
                var formatter = new SimpleHTMLFormatter("<mark>", "</mark>");
                var fragmenter = new SimpleFragmenter(MaxSearchResultLength);
                var scorer = new QueryScorer(query);
                var highlighter = new Highlighter(formatter, scorer);
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

                highlighter.TextFragmenter = fragmenter;

                if (hits.TotalHits == 1) markup.Append(Template.Found_1_article);
                else markup.AppendFormat(Template.Found_several_articles, hits.TotalHits);

                var dateUtilities = new DateUtilities();
                var userCache = new UserCache();
                var userUtilities = new UserUtilities();

                foreach (ScoreDoc scoreDoc in hits.ScoreDocs)
                {
                    var doc = searcher.GetSearcher().Doc(scoreDoc.Doc);
                    var rawText = Regex.Replace(doc.Get("raw_text"), "<.*?>", string.Empty);

                    Lucene.Net.Analysis.TokenStream stream = analyzer.TokenStream("", new StringReader(rawText));
                    var highlightedText = highlighter.GetBestFragments(stream, rawText, 1, "...").Replace("'", "''");

                    if (highlightedText == String.Empty) // Sometimes the highlighter fails to emit text...
                        highlightedText = rawText.Replace("'", "''");

                    if (highlightedText.Length > MaxSearchResultLength + 50)
                        highlightedText = highlightedText.Substring(0, MaxSearchResultLength + 50);

                    var readablePublishedDate =
                        dateUtilities.GetReadableDateAndTime(new DateTime(long.Parse(doc.Get("publishedDate"))));

                    // Get the author
                    var author = doc.Get("author");
                    var authorEntity = userCache.GetUser(author);

                    // Get authros partition key to gain access to her account. Has the author been deleted? 
                    var authorPartitionKey = authorEntity == null ? String.Empty : authorEntity.PartitionKey;

                    var rollupImage = doc.Get("rollupImage");

                    // Two list item templates, one with a rollup image and one without
                    if (String.IsNullOrWhiteSpace(rollupImage))
                        markup.AppendFormat(Template.Search_article_result,
                            doc.Get("title"),
                            baseUri + doc.Get("virtualPath").Substring(1),
                            readablePublishedDate,
                            HttpUtility.HtmlEncode(userUtilities.GetDisplayName(author)),
                            authorPartitionKey,
                            highlightedText);
                    else
                        markup.AppendFormat(Template.Search_article_result2,
                            doc.Get("title"),
                            baseUri + doc.Get("virtualPath").Substring(1),
                            readablePublishedDate,
                            HttpUtility.HtmlEncode(userUtilities.GetDisplayName(author)),
                            authorPartitionKey,
                            highlightedText,
                            rollupImage,
                            doc.Get("rollupText"));
                }

            }

            // Return the search result as HTML markup
            return markup.ToString();
        }

        const int MaxQueryTermLength = 500;
        const int MaxReturnResults = 5;
        const int MaxNumberOfLettersInText = 100;

        /// <summary>
        /// Search the Web Content Management system.
        /// </summary>
        /// <param name="queryTerms">Query</param>
        /// <returns>Query results in a VeraWAF.WebPages.Dal.Interchange.QueryResults data structure</returns>
        public QueryResults ProcessQuery(string queryTerms)
        {
            var searchResult = new QueryResults();

            if (!String.IsNullOrWhiteSpace(queryTerms)
                && !queryTerms.StartsWith("*")
                && !queryTerms.StartsWith("?")
                && queryTerms.Length < MaxQueryTermLength)
            {
                Lucene.Net.Search.Query query;
                var searcher = new LuceneClient();
                var hits = searcher.Search(queryTerms, out query);

                if (hits != null && hits.TotalHits > 0)
                {
                    searchResult.NumberOfHits = hits.TotalHits;

                    // TODO: Refactor this code to use the FastVectorHighlighter since one can set the number of characters returned
                    // Highlight the HTML5 way
                    var formatter = new SimpleHTMLFormatter("<mark>", "</mark>");
                    var fragmenter = new SimpleFragmenter(MaxNumberOfLettersInText);
                    var scorer = new QueryScorer(query);
                    var highlighter = new Highlighter(formatter, scorer);
                    var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

                    highlighter.TextFragmenter = fragmenter;

                    var max = hits.TotalHits > MaxReturnResults ? MaxReturnResults : hits.TotalHits;

                    foreach (ScoreDoc scoreDoc in hits.ScoreDocs)
                    {
                        var doc = searcher.GetSearcher().Doc(scoreDoc.Doc);
                        var rawText = Regex.Replace(doc.Get("raw_text"), "<.*?>", String.Empty);

                        var stream = analyzer.TokenStream(String.Empty, new StringReader(rawText));
                        var highlightedText = highlighter.GetBestFragments(stream, rawText, 1, "...").Replace("'", "''");

                        if (highlightedText == String.Empty) // Sometimes the highlighter fails to emit text...
                            highlightedText = rawText.Substring(0, MaxNumberOfLettersInText);

                        var firstCharacter = highlightedText[0];
                        if (firstCharacter == '.' || firstCharacter == ',' || firstCharacter == ':'
                            || firstCharacter == ';' || firstCharacter == '?' || firstCharacter == '!')
                            highlightedText = highlightedText.Substring(1);

                        searchResult.Items.Add(new QueryResult
                        {
                            Title = doc.Get("title"),
                            Url = doc.Get("virtualPath"),
                            PublishedDate = doc.Get("publishedDate"),
                            Author = doc.Get("author"),
                            Text = highlightedText,
                            ImageUrl = doc.Get("rollupImage")
                        });
                    }
                }
            }

            return searchResult;
        }

    }
}
