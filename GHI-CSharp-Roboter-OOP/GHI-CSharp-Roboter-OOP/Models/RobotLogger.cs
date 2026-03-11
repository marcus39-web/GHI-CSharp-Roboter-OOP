using System;
using System.IO;
using System.Threading;

namespace GHI_CSharp_Roboter_OOP.Models
{
    public static class RobotLogger
    {
        private static readonly object _logLock = new object();
        private const string LogFile = "robot_log.txt";

        public static void Log(string message, string level = "INFO")
        {
            try
            {
                lock (_logLock)
                {
                    File.AppendAllText(LogFile, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}\n");
                }
            }
            catch { }
        }
    }
}
