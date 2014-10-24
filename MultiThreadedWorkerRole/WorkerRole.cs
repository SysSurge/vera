using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using VeraWAF.ThreadedWorkerRoleLib;

namespace VeraWAF.MultiThreadedWorkerRole
{
    public class WorkerRole : ThreadedRoleEntryPoint
    {

        List<WorkerEntryPoint> GetWorkerRoles()
        {
            // Add all your worker roles here and they will run in their own thread
            return new List<WorkerEntryPoint> {
                // Add a worker that processes e-mails placed on the message queue. Runs in its own thread.
                new EmailWorker {
                    MaxThreadSleepInSeconds = int.Parse(ConfigurationManager.AppSettings["EmailWorkerMaxThreadSleepSeconds"])
                }
                // Add a worker that cleans up old session states that stored are stored in the table storage
                //, new SessionStateWorker {
                //    MaxThreadSleepInSeconds = int.Parse(ConfigurationManager.AppSettings["SessionStateWorkerMaxThreadSleepSeconds"])
                //}                
                // Example of a third worker role, this would work in its own thread seperate from any other workers
                /* , new MessageWorker{
                    MaxThreadSleepInSeconds = int.Parse(ConfigurationManager.AppSettings["MessageWorkerMaxThreadSleepSeconds"])
                }*/   
            };
        }

        public override bool OnStart()
        {
            //new AzureDiagnostics().EnableAzureDiagnostics();

            Trace.TraceInformation("WebRole started");

            return OnStart(GetWorkerRoles().ToArray());
        }

        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.TraceInformation("Worker Role entry point called");

            base.Run();
        }

    }
}
