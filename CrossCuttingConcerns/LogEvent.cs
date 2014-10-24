using VeraWAF.AzureTableStorage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;

namespace VeraWAF.CrossCuttingConcerns
{
    public enum ELogEventTypes
    {
        Info, Warning, Error
    };

    public class LogEvent
    {
        public void AddEvent(ELogEventTypes eventType, string msg, string applicationName)
        {
            string evType;

            switch (eventType)
            {
                case ELogEventTypes.Info:
                    evType = "Info";
                    break;
                case ELogEventTypes.Warning:
                    evType = "Warning";
                    break;
                case ELogEventTypes.Error:
                    evType = "Error";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            try
            {
                var datasource = new AzureTableStorageDataSource();
                var logEntity = new CloudLogEntity
                {
                    PartitionKey = DateTime.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture),
                    RowKey = evType,
                    ApplicationName = applicationName,
                    Message = msg
                };

                datasource.Insert(logEntity);
            }
            catch(Exception)
            {
                // Don't throw exception is the tables have not been created yet
            }
        }
    }
}