using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using SchoolApi.APi.Exceptions;
using SchoolApi.API.Exceptions;
using SchoolApi.API.Mapper;
using SchoolApi.Business.Data;
using SchoolApi.Business.Repository;
using SchoolApi.Business.Services;

var builder = WebApplication.CreateBuilder(args);

var serverVersion = ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("database"));

//builder.Services.AddDbContext<StudentDbContext>(options =>
//           options.UseMySql(builder.Configuration.GetConnectionString("database"), serverVersion,
//               b => b.MigrationsAssembly("SchoolApi.API")) 
//               .EnableDetailedErrors()
//               .EnableSensitiveDataLogging());
builder.Services.AddDbContext<StudentDbContext>(options => {
    options.UseMySql(builder.Configuration.GetConnectionString("database"), serverVersion).EnableDetailedErrors();
});
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(StudentAutoMapperProfile).Assembly);
builder.Services.AddScoped<IStudentRepo, StudentRepo>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
builder.Services.AddTransient<CustomExceptionHandler>();
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
var app = builder.Build();

app.UseMiddleware<CustomExceptionHandler>();
app.UseMiddleware<CustomExceptionHandler>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("AllowAllOrigins");
app.MapControllers();

app.Run();
