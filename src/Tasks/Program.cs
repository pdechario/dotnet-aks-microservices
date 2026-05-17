var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var tasks = new List<TaskOrder>
{
    new("t1", "Deploy Helm chart", "u1", "open")
};

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "tasks" }));

app.MapGet("/tasks", () => Results.Ok(tasks));

app.MapGet("/tasks/{id}", (string id) =>
{
    var task = tasks.FirstOrDefault(t => t.Id == id);
    return task is null ? Results.NotFound() : Results.Ok(task);
});

app.MapPost("/tasks", (TaskOrder task) =>
{
    tasks.Add(task);
    return Results.Created($"/tasks/{task.Id}", task);
});

app.MapPut("/tasks/{id}/complete", (string id) =>
{
    var index = tasks.FindIndex(t => t.Id == id);

    if (index == -1)
    {
        return Results.NotFound();
    }

    var completed = tasks[index] with { Status = "complete" };
    tasks[index] = completed;

    return Results.Ok(completed);
});

app.Run();

record TaskOrder(string Id, string Title, string AssignedUserId, string Status);