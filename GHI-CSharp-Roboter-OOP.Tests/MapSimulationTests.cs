using Xunit;
using GHI_CSharp_Roboter_OOP.Models;

namespace GHI_CSharp_Roboter_OOP.Tests
{
    public class MapSimulationTests
    {
        [Fact]
        public void Can_Generate_And_Get_SimulationData()
        {
            var sim = new MapSimulation();
            sim.GenerateRandomMap(10, 10);
            var map = sim.GetCurrentMap();
            Assert.NotNull(map);
            Assert.True(map.Width > 0 && map.Height > 0);
        }
    }
}
