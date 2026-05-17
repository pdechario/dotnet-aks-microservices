var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var users = new List<User>
{
    new("u1", "Percie", "percie@example.com")
};

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "users-api" }));

app.MapGet("/users", () => Results.Ok(users));

app.MapGet("/users/{id}", (string id) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    return user is null ? Results.NotFound() : Results.Ok(user);
});

app.MapPost("/users", (User user) =>
{
    users.Add(user);
    return Results.Created($"/users/{user.Id}", user);
});

app.Run();

record User(string Id, string Name, string Email);