using System;
using System.Diagnostics;
using System.Threading;

namespace VeraWAF.WebPages.Bll
{
    /// <summary>
    /// Helper class for system diagnostics
    /// </summary>
    public class DiagnosticHelper
    {
        /// <summary>
        /// Contain system diagnostic information
        /// </summary>
        public struct DiagnosticStatistics
        {
            /// <summary>
            /// % Processor load
            /// </summary>
            public int processor;

            /// <summary>
            /// Available MBytes of internal memory
            /// </summary>
            public int memory;
        }

        /// <summary>
        /// Get the current system processor usage and available internal memory resources
        /// </summary>
        /// <param name="samplingIntervalMs"></param>
        public DiagnosticStatistics GetPerformanceStatistics(int samplingIntervalMs)
        {
            // Get the percentage processor load
            var procPercentage = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            procPercentage.NextValue();

            // Get the number of available internal memory in MBytes
            var memAvailMb = new PerformanceCounter("Memory", "Available MBytes");
            memAvailMb.NextValue();

            /*
            * Wait while sampling data. Also make sure that we don't consume too much resources by limiting the 
            * sampling timeframe to a maximum of three seconds.
            */
            const int maxSamplingIntervalMs = 3000;
            if (samplingIntervalMs > maxSamplingIntervalMs) samplingIntervalMs = maxSamplingIntervalMs;

            // Wait while gathering performance data
            Thread.Sleep(samplingIntervalMs);

            var stats = new DiagnosticStatistics();
            stats.memory = (int)procPercentage.NextValue();
            stats.processor = (int)memAvailMb.NextValue();

            return stats;
        }
    }
}
