using Microsoft.AspNetCore.Mvc;
using GHI_CSharp_Roboter_OOP.Models;
using System.Text.Json.Serialization;
using System.Linq;
using System.Collections.Generic;
using System;

namespace GHI_CSharp_Roboter_OOP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebControlController : ControllerBase
    {
        private static RobotGateway? _gateway;
        private readonly CategorizationDatabase _db;

        public WebControlController(CategorizationDatabase db)
        {
            _db = db;
        }

        [HttpGet("history")]
        public IActionResult GetHistory()
        {
            // Holt die Daten und nutzt 'dynamic', damit Visual Studio keine Fehler meldet
            var rawData = _db.GetHistory(100).Cast<dynamic>().ToList();

            // Mapping: Wir bauen das Objekt exakt so, wie deine index.html es mit "item.X" abruft
            var tableData = rawData.Select(h => {
                string cat = h.Category ?? "";
                int x = 0, y = 0, d = 0;

                // Extrahiert die Zahlen aus dem Text "(X:40 Y:122 D:80)" für die Tabellenspalten
                try
                {
                    if (cat.Contains("X:"))
                    {
                        var parts = cat.Split(new[] { ':', ' ', ')', '(' }, StringSplitOptions.RemoveEmptyEntries);
                        x = int.Parse(parts[Array.IndexOf(parts, "X") + 1]);
                        y = int.Parse(parts[Array.IndexOf(parts, "Y") + 1]);
                        d = int.Parse(parts[Array.IndexOf(parts, "D") + 1]);
                    }
                }
                catch { /* Falls Extraktion scheitert, bleiben Werte auf 0 */ }

                return new
                {
                    Zeitpunkt = h.Zeitpunkt, // Wichtig für item.Zeitpunkt
                    Quelle = h.Quelle,       // Wichtig für item.Quelle
                    Category = cat,          // Wichtig für item.Category
                    posX = x,                // Wichtig für item.posX (Karte)
                    posY = y,                // Wichtig für item.posY (Karte)
                    Distanz = d              // Wichtig für item.Distanz (Tabelle)
                };
            });

            return Ok(tableData);
        }

        [HttpPost("command")]
        public IActionResult Command([FromBody] CommandRequest request)
        {
            // Gateway initialisieren falls nötig
            if (_gateway == null) _gateway = new RobotGateway("127.0.0.1", 4000, simulate: true);
            if (request == null) return BadRequest();

            string zeit = DateTime.Now.ToString("HH:mm:ss");
            // Der Text wird so formatiert, dass wir ihn oben im GET wieder zerlegen können
            string logText = $"{request.Command} (X:{request.PosX} Y:{request.PosY} D:{request.Distance})";

            // In DB speichern und an Simulator senden
            _db.SaveRobotAction("Web-Interface", logText, zeit);
            _gateway.Send(logText);

            return Ok(new { ok = true });
        }

        // Die Klasse für die Swagger-Eingabe
        public class CommandRequest
        {
            [JsonPropertyName("command")] public string? Command { get; set; }
            [JsonPropertyName("posX")] public int PosX { get; set; }
            [JsonPropertyName("posY")] public int PosY { get; set; }
            [JsonPropertyName("distance")] public int Distance { get; set; }
        }
    }
}