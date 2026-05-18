# Dotnet AKS Microservices - Project Guidelines

## Project Structure

```
.
├── product/                           # Business domain services
│   ├── tasks/
│   │   ├── src/                       # Source code (Tasks.csproj, Program.cs, etc.)
│   │   ├── tests/                     # Tests
│   │   ├── docker-compose.yml         # Service-specific compose
│   │   └── Dockerfile.Tasks           # Service Dockerfile
│   ├── users/
│   ├── notifications/
│   └── taskscli/                      # Product client CLI tool
├── platform/                          # Shared infrastructure & edge
│   ├── gateway/
│   │   ├── src/                       # Source code (Gateway.csproj, Program.cs, etc.)
│   │   ├── tests/                     # Tests
│   │   ├── docker-compose.yml         # Service-specific compose
│   │   └── Dockerfile.Gateway         # Service Dockerfile
│   ├── common/
│   │   └── src/                       # Shared NuGet package (Common.csproj)
│   ├── observability/                 # Observability setup (stub)
│   └── middleware/                    # Future middleware extraction (stub)
├── deployment/                        # Orchestration & containerization
│   ├── helm/
│   ├── k8s/
│   └── docker/
├── infrastructure/                    # Cloud resources & IaC (stubs)
│   ├── aks/
│   ├── acr/
│   ├── networking/
│   └── terraform/
├── packages/                          # Local NuGet feed
├── nuget.config                       # NuGet configuration pointing to local packages
├── docker-compose.yml                 # Root compose for all services
└── build.sh                           # Build script for Common package
```

## Architectural Layers

### Product Layer (`product/`)
Business domain services independent of platform concerns:
- **tasks** — Task management service
- **users** — User management service
- **notifications** — Notification service
- **taskscli** — CLI client for interacting with product services

Each service is a self-contained, independently deployable unit.

### Platform Layer (`platform/`)
Shared infrastructure and edge routing:
- **gateway** — API Gateway (YARP) for routing and edge concerns
- **common** — Shared libraries (middleware, utilities, response models)
- **observability** — Telemetry and monitoring setup
- **middleware** — Extraction point for shared middleware patterns

### Deployment Layer (`deployment/`)
Orchestration and containerization configuration:
- **helm/** — Helm charts for Kubernetes deployment
- **k8s/** — Raw Kubernetes manifests
- **docker/** — Docker-specific configuration

### Infrastructure Layer (`infrastructure/`)
Cloud resources and infrastructure-as-code (stubs for future setup):
- **aks/** — Azure Kubernetes Service configuration
- **acr/** — Azure Container Registry configuration
- **networking/** — Network infrastructure (VNets, NSGs, etc.)
- **terraform/** — Terraform modules for cloud resources

## Docker & Containerization

### Dockerfile Naming Convention

All Dockerfiles **must** be named with the format: `Dockerfile.[ProjectName]`

Examples:
- `Dockerfile.Gateway` - for Gateway service
- `Dockerfile.Tasks` - for Tasks service
- `Dockerfile.Users` - for Users service
- `Dockerfile.Notifications` - for Notifications service

This naming convention makes it clear which Dockerfile corresponds to which service and allows multiple Dockerfiles in the same directory if needed.

### Building Services

Each service has:
- Individual `docker-compose.yml` for isolated testing
- `Dockerfile.[ProjectName]` at the service root

Build a single service:
```bash
docker-compose -f product/tasks/docker-compose.yml build
```

Build all services from root:
```bash
docker-compose build
```

## NuGet Package Management

### Packages

All projects are packaged as NuGet packages and can be published to a feed:

- **DotnetAksMicroservices.Common** - Shared utilities and middleware
- **DotnetAksMicroservices.Gateway** - Gateway service
- **DotnetAksMicroservices.Tasks** - Tasks service
- **DotnetAksMicroservices.Users** - Users service
- **DotnetAksMicroservices.Notifications** - Notifications service
- **DotnetAksMicroservices.TasksCli** - CLI tool for Tasks service

### Building Packages

Run the build script to pack all projects:
```bash
./scripts/build.sh
```

This builds and packs all projects to `./packages/` where they can be consumed locally via `nuget.config`.

### Local NuGet Feed

The `nuget.config` file configures:
- `./packages/` as the local NuGet feed (checked first)
- `nuget.org` as fallback for external packages

When updating any package:
1. Make changes to the project
2. Run `./scripts/build.sh` to pack and update the local feed
3. Run `dotnet restore` to get the new version

## API Standards

All services follow these standards:

### Middleware Pipeline
- Exception handling middleware (global error handling)
- Structured logging middleware (request context logging)

### Features
- API versioning (v1, v2, etc.) with `Asp.Versioning.Http`
- Health checks at `/health` endpoint
- Structured logging with Serilog
- OpenAPI/Swagger ready

### Endpoint Naming
- Use route groups: `/v{version:apiVersion}/[resource]`
- Include endpoint names and summaries for documentation
- Example: `GET /v1/tasks`, `POST /v1/tasks`, etc.

## Building & Running

### Prerequisites
- .NET 8 SDK
- Docker & Docker Compose

### Build Scripts

All build operations are available via bash scripts in the `scripts/` directory:

```bash
./scripts/help.sh                      # Show all available commands

# Build
./scripts/build.sh                     # Build entire solution + Common package
./scripts/build-service.sh gateway     # Build specific service (layer-aware)
./scripts/build-service.sh tasks       # Automatically finds service in product/tasks/

# Test
./scripts/test.sh                      # Run all tests
./scripts/test.sh gateway              # Test specific service

# Run
./scripts/run.sh                       # Start all services with Docker Compose

# Utility
./scripts/restore.sh                   # Restore NuGet packages
./scripts/clean.sh                     # Clean build artifacts
```

### Manual Commands (if not using scripts)

Build everything:
```bash
dotnet build
```

Test a specific service:
```bash
dotnet test product/tasks/tests/
```

Run with Docker Compose:
```bash
docker-compose up --build
```

### Service Ports (Docker Compose)
- Gateway: 5000 → 8080 (internal)
- Tasks: 5002 → 8080 (internal)
- Users: 5001 → 8080 (internal)
- Notifications: 5003 → 8080 (internal)

Gateway routes to other services via internal Docker network addresses (e.g., `http://tasks:8080`).

## Project References & Dependencies

### Services are Independent
Each service:
- Lives in its own directory under `product/[name]` or `platform/[name]`
- Is a completely independent project
- References the Common library as a NuGet package (not project reference)
- Can be deployed independently

This design allows services to:
- Be developed and tested in isolation
- Be deployed to different servers/containers
- Use different versions of shared infrastructure (when needed)

### Common Library
- Located in `platform/common/src`
- Provides:
  - Exception handling middleware
  - Structured logging middleware
  - API response models
  - Service collection extensions
- Updated via `build.sh` script, then version bump in `.csproj` files

## Code Quality

### Nullable Reference Types
All projects have `<Nullable>enable</Nullable>` - ensure proper null handling.

### Implicit Usings
All projects have `<ImplicitUsings>enable</ImplicitUsings>` - common namespaces are automatically included.

## Directory Flattening

Source files are organized with minimal nesting:
- Services: `product/[service]/src/` (not `product/[service]/src/[ProjectName]/`)
- Platform: `platform/[component]/src/` (not `platform/[component]/src/[ProjectName]/`)

This reduces path depth while maintaining clear organizational boundaries.
