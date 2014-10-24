using System;

namespace VeraWAF.WebPages.Dal.Interchange {

    [Serializable]
    public class QueryResult {
        public string Title;
        public string Url;
        public string PublishedDate;
        public string Author;
        public string Text;
        public string ImageUrl;
    }

}