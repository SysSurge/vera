using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VeraWAF.WebPages;

namespace VeraWAF.WebPages.Controls
{
    public partial class SocialSignIn : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            litMarkup.Text = VeraWAF.WebPages.Bll.Resources.ThirdParty.JanrainCode;
        }
    }
}