using System;
using Microsoft.WindowsAzure.StorageClient;

namespace VeraWAF.AzureTableStorage
{
    /// <summary>
    /// WADWindowsEventLogsTable entity.
    /// The WADWindowsEventLogsTable table contains Windows events
    /// </summary>
    public class WADWindowsEventLogEntity : TableServiceEntity
    {
        /// <summary>
        /// Holds Windows event
        /// </summary>
        /// <param name="partitionKey">Event timestamp</param>
        /// <param name="rowKey">Azure instance and role that logged the event</param>
        public WADWindowsEventLogEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
        }

        public WADWindowsEventLogEntity()
            : this(Guid.NewGuid().ToString(), String.Empty)
        {
        }

        /// <summary>
        /// Event tick counter (timestamp)
        /// </summary>
        public System.Int64 EventTickCount { get; set; }

        /// <summary>
        /// Azure deployment ID
        /// </summary>
        public string DeploymentId		{ get; set; }

        /// <summary>
        /// Azure Role
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Azure Role instance
        /// </summary>
        public string RoleInstance		{ get; set; }

        /// <summary>
        /// Provider ID
        /// </summary>
        public string ProviderGuid		{ get; set; }

        /// <summary>
        /// Provider name
        /// </summary>
        public string ProviderName		{ get; set; }

        /// <summary>
        /// Event ID
        /// </summary>
        public System.Int32 EventId		{ get; set; }

        /// <summary>
        /// Event level
        /// </summary>
        public System.Int32 Level { get; set; }

        /// <summary>
        /// Process ID
        /// </summary>
        public System.Int32 Pid { get; set; }

        /// <summary>
        /// Thread ID
        /// </summary>
        public System.Int32 Tid { get; set; }

        /// <summary>
        /// Event channel
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// Event description text
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Event as raw XML
        /// </summary>
        public string RawXml { get; set; }
    }
}
