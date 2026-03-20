using Microsoft.AspNetCore.Mvc;
using GHI_CSharp_Roboter_OOP.Models;
using System.Data.SqlClient;
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
        public IActionResult GetHistory([FromQuery] string? room = null)
        {
            var rawData = _db.GetHistory(100, room).Cast<dynamic>().ToList();

            var tableData = rawData.Select(h => {
                string cat = h.Category ?? "";
                int x = 0, y = 0, d = 0;

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
                catch { }

                return new
                {
                    Zeitpunkt = h.Zeitpunkt,
                    Quelle = h.Quelle,
                    Category = cat,
                    posX = x,
                    posY = y,
                    Distanz = d,
                    roomName = h.roomName // für das Frontend
                };
            });

            return Ok(tableData);
        }

        [HttpPost("command")]
        public IActionResult Command([FromBody] CommandRequest request)
        {
            if (_gateway == null) _gateway = new RobotGateway("127.0.0.1", 4000, simulate: true);
            if (request == null) return BadRequest();

            string logText = $"{request.Command} (X:{request.PosX} Y:{request.PosY} D:{request.Distance})";
            _db.SaveRobotAction("Web-Interface", logText, request.Distance, request.PosX, request.PosY, request.RoomName);
            _gateway.Send(logText);

            return Ok(new { ok = true });
        }

        // --- NEU: KI-Vorhersage ---
        [HttpPost("predict")]
        public IActionResult Predict([FromBody] PredictRequest req)
        {
            string ergebnis = "Normalfahrt";

            // Eine kleine Logik für die Demo
            if (req.Distance > 100) ergebnis = "Schnellstraße / Freie Bahn";
            else if (req.Distance < 20) ergebnis = "ACHTUNG: Hindernis nah!";
            else if (req.Command?.ToLower().Contains("porsche") == true) ergebnis = "Sportmodus aktiv";

            return Ok(new { Prediction = ergebnis });
        }

        // --- NEU: JPG Export ---
        [HttpPost("export")]
        public IActionResult Export([FromBody] ExportRequest req)
        {
            // Da wir keine echte Bild-Engine im Controller haben, simulieren wir den Erfolg
            // In einem echten System würde hier System.Drawing oder SkiaSharp ein JPG erstellen
            return Ok(new
            {
                Ok = true,
                DownloadUrl = "https://via.placeholder.com/800x600.png?text=Export+fuer+" + req.RoomName
            });
        }

        // --- NEU: Alle gespeicherten Räume abrufen ---
        [HttpGet("rooms")]
        public IActionResult GetRooms()
        {
            var rooms = new List<string>();
            try
            {
                using var conn = new System.Data.SqlClient.SqlConnection(_db.Host.Contains("Data Source") ? _db.Host : $"Server={_db.Host};Database={_db.Database};Integrated Security=True;TrustServerCertificate=True;");
                conn.Open();
                var cmd = new System.Data.SqlClient.SqlCommand("SELECT DISTINCT RoomName FROM Samples WHERE RoomName IS NOT NULL AND RoomName <> '' ORDER BY RoomName", conn);
                using var r = cmd.ExecuteReader();
                while (r.Read())
                {
                    rooms.Add(r["RoomName"].ToString());
                }
            }
            catch { }
            return Ok(rooms);
        }

        // --- Hilfsklassen ---
        public class CommandRequest
        {
            [JsonPropertyName("command")] public string? Command { get; set; }
            [JsonPropertyName("posX")] public int PosX { get; set; }
            [JsonPropertyName("posY")] public int PosY { get; set; }
            [JsonPropertyName("distance")] public int Distance { get; set; }
            [JsonPropertyName("roomName")] public string? RoomName { get; set; }
        }

        public class PredictRequest
        {
            public string? Command { get; set; }
            public int Distance { get; set; }
        }

        public class ExportRequest
        {
            public string? RoomName { get; set; }
            public string? ExportDate { get; set; }
        }
    }
}