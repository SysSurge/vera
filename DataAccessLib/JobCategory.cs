using System;

namespace VeraWAF.WebPages.Dal
{
    public class JobCategory
    {
        readonly string[] _categories =
            {
                "Accounting",
                "Administrative",
                "Banking",
                "Business",
                "Creative Design",
                "Customer Service",
                "Editorial",
                "Engineering",
                "Finance",
                "Human Resources",
                "IT",
                "Legal",
                "Logistics",
                "Maintenance",
                "Manufacturing",
                "Marketing",
                "Project Management",
                "Quality Assurance",
                "R&D",
                "Sales"
            };

        public JobCategory()
        {
            Array.Sort(_categories);
        }

        public String[] GetCategories
        {
            get { return _categories; }
        }
    }
}