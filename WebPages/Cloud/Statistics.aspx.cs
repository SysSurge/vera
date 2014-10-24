using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web.Security;
using System.Web.UI;
using VeraWAF.AzureTableStorage;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Dal;

namespace VeraWAF.WebPages.Admin {
    public partial class Statistics : PageTemplateBase
    {
        void GetUserInfo() {
            int totalNumberOfUsers;
            NumberOfUsersOnline.Text = Membership.GetNumberOfUsersOnline().ToString(CultureInfo.InvariantCulture);

            Membership.GetAllUsers(0, 1, out totalNumberOfUsers);

            NumberOfUsers.Text = totalNumberOfUsers.ToString(CultureInfo.InvariantCulture);
        }

        protected void Page_Load(object sender, EventArgs e) {
            GetUserInfo();
        }
    }
}