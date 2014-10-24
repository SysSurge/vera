using System;

namespace VeraWAF.WebPages.Dal
{
    public class Industries
    {
        readonly String[] _industryCategories =
            {
                "Advertising",
                "Aeronautics",
                "Agriculture and Fishing",
                "Automotive",
                "Biotech",
                "Construction",
                "Education",
                "Engineering",
                "Food Services",
                "Government",
                "Healthcare",
                "Hospitality",
                "Insurance",
                "Personal Services",
                "Real Estate",
                "Retail",
                "Security",
                "Technology",
                "Telecommunications",
                "Transportation"
            };

        public Industries()
        {
            Array.Sort(_industryCategories);
        }

        public String[] GetCategories
        {
            get { return _industryCategories; }
        }
    }
}