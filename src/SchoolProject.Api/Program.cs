using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolProject.Api.DTOs;
using SchoolProject.Api.Exceptions;
using SchoolProject.Api.Filter;
using SchoolProject.Api.Mapper;
using SchoolProject.Api.Validators;
using SchoolProject.Buisness.Data;
using SchoolProject.Buisness.Repository;
using SchoolProject.Buisness.Services;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddControllers(options =>
// {
//     // Customize model state invalid response
//     options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => "This field is required.");
// }).ConfigureApiBehaviorOptions(options =>
// {
//     options.InvalidModelStateResponseFactory = context =>
//     {
//         var traceId = Guid.NewGuid(); // Generate a new Trace ID for each error response
//         var errors = context.ModelState
//             .Where(e => e.Value.Errors.Count > 0)
//             .ToDictionary(
//                 kvp => kvp.Key,
//                 kvp => kvp.Value.Errors.Select(err => err.ErrorMessage).ToArray()
//             );

//         var errorDetails = new ErrorDetails
//         {
//             TraceId = traceId,
//             Message = "One or more validation errors occurred.",
//             StatusCode = (int)HttpStatusCode.BadRequest,
//             Instance = context.HttpContext.Request.Path,
//             ExceptionMessage = "Validation failed.",
//             Errors = errors // Assuming you add a property to ErrorDetails to hold validation errors
//         };

//         return new BadRequestObjectResult(errorDetails);
//     };
// });
var serverVersion = ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("database"));

builder.Services.AddDbContext<StudentDbContext>(options => {
    options.UseMySql(builder.Configuration.GetConnectionString("database"), serverVersion).EnableDetailedErrors();
});
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(StudentAutoMapperProfile).Assembly);
builder.Services.AddScoped<IStudentRepo, StudentRepo>();
builder.Services.AddScoped<IStudentService, StudentService>();

builder.Services.AddFluentValidationAutoValidation(fv => fv.DisableDataAnnotationsValidation = true);
builder.Services.AddScoped<ModelValidationFilter>();
builder.Services.AddControllers(options => options.Filters.Add<ModelValidationFilter>());
builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
 
// builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddValidatorsFromAssemblyContaining<StudentValidator>(); 
builder.Services.AddValidatorsFromAssemblyContaining<StudentUpdateValidator>();
// builder.Services.AddScoped<ModelValidationFilter>(); 
// builder.Services.AddControllers(options =>
// {
//     // Add the ModelValidationFilter globally
//     options.Filters.Add<ModelValidationFilter>();
// });


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
app.UseExceptionHandler(_=>{ });
app.Run();
