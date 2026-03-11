using System;
using Xunit;
using GHI_CSharp_Roboter_OOP.Models;
using System.IO;

namespace GHI_CSharp_Roboter_OOP.Tests
{
    public class BrainBotRemoteSimulationTests
    {
        [Fact]
        public void Connect_Disconnect_Simulation_Works()
        {
            var remote = new BrainBotRemote("127.0.0.1", 5000, simulate: true);
            Assert.True(remote.Connect());
            remote.Disconnect();
            // Kein Exception = Test bestanden
        }

        [Fact]
        public void SendCommand_Simulation_WritesTestData()
        {
            var remote = new BrainBotRemote("127.0.0.1", 5000, simulate: true);
            remote.Connect();
            string testFile = "learning_data.jsonl";
            if (File.Exists(testFile)) File.Delete(testFile);
            remote.SendCommand("FORWARD");
            remote.SendCommand("STOP");
            remote.Disconnect();
            Assert.True(File.Exists(testFile));
            var lines = File.ReadAllLines(testFile);
            Assert.True(lines.Length >= 2);
            Assert.Contains("FORWARD", lines[0]);
            Assert.Contains("STOP", lines[1]);
        }
    }
}
