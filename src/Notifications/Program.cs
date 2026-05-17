var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var notifications = new List<Notification>
{
    new("n1", "u1", "Task assigned")
};

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "notifications" }));

app.MapGet("/notifications", () => Results.Ok(notifications));

app.MapPost("/notifications", (Notification notification) =>
{
    notifications.Add(notification);
    return Results.Created($"/notifications/{notification.Id}", notification);
});

app.Run();

record Notification(string Id, string UserId, string Message);