using System;
using Xunit;
using GHI_CSharp_Roboter_OOP.Models;

namespace GHI_CSharp_Roboter_OOP.Tests
{
    public class RobotGatewaySimulationTests
    {
        [Fact]
        public void Gateway_Connect_Send_Disconnect_Simulation()
        {
            var gateway = new RobotGateway("127.0.0.1", 5000, simulate: true);
            var (ok1, msg1) = gateway.Connect();
            Assert.True(ok1);
            var (ok2, msg2) = gateway.Send("FORWARD");
            Assert.True(ok2);
            var (ok3, msg3) = gateway.Disconnect();
            Assert.True(ok3);
        }

        [Fact]
        public void Gateway_EmergencyStop_Simulation()
        {
            var gateway = new RobotGateway("127.0.0.1", 5000, simulate: true);
            gateway.Connect();
            var (ok, msg) = gateway.EmergencyStop();
            Assert.True(ok);
        }
    }
}
