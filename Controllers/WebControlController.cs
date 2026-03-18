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
        private static readonly PredictionService _predictionService = new PredictionService();

        private void EnsureGateway()
        {
            if (_gateway == null)
            {
                lock (_initLock)
                {
                    if (_gateway == null)
                    {
                        // Wichtig: Port auf 8000 geändert, da die Web-App selbst auf 5000 läuft!
                        _gateway = new RobotGateway("127.0.0.1", 8000, simulate: true);
                    }
                }
            }
        }

        // Geändert auf HttpGet, damit du es im Browser unter /api/webcontrol/status prüfen kannst
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            EnsureGateway();
            return Ok(new
            {
                ok = true,
                message = "BrainBot Online",
                connected = _gateway!.Connected,
                mode = "Simulation"
            });
        }

        [HttpPost("connect")]
        public IActionResult Connect()
        {
            EnsureGateway();
            var (ok, message) = _gateway!.Connect();
            return Ok(new { ok, message, connected = _gateway.Connected });
        }

        [HttpPost("command")]
        public IActionResult Command([FromBody] CommandRequest request)
        {
            EnsureGateway();
            if (string.IsNullOrWhiteSpace(request.Command))
                return BadRequest(new { ok = false, message = "Kommando fehlt" });

            var (ok, message) = _gateway!.Send(request.Command);

            // Hier passiert die Magie: Wir holen uns direkt eine KI-Vorhersage für das Log
            // In einer echten App würde das Gateway diese Daten nun in die neue DB-Struktur schreiben
            return Ok(new { ok, message, connected = _gateway.Connected });
        }

        [HttpPost("predict")]
        public IActionResult Predict([FromBody] PredictRequest req)
        {
            if (req == null || string.IsNullOrEmpty(req.Command))
                return BadRequest(new { ok = false, message = "Daten unvollständig" });

            string prediction = _predictionService.Predict(req.Command, req.Distance ?? 0);
            return Ok(new { ok = true, prediction });
        }

        // Hilfsklassen für die API-Requests
        public class CommandRequest { public string? Command { get; set; } }
        public class PredictRequest
        {
            public string? Command { get; set; }
            public int? Distance { get; set; }
        }
    }
}