using System;
using System.Web.UI;

namespace VeraWAF.WebPages.Controls
{
    public partial class Submit : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public string Text
        {
            get { return txt.Text; }
            set { txt.Text = value; }
        }
    }
}