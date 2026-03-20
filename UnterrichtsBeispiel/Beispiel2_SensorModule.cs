// =====================
// 2. SensorModule Beispiel
// =====================
// Aufgabe: Implementiere ein SensorModule mit Abstandssensor und Liniensensor.

namespace UnterrichtsBeispiel
{
    public class SensorModule
    {
        private bool simulate = true;

        // Simuliere Abstandssensor
        public int GetDistance()
        {
            if (simulate)
                return new Random().Next(10, 100);
            else
                return 0; // Echten Sensor auslesen
        }

        // Simuliere Liniensensor
        public bool IsOnLine()
        {
            if (simulate)
                return new Random().Next(0, 2) == 1;
            else
                return false; // Echten Sensor abfragen
        }
    }
}
