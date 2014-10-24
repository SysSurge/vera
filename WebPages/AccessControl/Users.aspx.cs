using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using VeraWAF.AzureTableStorage;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Dal;
using VeraWAF.WebPages.Bll;

namespace VeraWAF.WebPages.AccessControl
{
    public partial class Users : PageTemplateBase
    {
        /// <summary>
        /// Max number of items per page
        /// </summary>
        const int pageSize = 25;

        /// <summary>
        /// Total number of users
        /// </summary>
        int totalUsers;

        /// <summary>
        /// Total number of pages
        /// </summary>
        int totalPages;

        /// <summary>
        /// Current page number
        /// </summary>
        int currentPage = 1;

        /// <summary>
        /// Text utilities
        /// </summary>
        TextUtilities _textUtils;

        /// <summary>
        /// Class constructor
        /// </summary>
        public Users()
        {
            _textUtils = new TextUtilities();
        }

        /// <summary>
        /// Clips a string
        /// </summary>
        /// <param name="inString"></param>
        /// <param name="maxLen"></param>
        /// <returns>Clipped string</returns>
        protected string ClipString(string inString, int maxLen)
        {
            if (String.IsNullOrEmpty(inString)) return inString;

            return Server.HtmlEncode(_textUtils.ClipLeft(inString, maxLen));
        }

        /// <summary>
        /// Create a HTML mailto link 
        /// </summary>
        /// <param name="inEmail">E-mail address</param>
        /// <returns>HTML mailto anchor element</returns>
        protected string FormatEmail(string inEmail)
        {
            if (inEmail == null) return String.Empty;

            string cleanEmail = Server.HtmlEncode(inEmail);
            string emailText = _textUtils.ClipLeft(cleanEmail, 30);
            
            return String.Format("<a href=\"mailto:{0}\">{1}</a>", cleanEmail, emailText);
        }

        /// <summary>
        /// Load all users that matches the filter
        /// </summary>
        /// <param name="filter">Filter</param>
        void GetUsers(string filter)
        {
            NumberOfUsersOnline.Text = Membership.GetNumberOfUsersOnline().ToString();

            var userCache = new UserCache();
            totalUsers = userCache.GetUsers().Count();

            if (String.IsNullOrWhiteSpace(filter))
            {
                UserGrid.DataSource = userCache.GetUsers().Skip((currentPage - 1) * pageSize).Take(pageSize);
            }
            else
            {
                // Add a filter to the Linq query
                UserGrid.DataSource = userCache.GetUsers().Where(user => 
                    (user.Email != null && user.Email.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                    || (user.Description != null && user.Description.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                    || (user.Employer != null && user.Employer.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                    || (user.FullName != null && user.FullName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                    || (user.ProfileComment != null && user.ProfileComment.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                    || (user.Username != null && user.Username.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                    ).Skip((currentPage - 1) * pageSize).Take(pageSize);
            }

            NumberOfUsers.Text = totalUsers.ToString();

            totalPages = ((totalUsers - 1) / pageSize) + 1;

            // Ensure that we do not navigate past the last page of users.
            if (currentPage > totalPages)
            {
                currentPage = totalPages;
                GetUsers(filter);
                return;
            }

            UserGrid.DataBind();
            CurrentPageLabel.Text = currentPage.ToString();
            TotalPagesLabel.Text = totalPages.ToString();

            NextButton.Visible = currentPage != totalPages;

            PreviousButton.Visible = currentPage != 1;

            NavigationPanel.Visible = totalUsers > 0;
        }

        /// <summary>
        /// Called when the user clicks the next button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void NextButton_OnClick(object sender, EventArgs args)
        {
            currentPage = Convert.ToInt32(CurrentPageLabel.Text);
            currentPage++;
            GetUsers(txtQueryFilter.Text);
        }

        /// <summary>
        /// Called when the user clicks the previous button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void PreviousButton_OnClick(object sender, EventArgs args)
        {
            currentPage = Convert.ToInt32(CurrentPageLabel.Text);
            currentPage--;
            GetUsers(txtQueryFilter.Text);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            GetUsers(txtQueryFilter.Text);
        }

        /// <summary>
        /// CAlled when the user clicks the edit item link
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void UsersGrid_Edit(object source, DataGridCommandEventArgs e)
        {
            // Go to a more detailed view of the log item entry
            var partitionKey = ((DataGrid)source).Items[e.Item.ItemIndex].Cells[0].Text;
            Response.Redirect(String.Format("/Admin/EditUser.aspx?id={0}",
                partitionKey), true);
        }
    }
}