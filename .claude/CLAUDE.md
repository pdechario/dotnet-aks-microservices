# Dotnet AKS Microservices - Project Guidelines

## Project Structure

```
.
├── product/                           # Business domain services
│   ├── tasks/
│   │   ├── Tasks.csproj               # Project file
│   │   ├── Program.cs                 # Entry point
│   │   ├── src/                       # Source code files
│   │   ├── tests/                     # Tests
│   │   ├── docker-compose.yml         # Service-specific compose
│   │   └── Dockerfile.Tasks           # Service Dockerfile
│   ├── users/
│   ├── notifications/
│   └── taskscli/                      # Product client CLI tool
├── platform/                          # Shared infrastructure & edge
│   ├── gateway/
│   │   ├── Gateway.csproj             # Project file
│   │   ├── Program.cs                 # Entry point
│   │   ├── src/                       # Source code files
│   │   ├── tests/                     # Tests
│   │   ├── docker-compose.yml         # Service-specific compose
│   │   └── Dockerfile.Gateway         # Service Dockerfile
│   ├── common/
│   │   ├── Common.csproj              # Project file
│   │   └── src/                       # Source code files
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
├── nuget.template.config              # NuGet config template (safe to commit, no secrets)
├── nuget.config                       # Generated NuGet config (gitignored, has credentials)
├── docker-compose.yml                 # Root compose for all services
└── build.sh                           # Build and publish script
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
- `Dockerfile.[ProjectName]` at the service root (expects pre-built artifacts in `bin/Release/net8.0/publish/`)
- Published artifacts are prepared by build script

Publish all services for Docker (includes `dotnet build` + `dotnet publish`):
```bash
./scripts/build.sh
```

Build all Docker images from root:
```bash
docker-compose build
```

Services are containerized from pre-published artifacts — the workflow publishes in CI, developers publish locally before docker-compose.

## NuGet Package Management

### Packages

All projects are packaged as NuGet packages and published to GitHub Packages:

- **DotnetAksMicroservices.Common** - Shared utilities and middleware
- **DotnetAksMicroservices.Gateway** - Gateway service
- **DotnetAksMicroservices.Tasks** - Tasks service
- **DotnetAksMicroservices.Users** - Users service
- **DotnetAksMicroservices.Notifications** - Notifications service
- **DotnetAksMicroservices.TasksCli** - CLI tool for Tasks service

### Local Development

Services use **ProjectReference** to Common.csproj, so `dotnet build` works without any bootstrap:

```bash
./scripts/build.sh                # Restore, build, and publish for Docker
dotnet build                      # Build solution with ProjectReference to Common
```

### CI/GitHub Actions

The `.github/workflows/publish-packages.yml` workflow:
1. Publishes Common to GitHub Packages first
2. Uses a **build matrix** to publish all services in parallel
3. Services use **PackageReference** to Common (enabled by `GITHUB_ACTIONS=true` env var)

Each service .csproj has conditional references:
```xml
<ItemGroup Condition="'$(GITHUB_ACTIONS)' != 'true'">
  <ProjectReference Include="../../platform/common/Common.csproj" />
</ItemGroup>

<ItemGroup Condition="'$(GITHUB_ACTIONS)' == 'true'">
  <PackageReference Include="DotnetAksMicroservices.Common" Version="1.0.0" />
</ItemGroup>
```

For platform services, the path is shorter:
```xml
<ItemGroup Condition="'$(GITHUB_ACTIONS)' != 'true'">
  <ProjectReference Include="../common/Common.csproj" />
