using System;

namespace GHI_CSharp_Roboter_OOP.Models
{
    public class PredictionService
    {
        private static readonly string[] Categories = { "Flur", "Hindernis", "Raum", "Korridor" };
        private static readonly Random Rnd = new Random();


        public string Predict(string? command, int? distance)
        {         
            return Categories[Rnd.Next(Categories.Length)];
        }
    }
}
