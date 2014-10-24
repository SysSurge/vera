using System;
using VeraWAF.ThreadedWorkerRoleLib;

namespace VeraWAF.MultiThreadedWorkerRole
{
    /// <summary>
    /// This is an example of a worker, see the email worker to see one that actually does something
    /// </summary>
    public class MessageWorker : WorkerEntryPoint
    {
        protected override int ProcessItems()
        {
            throw new NotImplementedException();
        }
    }
}
