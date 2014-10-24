using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.Services.Client;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.SessionState;
using VeraWAF.AzureTableStorage;

namespace VeraWAF.WebPages.Bll
{
    /// <summary>
    /// Session State Store Provider that stores its data in the Azure Table Storage.
    /// Based on the Access example found at http://msdn.microsoft.com/en-us/library/ms178589.aspx
    /// </summary>
    public class AzureSessionStateProvider : SessionStateStoreProviderBase
    {
        public string ApplicationName { get; set; }
        SessionStateSection pConfig = null;

        public override void Initialize(string name, NameValueCollection config)
        {
            // Initialize values from web.config.
            if (config == null) throw new ArgumentNullException("config");

            if (String.IsNullOrEmpty(name)) name = "OdbcSessionStateStore";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Sample ODBC Session State Store provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);

            // Initialize the ApplicationName property.
            ApplicationName = ConfigurationManager.AppSettings["ApplicationName"];

            // Get <sessionState> configuration element.
            var applicationVirtualPath = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;
            var cfg = WebConfigurationManager.OpenWebConfiguration(applicationVirtualPath);
            pConfig = (SessionStateSection)cfg.GetSection("system.web/sessionState");
        }

        public override void Dispose()
        {
        }

        public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback)
        {
            return false;
        }

        public override void InitializeRequest(HttpContext context)
        {
        }

        void UpdateSessionLockedState(SessionStateEntity session, bool locked)
        {
            session.Locked = locked;

            if (locked) session.LockDate = DateTime.Now;

            var sessionStateDataSource = new AzureTableStorageDataSource();
            sessionStateDataSource.Update(session);
        }

        bool LockRecord(string sessionId)
        {
            var sessionStateDataSource = new AzureTableStorageDataSource();
            var sessionStateEntities = sessionStateDataSource.GetSessions(sessionId, ApplicationName, false, DateTime.Now);

            SessionStateEntity session;

            try
            {
                session = sessionStateEntities.FirstOrDefault();
            }
            catch (DataServiceQueryException)
            {
                // If the table is empty a DataServiceQueryException is thrown
                return true;
            }

            if (session == null) return true;

            UpdateSessionLockedState(session, true);

            return false;
        }

        SessionStateEntity GetSession(string sessionId)
        {
            var sessionStateDataSource = new AzureTableStorageDataSource();
            var sessionStateEntities = sessionStateDataSource.GetSessions(sessionId, ApplicationName);

            try
            {
                return sessionStateEntities.FirstOrDefault();
            }
            catch (DataServiceQueryException)
            {
                // If the table is empty a DataServiceQueryException is thrown
                return null;
            }
        }

        SessionStateEntity GetSession(string sessionId, int lockId)
        {
            var sessionStateDataSource = new AzureTableStorageDataSource();
            var sessionStateEntities = sessionStateDataSource.GetSessions(sessionId, ApplicationName, lockId);

            return sessionStateEntities.FirstOrDefault();
        }

        void DeleteSession(string sessionId)
        {
            var sessionStateDataSource = new AzureTableStorageDataSource();
            var sessionStateEntities = sessionStateDataSource.GetSessions(sessionId, ApplicationName);

            var sessionStateEntity = sessionStateEntities.FirstOrDefault();
            if (sessionStateEntity != null) sessionStateDataSource.Delete(sessionStateEntity);
        }

        /// <summary>
        /// DeSerialize is called by the GetSessionStoreItem method to convert the Base64 string stored in 
        /// SessionStateEntity::SessionItems property to a SessionStateItemCollection.
        /// </summary>
        /// <param name="context">Current Http context</param>
        /// <param name="serializedItems">Serialized data from SessionStateEntity::SessionItems</param>
        /// <param name="timeout">Milliseconds until SessionStateStoreData times out</param>
        /// <returns></returns>
        SessionStateStoreData Deserialize(HttpContext context, string serializedItems, int timeout)
        {
            var ms = new MemoryStream(Convert.FromBase64String(serializedItems));

            var sessionItems = new SessionStateItemCollection();

            if (ms.Length > 0)
            {
                var reader = new BinaryReader(ms);
                sessionItems = SessionStateItemCollection.Deserialize(reader);
            }

            return new SessionStateStoreData(sessionItems,
              SessionStateUtility.GetSessionStaticObjects(context),
              timeout);
        }

        SessionStateStoreData GetSessionStoreItem(
            bool lockRecord,
            HttpContext context,
            string id,
            out bool locked,
            out TimeSpan lockAge,
            out object lockId,
            out SessionStateActions actionFlags)
        {
            // Initial values for return value and out parameters.
            SessionStateStoreData item = null;
            lockAge = TimeSpan.Zero;
            lockId = null;
            locked = false;
            actionFlags = 0;

            // String to hold serialized SessionStateItemCollection.
            var serializedItems = String.Empty;

            // True if a record is found in the database.
            var foundRecord = false;

            // True if the returned session item is expired and needs to be deleted.
            var deleteData = false;

            // Timeout value from the data store.
            var timeout = 0;

            // lockRecord is true when called from GetItemExclusive and false when called from GetItem.
            // Obtain a lock if possible. Ignore the record if it is expired.
            if (lockRecord) locked = LockRecord(id);

            var session = GetSession(id);
            if (session != null)
            {
                var expires = session.Expires;

                if (expires < DateTime.Now)
                {
                    // The record was expired. Mark it as not locked.
                    locked = false;

                    // The session was expired. Mark the data for deletion.
                    deleteData = true;
                }
                else foundRecord = true;

                serializedItems = session.SessionItems;
                lockId = session.LockId;
                lockAge = DateTime.Now.Subtract(session.LockDate);
                actionFlags = (SessionStateActions)session.Flags;
                timeout = session.TimeOut;
            }

            // If the returned session item is expired then delete the record from the data source.
            if (deleteData) DeleteSession(id);

            // The record was not found. Ensure that locked is false.
            if (!foundRecord) locked = false;

            // If the record was found and you obtained a lock, then set 
            // the lockId, clear the actionFlags,
            // and create the SessionStateStoreItem to return.
            if (foundRecord && !locked)
            {
                session.Flags = 0;

                lockId = (int)lockId + 1;   // Marked as an "out" parameter
                session.LockId = (int)lockId;

                var sessionDataSource = new AzureTableStorageDataSource();
                sessionDataSource.Update(session);

                // If the actionFlags parameter is not InitializeItem then deserialize the stored SessionStateItemCollection.
                item = actionFlags == SessionStateActions.InitializeItem ?
                    CreateNewStoreData(context, (int)pConfig.Timeout.TotalMinutes) : Deserialize(context, serializedItems, timeout);
            }

            return item;
        }

        public override SessionStateStoreData GetItem(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
        {
            return GetSessionStoreItem(false, context, id, out locked, out lockAge, out lockId, out actions);
        }

        public override SessionStateStoreData GetItemExclusive(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
        {
            return GetSessionStoreItem(true, context, id, out locked, out lockAge, out lockId, out actions);
        }

        public override void ReleaseItemExclusive(HttpContext context, string id, object lockId)
        {
            var session = GetSession(id, (int)lockId);
            if (session != null)
            {
                session.Locked = false;
                session.Expires = DateTime.Now.AddMinutes(pConfig.Timeout.TotalMinutes);

                var sessionStateDataSource = new AzureTableStorageDataSource();
                sessionStateDataSource.Update(session);
            }
        }

        /// <summary>
        /// Serialize is called by the SetAndReleaseItemExclusive method to 
        /// convert the SessionStateItemCollection into a Base64 string to    
        /// be stored in an Access Memo field.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        string Serialize(SessionStateItemCollection items)
        {
            var ms = new MemoryStream();
            var writer = new BinaryWriter(ms);

            if (items != null) items.Serialize(writer);

            writer.Close();

            return Convert.ToBase64String(ms.ToArray());
        }

        SessionStateEntity GetSession(string sessionId, DateTime expiration)
        {
            var sessionStateDataSource = new AzureTableStorageDataSource();
            var sessionStateEntities = sessionStateDataSource.GetSessions(sessionId, ApplicationName, expiration);

            try
            {
                return sessionStateEntities.FirstOrDefault();
            }
            catch (DataServiceQueryException)
            {
                // If the table don't exist a DataServiceQueryException is thrown
                return null;
            }
        }

        void DeleteSession(string sessionId, DateTime expiration)
        {
            var sessionDataSource = new AzureTableStorageDataSource();
            var session = GetSession(sessionId, expiration);

            if (session != null) sessionDataSource.Delete(session);
        }

        void CreateSession(string sessionId, int timeOut, string sessionItems)
        {
            var sessionEntity = new SessionStateEntity(sessionId, String.Empty)
            {
                ApplicationName = this.ApplicationName,
                Expires = DateTime.Now.AddMinutes((Double)timeOut),
                LockDate = DateTime.Now,
                LockId = 0,
                TimeOut = timeOut,
                Locked = false,
                SessionItems = sessionItems,
                Flags = 0
            };

            var sessionDataSource = new AzureTableStorageDataSource();

            try
            {
                sessionDataSource.Insert(sessionEntity);
            }
            catch (DataServiceRequestException)
            {
                // If the table don't exist a DataServiceRequestException is thrown
            }
        }

        public override void SetAndReleaseItemExclusive(HttpContext context, string id, SessionStateStoreData item, object lockId, bool newItem)
        {
            // Serialize the SessionStateItemCollection as a string.
            var sessionItems = Serialize((SessionStateItemCollection)item.Items);

            if (newItem)
            {
                DeleteSession(id, DateTime.Now);
                CreateSession(id, item.Timeout, sessionItems);
            }
            else
            {
                var session = GetSession(id, (int)lockId);
                if (session == null) return;

                session.Expires = DateTime.Now.AddMinutes((Double)item.Timeout);
                session.SessionItems = sessionItems;
                session.Locked = false;

                var sessionDataSource = new AzureTableStorageDataSource();
                sessionDataSource.Update(session);
            }
        }

        void DeleteSession(string sessionId, int lockId)
        {
            var sessionDataSource = new AzureTableStorageDataSource();
            var session = GetSession(sessionId, lockId);

            if (session != null) sessionDataSource.Delete(session);
        }

        public override void RemoveItem(HttpContext context, string id, object lockId, SessionStateStoreData item)
        {
            DeleteSession(id, (int)lockId);
        }

        void UpdateSessionExpirationDate(SessionStateEntity session, DateTime expires)
        {
            var sessionDataSource = new AzureTableStorageDataSource();
            session.Expires = expires;
            try
            {
                sessionDataSource.Update(session);
            }
            catch (DataServiceRequestException)
            {
                // A DataServiceClientException is sometimes thrown here when starting up the solution 
            }
            catch (DataServiceClientException)
            {
                // A DataServiceClientException is sometimes thrown here when starting up the solution 
            }
        }

        public override void ResetItemTimeout(HttpContext context, string id)
        {
            var session = GetSession(id);
            var expires = DateTime.Now.AddMinutes(pConfig.Timeout.TotalMinutes);

            if (session != null) UpdateSessionExpirationDate(session, expires);
        }

        public override SessionStateStoreData CreateNewStoreData(HttpContext context, int timeout)
        {
            return new SessionStateStoreData(
                new SessionStateItemCollection(),
                SessionStateUtility.GetSessionStaticObjects(context),
                timeout);
        }

        public override void CreateUninitializedItem(HttpContext context, string id, int timeout)
        {
            var sessionEntity = new SessionStateEntity(id, String.Empty)
            {
                ApplicationName = this.ApplicationName,
                Expires = DateTime.Now.AddMinutes((Double)timeout),
                LockDate = DateTime.Now,
                LockId = 0,
                TimeOut = timeout,
                Locked = false,
                SessionItems = String.Empty,
                Flags = 1
            };

            var sessionDataSource = new AzureTableStorageDataSource();

            try
            {
                sessionDataSource.Insert(sessionEntity);
            }
            catch (DataServiceQueryException)
            {
                /* 
                 * Todo: CreateUninitializedItem is sometimes called twize with the same session id for an unknown reason, this 
                 * catch block was made to avoid an error if the session item already existed. Fix later.
                */
            }
            catch (DataServiceRequestException)
            {
                // During solution startup a DataServiceRequestException may be thrown
            }
        }

        public override void EndRequest(HttpContext context)
        {
        }
    }
}