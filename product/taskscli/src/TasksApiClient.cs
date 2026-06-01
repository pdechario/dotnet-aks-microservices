using System.Net.Http.Json;

namespace DotnetAksMicroservices.Product.TasksCli;

public record TaskOrder(string Id, string Title, string Details, string AssignedUserId, string Status);

public class TasksApiClient
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "http://localhost:5002";

    public TasksApiClient()
    {
        _httpClient = new HttpClient();
    }

    public async Task<List<TaskOrder>> GetAllTasksAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<TaskOrder>>($"{BaseUrl}/tasks");
            return response ?? new List<TaskOrder>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching tasks: {ex.Message}");
            return new List<TaskOrder>();
        }
    }

    public async Task<TaskOrder?> GetTaskByIdAsync(string id)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<TaskOrder>($"{BaseUrl}/tasks/{id}");
            return response;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            Console.WriteLine($"Task with id '{id}' not found.");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching task: {ex.Message}");
            return null;
        }
    }

    public async Task<TaskOrder?> CreateTaskAsync(string id, string title, string details, string assignedUserId)
    {
        try
        {
            var newTask = new TaskOrder(id, title, details, assignedUserId, "open");
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/tasks", newTask);

            if (response.IsSuccessStatusCode)
            {
                var createdTask = await response.Content.ReadFromJsonAsync<TaskOrder>();
                return createdTask;
            }

            Console.WriteLine($"Error creating task: {response.StatusCode}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating task: {ex.Message}");
            return null;
        }
    }

    public async Task<TaskOrder?> CompleteTaskAsync(string id)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/tasks/{id}/complete", new { });

            if (response.IsSuccessStatusCode)
            {
                var completedTask = await response.Content.ReadFromJsonAsync<TaskOrder>();
                return completedTask;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine($"Task with id '{id}' not found.");
                return null;
            }

            Console.WriteLine($"Error completing task: {response.StatusCode}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error completing task: {ex.Message}");
            return null;
        }
    }
}
