
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using SchoolApi.Core.Business.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SchoolApi.Core.Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.Mvc;
using SchoolApi.Core.Business.Constants;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Serilog.Filters;


namespace SchoolApi.Core.Business.CommonConfig
{
   public static class ConfigurationExtensions
    {
        public static void ConfigureDatabase<TContext>(this IServiceCollection services, IConfiguration configuration) 
            where TContext : DbContext
        {
            var connectionStringKey = configuration.GetConnectionString("database");
            var serverVersion = ServerVersion.AutoDetect(connectionStringKey);
            
            services.AddDbContext<TContext>(options =>
                options.UseMySql(connectionStringKey, serverVersion)
                    .EnableDetailedErrors());
        }
    public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            //var jwtSettings = configuration.GetSection("Jwt");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.SaveToken = true;

                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = configuration["Jwt:Audience"],
                        ValidIssuer = configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = async context =>
                        {
                            var httpContext = context.HttpContext;
                            var exception = context.Exception;
                            var customExceptionHandler = new CustomExceptionHandler();
                            string message;

                            if (exception is SecurityTokenExpiredException)
                            {
                                message = ExceptionMessages.ExpiredToken;
                                await WriteResponseAsync(httpContext, StatusCodes.Status401Unauthorized, message);
                                throw new CustomException(StatusCodes.Status401Unauthorized, ExceptionMessages.ExpiredToken);
                            }
                            else if (exception is SecurityTokenInvalidSignatureException)
                            {
                                message = ExceptionMessages.InvalidSignature;
                                await WriteResponseAsync(httpContext, StatusCodes.Status401Unauthorized, message);
                                throw new CustomException(StatusCodes.Status401Unauthorized, ExceptionMessages.InvalidSignature);
                            }
                            else if (exception is SecurityTokenMalformedException)
                            {
                                message = ExceptionMessages.InvalidFormat;
                                await WriteResponseAsync(httpContext, StatusCodes.Status400BadRequest, message);
                                throw new CustomException(StatusCodes.Status400BadRequest, ExceptionMessages.InvalidFormat);                                
                            }
                            else
                            {
                                message = ExceptionMessages.UnexpectedError;
                                await WriteResponseAsync(httpContext, StatusCodes.Status500InternalServerError, message);
                                throw new CustomException(StatusCodes.Status500InternalServerError, ExceptionMessages.UnexpectedError);
                            }
                        },

                        OnChallenge = context =>
                        {
                            throw new CustomException(StatusCodes.Status401Unauthorized, ExceptionMessages.UnauthorizedAccess);
                        },

                        OnForbidden = context =>
                        {
                            throw new CustomException(StatusCodes.Status403Forbidden, ExceptionMessages.AccessForbidden);
                        },

                        OnTokenValidated = context =>
                        {
                            return Task.CompletedTask;
                        },
                    };
                });
        }

    private static async Task WriteResponseAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode; 
        context.Response.ContentType = "application/json"; 

        var responseObj = new 
        {
            statusCode, 
            message      
        }; 


        await context.Response.WriteAsJsonAsync(responseObj);
    }

        public static void ConfigureSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            var swaggerConfig = configuration.GetSection("Swagger");

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(opt =>
            {
                var title = swaggerConfig.GetValue<string>("Title");
                var version = swaggerConfig.GetValue<string>("Version");

                opt.SwaggerDoc(version, new OpenApiInfo { Title = title, Version = version });
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });

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

                //opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
            });
        }


        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });
        }

        public static void ConfigureLogging(this IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public static void ConfigureFluentValidation(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation(fv => fv.DisableDataAnnotationsValidation = true);
            services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
            
        }

        public static void ConfigureExceptionHandling(this IServiceCollection services)
        {
            services.AddExceptionHandler<CustomExceptionHandler>();
        }
        public static void AddSerilogLogging(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .Enrich.FromLogContext()
                    .WriteTo.Logger(lg => lg
                        .Filter.ByIncludingOnly(Matching.WithProperty("RequestLog"))
                        .WriteTo.File("Logs/api_requests.log", rollingInterval: RollingInterval.Day)
                    )
                    .WriteTo.Logger(lg => lg
                        .Filter.ByIncludingOnly(evt => evt.Exception != null && !evt.Exception.Data.Contains("HandledByCustomHandler"))
                        .WriteTo.File("Logs/exceptions.log", rollingInterval: RollingInterval.Day)
                    )
                    .WriteTo.Console();
            });
        }
    }
}