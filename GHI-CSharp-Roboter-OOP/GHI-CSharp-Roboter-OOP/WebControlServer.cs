using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using GHI_CSharp_Roboter_OOP.Models;

namespace GHI_CSharp_Roboter_OOP
{
    public class WebControlServer
    {
        private readonly RobotGateway _gateway;
        private readonly CategorizationDatabase _db;

        public WebControlServer(string robotIp, int robotPort, CategorizationDatabase database)
        {
            // Immer Simulationsmodus aktivieren
            _gateway = new RobotGateway(robotIp, robotPort, simulate: true);
            _db = database;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => {
                    string path1 = Path.Combine(AppContext.BaseDirectory, "wwwroot", "web_control", "index.html");
                    string path2 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "web_control", "index.html");

                    string finalPath = File.Exists(path1) ? path1 : (File.Exists(path2) ? path2 : "");

                    if (!string.IsNullOrEmpty(finalPath))
                    {
                        context.Response.ContentType = "text/html";
                        await context.Response.SendFileAsync(finalPath);
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsync($"FEHLER: index.html wurde nicht gefunden.");
                    }
                });

                endpoints.MapGet("/api/history", async context => {
                    var data = _db.GetHistory(50);
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(data));
                });

                endpoints.MapPost("/api/export", async context => {
                    try
                    {
                        using var reader = new StreamReader(context.Request.Body);
                        var body = await reader.ReadToEndAsync();
                        var req = JsonConvert.DeserializeObject<ExportRequest>(body);

                        string exportDir = Path.Combine(AppContext.BaseDirectory, "wwwroot", "exports");
                        if (!Directory.Exists(exportDir)) Directory.CreateDirectory(exportDir);

                        string fileName = $"Export_{DateTime.Now:yyyyMMdd_HHmm}.jpg";
                        string fullPath = Path.Combine(exportDir, fileName);

                        if (req != null)
                        {
                            // --- FIX FÜR CA1416 WARNUNG ---
#pragma warning disable CA1416
                            _db.GenerateTechnicalDrawing(fullPath, req);
#pragma warning restore CA1416
                        }

                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new { ok = true, downloadUrl = $"/exports/{fileName}" }));
                    }
                    catch (Exception ex)
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new { ok = false, error = ex.Message }));
                    }
                });

                endpoints.MapPost("/api/command", async context => {
                    using var reader = new StreamReader(context.Request.Body);
                    var payload = JsonConvert.DeserializeObject<CommandRequest>(await reader.ReadToEndAsync());
                    var (ok, msg) = _gateway.Send(payload?.Command ?? "STOP");
                    if (ok) _db.SaveRobotAction("WebInterface", payload?.Command ?? "STOP", "Success");
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new { ok, message = msg }));
                });
            });
        }

        public class CommandRequest { public string Command { get; set; } = string.Empty; }

        public class ExportRequest
        {
            public string RoomName { get; set; } = "Unbekannt";
            public int Width { get; set; } = 300;
            public int Depth { get; set; } = 200;
            public int Radius { get; set; }
            public int RecessWidth { get; set; }
            public int RecessDepth { get; set; }
        }
    }
}