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

var tasks = new List<TaskOrder>
{
    new("t1", "Deploy Helm chart", "Deploy the Helm chart to production", "u1", "open")
};

var group = app.MapGroup("/v{version:apiVersion}/tasks")
    .WithName("Tasks")
    .WithOpenApi();

group.MapGet("/", () => Results.Ok(tasks))
    .WithName("GetAllTasks")
    .WithSummary("Retrieve all tasks");

group.MapGet("/{id}", (string id) =>
{
    var task = tasks.FirstOrDefault(t => t.Id == id);
    return task is null ? Results.NotFound() : Results.Ok(task);
})
    .WithName("GetTaskById")
    .WithSummary("Retrieve a specific task");

group.MapPost("/", (TaskOrder task) =>
{
    tasks.Add(task);
    return Results.Created($"/v1/tasks/{task.Id}", task);
})
    .WithName("CreateTask")
    .WithSummary("Create a new task");

group.MapPut("/{id}/complete", (string id) =>
{
    var index = tasks.FindIndex(t => t.Id == id);

    if (index == -1)
        return Results.NotFound();

    var completed = tasks[index] with { Status = "complete" };
    tasks[index] = completed;

    return Results.Ok(completed);
})
    .WithName("CompleteTask")
    .WithSummary("Mark a task as complete");

app.Run();

record TaskOrder(string Id, string Title, string Details, string AssignedUserId, string Status);