using DotnetAksMicroservices.Platform.Common.Extensions;
using DotnetAksMicroservices.Platform.Common.Middleware;
using DotnetAksMicroservices.Product.Notifications;
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

var notifications = new List<Notification>
{
    new("n1", "u1", "Task assigned")
};

var group = app.MapGroup("/v{version:apiVersion}/notifications")
    .WithName("Notifications");

group.MapGet("/", () => Results.Ok(notifications))
    .WithName("GetAllNotifications")
    .WithSummary("Retrieve all notifications");

group.MapPost("/", (Notification notification) =>
{
    notifications.Add(notification);
    return Results.Created($"/v1/notifications/{notification.Id}", notification);
})
    .WithName("CreateNotification")
    .WithSummary("Create a new notification");

app.Run();