</ItemGroup>
```

### NuGet Configuration

**nuget.template.config** — Committed to repo, contains placeholder for credentials:
```xml
<add key="Username" value="GITHUB_USERNAME_PLACEHOLDER" />
<add key="ClearTextPassword" value="GITHUB_TOKEN_PLACEHOLDER" />
```

**nuget.config** — Generated locally by setup script, gitignored (contains your token):
```bash
./scripts/setup-github-packages.sh
```

This generates nuget.config with your actual GitHub token for local development.

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
./scripts/help.sh                         # Show all available commands

# Build & Publish
./scripts/build.sh                        # Build & publish all services for Docker
./scripts/setup-github-packages.sh        # Generate nuget.config from template
./scripts/build-service.sh gateway        # Build specific service (layer-aware)
./scripts/build-service.sh tasks          # Automatically finds service in product/tasks/

# Test
./scripts/test.sh                         # Run all tests
./scripts/test.sh gateway                 # Test specific service

# Run
./scripts/run.sh                          # Start all services with Docker Compose

# Utility
./scripts/restore.sh                      # Restore NuGet packages
./scripts/clean.sh                        # Clean build artifacts
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
- Uses **ProjectReference** locally (for instant builds without NuGet bootstrap)
- Uses **PackageReference** in CI (after Common is published to GitHub Packages)
- Can be deployed independently

This design allows services to:
- Be developed and tested in isolation with fast local builds
- Be built in CI using the published NuGet packages
- Be deployed to different servers/containers
- Use different versions of shared infrastructure (when needed)

### Common Library
- Located in `platform/common`
- Namespace: `DotnetAksMicroservices.Platform.Common`
- Assembly name: `Platform.Common`
- Provides:
  - Exception handling middleware (ExceptionHandlingMiddleware)
  - Structured logging middleware (StructuredLoggingMiddleware)
  - Service collection extensions (AddCommonServices)
  - Application builder extensions (UseExceptionHandling, UseStructuredLogging)
- Published to GitHub Packages by CI workflow
- Local references use ProjectReference for rapid development

## Namespace and Assembly Conventions

### Pattern

Services follow a three-part naming pattern aligned across namespaces, assemblies, and packages:

- **Namespace**: `DotnetAksMicroservices.[Layer].[Component][.SubFolder]`
  - Example: `DotnetAksMicroservices.Platform.Common.Extensions`
  - Example: `DotnetAksMicroservices.Product.Tasks`
- **Assembly name** (set via `<AssemblyName>` in `.csproj`): `[Layer].[Component]`
  - Example: `Platform.Common`
  - Example: `Product.Tasks`
- **PackageId** (NuGet identity): `[Layer].[Component]`
  - Example: `Platform.Common`
  - Example: `Product.Tasks`

### Project-to-Convention Mapping

| Project | Layer | Component | AssemblyName | Root Namespace | PackageId |
|---|---|---|---|---|---|
| Common | Platform | Common | `Platform.Common` | `DotnetAksMicroservices.Platform.Common` | `Platform.Common` |
| Observability | Platform | Observability | `Platform.Observability` | `DotnetAksMicroservices.Platform.Observability` | `Platform.Observability` |
| Gateway | Platform | Gateway | `Platform.Gateway` | `DotnetAksMicroservices.Platform.Gateway` | `Platform.Gateway` |
| Tasks | Product | Tasks | `Product.Tasks` | `DotnetAksMicroservices.Product.Tasks` | `Product.Tasks` |
| Users | Product | Users | `Product.Users` | `DotnetAksMicroservices.Product.Users` | `Product.Users` |
| Notifications | Product | Notifications | `Product.Notifications` | `DotnetAksMicroservices.Product.Notifications` | `Product.Notifications` |
| TasksCli | Product | TasksCli | `Product.TasksCli` | `DotnetAksMicroservices.Product.TasksCli` | `Product.TasksCli` |

### Docker ENTRYPOINT Matching

Dockerfiles reference the assembly name (from `<AssemblyName>`), not the project name:

```dockerfile
ENTRYPOINT ["dotnet", "Platform.Common.dll"]
ENTRYPOINT ["dotnet", "Product.Tasks.dll"]
```

### Sub-namespaces

Common library subdivides by folder for organizational clarity:

- `DotnetAksMicroservices.Platform.Common.Extensions` → `src/Extensions/`
- `DotnetAksMicroservices.Platform.Common.Middleware` → `src/Middleware/`

Product services map domain models to their own namespace at the service root:

- `DotnetAksMicroservices.Product.Tasks.TaskOrder` → `TaskOrder.cs` in service root
- `DotnetAksMicroservices.Product.Users.User` → `User.cs` in service root

## Code Quality

### Nullable Reference Types
All projects have `<Nullable>enable</Nullable>` - ensure proper null handling.

### Implicit Usings
All projects have `<ImplicitUsings>enable</ImplicitUsings>` - common namespaces are automatically included.

## Directory Structure

Services have a flat structure to minimize nesting:
- `.csproj` file at service root (e.g., `product/tasks/Tasks.csproj`)
- `Program.cs` at service root (for entry points)
- Model files at service root (e.g., `TaskOrder.cs`, `User.cs`)
- Source code files in `src/` subdirectory when appropriate (e.g., `src/TasksApiClient.cs`)
- `bin/` and `obj/` generated at build time

This reduces path depth and minimizes nesting.
