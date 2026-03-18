using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace GHI_CSharp_Roboter_OOP.Models
{
    public class MapSimulation
    {
        private readonly object _lock = new object();
        private readonly string _snapshotDir;
        public int Width { get; private set; } = 900;
        public int Height { get; private set; } = 600;
        public bool Running { get; private set; } = true;
        public int Tick { get; private set; } = 0;

        // FIX CS8618: Initialisierung verhindert die Warnung
        public List<Dictionary<string, object>> Obstacles { get; private set; } = new();
        public List<Dictionary<string, object>> Robots { get; private set; } = new();

        public MapSimulation()
        {
            _snapshotDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "map_snapshots");
            if (!Directory.Exists(_snapshotDir))
            {
                Directory.CreateDirectory(_snapshotDir);
            }
            InitializeWorld();
        }

        private void InitializeWorld()
        {
            Obstacles = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object> { { "x", 220 }, { "y", 120 }, { "w", 140 }, { "h", 90 } },
                new Dictionary<string, object> { { "x", 520 }, { "y", 200 }, { "w", 180 }, { "h", 80 } },
                new Dictionary<string, object> { { "x", 300 }, { "y", 390 }, { "w", 120 }, { "h", 120 } }
            };

            Robots = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object> { { "id", "R1" }, { "color", "#4dabf7" }, { "x", 80.0 }, { "y", 80.0 }, { "heading", 0.0 }, { "speed", 2.4 }, { "trail", new List<List<double>>() } },
                new Dictionary<string, object> { { "id", "R2" }, { "color", "#69db7c" }, { "x", 780.0 }, { "y", 100.0 }, { "heading", Math.PI }, { "speed", 2.0 }, { "trail", new List<List<double>>() } },
                new Dictionary<string, object> { { "id", "R3" }, { "color", "#ffd43b" }, { "x", 450.0 }, { "y", 520.0 }, { "heading", -Math.PI / 2 }, { "speed", 2.2 }, { "trail", new List<List<double>>() } }
            };
        }

        public void SetRunning(bool running)
        {
            lock (_lock)
            {
                Running = running;
            }
        }

        public void Reset()
        {
            lock (_lock)
            {
                Tick = 0;
                Running = true;
                InitializeWorld();
            }
        }

        public void Step()
        {
            lock (_lock)
            {
                StepUnlocked();
            }
        }

        private void StepUnlocked()
        {
            Tick++;

            foreach (var robot in Robots)
            {
                double heading = Convert.ToDouble(robot["heading"]);
                double speed = Convert.ToDouble(robot["speed"]);
                double nx = Convert.ToDouble(robot["x"]) + Math.Cos(heading) * speed;
                double ny = Convert.ToDouble(robot["y"]) + Math.Sin(heading) * speed;

                if (nx < 20 || nx > Width - 20)
                {
                    heading = Math.PI - heading;
                    nx = Math.Max(20, Math.Min(Width - 20, nx));
                }
                if (ny < 20 || ny > Height - 20)
                {
                    heading = -heading;
                    ny = Math.Max(20, Math.Min(Height - 20, ny));
                }

                robot["x"] = nx;
                robot["y"] = ny;
                robot["heading"] = heading;

                if (robot["trail"] is List<List<double>> trail)
                {
                    trail.Add(new List<double> { Math.Round(nx, 2), Math.Round(ny, 2) });
                    if (trail.Count > 180)
                    {
                        trail.RemoveRange(0, trail.Count - 180);
                    }
                }
            }
        }

        public Dictionary<string, object> GetState()
        {
            lock (_lock)
            {
                if (Running)
                {
                    StepUnlocked();
                }

                return new Dictionary<string, object>
                {
                    { "ok", true },
                    { "timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
                    { "tick", Tick },
                    { "running", Running },
                    { "map", new Dictionary<string, object> { { "width", Width }, { "height", Height }, { "obstacles", Obstacles } } },
                    { "robots", Robots }
                };
            }
        }

        public List<Dictionary<string, object>> ListSnapshots()
        {
            return Directory.GetFiles(_snapshotDir, "*.json")
                .Select(file => new FileInfo(file))
                .OrderByDescending(info => info.LastWriteTime)
                .Select(info => new Dictionary<string, object>
                {
                    { "filename", info.Name },
                    { "modified", new DateTimeOffset(info.LastWriteTime).ToUnixTimeSeconds() },
                    { "size", info.Length }
                })
                .ToList();
        }

        // --- HIER WAR DAS PROBLEM: Methode SaveSnapshot vervollständigt ---
        public (bool ok, string message, string? filename) SaveSnapshot(string? name = null)
        {
            lock (_lock)
            {
                string cleanName = string.IsNullOrWhiteSpace(name) ? "" : name.Trim().Replace(" ", "_");
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string filename = string.IsNullOrWhiteSpace(cleanName) ? $"map_snapshot_{timestamp}.json" : $"map_{cleanName}_{timestamp}.json";

                string safeFilename = new string(filename.Where(c => char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == '.').ToArray());
                string targetPath = Path.Combine(_snapshotDir, safeFilename);

                var payload = new
                {
                    version = 1,
                    saved_at = DateTime.Now.ToString("o"),
                    tick = Tick,
                    running = Running,
                    map = new { width = Width, height = Height, obstacles = Obstacles },
                    robots = Robots
                };

                try
                {
                    File.WriteAllText(targetPath, JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true }));
                    return (true, $"Snapshot gespeichert: {safeFilename}", safeFilename);
                }
                catch (Exception)
                {
                    return (false, "Snapshot konnte nicht gespeichert werden", null);
                }
            }
        }
    } // Ende Klasse
} // Ende Namespace