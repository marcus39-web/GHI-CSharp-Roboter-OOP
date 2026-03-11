using Xunit;
using GHI_CSharp_Roboter_OOP.Models;
using System.Linq;

namespace GHI_CSharp_Roboter_OOP.Tests
{
    /// <summary>
    /// Testklasse für die Datenbank-Komponente zur Kategorisierung von Roboteraktionen.
    /// Zeigt, wie Aktionen gespeichert und die Historie abgerufen werden kann.
    /// Ideal für Schulungs- und Demonstrationszwecke.
    /// </summary>
    public class CategorizationDatabaseTests
    {
        /// <summary>
        /// Testet das Speichern einer Roboteraktion und das anschließende Abrufen der Historie.
        /// Erwartet, dass die gespeicherte Aktion in der Historie enthalten ist.
        /// </summary>
        [Fact]
        public void Can_Save_And_Get_History()
        {
            // Arrange: Erzeuge eine neue Instanz der Datenbank (in-memory oder Test-DB)
            var db = new CategorizationDatabase();

            // Act: Speichere eine Beispielaktion (z.B. Befehl "FORWARD" von "TestUser")
            db.SaveRobotAction("TestUser", "FORWARD", "Success");

            // Abrufen der letzten 10 Aktionen aus der Historie
            var history = db.GetHistory(10).ToList();

            // Assert: Die Historie darf nicht leer sein
            Assert.NotEmpty(history);

            // Assert: Die gespeicherte Aktion muss in der Historie enthalten sein
            Assert.Contains(history, h => h.Command == "FORWARD");
        }
    }
}
