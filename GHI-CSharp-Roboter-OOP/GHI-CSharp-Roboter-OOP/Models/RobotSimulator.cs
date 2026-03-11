using System;
using System.Collections.Generic;
using System.Threading;

namespace GHI_CSharp_Roboter_OOP.Models
{
    // Simuliert das Verhalten des Roboters für Test- und Entwicklungszwecke
    public class RobotSimulator
    {
        private bool _connected = false;
        private static readonly string[] Commands = { "FORWARD", "BACKWARD", "LEFT", "RIGHT", "STOP", "HEARTBEAT" };
        private static readonly string[] Categories = { "Flur", "Hindernis", "Raum", "Korridor" };
        private Random _rnd = new Random();

        public bool Connect()
        {
            _connected = true;
            RobotLogger.Log("[SIM] Verbindung zur Simulation aufgebaut.", "INFO");
            return true;
        }

        public void Disconnect()
        {
            _connected = false;
            RobotLogger.Log("[SIM] Verbindung zur Simulation getrennt.", "INFO");
        }

        public bool SendCommand(string command)
        {
            if (!_connected)
            {
                RobotLogger.Log($"[SIM] Konnte Befehl '{command}' nicht senden: Nicht verbunden.", "WARN");
                return false;
            }
            // Simuliere Verarbeitung und Testdaten-Generierung
            int distance = _rnd.Next(10, 200); // Zufällige Distanz
            string category = Categories[_rnd.Next(Categories.Length)];
            var entry = new LearningDataEntry
            {
                Timestamp = DateTime.Now,
                Command = command,
                Distance = distance,
                Category = category
            };
            LearningDataLogger.Log(entry);
            RobotLogger.Log($"[SIM] Befehl simuliert: {command} | Distanz: {distance} | Kategorie: {category}", "INFO");
            return true;
        }
    }
}
