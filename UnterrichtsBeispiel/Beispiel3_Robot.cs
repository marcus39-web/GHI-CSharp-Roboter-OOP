// =====================
// 3. Robot Komposition Beispiel
// =====================
// Aufgabe: Kombiniere MotorController und SensorModule zu einer Robot-Klasse.

namespace UnterrichtsBeispiel
{
    public class Robot
    {
        private MotorController motor = new MotorController();
        private SensorModule sensor = new SensorModule();

        // Fahre bis Hindernis erkannt
        public void DriveUntilObstacle()
        {
            while (sensor.GetDistance() > 20)
            {
                motor.DriveForward();
                System.Threading.Thread.Sleep(500);
            }
            motor.Stop();
            Console.WriteLine("Hindernis erkannt, Roboter stoppt.");
        }

        // Fahre Quadrat
        public void DriveSquare()
        {
            for (int i = 0; i < 4; i++)
            {
                motor.DriveForward();
                System.Threading.Thread.Sleep(1000);
                motor.Stop();
                Console.WriteLine("Simulation: Roboter dreht 90 Grad.");
            }
        }
    }
}
