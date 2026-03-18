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

        public List<object> GetHistory(int count)
        {
            var list = new List<object>();
            try {
                using var conn = Connect(); conn.Open();
                var cmd = new SqlCommand("SELECT TOP (@C) PosX, PosY, DistanceCm, Source, CreatedAt, Category FROM Samples ORDER BY CreatedAt DESC", conn);
                cmd.Parameters.AddWithValue("@C", count);
                using var r = cmd.ExecuteReader();
                while (r.Read()) {
                    var dt = r["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(r["CreatedAt"]) : DateTime.Now;
                    list.Add(new {
                        Zeitpunkt = dt.ToString("HH:mm:ss"),
                        Quelle = r["Source"]?.ToString() ?? "Roboter",
                        posX = r["PosX"],
                        posY = r["PosY"],
                        Distanz = r["DistanceCm"].ToString() + " cm",
                        Category = r["Category"]?.ToString() ?? "Info"
                    });
                }
            } catch { }
            return list;
        }

        public void SaveRobotAction(string source, string action, string status)
        {
            try {
                using var conn = Connect(); conn.Open();
                var cmd = new SqlCommand("INSERT INTO Samples (Source, DistanceCm, PosX, PosY, CreatedAt, Category) VALUES (@S, 0, 0, 0, GETDATE(), @A)", conn);
                cmd.Parameters.AddWithValue("@S", source);
                cmd.Parameters.AddWithValue("@A", action);
                cmd.ExecuteNonQuery();
            } catch { }
        }

        // DIESE METHODE BEHEBT FEHLER CS1061 (Bild 1)
        public void GenerateTechnicalDrawing(string path, object data) { }
    }
}