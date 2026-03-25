using Microsoft.AspNetCore.Mvc;
using GHI_CSharp_Roboter_OOP.Models;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
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

        // --- 1. Verlaufsdaten für Karte und Tabelle ---
        [HttpGet("history")]
        public IActionResult GetHistory([FromQuery] string? room = null)
        {
            // Holt die Daten (71 Zeilen laut DB-Stand) aus der Datenbank
            var rawData = _db.GetHistory(100, room);

            // Mappt die Daten sauber auf die Felder, die die index.html erwartet
            var formattedData = rawData.Select(h => {
                // Wir nutzen dynamisches Mapping, um auf die Keys der anonymen Objekte zuzugreifen
                var dict = h.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(h));

                return new
                {
                    zeitpunkt = dict.GetValueOrDefault("Zeitpunkt")?.ToString() ?? "",
                    quelle = dict.GetValueOrDefault("Quelle")?.ToString() ?? "System",
                    // Nur X und Y Achsen für das 1000mm Raster
                    posX = dict.GetValueOrDefault("posX") ?? 0,
                    posY = dict.GetValueOrDefault("posY") ?? 0,
                    distanz = dict.GetValueOrDefault("Distanz")?.ToString() ?? "0 cm",
                    category = dict.GetValueOrDefault("Category")?.ToString() ?? "Info",
                    roomName = dict.GetValueOrDefault("roomName")?.ToString() ?? ""
                };
            });

            return Ok(formattedData);
        }

        // --- 2. Dynamische Raumliste für das Dropdown ---
        [HttpGet("rooms")]
        public IActionResult GetRooms()
        {
            var rooms = new List<string>();
            try
            {
                // Nutzt die Verbindungseinstellungen deiner Datenbank-Klasse
                using var conn = new System.Data.SqlClient.SqlConnection(
                    $"Server={_db.Host};Database={_db.Database};Integrated Security=True;TrustServerCertificate=True;");
                conn.Open();

                // Holt alle existierenden Raumnamen für dein Menü
                var cmd = new System.Data.SqlClient.SqlCommand(
                    "SELECT DISTINCT RoomName FROM Samples WHERE RoomName IS NOT NULL AND RoomName <> '' ORDER BY RoomName", conn);

                using var r = cmd.ExecuteReader();
                while (r.Read())
                {
                    rooms.Add(r["RoomName"].ToString() ?? "");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fehler beim Laden der Räume: " + ex.Message);
            }
            return Ok(rooms);
        }

        // --- 3. Steuerungsbefehle ---
        [HttpPost("command")]
        public IActionResult Command([FromBody] CommandRequest request)
        {
            if (_gateway == null) _gateway = new RobotGateway("127.0.0.1", 4000, simulate: true);
            if (request == null) return BadRequest();

            string logText = $"{request.Command} (X:{request.PosX} Y:{request.PosY})";
            _db.SaveRobotAction("Web-Interface", logText, request.Distance, request.PosX, request.PosY, request.RoomName);
            _gateway.Send(logText);

            return Ok(new { ok = true });
        }

        // --- 4. KI-Vorhersage ---
        [HttpPost("predict")]
        public IActionResult Predict([FromBody] PredictRequest req)
        {
            string ergebnis = "Normalfahrt";
            if (req.Distance > 100) ergebnis = "Freie Bahn";
            else if (req.Distance < 20) ergebnis = "Hindernis nah!";

            return Ok(new { Prediction = ergebnis });
        }

        // --- 5. JPG Export Simulation ---
        [HttpPost("export")]
        public IActionResult Export([FromBody] ExportRequest req)
        {
            return Ok(new
            {
                Ok = true,
                DownloadUrl = "https://via.placeholder.com/800x600.png?text=Bericht+" + req.RoomName
            });
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
        }
    }
}