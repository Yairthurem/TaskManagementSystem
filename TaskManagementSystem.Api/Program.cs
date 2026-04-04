using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Api.Data;
using TaskManagementSystem.Api.Interfaces;
using TaskManagementSystem.Api.Models;
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
            policy.WithOrigins("http://localhost:5173", "http://localhost:3000", "http://localhost:8080")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Register FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<TaskCreateDtoValidator>();

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
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// Messaging and Background Services
builder.Services.AddScoped<IMessagePublisher, RabbitMqPublisher>();
builder.Services.AddHostedService<TaskManagementSystem.Api.BackgroundServices.TaskReminderWorker>();
builder.Services.AddHostedService<TaskManagementSystem.Api.BackgroundServices.ReminderConsumerWorker>();

var app = builder.Build();

app.UseMiddleware<TaskManagementSystem.Api.Middlewares.GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

// Automate migrations on startup (Simplified for Docker Deployment)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    // Runtime Seed if empty
    if (!db.Users.Any())
    {
        db.Users.AddRange(
            new User { FirstName = "Alice", LastName = "Smith", Email = "alice@example.com" },
            new User { Id = 2, FirstName = "Bob", LastName = "Jones", Email = "bob@example.com" }
        );
    }
    if (!db.Tags.Any())
    {
        db.Tags.AddRange(
            new Tag { Name = "Critical" },
            new Tag { Name = "Bug" },
            new Tag { Name = "Feature" },
            new Tag { Name = "Done" }
        );
    }
    db.SaveChanges();
}

app.Run();
