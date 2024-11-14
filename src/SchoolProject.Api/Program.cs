using FluentValidation;
using SchoolProject.Api.Mapper;
using SchoolProject.Buisness.Data;
using SchoolProject.Buisness.Repository;
using SchoolProject.Buisness.Services;
using SchoolApi.Core.Business.CommonConfig;





var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.ConfigureDatabase<StudentDbContext>(builder.Configuration); 
builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.ConfigureSwagger(builder.Configuration);
builder.Services.ConfigureCors();
builder.Services.ConfigureLogging();
builder.Services.ConfigureFluentValidation();
builder.Services.ConfigureExceptionHandling();


builder.Services.AddScoped<IStudentRepo, StudentRepo>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddAutoMapper(typeof(StudentAutoMapperProfile).Assembly);
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
