using System;
using System.Globalization;
using System.Web.Security;
using System.Web.UI;
using VeraWAF.AzureTableStorage;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Dal;

namespace VeraWAF.WebPages.Controls {
    public partial class Favorite : UserControl {
        /// <summary>
        /// Vera base page 
        /// </summary>
        PageTemplateBase _page;

        /// <summary>
        /// Facorite cache
        /// </summary>
        readonly FavoriteCache _favoriteCache;

        /// <summary>
        /// Favorite item id
        /// </summary>
        public string FavoriteItemId { get; set; }

        /// <summary>
        /// Class constructor
        /// </summary>
        public Favorite()
        {
            _favoriteCache = new FavoriteCache();
        }

        string GetProviderUserKey() {
            var user = Membership.GetUser();
            return user == null ? null : user.ProviderUserKey.ToString();
        }

        /// <summary>
        /// Initiate the favorite control
        /// </summary>
        /// <param name="page">Page entity</param>
        void InitFavoriteControl(PageEntity page)
        {
            FavoriteItemId = page.PartitionKey;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Get the parent page
            _page = Page as PageTemplateBase;
            if (_page == null)
                throw new ApplicationException("The page does not inherit from the VeraWAF.Core.Templates.PageTemplateBase page template.");

            // Get page entity data
            var pageEntity = _page.GetPageEntity();
            if (pageEntity != null)
            {
                Visible = true;

                InitFavoriteControl(_page.GetPageEntity());
                if (FavoriteItemId == null) return;

                var favorites = _favoriteCache.GetFavorites(FavoriteItemId);
                numFavorites.InnerText = favorites == null ? "0" : favorites.Count.ToString(CultureInfo.InvariantCulture);

                var providerUserKey = GetProviderUserKey();
                btnFavorite.Text = providerUserKey != null && _favoriteCache.UserHasFavorited(FavoriteItemId, providerUserKey) ? "★" : "☆";
            }
            else Visible = false;
        }

        protected void btnFavorite_OnClick(object sender, EventArgs e)
        {
            var providerUserKey = GetProviderUserKey();
            if (providerUserKey == null) FormsAuthentication.RedirectToLoginPage();
            else
            {
                _favoriteCache.AddFavorite(FavoriteItemId, providerUserKey);

                Response.Redirect(Request.Url.AbsolutePath);
            }
        }
    }
}