// =====================
// 4. Hauptprogramm Beispiel
// =====================
// Aufgabe: Starte die Simulation und nutze die Robot-Klasse.

namespace UnterrichtsBeispiel
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Robot roboter = new Robot();
            roboter.DriveUntilObstacle();
            roboter.DriveSquare();
        }
    }
}
