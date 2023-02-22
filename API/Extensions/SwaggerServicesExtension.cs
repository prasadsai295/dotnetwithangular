using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using API.Filters;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;

namespace API.Extensions
{
    public static class SwaggerServicesExtension
    {
        public static IServiceCollection AddSwaggerExtensionServices(this IServiceCollection services, IConfiguration config){

            services.AddApiVersioning();

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
            services.AddSwaggerGen(options =>
            {
                //options.SwaggerDoc("v1", new OpenApiInfo
                //{
                //    Version = "v1",
                //    Title = "Loyalty Configuration Services",
                //});
                options.OperationFilter<SwaggerDefaultValues>();

                // ADD SECURITY DEFINITION & ADD SECURITY REQUIREMENT
                // These 2 extensions enable the Swagger UI to include the Authorization header in its requests
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Added \"Bearer\" + [space].\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });

                // For basic functionality, above is all that's required, however for summary, remarks, and other comment
                // based swagger and xml file must be generated (in the csproj, the GenerateDocumentationFile tag = true)
                var currentAssembly = Assembly.GetExecutingAssembly();
                var xmlDocs = currentAssembly.GetReferencedAssemblies()
                .Union(new AssemblyName[] { currentAssembly.GetName() })
                .Select(a => Path.Combine(Path.GetDirectoryName(currentAssembly.Location), $"{a.Name}.xml"))
                .Where(f => File.Exists(f)).ToArray();
                Array.ForEach(xmlDocs, (d) =>
                {
                    options.IncludeXmlComments(d);
                });
            });

            return services;
        }

        public static WebApplication UseSwaggerExtensionServices(this WebApplication app){
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

            // SWAGGER
            // Enable middleware to serve generated Swagger as a JSON endpoint but customize the pathbase.
            app.Use((context, next) =>
            {
                context.Request.PathBase = new PathString("/myapp");
                return next();
            }).UseSwagger();

            // SWAGGER UI
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                //c.SwaggerEndpoint("/loyaltyconfiguration/swagger/v1/swagger.json", "Kona Loyalty Configuration Endpoints");
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint($"/myapp/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });

            return app;
        }
    }
}