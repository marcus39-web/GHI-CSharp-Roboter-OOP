using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace GHI_CSharp_Roboter_OOP.Models
{
    public class CategorizationDatabase
    {
        public string Host { get; set; } = "(localdb)\\MSSQLLocalDB";
        public string Database { get; set; } = "GHI-CSharp-Roboter-OOP";

        private SqlConnection Connect() => new SqlConnection($"Server={Host};Database={Database};Integrated Security=True;TrustServerCertificate=True;");


        public List<object> GetHistory(int count, string? roomName = null)
        {
            var list = new List<object>();
            try {
                using var conn = Connect(); conn.Open();
                string sql = "SELECT TOP (@C) PosX, PosY, DistanceCm, Source, CreatedAt, Category, RoomName FROM Samples ";
                if (!string.IsNullOrEmpty(roomName))
                    sql += "WHERE RoomName = @RoomName ";
                sql += "ORDER BY CreatedAt DESC";
                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@C", count);
                if (!string.IsNullOrEmpty(roomName))
                    cmd.Parameters.AddWithValue("@RoomName", roomName);
                using var r = cmd.ExecuteReader();
                while (r.Read()) {
                    var dt = r["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(r["CreatedAt"]) : DateTime.Now;
                    list.Add(new {
                        Zeitpunkt = dt.ToString("HH:mm:ss"),
                        Quelle = r["Source"]?.ToString() ?? "Roboter",
                        posX = r["PosX"],
                        posY = r["PosY"],
                        Distanz = r["DistanceCm"].ToString() + " cm",
                        Category = r["Category"]?.ToString() ?? "Info",
                        roomName = r["RoomName"]?.ToString() ?? ""
                    });
                }
            } catch { }
            return list;
        }

        public void SaveRobotAction(string source, string action, int distance, int posX, int posY, string? roomName = null)
        {
            try
            {
                using var conn = Connect();
                conn.Open();

                var sql = @"INSERT INTO Samples 
                    (Source, DistanceCm, SafeDistanceCm, PosX, PosY, CreatedAt, Category, RawPayload, RoomName) 
                    VALUES 
                    (@S, @D, 0, @X, @Y, GETDATE(), @A, '{}', @RoomName)";

                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@S", source);
                cmd.Parameters.AddWithValue("@A", action);
                cmd.Parameters.AddWithValue("@D", distance);
                cmd.Parameters.AddWithValue("@X", posX);
                cmd.Parameters.AddWithValue("@Y", posY);
                cmd.Parameters.AddWithValue("@RoomName", (object?)roomName ?? DBNull.Value);

                cmd.ExecuteNonQuery();
                Console.WriteLine($"[SQL-SUCCESS] Gespeichert: {action} (Raum: {roomName})");
            }
            catch (Exception ex)
            {
                Console.WriteLine("!!! SQL-FEHLER: " + ex.Message);
                // Wir werfen keinen Fehler mehr, damit die API nicht abstürzt, 
                // aber wir sehen den Grund in der Konsole.
            }
        }


        public void GenerateTechnicalDrawing(string path, object data) { }
    }
}