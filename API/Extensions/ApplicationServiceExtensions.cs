using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Filters;
using API.Interfaces;
using API.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddMvcCore(options =>
            {
                options.Filters.Add(typeof(ValidateModelAttribute));
                //options.Filters.Add(new ValidateAntiForgeryTokenAttribute());
            });

            services.AddMvc(setup => { }).AddFluentValidation();

            // ADD CONTROLLERS
            // This method configures the MVC services for the commonly used features with controllers for an API.
            services.AddControllers(options =>
            {
                services.BuildServiceProvider()
                                .GetService<IStringLocalizerFactory>();

            }).AddDataAnnotationsLocalization()
            .AddNewtonsoftJson(setUpAction =>
            {
                setUpAction.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                // CONTRACT RESOLVER
                // CamelCasePropertyNamesContractResolver - this is to allow for serializing/deserializing camelcase properties
                setUpAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                setUpAction.SerializerSettings.Formatting = Formatting.Indented;
            });
           

            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseNpgsql(config.GetConnectionString("DefaultConn"));
            });

            services.AddCors(policy =>
            {
                policy.AddPolicy("MyPolicy", cors =>
                {
                    cors.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200", "http://localhost:4200");
                });
            });

            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IUserRepository, UserRepository>();
            
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            return services;
        }

    }
}