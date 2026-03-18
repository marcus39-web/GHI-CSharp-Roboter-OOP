using GHI_CSharp_Roboter_OOP.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// 1. CORS - Erlaubt dem Browser den Zugriff
builder.Services.AddCors(o => o.AddPolicy("All", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// 2. Datenbank registrieren
builder.Services.AddSingleton<CategorizationDatabase>();

// WICHTIG: Verhindert, dass C# die Namen (PosX, DistanceCm) in Kleinbuchstaben umwandelt!
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(o => o.SerializerOptions.PropertyNamingPolicy = null);

var app = builder.Build();
app.UseCors("All");

// 3. Statische Dateien (index.html laden)
var def = new DefaultFilesOptions();
def.DefaultFileNames.Clear();
def.DefaultFileNames.Add("web_control/index.html");
app.UseDefaultFiles(def);
app.UseStaticFiles();

// --- API ENDPUNKTE ---

// Historie für die Tabelle abrufen
app.MapGet("/api/webcontrol/history", (CategorizationDatabase db) => {
    var data = db.GetHistory(100);
    return Results.Json(data, contentType: "application/json");
});

// Befehle senden
app.MapPost("/api/webcontrol/command", (CategorizationDatabase db, ControlCommand cmd) => {
    db.SaveRobotAction("Web-UI", $"{cmd.Direction} {cmd.Value}", "Aktiv");
    return Results.Ok(new { status = "OK" });
});

// NEU: Endpunkt für den JPG-Export (v1.0.2)
// Damit der grüne Button im Frontend endlich eine Antwort bekommt!
app.MapPost("/api/webcontrol/export", () => {
    Console.WriteLine("📸 JPG Export v1.0.2: Befehl vom Frontend empfangen!");
    // Hier simulieren wir den Erfolg für das Frontend
    return Results.Ok(new
    {
        success = true,
        message = "Export erfolgreich im Server-Log registriert!"
    });
});

app.Run();

public record ControlCommand(string Direction, string Value);