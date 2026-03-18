using GHI_CSharp_Roboter_OOP.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// 1. CORS - Erlaubt dem Browser den Zugriff
builder.Services.AddCors(o => o.AddPolicy("All", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// 2. Datenbank registrieren
builder.Services.AddSingleton<CategorizationDatabase>();

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(o => o.SerializerOptions.PropertyNamingPolicy = null);

var app = builder.Build();

app.UseCors("All");

// 3. Statische Dateien (index.html laden)
var def = new DefaultFilesOptions();
def.DefaultFileNames.Clear();
def.DefaultFileNames.Add("web_control/index.html");
app.UseDefaultFiles(def);
app.UseStaticFiles();

// 4. API Endpunkte - PFADE AN FRONTEND ANGEPASST
// Der Browser sucht laut Log nach "/api/webcontrol/history"
app.MapGet("/api/webcontrol/history", (CategorizationDatabase db) => {
    var data = db.GetHistory(100);
    // Wir geben die Daten ganz direkt zurück
    return Results.Json(data, contentType: "application/json");
});

// Falls du auch Befehle sendest, passen wir den Pfad hier vorsorglich auch an
app.MapPost("/api/webcontrol/command", (CategorizationDatabase db, ControlCommand cmd) => {
    db.SaveRobotAction("Web-UI", $"{cmd.Direction} {cmd.Value}", "Aktiv");
    return Results.Ok(new { status = "OK" });
});

app.Run();

public record ControlCommand(string Direction, string Value);