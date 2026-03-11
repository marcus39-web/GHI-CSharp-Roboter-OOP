using Xunit;
using GHI_CSharp_Roboter_OOP.Models;
using System.IO;

namespace GHI_CSharp_Roboter_OOP.Tests
{
    public class CategorizationReportServiceTests
    {
        [Fact]
        public void Can_Generate_Technical_Drawing()
        {
            var db = new CategorizationDatabase();
            var service = new CategorizationReportService(db);
            string file = "test_export.jpg";
            if (File.Exists(file)) File.Delete(file);
            var req = new WebControlServer.ExportRequest
            {
                RoomName = "TestRoom",
                Width = 200,
                Depth = 150,
                Radius = 20,
                RecessWidth = 10,
                RecessDepth = 5
            };
            service.GenerateTechnicalDrawing(file, req);
            Assert.True(File.Exists(file));
        }
    }
}
