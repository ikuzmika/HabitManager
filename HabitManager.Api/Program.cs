using HabitManager.Api.ErrorHandling;
using HabitManager.Application.Interfaces;
using HabitManager.Application.Services;
using HabitManager.Infrastructure.Persistence;
using HabitManager.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString =
    builder.Configuration.GetConnectionString("HabitDatabase")
    ?? throw new InvalidOperationException(
        "Connection string 'HabitDatabase' was not found.");

builder.Services.AddDbContext<HabitDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IHabitRepository, HabitRepository>();
builder.Services.AddScoped<HabitApplicationService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();