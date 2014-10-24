using System;
using System.Collections.Generic;

namespace VeraWAF.WebPages.Dal.Interchange {

    [Serializable]
    public class QueryResults
    {
        public int NumberOfHits;
        public List<QueryResult> Items = new List<QueryResult>();
    }

}