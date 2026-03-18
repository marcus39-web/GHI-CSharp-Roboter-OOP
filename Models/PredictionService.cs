using System;

namespace GHI_CSharp_Roboter_OOP.Models
{
    public class PredictionService
    {
        private static readonly string[] Categories = { "Flur", "Hindernis", "Raum", "Korridor" };
        private static readonly Random Rnd = new Random();

        // Später kann hier ein echtes ML.NET-Modell geladen werden
        public string Predict(string? command, int? distance)
        {
            // Simulationsmodus: Zufällige Kategorie
            // TODO: ML.NET-Modell hier einbinden
            return Categories[Rnd.Next(Categories.Length)];
        }
    }
}
