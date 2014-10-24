using System;
using System.Configuration;
using System.Globalization;
using System.Web.UI;
using VeraWAF.WebPages.Bll;

namespace VeraWAF.WebPages.Controls {
    public partial class Footer : UserControl {

        string GetCopyrightYearSpan(int startYear) {
            var currentYear = DateTime.Now.Year;
            var difference = currentYear - startYear;
            string yearSpan;

            switch (difference) {
                case 0:
                    yearSpan = currentYear.ToString(CultureInfo.InvariantCulture);
                    break;
                case 1:
                    yearSpan = String.Format("{0},{1}", startYear, currentYear);
                    break;
                default:
                    yearSpan = String.Format("{0}-{1}", startYear, currentYear);
                    break;
            }

            return yearSpan;
        }

        int GetSiteLaunchYear() {
            return new RuntimeConfiguration().GetSiteLaunchDate().Year;
        }

        /// <summary>
        /// Add some information about the company/organization
        /// </summary>
        void AddCompanyInfoFields()
        {
            // Company street address line 1
            litCompanyStreet1.Text = ConfigurationManager.AppSettings["companyStreetAddress1"];

            // Company street address line 2
            var street2 = ConfigurationManager.AppSettings["companyStreetAddress1"];
            if (!String.IsNullOrWhiteSpace(street2))
                litCompanyStreet2.Text = String.Format("<br />{0}",
                    ConfigurationManager.AppSettings["companyStreetAddress2"]);

            // ZIP
            litCompanyZip.Text = ConfigurationManager.AppSettings["companyZipCode"];

            // City
            litCompanyCity.Text = ConfigurationManager.AppSettings["companyCity"];

            // Country
            litCompanyCountry.Text = ConfigurationManager.AppSettings["companyCountry"];

            // Company e-mail
            var email = ConfigurationManager.AppSettings["companyContactEmail"];
            if (!String.IsNullOrWhiteSpace(email))
                litCompanyEmail.Text = String.Format("<a href=\"mailto:{0}\" itemprop=\"email\">{0}</a>", email);

            // Company phone
            litCompanyPhone.Text = ConfigurationManager.AppSettings["companyContactPhone"];
        }

        /// <summary>
        /// Add company name to various fields
        /// </summary>
        void SetCompanyNameFields()
        {
            var companyName = ConfigurationManager.AppSettings["companyName"];

            litCompanyName1.Text = companyName;
            litCompanyName2.Text = companyName;
            litCompanyName3.Text = companyName;
            litCompanyName4.Text = companyName;
        }

        /// <summary>
        /// Set social home links
        /// </summary>
        void SetSocialLinks()
        {
            twitter.HRef = ConfigurationManager.AppSettings["twitterHome"];
            facebook.HRef = ConfigurationManager.AppSettings["facebookHome"];
            linkedin.HRef = ConfigurationManager.AppSettings["linkedInHome"];
        }

        protected void Page_Load(object sender, EventArgs e) {
            // Set the copyright message
            litCopyrightYear.Text = GetCopyrightYearSpan(GetSiteLaunchYear());

            SetCompanyNameFields();
            AddCompanyInfoFields();
            SetSocialLinks();
        }
    }
}