using Microsoft.AspNetCore.Mvc;
using GHI_CSharp_Roboter_OOP.Models;

namespace GHI_CSharp_Roboter_OOP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebControlController : ControllerBase
    {
        private static RobotGateway? _gateway;
        private static readonly object _initLock = new object();

        // Initialisierung (Singleton-artig)
        private void EnsureGateway()
        {
            if (_gateway == null)
            {
                lock (_initLock)
                {
                    if (_gateway == null)
                    {
                        // Immer Simulationsmodus aktivieren
                        _gateway = new RobotGateway("127.0.0.1", 5000, simulate: true);
                    }
                }
            }
        }

        [HttpPost("status")]
        public IActionResult Status()
        {
            EnsureGateway();
            return Ok(new { ok = true, message = "status", connected = _gateway!.Connected });
        }

        [HttpPost("connect")]
        public IActionResult Connect()
        {
            EnsureGateway();
            var (ok, message) = _gateway!.Connect();
            return Ok(new { ok, message, connected = _gateway.Connected });
        }

        [HttpPost("disconnect")]
        public IActionResult Disconnect()
        {
            EnsureGateway();
            var (ok, message) = _gateway!.Disconnect();
            return Ok(new { ok, message, connected = _gateway.Connected });
        }

        [HttpPost("emergency-stop")]
        public IActionResult EmergencyStop()
        {
            EnsureGateway();
            var (ok, message) = _gateway!.EmergencyStop();
            return Ok(new { ok, message, connected = _gateway.Connected });
        }

        public class CommandRequest { public string? Command { get; set; } }

        [HttpPost("command")]
        public IActionResult Command([FromBody] CommandRequest request)
        {
            EnsureGateway();
            if (string.IsNullOrWhiteSpace(request.Command))
                return BadRequest(new { ok = false, message = "command fehlt" });
            var (ok, message) = _gateway!.Send(request.Command);
            return Ok(new { ok, message, connected = _gateway.Connected });
        }

        private static readonly PredictionService _predictionService = new PredictionService();

        [HttpPost("predict")]
        public IActionResult Predict([FromBody] PredictRequest req)
        {
            string prediction = _predictionService.Predict(req.Command, req.Distance);
            return Ok(new { ok = true, prediction });
        }

        public class PredictRequest
        {
            public string? Command { get; set; }
            public int? Distance { get; set; }
        }
    }
}
