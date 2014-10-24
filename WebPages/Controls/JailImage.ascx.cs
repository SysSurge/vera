using System;
using System.Web.UI;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Bll.Cloud;

namespace VeraWAF.WebPages.Controls {

    /// <summary>
    /// Dynamically loads an image when it is made visible in the browser viewport.
    /// Dynamically loading images can improve Web page load time. Image is displayed
    /// as normal if JavaScript is disabled.
    /// </summary>
    public partial class JailImage : UserControl {

        /// <summary>
        /// Image alt text
        /// </summary>
        public string alt { get; set; }

        /// <summary>
        /// Image url
        /// </summary>
        public string src { get; set; }

        /// <summary>
        /// Image CSS class
        /// </summary>
        public string css { get; set; }

        /// <summary>
        /// Image width
        /// </summary>
        public string width { get; set; }

        /// <summary>
        /// Image height
        /// </summary>
        public string height { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Any image url specified?
            if (!String.IsNullOrWhiteSpace(src))
            {
                // Image alt text specified?
                if (String.IsNullOrWhiteSpace(alt)) alt = String.Empty;

                // Turn image url into a CDN url if CDN is in use
                src = new CdnUtilities().GetCdnUrl(src);

                // Create the HTML markup for the image
                if (String.IsNullOrWhiteSpace(width) || String.IsNullOrWhiteSpace(height))
                {
                    if (String.IsNullOrWhiteSpace(css))
                        // <img data-href="{0}" src="/Images/blank.gif" alt="{1}" class="jail hide" /><noscript><img src="{0}" alt="{1}" /></noscript>
                        litJailImageMarkup.Text = String.Format(VeraWAF.WebPages.Bll.Resources.Controls.JailImageMarkup2, src, alt);
                    else
                        // <img data-href="{0}" src="/Images/blank.gif" alt="{1}" class="jail hide {2}" /><noscript><img src="{0}" alt="{1}" class="{2}" /></noscript>
                        litJailImageMarkup.Text = String.Format(VeraWAF.WebPages.Bll.Resources.Controls.JailImageMarkup2, src, alt, css);
                }
                else
                {
                    // <img data-href="{0}" src="/Images/blank.gif" width="{1}" height="{2}" class="jail hide {3}" alt="{4}" /><noscript><img src="{0}" width="{1}" height="{2}" class="{3}" alt="{4}" /></noscript>
                    litJailImageMarkup.Text = String.Format(VeraWAF.WebPages.Bll.Resources.Controls.JailImageMarkup, src,
                        width.Replace("px", String.Empty), height.Replace("px", String.Empty), css, alt);
                }
            }
        }

    }
}