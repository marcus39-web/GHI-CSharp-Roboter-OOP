using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GHI_CSharp_Roboter_OOP.Models;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// 1. Services registrieren
builder.Services.AddControllers(); // Aktiviert deine Controller-Klasse
builder.Services.AddSingleton<CategorizationDatabase>(); // DB als Singleton für alle verfügbar

var app = builder.Build();

// 2. Middleware-Konfiguration
app.UseDefaultFiles(); // Sucht automatisch nach index.html
app.UseStaticFiles();  // Erlaubt Zugriff auf den wwwroot-Ordner

app.UseRouting();
app.UseAuthorization();

// 3. Routen-Mapping
app.MapControllers(); // Registriert [ApiController]-Routen wie /api/WebControl/status

// 4. Fallback-Route für die index.html (behebt deinen Pfad-Fehler)
app.MapGet("/", async (context) =>
{
    // Wir suchen die Datei an verschiedenen möglichen Orten
    string[] searchPaths = {
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "web_control", "index.html"),
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "web_control", "index.html")
    };

    string foundPath = "";
    foreach (var path in searchPaths)
    {
        if (File.Exists(path)) { foundPath = path; break; }
    }

    if (!string.IsNullOrEmpty(foundPath))
    {
        context.Response.ContentType = "text/html";
        await context.Response.SendFileAsync(foundPath);
    }
    else
    {
        await context.Response.WriteAsync("FEHLER: index.html wurde in keinem der Pfade gefunden. " +
            "Pruefen Sie, ob der Ordner 'wwwroot/web_control' im Projekt existiert.");
    }
});

app.Run();