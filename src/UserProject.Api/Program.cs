using Microsoft.EntityFrameworkCore;
using UserProject.Business.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var serverVersion = ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("database"));

builder.Services.AddDbContext<UserDbContext>(options => {
    options.UseMySql(builder.Configuration.GetConnectionString("database"), serverVersion).EnableDetailedErrors();
});
builder.Services.AddControllers();


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
app.UseCors("AllowAllOrigins");
app.MapControllers();
app.Run();
