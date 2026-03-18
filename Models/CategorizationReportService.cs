using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Runtime.Versioning; // Wichtig für die Fehlerbehebung der CA1416 Warnung

namespace GHI_CSharp_Roboter_OOP.Models
{
    public class CategorizationReportService
    {
        private readonly string _exportDir;

        public CategorizationReportService()
        {
            _exportDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "categorization_exports");
            if (!Directory.Exists(_exportDir))
            {
                Directory.CreateDirectory(_exportDir);
            }
        }

        public Dictionary<string, object> BuildSummary(List<Dictionary<string, object>> entries)
        {
            var totals = CalculateTotals(entries);
            var blocks = new Dictionary<string, List<Dictionary<string, object>>>
            {
                { "day", AggregateBlocks(entries, "day") },
                { "week", AggregateBlocks(entries, "week") },
                { "month", AggregateBlocks(entries, "month") },
                { "year", AggregateBlocks(entries, "year") }
            };

            return new Dictionary<string, object>
            {
                { "ok", true },
                { "created_at", DateTime.Now.ToString("o") },
                { "totals", totals },
                { "blocks", blocks },
                { "recent", entries.Take(50).ToList() }
            };
        }

        public Dictionary<string, int> CalculateTotals(List<Dictionary<string, object>> entries)
        {
            var totals = new Dictionary<string, int> { { "total", 0 }, { "OBSTACLE", 0 }, { "CLEAR", 0 }, { "OTHER", 0 } };

            foreach (var entry in entries)
            {
                // Null-Safe Abfrage der Decision
                var decision = entry.GetValueOrDefault("decision", "UNKNOWN")?.ToString()?.ToUpper() ?? "UNKNOWN";
                totals["total"]++;
                if (totals.ContainsKey(decision))
                {
                    totals[decision]++;
                }
                else
                {
                    totals["OTHER"]++;
                }
            }

            return totals;
        }

        public List<Dictionary<string, object>> AggregateBlocks(List<Dictionary<string, object>> entries, string period)
        {
            var grouped = new Dictionary<string, Dictionary<string, object>>();

            foreach (var entry in entries)
            {
                var createdAtVal = entry.GetValueOrDefault("created_at", DateTime.Now.ToString());
                var timestamp = DateTime.Parse(createdAtVal?.ToString() ?? DateTime.Now.ToString());
                var key = GetPeriodKey(timestamp, period);
                var decision = entry.GetValueOrDefault("decision", "UNKNOWN")?.ToString()?.ToUpper() ?? "UNKNOWN";

                if (!grouped.ContainsKey(key))
                {
                    grouped[key] = new Dictionary<string, object> { { "block", key }, { "total", 0 }, { "OBSTACLE", 0 }, { "CLEAR", 0 }, { "OTHER", 0 } };
                }

                grouped[key]["total"] = (int)grouped[key]["total"] + 1;
                if (grouped[key].ContainsKey(decision))
                {
                    grouped[key][decision] = (int)grouped[key][decision] + 1;
                }
                else
                {
                    grouped[key]["OTHER"] = (int)grouped[key]["OTHER"] + 1;
                }
            }

            return grouped.Values.OrderByDescending(g => g["block"]).ToList();
        }

        private string GetPeriodKey(DateTime timestamp, string period)
        {
            return period switch
            {
                "day" => timestamp.ToString("yyyy-MM-dd"),
                "week" => $"{timestamp.Year}-W{GetIso8601WeekOfYear(timestamp):D2}",
                "month" => timestamp.ToString("yyyy-MM"),
                "year" => timestamp.ToString("yyyy"),
                _ => timestamp.ToString("yyyy-MM-dd")
            };
        }

        private int GetIso8601WeekOfYear(DateTime time)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        // Löst die gelben CA1416 Warnungen (Plattformkompatibilität)
        [SupportedOSPlatform("windows")]
        public (bool, string) ExportJpg(string roomName, Dictionary<string, object> geometry, string period, List<Dictionary<string, object>> entries)
        {
            try
            {
                var summary = BuildSummary(entries);
                var allBlocks = (Dictionary<string, List<Dictionary<string, object>>>)summary["blocks"];

                var imageWidth = 1400;
                var imageHeight = 900;

                using var bitmap = new Bitmap(imageWidth, imageHeight);
                using var graphics = Graphics.FromImage(bitmap);
                graphics.Clear(Color.White);

                // 'using' stellt sicher, dass die Schriftart korrekt entladen wird
                using var titleFont = new Font("Arial", 16, FontStyle.Bold);
                using var textFont = new Font("Arial", 12);

                graphics.DrawString("BrainBot Kategorisierung - Report", titleFont, Brushes.Black, new PointF(40, 30));
                graphics.DrawString($"Erstellt am: {DateTime.Now:O}", textFont, Brushes.Black, new PointF(40, 70));
                graphics.DrawString($"Zimmer: {roomName}", textFont, Brushes.Black, new PointF(40, 100));

                var filename = $"cat_report_{roomName}_{period}_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                var outputPath = Path.Combine(_exportDir, filename);

                bitmap.Save(outputPath, ImageFormat.Jpeg);

                return (true, filename);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}