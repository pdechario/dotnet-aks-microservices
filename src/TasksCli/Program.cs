using TasksCli;

var client = new TasksApiClient();

while (true)
{
    Console.WriteLine("\n=== Tasks CLI ===");
    Console.WriteLine("1. Create task");
    Console.WriteLine("2. List all tasks");
    Console.WriteLine("3. Get task by ID");
    Console.WriteLine("4. Complete task");
    Console.WriteLine("5. Exit");
    Console.Write("Select an option: ");

    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            await CreateTask();
            break;
        case "2":
            await ListTasks();
            break;
        case "3":
            await GetTask();
            break;
        case "4":
            await CompleteTask();
            break;
        case "5":
            Console.WriteLine("Goodbye!");
            return;
        default:
            Console.WriteLine("Invalid option. Please try again.");
            break;
    }
}

async Task CreateTask()
{
    Console.WriteLine("\n--- Create Task ---");

    Console.Write("Task ID: ");
    var id = Console.ReadLine() ?? string.Empty;

    Console.Write("Task Title: ");
    var title = Console.ReadLine() ?? string.Empty;

    Console.Write("Task Details: ");
    var details = Console.ReadLine() ?? string.Empty;

    Console.Write("Assigned User ID: ");
    var assignedUserId = Console.ReadLine() ?? string.Empty;

    if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(title) ||
        string.IsNullOrWhiteSpace(details) || string.IsNullOrWhiteSpace(assignedUserId))
    {
        Console.WriteLine("Error: All fields are required.");
        return;
    }

    var task = await client.CreateTaskAsync(id, title, details, assignedUserId);
    if (task != null)
    {
        Console.WriteLine($"✓ Task created successfully!");
        Console.WriteLine($"  ID: {task.Id}");
        Console.WriteLine($"  Title: {task.Title}");
        Console.WriteLine($"  Details: {task.Details}");
        Console.WriteLine($"  Status: {task.Status}");
    }
}

async Task ListTasks()
{
    Console.WriteLine("\n--- All Tasks ---");

    var tasks = await client.GetAllTasksAsync();

    if (tasks.Count == 0)
    {
        Console.WriteLine("No tasks found.");
        return;
    }

    foreach (var task in tasks)
    {
        Console.WriteLine($"\nID: {task.Id}");
        Console.WriteLine($"Title: {task.Title}");
        Console.WriteLine($"Details: {task.Details}");
        Console.WriteLine($"Assigned to: {task.AssignedUserId}");
        Console.WriteLine($"Status: {task.Status}");
    }
}

async Task GetTask()
{
    Console.WriteLine("\n--- Get Task ---");

    Console.Write("Task ID: ");
    var id = Console.ReadLine() ?? string.Empty;

    if (string.IsNullOrWhiteSpace(id))
    {
        Console.WriteLine("Error: Task ID is required.");
        return;
    }

    var task = await client.GetTaskByIdAsync(id);
    if (task != null)
    {
        Console.WriteLine($"\nID: {task.Id}");
        Console.WriteLine($"Title: {task.Title}");
        Console.WriteLine($"Details: {task.Details}");
        Console.WriteLine($"Assigned to: {task.AssignedUserId}");
        Console.WriteLine($"Status: {task.Status}");
    }
}

async Task CompleteTask()
{
    Console.WriteLine("\n--- Complete Task ---");

    Console.Write("Task ID: ");
    var id = Console.ReadLine() ?? string.Empty;

    if (string.IsNullOrWhiteSpace(id))
    {
        Console.WriteLine("Error: Task ID is required.");
        return;
    }

    var task = await client.CompleteTaskAsync(id);
    if (task != null)
    {
        Console.WriteLine($"✓ Task completed!");
        Console.WriteLine($"  ID: {task.Id}");
        Console.WriteLine($"  Status: {task.Status}");
    }
}
