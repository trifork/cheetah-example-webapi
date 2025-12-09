using System;
using System.Diagnostics;
using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Cheetah.WebApi.Infrastructure.Installers;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Prometheus;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace Cheetah.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateBootstrapLogger();
            try
            {
                IdentityModelEventSource.ShowPII = true;

                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog(
                    (ctx, lc) =>
                    {
                        lc.Enrich.FromLogContext()
                            .Enrich.WithProperty("AppName", ctx.HostingEnvironment.ApplicationName)
                            .Enrich.WithProperty(
                                "Environment",
                                ctx.HostingEnvironment.EnvironmentName
                            )
                            .Enrich.WithProperty(
                                "AssemblyVersion",
                                Assembly.GetEntryAssembly()?.GetName().Version?.ToString()
                                    ?? "1.0.0"
                            )
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                            .MinimumLevel.Override("System", LogEventLevel.Error)
                            .MinimumLevel.Override(
                                "System.Net.Http.HttpClient",
                                LogEventLevel.Warning
                            )
                            .WriteTo.Console();
                        //.WriteTo.Console(new RenderedCompactJsonFormatter());

                        lc.MinimumLevel.Debug();

                        if (Debugger.IsAttached)
                        {
                            Serilog.Debugging.SelfLog.Enable(Console.WriteLine);
                        }
                    },
                    true
                );

                var config = builder.Configuration.AddEnvironmentVariables().Build();

                //Use custom DI installers
                builder.Services.InstallFluentValidation();
                builder.Services.InstallServices(builder.Environment, config);
                builder.Services.InstallOpenAPI();
                builder.Services.InstallKafka(config);
                builder.Services.InstallHealthChecks();

                // Add hosted services
                builder.Services.AddHostedService<TopicSubscriberService>();

                var app = builder.Build();
                var apiVersionDescriptionProvider =
                    app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

                // It's important that the UseSerilogRequestLogging() call appears before handlers such as MVC.
                // The middleware will not time or log components that appear before it in the pipeline.
                // ref: https://github.com/serilog/serilog-aspnetcore
                app.UseSerilogRequestLogging();
                app.UseHttpMetrics(m => m.CaptureMetricsUrl = false);

                // Configure the HTTP request pipeline.
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    // build a swagger endpoint for each discovered API version
                    foreach (
                        var description in apiVersionDescriptionProvider.ApiVersionDescriptions
                    )
                    {
                        c.SwaggerEndpoint(
                            $"/swagger/v{description.ApiVersion}/swagger.json",
                            description.GroupName.ToUpperInvariant()
                        );
                    }
                });

                app.UseCors(corsPolicyBuilder =>
                    corsPolicyBuilder.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin()
                );

                app.MapHealthChecks(
                    "/health",
                    new HealthCheckOptions
                    {
                        Predicate = _ => true,
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                    }
                );
                app.MapHealthChecks(
                    "/health/ready",
                    new HealthCheckOptions() { Predicate = (check) => check.Tags.Contains("ready") }
                );

                app.UseExceptionHandler(
                    app.Environment.IsDevelopment() ? "/error-development" : "/error"
                );

                app.UseRouting();

                app.UseAuthorization();
                app.MapControllers();

#pragma warning disable ASP0014
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapMetrics();
                });
#pragma warning restore ASP0014
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Web host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
