using System.Reflection;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SchoolApi.Core.Business.Exceptions;
using SchoolApi.Core.Business.Models;
using UserProject.Api.Mapper;
using UserProject.Api.Validators;
using UserProject.Business.Data;
using UserProject.Business.Repository;
using UserProject.Business.Services;
using SchoolApi.Core.Business.CommonConfig;

// var builder = WebApplication.CreateBuilder(args);


// var serverVersion = ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("database"));

// builder.Services.AddDbContext<UserDbContext>(options => {
//     options.UseMySql(builder.Configuration.GetConnectionString("database"), serverVersion).EnableDetailedErrors();
// });
// builder.Services.AddControllers();
// builder.Services.AddAutoMapper(typeof(UserAutoMapperProfile).Assembly);
// builder.Services.AddScoped<IAuthService, AuthService>();
// builder.Services.AddScoped<IAuthRepo, AuthRepo>();
// builder.Services.AddScoped<IUserRepo, UserRepo>();

// builder.Services.AddFluentValidationAutoValidation(fv => fv.DisableDataAnnotationsValidation = true);
// builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
 

// builder.Services.AddValidatorsFromAssemblyContaining<Program>();
// //builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>(); 


// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
// builder.Services.AddExceptionHandler<CustomExceptionHandler>();

// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowAllOrigins",
//         builder =>
//         {
//             builder.AllowAnyOrigin()
//                    .AllowAnyMethod()
//                    .AllowAnyHeader();
//         });
// });
// builder.Services.AddSwaggerGen(opt =>
// {
//     opt.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "MyAPI", Version = "v1" });
//     opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//     {
//         In = ParameterLocation.Header,
//         Description = "Please enter token",
//         Name = "Authorization",
//         Type = SecuritySchemeType.Http,
//         BearerFormat = "JWT",
//         Scheme = "bearer"
//     });
 
//     opt.AddSecurityRequirement(new OpenApiSecurityRequirement
//     {
//         {
//             new OpenApiSecurityScheme
//             {
//                 Reference = new OpenApiReference
//                 {
//                     Type=ReferenceType.SecurityScheme,
//                     Id="Bearer"
//                 }
//             },
//             new string[]{}
//         }
//     });

//      opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));

// });

// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.RequireHttpsMetadata = false;
//         options.SaveToken = true;

//         options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
//         {
//             ValidateIssuer = true,
//             ValidateAudience = true,
//             ValidAudience = builder.Configuration["Jwt:Audience"],
//             ValidIssuer = builder.Configuration["Jwt:Issuer"],
//             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
//             ValidateIssuerSigningKey = true
//         };

//         options.Events = new JwtBearerEvents
//         {
//             OnAuthenticationFailed = async context =>
//             {
//                 var exception = context.Exception;
//                 var customExceptionHandler = new CustomExceptionHandler();
//                 var cancellationToken = context.HttpContext.RequestAborted;

//                 if (exception is SecurityTokenExpiredException)
//                 {
//                     throw new CustomException(StatusCodes.Status401Unauthorized, "Token has expired.");
 
//                 }
//                 else if (exception is SecurityTokenInvalidSignatureException)
//                 {
//                     throw new CustomException(StatusCodes.Status401Unauthorized, "Token has an invalid signature.");

//                 }
//                 else if (exception is SecurityTokenMalformedException)
//                 {
//                     throw new CustomException(StatusCodes.Status400BadRequest, "Token format is invalid.");
//                 }
//                 else
//                 {
//                     throw new CustomException(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
//                 }

  
//             },

           
//             OnChallenge =   context =>
//             {
//                 throw new CustomException(StatusCodes.Status401Unauthorized,"Unauthorized access. Token validation failed.");
//             },

//             OnForbidden =   context =>
//             {

//                 throw new CustomException(StatusCodes.Status403Forbidden, "Access forbidden. You do not have permission to access this resource.");
//             },

//             OnTokenValidated = context =>
//             {
//                 return Task.CompletedTask;
//             },
//         };
//     });

// var app = builder.Build();
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.UseHttpsRedirection();
// app.UseAuthentication();
// app.UseAuthorization();
// app.UseCors("AllowAllOrigins");
// app.MapControllers();
// app.UseExceptionHandler(_=>{ });
// app.Run();


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.ConfigureDatabase<UserDbContext>(builder.Configuration);  
builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.ConfigureSwagger(builder.Configuration);
builder.Services.ConfigureCors();
builder.Services.ConfigureLogging();
builder.Services.ConfigureFluentValidation();
builder.Services.ConfigureExceptionHandling();


builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepo, AuthRepo>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddAutoMapper(typeof(UserAutoMapperProfile).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<Program>();


var app = builder.Build();
app.UseExceptionHandler(_ => { });
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

app.Run();
