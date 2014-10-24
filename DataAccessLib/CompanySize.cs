using System;

namespace VeraWAF.WebPages.Dal
{
    public class CompanySize
    {
        readonly String[] _companySizeCategories =
            {
                Resources.Marketing.CompanySize1,
                Resources.Marketing.CompanySize2,
                Resources.Marketing.CompanySize3,
                Resources.Marketing.CompanySize4,
                Resources.Marketing.CompanySize5,
                Resources.Marketing.CompanySize6,
                Resources.Marketing.CompanySize7,
                Resources.Marketing.CompanySize8,
                Resources.Marketing.CompanySize9,
                Resources.Marketing.CompanySize10,
                Resources.Marketing.CompanySize11,
                Resources.Marketing.CompanySize12,
                Resources.Marketing.CompanySize13
            };

        public String[] GetCategories
        {
            get { return _companySizeCategories; }
        }
    }
}