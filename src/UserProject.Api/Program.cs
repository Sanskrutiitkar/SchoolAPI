
using FluentValidation;
using UserProject.Api.Mapper;
using UserProject.Business.Data;
using UserProject.Business.Repository;
using UserProject.Business.Services;
using SchoolApi.Core.Business.CommonConfig;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.ConfigureDatabase<UserDbContext>(builder.Configuration);  
builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.ConfigureSwagger(builder.Configuration);
builder.Services.ConfigureCors();
builder.Services.ConfigureLogging();
builder.Services.ConfigureFluentValidation();
builder.Services.ConfigureExceptionHandling();
//builder.AddSerilogLogging();

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
// app.UseSerilogRequestLogging(options =>
// {
//     options.EnrichDiagnosticContext = async (diagnosticContext, httpContext) =>
//     {
//         diagnosticContext.Set("RequestLog", true);
//         diagnosticContext.Set("RequestPath", httpContext.Request.Path);
//         diagnosticContext.Set("RequestMethod", httpContext.Request.Method);
//     };
// });
 
 

app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
