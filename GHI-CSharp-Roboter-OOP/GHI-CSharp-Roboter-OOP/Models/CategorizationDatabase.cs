using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Drawing;         // NuGet: System.Drawing.Common installieren!
using System.Drawing.Imaging;
using System.Runtime.Versioning; // Erforderlich für [SupportedOSPlatform]

namespace GHI_CSharp_Roboter_OOP.Models
{
    public class CategorizationDatabase
    {
        public string Host { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 1433;
        public string User { get; set; } = "sa";
        public string Password { get; set; } = "";
        public string Database { get; set; } = "BrainBotAI";
        public bool Enabled { get; set; } = true;

        private readonly string? _manualConnectionString;

        public CategorizationDatabase(string? connectionString)
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                this.Enabled = true;
                _manualConnectionString = connectionString;
            }
        }

        public CategorizationDatabase() { }

        private SqlConnection Connect()
        {
            if (!Enabled) throw new InvalidOperationException("MSSQL ist deaktiviert");
            var connStr = _manualConnectionString ?? $"Server={Host},{Port};Database={Database};User Id={User};Password={Password};TrustServerCertificate=True;";
            return new SqlConnection(connStr);
        }

        public (bool, string) Initialize()
        {
            if (!Enabled) return (false, "MSSQL deaktiviert");
            const string SCHEMA_SQL = @"
            IF OBJECT_ID('Samples', 'U') IS NULL
            CREATE TABLE Samples (
                Id BIGINT IDENTITY PRIMARY KEY,
                Source NVARCHAR(128) NOT NULL,
                DistanceCm INT NOT NULL, 
                SafeDistanceCm INT NOT NULL,
                PosX INT DEFAULT 0,
                PosY INT DEFAULT 0,
                RawPayload NVARCHAR(MAX) NULL,
                CreatedAt DATETIME DEFAULT GETDATE()
            );";
            try
            {
                using var connection = Connect();
                connection.Open();
                using var sqlCmd = connection.CreateCommand();
                sqlCmd.CommandText = SCHEMA_SQL;
                sqlCmd.ExecuteNonQuery();
                return (true, "MSSQL erfolgreich initialisiert");
            }
            catch (Exception ex) { return (false, $"Fehler: {ex.Message}"); }
        }

        public void SaveRobotAction(string source, string action, string status, int x = 0, int y = 0)
        {
            if (!Enabled) return;
            try
            {
                using var connection = Connect();
                connection.Open();
                double dist = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
                string sql = "INSERT INTO Samples (Source, DistanceCm, SafeDistanceCm, PosX, PosY, RawPayload) VALUES (@S, @D, 0, @X, @Y, @P)";
                using var cmd = new SqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@S", source);
                cmd.Parameters.AddWithValue("@D", (int)dist);
                cmd.Parameters.AddWithValue("@X", x);
                cmd.Parameters.AddWithValue("@Y", y);
                cmd.Parameters.AddWithValue("@P", JsonConvert.SerializeObject(new { Action = action, Status = status, Time = DateTime.Now }));
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex) { Console.WriteLine($"SQL-Fehler: {ex.Message}"); }
        }

        public List<dynamic> GetHistory(int count)
        {
            var list = new List<dynamic>();
            if (!Enabled) return list;
            try
            {
                using var connection = Connect();
                connection.Open();
                string sql = "SELECT TOP (@C) PosX, PosY, DistanceCm, Source, CreatedAt FROM Samples ORDER BY CreatedAt DESC";
                using var cmd = new SqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@C", count);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new
                    {
                        PosX = Convert.ToInt32(reader["PosX"]),
                        PosY = Convert.ToInt32(reader["PosY"]),
                        DistanceCm = Convert.ToInt32(reader["DistanceCm"]),
                        Source = reader["Source"].ToString(),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                    });
                }
            }
            catch (Exception ex) { Console.WriteLine($"Fehler: {ex.Message}"); }
            return list; // Fix für CS0161
        }

        // Fix für CS1061 & CA1416 (Plattform-Unterstützung explizit angeben)
        [SupportedOSPlatform("windows")]
        public void GenerateTechnicalDrawing(string path, WebControlServer.ExportRequest data)
        {
            using Bitmap bitmap = new Bitmap(800, 600);
            using Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.White);
            Pen pen = new Pen(Color.Black, 2);
            Font font = new Font("Arial", 12);

            g.DrawString("BrainBot - Technischer Bericht v1.0.2", new Font("Arial", 16, FontStyle.Bold), Brushes.Blue, 50, 20);
            g.DrawString($"Zimmer: {data.RoomName}", font, Brushes.Black, 50, 60);

            int w = Math.Min(data.Width * 2, 600);
            int h = Math.Min(data.Depth * 2, 400);
            g.DrawRectangle(pen, 100, 150, w, h);
            g.DrawLine(pen, 100, 135, 100 + w, 135);
            g.DrawString($"{data.Width} cm", font, Brushes.Black, 100 + (w / 2) - 20, 115);

            bitmap.Save(path, ImageFormat.Jpeg);
        }
    }
}