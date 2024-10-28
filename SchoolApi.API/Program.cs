using Microsoft.EntityFrameworkCore;
using SchoolApi.API.Mapper;
using SchoolApi.Business.Data;
using SchoolApi.Business.Repository;
using SchoolApi.Business.Services;

var builder = WebApplication.CreateBuilder(args);

var serverVersion = ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("database"));

builder.Services.AddDbContext<StudentDbContext>(options =>
           options.UseMySql(builder.Configuration.GetConnectionString("database"), serverVersion,
               b => b.MigrationsAssembly("SchoolApi.API")) 
               .EnableDetailedErrors()
               .EnableSensitiveDataLogging());

builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(StudentAutoMapperProfile).Assembly);
builder.Services.AddScoped<IStudentRepo, StudentRepo>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
