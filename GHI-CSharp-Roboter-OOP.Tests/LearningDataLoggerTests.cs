using System;
using Xunit;
using GHI_CSharp_Roboter_OOP.Models;
using System.IO;

namespace GHI_CSharp_Roboter_OOP.Tests
{
    public class LearningDataLoggerTests
    {
        [Fact]
        public void Log_WritesJsonLine()
        {
            string testFile = "learning_data.jsonl";
            if (File.Exists(testFile)) File.Delete(testFile);
            var entry = new LearningDataEntry
            {
                Timestamp = DateTime.Now,
                Command = "TEST",
                Distance = 42,
                Category = "TestCat"
            };
            LearningDataLogger.Log(entry);
            Assert.True(File.Exists(testFile));
            var lines = File.ReadAllLines(testFile);
            Assert.Single(lines);
            Assert.Contains("\"Command\":\"TEST\"", lines[0]);
            Assert.Contains("\"Distance\":42", lines[0]);
            Assert.Contains("\"Category\":\"TestCat\"", lines[0]);
        }
    }
}
