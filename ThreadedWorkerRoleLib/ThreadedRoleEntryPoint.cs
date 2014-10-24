using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;

namespace VeraWAF.ThreadedWorkerRoleLib
{
    /// <summary>
    /// See http://www.31a2ba2a-b718-11dc-8314-0800200c9a66.com/2010/12/running-multiple-threads-on-windows.html
    /// for information on how this multi-threaded worker role software design pattern works
    /// </summary>
    public abstract class ThreadedRoleEntryPoint : RoleEntryPoint
    {
        readonly List<Thread> Threads = new List<Thread>();
        WorkerEntryPoint[] Workers;
        protected EventWaitHandle EventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);

        void CreateThreadsForAllWorkers()
        {
            foreach (var worker in Workers) Threads.Add(new Thread(worker.ProtectedRun));            
        }

        void StartAllWorkerThreads()
        {
            foreach (var thread in Threads) thread.Start();            
        }

        void StartAllWorkerRolesInNewThreads()
        {
            CreateThreadsForAllWorkers();
            StartAllWorkerThreads();
        }

        /// <summary>
        /// Unhandled exceptions in worker roles will cause the thread to stop, this
        /// method will find the dead threads an restart them.
        /// </summary>
        void RestartDeadWorkerRoleThreads()
        {
            for (var i = 0; i < Threads.Count; i++)
                if (!Threads[i].IsAlive)
                {
                    Threads[i] = new Thread(Workers[i].Run);
                    Threads[i].Start();
                }            
        }

        void KeepThreadsAliveMonitorLoop()
        {
            var checkDeadThreadSleepInSeconds = int.Parse(ConfigurationManager.AppSettings["CheckDeadThreadSleepInSeconds"]);

            while (!EventWaitHandle.WaitOne(0))
            {
                RestartDeadWorkerRoleThreads();

                EventWaitHandle.WaitOne(checkDeadThreadSleepInSeconds);
            }
        }

        public override void Run()
        {
            StartAllWorkerRolesInNewThreads();

            KeepThreadsAliveMonitorLoop();
        }

        public bool OnStart(WorkerEntryPoint[] workers)
        {
            Workers = workers;

            foreach (var worker in workers) worker.OnStart();

            return base.OnStart();
        }

        public override bool OnStart()
        {
            throw new InvalidOperationException();
        }

        void AbortAllWorkerThreads()
        {
            foreach (var thread in Threads)
                while (thread.IsAlive)
                    thread.Abort();            
        }

        void WaitUntilAllWorkerThreadsAreDead()
        {
            foreach (var thread in Threads)
                while (thread.IsAlive) Thread.Sleep(10);            
        }

        void NotifyAllWorkerToStopLooping()
        {
            foreach (var worker in Workers) worker.OnStop();            
        }

        public override void OnStop()
        {
            EventWaitHandle.Set();

            AbortAllWorkerThreads();
            WaitUntilAllWorkerThreadsAreDead();
            NotifyAllWorkerToStopLooping();

            base.OnStop();
        }
    }
}
