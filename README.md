# dotnet-aks-microservices

Minimal .NET 8 microservices product for an AKS, Helm, and OpenTelemetry infrastructure portfolio project.

## Services

- Users.Api
- Tasks.Api
- Notifications.Api

Each service is an independent .NET 8 Minimal API running on port 8080 inside its container.

## Run locally

```bash
docker compose up --build