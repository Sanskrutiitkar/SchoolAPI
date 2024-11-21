using FluentValidation;
using SchoolProject.Api.Mapper;
using SchoolProject.Buisness.Data;
using SchoolProject.Buisness.Repository;
using SchoolProject.Buisness.Services;
using SchoolApi.Core.Business.CommonConfig;
using Serilog;
using Plain.RabbitMQ;
using RabbitMQ.Client;
using SchoolProject.Api.Listener;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSingleton<IEmailService, EmailService>();

builder.Services.AddSingleton<IConnectionProvider>(new ConnectionProvider("amqp://guest:guest@localhost:5672"));
builder.Services.AddSingleton<IPublisher>(p => new Publisher(
    p.GetService<IConnectionProvider>(),
    "student_exchange",   // Exchange name
    ExchangeType.Topic  // Exchange Type
));

// Register the Subscriber 
builder.Services.AddSingleton<ISubscriber>(s => new Subscriber(
    s.GetService<IConnectionProvider>(),
    "student_exchange",  // The exchange to subscribe to
    "student_event_queue",   // The queue name
    "student.*",   // The routing key to listen to
    ExchangeType.Topic  // Exchange type
));

builder.Services.AddHostedService<StudentEventEmailListener>();
builder.Services.ConfigureSwagger(builder.Configuration);
builder.Services.ConfigureDatabase<StudentDbContext>(builder.Configuration); 
builder.Services.ConfigureAuthentication(builder.Configuration);

builder.Services.ConfigureCors();
builder.Services.ConfigureLogging();
builder.Services.ConfigureFluentValidation();
builder.Services.ConfigureExceptionHandling();
//builder.AddSerilogLogging();

builder.Services.AddScoped<IStudentRepo, StudentRepo>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddAutoMapper(typeof(StudentAutoMapperProfile).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();
app.UseCors("AllowAllOrigins");
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
 
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
