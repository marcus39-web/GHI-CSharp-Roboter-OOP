using System;
using System.IO;
using Newtonsoft.Json;

namespace GHI_CSharp_Roboter_OOP.Models
{
    public static class LearningDataLogger
    {
        private static readonly object _logLock = new object();
        private const string LogFile = "learning_data.jsonl";

        public static void Log(LearningDataEntry entry)
        {
            try
            {
                lock (_logLock)
                {
                    string json = JsonConvert.SerializeObject(entry);
                    File.AppendAllText(LogFile, json + "\n");
                }
            }
            catch { }
        }
    }
}
