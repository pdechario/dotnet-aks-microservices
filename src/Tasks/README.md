# Tasks API

A lightweight REST API microservice for managing tasks. Built with ASP.NET Core, this service provides endpoints to create, retrieve, and update task orders within the microservices architecture.

## Features

- **Get all tasks** — Retrieve a list of all tasks in the system
- **Get a task by ID** — Look up a specific task
- **Create a task** — Add a new task to the system
- **Complete a task** — Mark a task as done
- **Health check** — Monitor service availability

## API Endpoints

### Health Check
```
GET /health
```
Returns service status.

**Response:**
```json
{
  "status": "healthy",
  "service": "tasks"
}
```

### List All Tasks
```
GET /tasks
```
Returns all tasks.

**Response:**
```json
[
  {
    "id": "t1",
    "title": "Deploy Helm chart",
    "assignedUserId": "u1",
    "status": "open"
  }
]
```

### Get a Specific Task
```
GET /tasks/{id}
```
Returns a single task by ID. Returns 404 if not found.

**Example:**
```
GET /tasks/t1
```

### Create a Task
```
POST /tasks
```
Creates a new task.

**Request Body:**
```json
{
  "id": "t2",
  "title": "Update documentation",
  "assignedUserId": "u2",
  "status": "open"
}
```

**Response:** 201 Created with the new task.

### Complete a Task
```
PUT /tasks/{id}/complete
```
Marks a task as complete. Returns 404 if the task doesn't exist.

**Example:**
```
PUT /tasks/t1/complete
```

**Response:**
```json
{
  "id": "t1",
  "title": "Deploy Helm chart",
  "assignedUserId": "u1",
  "status": "complete"
}
```

## Task Object

A task has the following structure:

| Field | Type | Description |
|-------|------|-------------|
| `id` | string | Unique identifier for the task |
| `title` | string | Task description or name |
| `assignedUserId` | string | ID of the user assigned to the task |
| `status` | string | Current status: `open` or `complete` |

## Usage Example

```bash
# Check service health
curl http://localhost:5000/health

# Get all tasks
curl http://localhost:5000/tasks

# Get a specific task
curl http://localhost:5000/tasks/t1

# Create a new task
curl -X POST http://localhost:5000/tasks \
  -H "Content-Type: application/json" \
  -d '{"id":"t3","title":"Review PR","assignedUserId":"u2","status":"open"}'

# Complete a task
curl -X PUT http://localhost:5000/tasks/t1/complete
```

## Running the Service

```bash
dotnet run
```

The service will start and listen for HTTP requests. See the main project README for information on deploying to Kubernetes or Docker.
