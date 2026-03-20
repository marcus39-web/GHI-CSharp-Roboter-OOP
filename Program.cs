using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.IO;
using GHI_CSharp_Roboter_OOP.Models; // Wichtig für die Database

var builder = WebApplication.CreateBuilder(args);

// --- SERVICES REGISTRIEREN ---
// WICHTIG: Erlaubt dem Programm, deine Controller-Dateien (WebControlController.cs) zu finden
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options => options.AddPolicy("All", p =>
    p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// Deine Datenbank als Singleton
builder.Services.AddSingleton<CategorizationDatabase>();

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
    options.SerializerOptions.PropertyNamingPolicy = null);

var app = builder.Build();

// --- MIDDLEWARE CONFIG ---
app.UseSwagger();
app.UseSwaggerUI(options => {
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "BrainBot API v1");
    options.RoutePrefix = "swagger";
});

app.UseCors("All");

// Statische Dateien (Frontend)
app.UseDefaultFiles(new DefaultFilesOptions { DefaultFileNames = { "web_control/index.html" } });
app.UseStaticFiles();

// --- ROUTING ---
// WICHTIG: Leitet Anfragen wie /api/webcontrol/command an deinen Controller weiter
app.MapControllers();

// Hinweis: Die alten app.MapGet und app.MapPost Blöcke wurden entfernt, 
// damit dein WebControlController nicht blockiert wird.

app.Run();