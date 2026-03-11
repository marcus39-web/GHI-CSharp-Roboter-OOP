using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace GHI_CSharp_Roboter_OOP
{
    public class MainEntry
    {
        // Diese Klasse entspricht der main.py und dient als Einstiegspunkt für spezielle Logik
        // In ASP.NET Core übernimmt Program.cs den Webserver-Start
        // Hier können zusätzliche Initialisierungen oder Kommandozeilen-Logik ergänzt werden
        public static void Run(string[] args)
        {
            // Beispiel: Kommandozeilenparameter auswerten (falls benötigt)
            // In ASP.NET Core werden die meisten Einstellungen in Program.cs/Startup.cs vorgenommen
            Console.WriteLine("MainEntry gestartet. Zusätzliche Logik kann hier ergänzt werden.");
        }
    }
}
