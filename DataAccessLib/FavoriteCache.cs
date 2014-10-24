using System.Collections.Generic;
using System.Configuration;
using System.Data.Services.Client;
using System.Web.Caching;
using System.Web.Hosting;
using VeraWAF.AzureTableStorage;

namespace VeraWAF.WebPages.Dal {
    public class FavoriteCache {
        public const string CacheKey = "Favorites";
        private readonly string _applicationName;

        public FavoriteCache()
        {
            _applicationName = ConfigurationManager.AppSettings["ApplicationName"];
        }

        public FavoriteCache(string applicationName) {
            _applicationName = applicationName;
        }

        private void AddFavoritesToCache(Dictionary<string, Dictionary<string, FavoriteEntity>> Favorites)
        {
            HostingEnvironment.Cache.Add(CacheKey, Favorites, null,
                                         Cache.NoAbsoluteExpiration,
                                         Cache.NoSlidingExpiration,
                                         CacheItemPriority.Default, null);
        }

        private Dictionary<string, Dictionary<string, FavoriteEntity>> GetFavoritesFromCache()
        {
            return (Dictionary<string, Dictionary<string, FavoriteEntity>>)HostingEnvironment.Cache.Get(CacheKey);
        }

        private IEnumerable<FavoriteEntity> GetFavoritesFromStore()
        {
            var datasource = new AzureTableStorageDataSource();
            var applicationName = ConfigurationManager.AppSettings["ApplicationName"];
            return datasource.GetFavorites(applicationName);
        }

        public Dictionary<string, Dictionary<string, FavoriteEntity>> GetFavoritesIndexedByItemId()
        {
            var favorites = GetFavoritesFromCache();
            if (favorites == null)
            {
                favorites = new Dictionary<string, Dictionary<string, FavoriteEntity>>();

                try
                {
                    var storedFavorites = GetFavoritesFromStore();
                    foreach (var favorite in storedFavorites)
                        if (favorites.ContainsKey(favorite.PartitionKey))
                            favorites[favorite.PartitionKey].Add(favorite.RowKey, favorite);
                        else
                            favorites.Add(favorite.PartitionKey, new Dictionary<string, FavoriteEntity> { { favorite.RowKey, favorite } });
                }
                catch (DataServiceQueryException)
                {
                    // Exception caught if there are no Favorites in the table
                }

                AddFavoritesToCache(favorites);
            }
            return favorites;
        }

        public Dictionary<string, FavoriteEntity> GetFavorites(string favoriteItemId)
        {
            var favorites = GetFavoritesIndexedByItemId();
            return favorites.ContainsKey(favoriteItemId) ? favorites[favoriteItemId] : null;
        }

        public void Clear()
        {
            HostingEnvironment.Cache.Remove(CacheKey);
        }

        public bool UserHasFavorited(string favoriteItemId, object user)
        {
            var votingUserString = (string)user;
            var favorites = GetFavoritesIndexedByItemId();

            return favorites.ContainsKey(favoriteItemId) && favorites[favoriteItemId].ContainsKey(votingUserString);
        }

        public void AddFavorite(string favoriteItemId, object user)
        {
            var votingUserString = (string)user;
            var dataSource = new AzureTableStorageDataSource();

            var favorites = GetFavoritesIndexedByItemId();
            if (favorites.ContainsKey(favoriteItemId))
            {
                if (favorites[favoriteItemId].ContainsKey(votingUserString))
                {
                    dataSource.Delete(favorites[favoriteItemId][votingUserString]);
                    favorites[favoriteItemId].Remove(votingUserString);
                }
                else
                {
                    // New favorite
                    var favorite = new FavoriteEntity(favoriteItemId, votingUserString)
                                   {
                                       ApplicationName = _applicationName
                                   };
                    
                    favorites[favoriteItemId].Add(votingUserString, favorite);
                    dataSource.Insert(favorite);
                }
            }
            else
            {
                var favorite = new FavoriteEntity(favoriteItemId, votingUserString)
                                {
                                    ApplicationName = _applicationName
                                };

                favorites.Add(favorite.PartitionKey, new Dictionary<string, FavoriteEntity> { {favorite.RowKey, favorite} });
                dataSource.Insert(favorite);
            }

            AddFavoritesToCache(favorites);
        }

    }
}