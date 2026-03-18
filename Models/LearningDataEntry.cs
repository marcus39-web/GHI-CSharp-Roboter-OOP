using System;

namespace GHI_CSharp_Roboter_OOP.Models
{
    public class LearningDataEntry
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Command { get; set; } = string.Empty;
        public int? Distance { get; set; } // Optional, falls Sensordaten vorhanden
        public string? Category { get; set; } // Optional, falls Kategorisierung vorhanden
    }
}
