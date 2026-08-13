using FunEvents.API.Endpoints;
using FunEvents.Application;
using FunEvents.Infrastructure;
using FunEvents.Infrastructure.Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();


builder.AddSqlServerDbContext<FunEventsDbContext>(
    connectionName: "funevents");

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
// Ensure database is migrated and seeded at startup (development convenience).
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FunEventsDbContext>();
    // Apply migrations and seed data. Await is allowed in top-level statements.
    //await dbContext.Database.MigrateAsync();
    await dbContext.EnsureSeedDataAsync();
}

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.MapEventEndpoints();
app.MapUserEndpoints();
app.MapReservationEndpoints();


app.Run();
