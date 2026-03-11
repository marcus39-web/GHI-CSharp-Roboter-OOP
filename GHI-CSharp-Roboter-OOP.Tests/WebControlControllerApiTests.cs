using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Newtonsoft.Json;

namespace GHI_CSharp_Roboter_OOP.Tests
{
    public class WebControlControllerApiTests : IClassFixture<WebApplicationFactory<GHI_CSharp_Roboter_OOP.Program>>
    {
        private readonly HttpClient _client;

        public WebControlControllerApiTests(WebApplicationFactory<GHI_CSharp_Roboter_OOP.Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Connect_And_Command_Api_Works()
        {
            var connectResp = await _client.PostAsync("/api/webcontrol/connect", null);
            connectResp.EnsureSuccessStatusCode();
            var cmd = new { Command = "FORWARD" };
            var content = new StringContent(JsonConvert.SerializeObject(cmd), Encoding.UTF8, "application/json");
            var cmdResp = await _client.PostAsync("/api/webcontrol/command", content);
            cmdResp.EnsureSuccessStatusCode();
            var disconnectResp = await _client.PostAsync("/api/webcontrol/disconnect", null);
            disconnectResp.EnsureSuccessStatusCode();
        }
    }
}
