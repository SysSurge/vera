using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI.WebControls;
using VeraWAF.WebPages;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Dal;

namespace VeraWAF.WebPages.Controls
{
    public partial class MarketingResearch : System.Web.UI.UserControl
    {
        void DataBindCompanySizeCombo()
        {
            CompanySizeCombo.DataSource = new List<string>(new CompanySize().GetCategories);
            CompanySizeCombo.DataBind();
        }

        void DataBindJobCategoryCombo()
        {
            JobCategoryCombo.DataSource = new List<string>(new JobCategory().GetCategories);
            JobCategoryCombo.DataBind();
        }

        void DataBindIndustryCombo()
        {
            IndustryCombo.DataSource = new List<string>(new Industries().GetCategories);
            IndustryCombo.DataBind();
        }

        void DataBindCountryCombo()
        {
            CountryCombo.DataSource = Country.CountryList.Select(countryWithCode => new Country(countryWithCode)).ToList();
            CountryCombo.DataTextField = "Name";
            CountryCombo.DataValueField = "Name";
            CountryCombo.DataBind();
        }

        void DatabindControls()
        {
            DataBindCountryCombo();
            DataBindIndustryCombo();
            DataBindJobCategoryCombo();
            DataBindCompanySizeCombo();
        }

        void SelectListControlByValue(ListControl dropDownList, string value)
        {
            dropDownList.ClearSelection();
            var foundValue = dropDownList.Items.FindByValue(value);
            if (foundValue != null) foundValue.Selected = true;
        }

        bool GetBooleanPropertyValue(string propertyName)
        {
            var value = HttpContext.Current.Profile.GetPropertyValue(propertyName);
            return value == null ? false : (bool) value;
        }

        void PopulateControlsWithCurrentSettings()
        {
            var profile = HttpContext.Current.Profile;

            SelectListControlByValue(CountryCombo, (string)profile.GetPropertyValue("Country"));
            SelectListControlByValue(IndustryCombo, (string)profile.GetPropertyValue("Industry"));
            SelectListControlByValue(JobCategoryCombo, (string)profile.GetPropertyValue("JobCategory"));
            SelectListControlByValue(CompanySizeCombo, (string)profile.GetPropertyValue("CompanySize"));
        }

        public bool ShowCurrentSettings { private get; set; }

        void StoreComboValueInProfile(ProfileBase profile, string profilePropertyName, ListControl dropDownList)
        {
            profile.SetPropertyValue(profilePropertyName, dropDownList.SelectedValue);
        }

        public void SendNotficationEmail(string userName)
        {
            var user = Membership.GetUser(userName);
            if (user == null) return;

            var applicationName = ConfigurationManager.AppSettings["ApplicationName"];
            var profile = ProfileBase.Create(userName);
            var fullName = (string)profile.GetPropertyValue("FullName");
            var displayName = String.IsNullOrWhiteSpace(fullName) ? userName : fullName;

            var emailAddress = ConfigurationManager.AppSettings["notificationsEmail"];

            // Now lets create an email message
            var emailMessage = new StringBuilder();
            var header = Bll.Resources.Email.EmailHeader.Replace("{0}", 
                Bll.Resources.Email.MarketingResearchHeader);
            emailMessage.Append(header);
            emailMessage.AppendFormat(Bll.Resources.Email.MarketingResearchBody1, applicationName);
            emailMessage.AppendFormat(Bll.Resources.Email.MarketingResearchBody2, userName);

            if (!String.IsNullOrWhiteSpace(fullName))
                emailMessage.AppendFormat(Bll.Resources.Email.MarketingResearchBody3, fullName);

            emailMessage.AppendFormat(Bll.Resources.Email.MarketingResearchBody4, user.Email);
            emailMessage.AppendFormat(Bll.Resources.Email.MarketingResearchBody5, DateTime.UtcNow);
            emailMessage.AppendFormat(Bll.Resources.Email.MarketingResearchBody6, new ServerTools().GetClientIpAddress());
            emailMessage.AppendFormat(Bll.Resources.Email.MarketingResearchBody7, CountryCombo.Text);
            emailMessage.AppendFormat(Bll.Resources.Email.MarketingResearchBody8, IndustryCombo.Text);
            emailMessage.AppendFormat(Bll.Resources.Email.MarketingResearchBody9, JobCategoryCombo.Text);
            emailMessage.AppendFormat(Bll.Resources.Email.MarketingResearchBody10, CompanySizeCombo.Text);

            var footer = String.Format(Bll.Resources.Email.EmailFooter, 
                String.Format(Bll.Resources.Email.MarketingResearchReason, applicationName));
            emailMessage.Append(footer);
            var fromEmail = ConfigurationManager.AppSettings["fromEmail"];

            // Send the email
            var messagingClient = new MessagingClient();
            var emailSubject = String.Format(Bll.Resources.Email.MarketingResearchSubject, displayName, applicationName);
            messagingClient.SendEmail(fromEmail, emailAddress, emailSubject, emailMessage.ToString());
        }

        public void StoreFields(ProfileBase profile, bool saveProfile = false)
        {
            StoreComboValueInProfile(profile, "Country", CountryCombo);
            StoreComboValueInProfile(profile, "Industry", IndustryCombo);
            StoreComboValueInProfile(profile, "JobCategory", JobCategoryCombo);
            StoreComboValueInProfile(profile, "CompanySize", CompanySizeCombo);

            if (saveProfile) profile.Save();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            bool showControl;
            if (!bool.TryParse(ConfigurationManager.AppSettings["ShowMarketingResearchForm"], out showControl) 
                || !showControl)
            {
                // Hide the marketing research control
                this.Visible = false;
                return;
            }

            if (Page.IsPostBack) return;

            DatabindControls();

            if (ShowCurrentSettings) PopulateControlsWithCurrentSettings();
        }
    }
}