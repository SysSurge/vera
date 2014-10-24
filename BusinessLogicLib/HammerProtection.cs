using System;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using System.Web.Profile;
using System.Web.Security;

namespace VeraWAF.WebPages.Bll
{
    public struct HammerTypes
    {
        public static string RequestHammering = "RequestHammering";
        public static string CommentsHammering = "CommentsHammering";
        public static string EmailHammering = "EmailHammering";
        public static string PasswordHammering = "PasswordHammering";
        public static string BetaApplicationHammering = "BetaApplicationHammering";
    }

    internal class IncrementingCacheCounter
    {
        public int Count;
        public DateTime ExpireDate;
    }

    public enum HammeringMode
    {
        Minutes,
        Hours
    }

    public class HammerProtection
    {
        private readonly long _maxNumberOfRequestAMinute;
        private readonly HammeringMode _mode;
        private readonly string _prefix;

        public HammerProtection(long maxNumberOfRequestAMinute = 1000, HammeringMode mode = HammeringMode.Minutes, string prefix = "RequestHammering")
        {
            _maxNumberOfRequestAMinute = maxNumberOfRequestAMinute;
            _mode = mode;
            _prefix = prefix;
        }

        DateTime GetExpireDate()
        {
            DateTime expireDate;

            switch (_mode) {
                case HammeringMode.Minutes:
                    expireDate = DateTime.Now.AddMinutes(1);
                    break;
                case HammeringMode.Hours:
                    expireDate = DateTime.Now.AddMinutes(60);
                    break;
                default:
                    throw new ApplicationException("Unknown hammering mode");
            }

            return expireDate;
        }

        public bool HostIsHammering(string hostAddress)
        {
            var user = HttpContext.Current.User;
            if (user != null && user.IsInRole("Admins")) return false;

            var cacheKey = string.Format("{0}_{1}", _prefix, hostAddress);

            var counter = HostingEnvironment.Cache[cacheKey] as IncrementingCacheCounter;
            if (counter == null)
                counter = new IncrementingCacheCounter { Count = 1, ExpireDate = GetExpireDate() };
            else
                counter.Count++;

            HostingEnvironment.Cache.Insert(cacheKey, counter, null, counter.ExpireDate, Cache.NoSlidingExpiration,
                                            CacheItemPriority.Default, null);

            return counter.Count > _maxNumberOfRequestAMinute;
        }

        public void Reset(string hostAddress)
        {
            var cacheKey = string.Format("{0}_{1}", _prefix, hostAddress);
            var counter = new IncrementingCacheCounter { Count = 1, ExpireDate = GetExpireDate() };
            HostingEnvironment.Cache.Insert(cacheKey, counter, null, counter.ExpireDate, Cache.NoSlidingExpiration,
                                        CacheItemPriority.Default, null);
        }
    }
}