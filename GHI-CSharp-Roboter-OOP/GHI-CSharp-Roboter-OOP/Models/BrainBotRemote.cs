using System;
using System.Net.Sockets;
using System.Text;

namespace GHI_CSharp_Roboter_OOP.Models
{
    // Platzhalter für die Übersetzung von BrainBotRemote aus basis_class.py
    public class BrainBotRemote
    {
        private readonly string _robotIp;
        private readonly int _robotPort;
        private readonly bool _simulate;
        private TcpClient? _client;
        private NetworkStream? _stream;
        private static readonly string[] SimCategories = { "Flur", "Hindernis", "Raum", "Korridor" };
        private static readonly Random SimRnd = new Random();

        public BrainBotRemote(string robotIp, int robotPort, bool simulate = false)
        {
            _robotIp = robotIp;
            _robotPort = robotPort;
            _simulate = simulate;
        }

        public bool Connect()
        {
            if (_simulate)
            {
                RobotLogger.Log($"[SIM] Verbindung zur Simulation aufgebaut.", "INFO");
                return true;
            }
            try
            {
                _client = new TcpClient();
                _client.Connect(_robotIp, _robotPort);
                _stream = _client.GetStream();
                RobotLogger.Log($"TCP-Verbindung aufgebaut zu {_robotIp}:{_robotPort}", "INFO");
                return true;
            }
            catch (Exception ex)
            {
                RobotLogger.Log($"Fehler beim Verbindungsaufbau zu {_robotIp}:{_robotPort}: {ex.Message}", "ERROR");
                Disconnect();
                return false;
            }
        }

        public void Disconnect()
        {
            if (_simulate)
            {
                RobotLogger.Log($"[SIM] Verbindung zur Simulation getrennt.", "INFO");
                return;
            }
            try { _stream?.Close(); } catch { }
            try { _client?.Close(); } catch { }
            _stream = null;
            _client = null;
            RobotLogger.Log($"TCP-Verbindung zu {_robotIp}:{_robotPort} getrennt.", "INFO");
        }

        public bool SendCommand(string command)
        {
            if (_simulate)
            {
                // Simuliere Verarbeitung und Testdaten-Generierung
                int distance = SimRnd.Next(10, 200); // Zufällige Distanz
                string category = SimCategories[SimRnd.Next(SimCategories.Length)];
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
            if (_stream == null)
            {
                RobotLogger.Log($"Konnte Befehl '{command}' nicht senden: Kein aktiver Stream.", "WARN");
                return false;
            }
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(command + "\n");
                _stream.Write(data, 0, data.Length);
                RobotLogger.Log($"Befehl an Roboter gesendet: {command}", "INFO");
                // Optional: Antwort lesen, falls Protokoll das vorsieht
                return true;
            }
            catch (Exception ex)
            {
                RobotLogger.Log($"Fehler beim Senden von '{command}': {ex.Message}", "ERROR");
                Disconnect();
                return false;
            }
        }
    }
}
