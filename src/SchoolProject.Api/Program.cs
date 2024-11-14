using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using SchoolApi.Core.Business.Exceptions;
using SchoolApi.Core.Business.Filter;
using SchoolApi.Core.Business.Models;
using SchoolProject.Api.Filter;
using SchoolProject.Api.Mapper;
using SchoolProject.Api.Validators;
using SchoolProject.Buisness.Data;
using SchoolProject.Buisness.Repository;
using SchoolProject.Buisness.Services;
using Serilog;

using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


var serverVersion = ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("database"));

builder.Services.AddDbContext<StudentDbContext>(options => {
    options.UseMySql(builder.Configuration.GetConnectionString("database"), serverVersion).EnableDetailedErrors();
});
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(StudentAutoMapperProfile).Assembly);
builder.Services.AddScoped<IStudentRepo, StudentRepo>();
builder.Services.AddScoped<IStudentService, StudentService>();
//builder.Services.AddScoped<ModelValidationFilter>();

builder.Services.AddFluentValidationAutoValidation(fv => fv.DisableDataAnnotationsValidation = true);
builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
 

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
//builder.Services.AddValidatorsFromAssemblyContaining<StudentValidator>(); 
//builder.Services.AddValidatorsFromAssemblyContaining<StudentUpdateValidator>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "MyAPI", Version = "v1" });
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
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
     opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateIssuerSigningKey = true
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = async context =>
            {
                var exception = context.Exception;

                // Use the CustomExceptionHandler to handle authentication failures
                var customExceptionHandler = new CustomExceptionHandler();
                var cancellationToken = context.HttpContext.RequestAborted;

                // Handle specific exceptions
                if (exception is SecurityTokenExpiredException)
                {
                    await customExceptionHandler.TryHandleAsync(context.HttpContext, 
                        new CustomException(StatusCodes.Status401Unauthorized, "Token has expired."), cancellationToken);
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized; // Set status code
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new { message = "Token has expired." });
                    return; // Stop further processing
                }
                else if (exception is SecurityTokenInvalidSignatureException)
                {
                    await customExceptionHandler.TryHandleAsync(context.HttpContext, 
                        new CustomException(StatusCodes.Status401Unauthorized, "Token has an invalid signature."), cancellationToken);
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized; // Set status code
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new { message = "Token has an invalid signature." });
                    return; // Stop further processing
                }
                else if (exception is SecurityTokenMalformedException)
                {
                    await customExceptionHandler.TryHandleAsync(context.HttpContext, 
                        new CustomException(StatusCodes.Status400BadRequest, "Token format is invalid."), cancellationToken);
                    context.Response.StatusCode = StatusCodes.Status400BadRequest; // Set status code
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new { message = "Token format is invalid." });
                    return; // Stop further processing
                }

                // If the exception is not handled, allow the default behavior
                context.Response.StatusCode = StatusCodes.Status500InternalServerError; // Default error handling
                await context.Response.WriteAsJsonAsync(new { message = "An unexpected error occurred." });
            },

            OnChallenge = async context =>
            {
                // Customize the challenge response when authentication fails
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var result = JsonConvert.SerializeObject(new { message = "Unauthorized access. Token validation failed." });
                await context.Response.WriteAsync(result);
                
                // Mark as handled to prevent further processing
            },

            OnForbidden = async context =>
            {
                // Customize the response for forbidden access
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                var result = JsonConvert.SerializeObject(new { message = "Access forbidden. You do not have permission to access this resource." });
                await context.Response.WriteAsync(result);
                
                // Mark as handled to prevent further processing
            },

            OnTokenValidated = context =>
            {
                // Handle successful token validation if necessary (e.g., custom logging or additional checks)
                return Task.CompletedTask;
            },
        };
    });
builder.Services.AddScoped<APILoggingFilter>();
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
    
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAllOrigins");
app.MapControllers();
app.UseExceptionHandler(_=>{ });
app.Run();
