using System;
using System.Data.Services.Client;
using VeraWAF.ThreadedWorkerRoleLib;

namespace VeraWAF.MultiThreadedWorkerRole
{
    class SessionStateWorker : WorkerEntryPoint
    {
        protected override int ProcessItems()
        {
            var processedItems = 0;
            try
            {
                foreach (var session in TableStorageDataSource.GetExpiredSessions(applicationName, DateTime.Now))
                {
                    TableStorageDataSource.Delete(session);
                    processedItems++;
                }
            }
            catch (DataServiceQueryException)
            {
                // Intentionally empty; is thrown if the SessionState table is not created yet
            }
            return processedItems;
        }
    }
}
