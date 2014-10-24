using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using VeraWAF.AzureQueue;
using VeraWAF.AzureTableStorage;
using ThreadState = System.Threading.ThreadState;

namespace VeraWAF.ThreadedWorkerRoleLib
{
    /// <summary>
    /// See http://www.31a2ba2a-b718-11dc-8314-0800200c9a66.com/2010/12/running-multiple-threads-on-windows.html
    /// for information on how this multi-threaded worker role software design pattern works
    /// </summary>
    public abstract class WorkerEntryPoint
    {
        protected internal AzureQueueDataSource QueuDataSource;
        protected internal AzureTableStorageDataSource TableStorageDataSource;

        /// Application name
        protected internal string applicationName;

        protected WorkerEntryPoint()
        {
            Debug.WriteLine("Creating new {0}", GetType());
            MinThreadSleepInSeconds = 0;
            MaxThreadSleepInSeconds = 10;
            CurrentThreadSleepInSeconds = MinThreadSleepInSeconds;

            applicationName = ConfigurationManager.AppSettings["ApplicationName"];

            QueuDataSource = new AzureQueueDataSource();
            TableStorageDataSource = new AzureTableStorageDataSource();
        }

        /// <summary>
        /// Current number of seconds to thread sleep. A value between MinThreadSleepInSeconds
        /// and MaxThreadSleepInSeconds.
        /// </summary>
        int CurrentThreadSleepInSeconds { get; set; }

        /// <summary>
        /// Minimum number of seconds to thread sleep
        /// </summary>
        public int MinThreadSleepInSeconds { protected get; set; }

        /// <summary>
        /// Maximum number of seconds to thread sleep
        /// </summary>
        public int MaxThreadSleepInSeconds { protected get; set; }

        /// <summary>
        /// If there was nothing to do for the worker then double the thread sleep duration from 
        /// last time, but make sure that the set max sleep duration MaxThreadSleepInSeconds is 
        /// respected.
        /// </summary>
        void DoubleThreadSleepDuration()
        {
            Debug.WriteLine("Old sleep duration was {0} ...", CurrentThreadSleepInSeconds);

            CurrentThreadSleepInSeconds *= 2;

            if (CurrentThreadSleepInSeconds == 0) CurrentThreadSleepInSeconds = 1;
            else if (CurrentThreadSleepInSeconds > MaxThreadSleepInSeconds)
                CurrentThreadSleepInSeconds = MaxThreadSleepInSeconds;

            Debug.WriteLine("... new sleep duration is {0}", CurrentThreadSleepInSeconds);
        }

        /// <summary>
        /// Blocks the current thread for CurrentThreadSleepInSeconds seconds
        /// </summary>
        void Sleep()
        {
            Debug.WriteLine("Thread is sleeping for {0} seconds", CurrentThreadSleepInSeconds);

            Thread.Sleep(TimeSpan.FromSeconds(CurrentThreadSleepInSeconds));

            Debug.WriteLine("Thread is done sleeping for {0} seconds", CurrentThreadSleepInSeconds);
        }

        /// <summary>
        /// Implements a Back Off software design pattern that works as follows:
        /// A thread sleep interval after processing the queue is started at 0 seconds. 
        /// Every time there are no items to process in the queue then the thread sleep interval is increased with one second until
        /// a set limit (let say one minute for the sake of the example) is reached. Every time there are items to process in the
        /// queue the thread sleep interval is set to 0 seconds again. Since you with Azure pay for the processing time you use the
        /// Back Off pattern is quite useful, but be careful to use it with care so that your users will not have to wait a very
        /// long time for something that should be done immidately.
        /// </summary>
        internal void BackOff()
        {
            Debug.WriteLine("No items processed, so using a Back Off software design pattern to conserve otherwise idle resources");

            DoubleThreadSleepDuration();
            Sleep();
        }

        /// <summary>
        /// Checks if the current thread is closing
        /// </summary>
        /// <returns>True if the thread is closing, false otherwise</returns>
        internal bool ThreadIsClosing()
        {
            return Thread.CurrentThread.ThreadState == ThreadState.AbortRequested;
        }

        /// <summary>
        /// Starts the worker thread. Will not return until thread abort is called.
        /// </summary>
        public void Run()
        {
            Debug.WriteLine("Worker was started");
            Debug.Indent();

            // Loop until the thread is closing
            while (!ThreadIsClosing())
            {
                var numberOfProcessedMessagesInQueue = ProcessItems();
                if (Convert.ToBoolean(numberOfProcessedMessagesInQueue))
                {
                    Debug.WriteLine("{0} items was processed; so resetting thread sleep duration", numberOfProcessedMessagesInQueue);
                    CurrentThreadSleepInSeconds = MinThreadSleepInSeconds;
                }
                else
                {
                    Debug.WriteLine("No items were processed; so backing off by doubling the thread sleep duration until the MaxThreadSleepInSeconds limit has been reached.");
                    BackOff();
                }
            }

            Debug.Unindent();
            Debug.WriteLine("Worker is stopping");
        }

        /// <summary>
        /// Proesses all items
        /// </summary>
        /// <returns>Number of items that was processed</returns>
        protected virtual int ProcessItems()
        {
            throw new NotImplementedException();
        }

        public virtual bool OnStart()
        {
            return true;
        }

        public virtual void OnStop() {}

        /// <summary>
        /// This method prevents unhandled exceptions from being thrown
        /// from the worker thread.
        /// </summary>
        internal void ProtectedRun()
        {
            try
            {
                // Call the Workers Run() method
                Run();
            }
            catch (SystemException)
            {
                // Exit Quickly on a System Exception
                throw;
            }
            catch (Exception)
            {
                // Intentionally empty block
            }
        }
    }
}
