using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;
using VeraWAF.CrossCuttingConcerns;

namespace VeraWAF.WebPages.Account
{
    public partial class Logout : PageTemplateBase
    {
        void ClearAuthenticationCookie()
        {
            var cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, String.Empty) { Expires = DateTime.Now.AddYears(-1) };
            Response.Cookies.Add(cookie1);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Membership.GetUser() != null)
                new LogEvent().AddEvent(ELogEventTypes.Info,
                    string.Format("User \"{0}\" has signed out", Membership.GetUser().UserName),
                    ConfigurationManager.AppSettings["ApplicationName"]
                    );

            ClearAuthenticationCookie();

            FormsAuthentication.SignOut();

            Response.Redirect("/");
        }
    }
}