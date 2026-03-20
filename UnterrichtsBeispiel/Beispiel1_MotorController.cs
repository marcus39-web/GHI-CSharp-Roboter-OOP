// =====================
// 1. MotorController Beispiel
// =====================
// Aufgabe: Implementiere einen einfachen MotorController mit Vorwärts- und Stoppfunktion.

namespace UnterrichtsBeispiel
{
    public class MotorController
    {
        private bool simulate = true;

        // Fahre vorwärts
        public void DriveForward()
        {
            if (simulate)
                Console.WriteLine("Simulation: Roboter fährt vorwärts.");
            else
                /* Echte Motoransteuerung */
                ;
        }

        // Stoppe Roboter
        public void Stop()
        {
            if (simulate)
                Console.WriteLine("Simulation: Roboter stoppt.");
            else
                /* Echte Motorsteuerung */
                ;
        }
    }
}
