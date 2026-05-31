using Common.Extensions;
using Common.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration
        .MinimumLevel.Information()
        .WriteTo.Console());

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new(1, 0);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
});

builder.Services.AddHealthChecks();
builder.Services.AddCommonServices();

var app = builder.Build();

app.UseExceptionHandling();
app.UseStructuredLogging();

app.MapHealthChecks("/health");

var users = new List<User>
{
    new("u1", "Percie", "percie@example.com")
};

var group = app.MapGroup("/v{version:apiVersion}/users")
    .WithName("Users");

group.MapGet("/", () => Results.Ok(users))
    .WithName("GetAllUsers")
    .WithSummary("Retrieve all users");

group.MapGet("/{id}", (string id) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    return user is null ? Results.NotFound() : Results.Ok(user);
})
    .WithName("GetUserById")
    .WithSummary("Retrieve a specific user");

group.MapPost("/", (User user) =>
{
    users.Add(user);
    return Results.Created($"/v1/users/{user.Id}", user);
})
    .WithName("CreateUser")
    .WithSummary("Create a new user");

app.Run();

record User(string Id, string Name, string Email);