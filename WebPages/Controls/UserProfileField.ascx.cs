using System;
using System.Reflection;
using System.Web.Security;
using System.Web.Profile;
using System.Web.UI;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;
using VeraWAF.AzureTableStorage;

namespace VeraWAF.WebPages.Controls
{
    /// <summary>
    /// Shows the content of an arbitrary user property in the VeraUsers table.
    /// </summary>
    public partial class UserProfileField : UserFieldControlBase
    {
        PageTemplateBase _page;

        /// <summary>
        /// Name of column in the VeraUsers table to show.
        /// Default value is "PartitionKey".
        /// </summary>
        /// <remarks>
        /// Combine with Format for more output control.
        /// </remarks> 
        public string PropertyName { get; set; }

        /// <summary>
        /// Class constructor
        /// </summary>
        public UserProfileField()
        {
            PropertyName = "PartitionKey";
        }

        /// <summary>
        /// Returns the user field's value
        /// </summary>
        public string Text 
        {
            get
            {
                return litMarkup.Text;
            }
        }

        /// <summary>
        /// Show the user property data PropertyName from the VeraUsers table.
        /// </summary>
        /// <remarks>
        /// Called by VeraWAF.WebPages.Bll.UserFieldControlBase after having loaded the user info.
        /// </remarks>
        /// <param name="user">User</param>
        protected override void FillControlFields(MembershipUser user)
        {
            // Get the user entity from the cache
            var userEntity = new Dal.UserCache().GetUser(user.ProviderUserKey);

            // Was the user found?
            if (userEntity == null)
            {
                litMarkup.Text = "Error: The requested user was not found.";
                return;
            }

            string rawMarkup;

            try
            {
                // Use .NET reflection to use named properties to access user entity data
                var myType = typeof(UserEntity);

                // Get the PropertyInfo object by passing the property name.
                var myPropInfo = myType.GetProperty(PropertyName);

                var userFieldData = myPropInfo.GetValue(userEntity);
                if (userFieldData == null)
                {
                    // Not found
                    litMarkup.Text = String.Empty;
                    return;
                }

                // If format is supplied then we don't try to convert the type
                if (String.IsNullOrEmpty(Format))
                {
                    try
                    {
                        // Try to convert the type
                        rawMarkup = Convert.ChangeType(userFieldData, typeof(string)) as string;
                    }
                    catch (InvalidCastException)
                    {
                        litMarkup.Text = String.Format("Error: Cannot convert a \"{0}\" to a System.String.", myPropInfo.PropertyType.FullName);
                        return;
                    }
                }
                else
                {
                    rawMarkup = String.Format(Format, userFieldData);
                }
            }
            catch (Exception)
            {
                rawMarkup = String.Format("Error: User property \"{0}\" not found.", PropertyName);
                return;
            }

            litMarkup.Text = ProcessFieldData(rawMarkup);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Get the parent page
            _page = Page as PageTemplateBase;
            if (_page == null)
                throw new ApplicationException("The page does not inherit from the VeraWAF.Core.Templates.PageTemplateBase page template.");

            // Get page entity data
            var pageEntity = _page.GetPageEntity();

            // Process user info and fill fields
            ProcessField(pageEntity);
        }
    }
}