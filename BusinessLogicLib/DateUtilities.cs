using System;
using System.Configuration;
using System.Globalization;

namespace VeraWAF.WebPages.Bll {
    public enum ReadableDateAntTimeTypes
    {
        Abbreviated,
        Full,
        NoSeconds
    }

    public class DateUtilities {
        /// <summary>
        /// Get a UTC ISO8601 date without the milliseconds part
        /// </summary>
        /// <param name="inDate">Date to convert</param>
        /// <returns>UTC ISO8601 date without the milliseconds part as string</returns>
        public string GetCustomIso8601Date(DateTime inDate)
        {
            return inDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
        }

        /// <summary>
        /// Get an easily readable date formatted like "Wednesday, October 01, 2008"
        /// </summary>
        /// <param name="inDate">Date to convert</param>
        /// <returns>Date as string</returns>
        public string GetReadableDate(DateTime inDate)
        {
            return inDate.ToUniversalTime().ToString("ddd MMM dd", CultureInfo.CreateSpecificCulture(ConfigurationManager.AppSettings["SiteCulture"]));
        }

        /// <summary>
        /// Get an easily readable date formatted like "Thursday, October 02, 2008 12:04:32 AM GMT", "Thursday, October 02, 2008 12:04 AM GMT",
        /// or "Thu Oct 2, 2008 12:04 AM GMT"
        /// </summary>
        /// <param name="inDate">Date to convert</param>
        /// <param name="readableDateAntTimeType">Type</param>
        /// <returns>Date as string</returns>
        public string GetReadableDateAndTime(DateTime inDate, ReadableDateAntTimeTypes readableDateAntTimeType = ReadableDateAntTimeTypes.Full)
        {
            String dateTimeText;
            const string gmt = " GMT";

            switch (readableDateAntTimeType)
            {
                case ReadableDateAntTimeTypes.Abbreviated:
                    dateTimeText = inDate.ToUniversalTime().ToString("ddd MMM d, yyyy hh:mm tt", CultureInfo.CreateSpecificCulture(
                        ConfigurationManager.AppSettings["SiteCulture"])) + gmt;
                    break;                    
                case ReadableDateAntTimeTypes.NoSeconds:
                    dateTimeText = inDate.ToUniversalTime().ToString("f", CultureInfo.CreateSpecificCulture(
                        ConfigurationManager.AppSettings["SiteCulture"])) + gmt;
                    break;
                case ReadableDateAntTimeTypes.Full:
                    dateTimeText = inDate.ToUniversalTime().ToString("U", CultureInfo.CreateSpecificCulture(
                        ConfigurationManager.AppSettings["SiteCulture"])) + gmt;
                    break;
                default:
                    throw new ArgumentException("readableDateAntTimeType is invalid");
            }
            return dateTimeText;
        }
    }
}