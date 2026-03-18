using System;
using System.Threading;

namespace GHI_CSharp_Roboter_OOP.Models
{
    // Thread-sichere Brücke zwischen Web-API und BrainBotRemote
    public class RobotGateway
    {
        private readonly string _robotIp;
        private readonly int _robotPort;
        private readonly BrainBotRemote _robot;
        private readonly object _lock = new object();
        public bool Connected { get; private set; }

        private Timer? _heartbeatTimer;
        private DateTime _lastHeartbeatResponse = DateTime.Now;
        private readonly TimeSpan _heartbeatInterval = TimeSpan.FromSeconds(2);
        private readonly TimeSpan _heartbeatTimeout = TimeSpan.FromSeconds(5);
        private bool _emergencyTriggered = false;

        /// <summary>
        /// Erstellt ein RobotGateway. Wenn simulate=true, wird der Simulationsmodus verwendet (keine echte Hardware nötig).
        /// </summary>
        public RobotGateway(string robotIp, int robotPort, bool simulate = false)
        {
            _robotIp = robotIp;
            _robotPort = robotPort;
            _robot = new BrainBotRemote(robotIp, robotPort, simulate);
            Connected = false;
            StartHeartbeat();
        }

        private void StartHeartbeat()
        {
            _heartbeatTimer = new Timer(HeartbeatCallback, null, 0, (int)_heartbeatInterval.TotalMilliseconds);
        }

        private void HeartbeatCallback(object? state)
        {
            lock (_lock)
            {
                if (!Connected || _emergencyTriggered)
                    return;
                try
                {
                    bool ok = _robot.SendCommand("HEARTBEAT");
                    if (ok)
                    {
                        _lastHeartbeatResponse = DateTime.Now;
                    }
                    else
                    {
                        // Keine Antwort, prüfe Timeout
                        if (DateTime.Now - _lastHeartbeatResponse > _heartbeatTimeout)
                        {
                            _emergencyTriggered = true;
                            EmergencyStop();
                            LogHeartbeatFailure();
                        }
                    }
                }
                catch
                {
                    if (DateTime.Now - _lastHeartbeatResponse > _heartbeatTimeout)
                    {
                        _emergencyTriggered = true;
                        EmergencyStop();
                        LogHeartbeatFailure();
                    }
                }
            }
        }


        private void Log(string message, string level = "INFO")
        {
            RobotLogger.Log(message, level);
        }

        private void LogHeartbeatFailure()
        {
            Log("HEARTBEAT TIMEOUT: Not-Aus ausgelöst.", "ERROR");
        }

        public (bool, string) Connect()
        {
            lock (_lock)
            {
                if (Connected)
                {
                    Log("Verbindungsversuch: bereits verbunden.", "INFO");
                    return (true, "bereits verbunden");
                }
                Connected = _robot.Connect();
                if (Connected)
                {
                    _lastHeartbeatResponse = DateTime.Now;
                    _emergencyTriggered = false;
                    Log($"Verbunden mit {_robotIp}:{_robotPort}", "INFO");
                    return (true, $"verbunden mit {_robotIp}:{_robotPort}");
                }
                Log($"Verbindung fehlgeschlagen zu {_robotIp}:{_robotPort}", "ERROR");
                return (false, "Verbindung fehlgeschlagen");
            }
        }

        public (bool, string) Disconnect()
        {
            lock (_lock)
            {
                if (!Connected)
                {
                    Log("Trennungsversuch: bereits getrennt.", "INFO");
                    return (true, "bereits getrennt");
                }
                _robot.SendCommand("STOP");
                _robot.Disconnect();
                Connected = false;
                _emergencyTriggered = false;
                Log("Verbindung getrennt.", "INFO");
                return (true, "Verbindung getrennt");
            }
        }

        public (bool, string) Send(string command)
        {
            lock (_lock)
            {
                if (!Connected)
                {
                    Log($"Befehl '{command}' gesendet, aber nicht verbunden.", "WARN");
                    return (false, "nicht verbunden (erst START drücken)");
                }
                bool success = _robot.SendCommand(command);
                // Lernmodus: Jeden Befehl loggen
                var entry = new LearningDataEntry
                {
                    Timestamp = DateTime.Now,
                    Command = command
                    // Distance und Category können später ergänzt werden
                };
                LearningDataLogger.Log(entry);
                if (success)
                {
                    Log($"Befehl gesendet: {command}", "INFO");
                    return (true, $"Befehl gesendet: {command}");
                }
                _robot.Disconnect();
                Connected = false;
                Log($"Senden fehlgeschlagen: {command}", "ERROR");
                return (false, $"Senden fehlgeschlagen: {command}");
            }
        }

        public (bool, string) EmergencyStop()
        {
            lock (_lock)
            {
                if (!Connected)
                {
                    Log("Not-Aus bestätigt (bereits getrennt)", "WARN");
                    return (true, "Not-Aus bestätigt (bereits getrennt)");
                }
                bool stopOk = _robot.SendCommand("STOP");
                _robot.Disconnect();
                Connected = false;
                if (stopOk)
                {
                    Log("Not-Aus ausgeführt: STOP gesendet und Verbindung getrennt", "ERROR");
                    return (true, "Not-Aus ausgeführt: STOP gesendet und Verbindung getrennt");
                }
                Log("Not-Aus versucht: Verbindung getrennt, STOP-Bestätigung fehlt", "ERROR");
                return (false, "Not-Aus versucht: Verbindung getrennt, STOP-Bestätigung fehlt");
            }
        }
    }
}
