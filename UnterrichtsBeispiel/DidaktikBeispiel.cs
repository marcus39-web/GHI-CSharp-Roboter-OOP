// =====================
// Didaktische Beispielstruktur für den Unterricht
// =====================
// Dieses Beispiel zeigt, wie man das Roboterprojekt in einzelne, didaktisch sinnvolle Module gliedert.
// Jeder Abschnitt kann im Unterricht einzeln behandelt und erweitert werden.

namespace UnterrichtsBeispiel
{
    // =====================
    // 1. Aufbau & Inbetriebnahme (Simulation)
    // =====================
    // Im Simulationsmodus ist keine Hardware nötig.

    // =====================
    // 2. Motorsteuerung (Simulation)
    // =====================
    public class MotorController
    {
        private bool simulate = true; // Simulationsmodus aktiv

        // Fahre vorwärts
        public void DriveForward()
        {
            if (simulate)
            {
                // Simulation: Logik für Vorwärtsfahrt
                Console.WriteLine("Simulation: Roboter fährt vorwärts.");
            }
            else
            {
                // Hier käme die echte Motoransteuerung
            }
        }

        // Stoppe Roboter
        public void Stop()
        {
            if (simulate)
            {
                Console.WriteLine("Simulation: Roboter stoppt.");
            }
            else
            {
                // Echte Motorsteuerung
            }
        }
    }

    // =====================
    // 3. Sensorik (Simulation)
    // =====================
    public class SensorModule
    {
        private bool simulate = true;

        // Simuliere Abstandssensor
        public int GetDistance()
        {
            if (simulate)
            {
                // Zufallswert für Simulation
                return new Random().Next(10, 100);
            }
            else
            {
                // Echten Sensor auslesen
                return 0;
            }
        }
    }

    // =====================
    // 4. Einfache Fahrbefehle (Simulation)
    // =====================
    public class Robot
    {
        private MotorController motor = new MotorController();
        private SensorModule sensor = new SensorModule();

        // Beispiel: Fahre bis Hindernis erkannt
        public void DriveUntilObstacle()
        {
            while (sensor.GetDistance() > 20)
            {
                motor.DriveForward();
                // Warte kurz (Simulation)
                System.Threading.Thread.Sleep(500);
            }
            motor.Stop();
            Console.WriteLine("Hindernis erkannt, Roboter stoppt.");
        }
    }

    // =====================
    // 5. Linienfolgen, Hinderniserkennung (Simulation)
    // =====================
    // Hier kann ein weiteres Simulationsmodul für Linien- oder Hinderniserkennung ergänzt werden.

    // =====================
    // 6. Erweiterung: KI/Simulation
    // =====================
    // Die KI-Logik kann als separates Modul eingebunden werden (z.B. ML.NET, Zufallsentscheidungen, etc.)

    // =====================
    // Hauptprogramm (Simulation starten)
    // =====================
    public class Program
    {
        public static void Main(string[] args)
        {
            // Beispiel: Roboter fährt bis Hindernis erkannt
            Robot roboter = new Robot();
            roboter.DriveUntilObstacle();
        }
    }
}
