using System;
using System.Linq;
using System.Text;
using System.Web.UI;

#if !DEBUG
using VeraWAF.WebPages.Bll.Resources;
#endif

namespace VeraWAF.WebPages
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}
