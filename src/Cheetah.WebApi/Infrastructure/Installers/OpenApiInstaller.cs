using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Cheetah.WebApi.Shared.Infrastructure.ServiceProvider;
using Cheetah.WebApi.Core.Config;

namespace Cheetah.WebApi.Infrastructure.Installers
{
    [InstallerPriority(Priorities.BeforeConfig)]
    public class OpenApiInstaller : IServiceCollectionInstaller
    {
        public void Install(IServiceCollection services, IHostEnvironment hostEnvironment)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services
                .AddControllers()
                //.ConfigureApiBehaviorOptions(x => { x.SuppressMapClientErrors = true; })
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            services.AddApiVersioning(options =>
                {
                    // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
                    options.ReportApiVersions = true;
                })
                .AddVersionedApiExplorer(
                    options =>
                    {
                        options.GroupNameFormat = "'v'VVV";
                        options.SubstituteApiVersionInUrl = true;
                    });

            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            //  services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            services.AddSwaggerGen(c =>
            {
                var xmlPath = Path.Combine(AppContext.BaseDirectory,
                    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
                c.IncludeXmlComments(xmlPath, true);
            });
        }
    }
}