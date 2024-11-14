using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using SchoolApi.Core.Business.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SchoolApi;

namespace SchoolApi.Core.Business.CommonConfig
{
   public static class ConfigurationExtensions
    {
        private static string apiTitle;

        // Add common services: FluentValidation, CORS, Swagger, etc.

        public static void AddCommonServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add FluentValidation
            services.AddFluentValidationAutoValidation(fv => fv.DisableDataAnnotationsValidation = true);
            //services.AddValidatorsFromAssemblyContaining<Program>();  

            // Add CORS policy for all origins (AllowAny)
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
            
            // Add Logging (Serilog configuration)
            //services.AddLogging();
        }
         public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;

                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = configuration["Jwt:Audience"],
                        ValidIssuer = configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                        ValidateIssuerSigningKey = true
                    };
                });
        }

        // Add Swagger configuration for OpenAPI docs

    public static void AddSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(opt =>
        {
            // Retrieve Swagger settings from appsettings.json
            var swaggerConfig = configuration.GetSection("Swagger");
            var apiTitle = swaggerConfig["Title"];
            var apiVersion = swaggerConfig["Version"];
            
            // Swagger documentation setup
            opt.SwaggerDoc(apiVersion, new OpenApiInfo { Title = apiTitle, Version = apiVersion });

            // Add security definitions for JWT Bearer authentication
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });

            // Add security requirements for Swagger
            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });

            // Include XML comments for better documentation in Swagger
            opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
        });
    }


        public static void AddExceptionHandling(this IServiceCollection services)
        {
            services.AddExceptionHandler<CustomExceptionHandler>();
        }
    }
}