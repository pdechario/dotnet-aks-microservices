# API Gateway

This is an API Gateway built with YARP (Yet Another Reverse Proxy) that routes requests to the microservices.

## Routes

The gateway exposes the following routes:

- **Tasks**: `/api/tasks/*` → forwards to Tasks service
- **Users**: `/api/users/*` → forwards to Users service  
- **Notifications**: `/api/notifications/*` → forwards to Notifications service
- **Health**: `/health` → gateway health check

## Running Locally

### Using Docker Compose

```bash
docker-compose up
```

The gateway will be available at `http://localhost:5000`

### Direct Service Calls

```bash
# Get all tasks
curl http://localhost:5000/api/tasks

# Get a user
curl http://localhost:5000/api/users/u1

# Get notifications
curl http://localhost:5000/api/notifications
```

## Architecture

The gateway uses YARP's configuration-based routing to forward requests to backend services:

```
Client → Gateway (port 5000) → Microservices
           - /api/tasks → Tasks Service (port 5002)
           - /api/users → Users Service (port 5001)
           - /api/notifications → Notifications Service (port 5003)
```

## Configuration

Route configuration is defined in `appsettings.json` under the `ReverseProxy` section. Each route specifies:

- **Match**: The path pattern to match incoming requests
- **ClusterId**: The target cluster containing backend destinations
- **Transforms**: Path transformation rules (strip `/api` prefix)

## Adding New Routes

To add a new microservice route:

1. Add a new route entry in `appsettings.json` under `ReverseProxy.Routes`
2. Add a corresponding cluster entry under `ReverseProxy.Clusters`
3. Specify the service address in the cluster's destinations

Example:
```json
"my-service-route": {
  "ClusterId": "my-service-cluster",
  "Match": {
    "Path": "/api/my-service{**catch-all}"
  },
  "Transforms": [
    {
      "PathPattern": "/api/my-service{**catch-all}",
      "PathReplacement": "/my-service{**catch-all}"
    }
  ]
},
```
