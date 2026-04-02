using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Api.Data;
using TaskManagementSystem.Api.Interfaces;
using TaskManagementSystem.Api.Repositories;
using TaskManagementSystem.Api.Services;
using TaskManagementSystem.Api.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add CORS policy for local React app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddControllers();

// Register FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<TaskCreateDtoValidator>();

// Swagger implicitly supported but we will ensure swagger UI runs nicely via default OpenApi
// Note: In .NET 9/10, AddOpenApi() is the standard without swashbuckle, but Swashbuckle can still be used for a UI. Let's use scalar or swagger-ui.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection for Repositories and Services
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<ITagService, TagService>();

// Messaging and Background Services
builder.Services.AddScoped<IMessagePublisher, RabbitMqPublisher>();
builder.Services.AddHostedService<TaskManagementSystem.Api.BackgroundServices.TaskReminderWorker>();
builder.Services.AddHostedService<TaskManagementSystem.Api.BackgroundServices.ReminderConsumerWorker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
