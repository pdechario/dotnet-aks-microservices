 Azure Free Tier Metrics Exporter — Task List

 Goal

 Build a monitoring stack that tracks all ~60 Azure free tier quotas, reports them to Grafana,
 and alerts at 80% of each limit. Starts with mock data (no Azure credentials needed).
 Runs in Docker Compose locally; designed to expand to AKS later.

 Architecture (for reference)

 - platform/observability/src/ — new .NET 8 Worker Service (Prometheus exporter)
 - deployment/docker/ — Prometheus + Grafana config files
 - Root docker-compose.yml — three new services added
 - All 60+ metrics encoded in appsettings.json, each Enabled: false by default

 ---
 Tasks

 - Task 1: Create the .csproj and solution entry
   - Create platform/observability/src/AzureMetricsExporter.csproj (net8.0, prometheus-net.AspNetCore, Azure.Identity,
 Azure.ResourceManager.Consumption, Serilog.AspNetCore, Serilog.Sinks.Console)
   - Add conditional ProjectReference to platform/common/src/Common.csproj (same pattern as other services)
   - Add project to root solution file
 - Task 2: Define metric models and config
   - Create Models/FreeTierMetric.cs — record with Id, Category, Resource, Limit, Unit, MeterName, Enabled
   - Create Options/AzureOptions.cs — SubscriptionId, TenantId, ClientId, ClientSecret, Enabled
   - Create appsettings.json — all ~60 free tier metrics (all Enabled: false), plus Azure: { Enabled: false }
 - Task 3: Implement IUsageService (mock + real)
   - Create Services/IUsageService.cs — interface: Task<IReadOnlyDictionary<string, double>> GetUsageAsync()
   - Create Services/MockUsageService.cs — returns 0.0 for every metric
   - Create Services/AzureUsageService.cs — queries Azure Consumption API, aggregates monthly usage by MeterName
 - Task 4: Implement the background metrics worker
   - Create Workers/MetricsCollectorWorker.cs — BackgroundService that refreshes every 60 min
   - Registers three Prometheus Gauges per enabled metric:
       - azure_free_tier_usage_current{id,category,resource,unit}
     - azure_free_tier_limit{id,category,resource,unit}
     - azure_free_tier_usage_ratio{id,category,resource,unit} (usage / limit)
 - Task 5: Wire up Program.cs
   - Minimal ASP.NET Core host
   - Register Serilog (console, same config as other services)
   - Register IUsageService — MockUsageService or AzureUsageService based on AzureOptions:Enabled
   - Register MetricsCollectorWorker as IHostedService
   - Map /metrics (prometheus-net), /health (AddHealthChecks)
 - Task 6: Create Dockerfile.Observability
   - Follow project naming convention: Dockerfile.Observability in platform/observability/
   - Pre-publish pattern: expects artifacts at bin/Release/net8.0/publish/
 - Task 7: Prometheus config
   - Create deployment/docker/prometheus.yml
   - Scrape job for observability-exporter:8080 every 5 minutes
 - Task 8: Grafana provisioning — data source + dashboard provider
   - Create deployment/docker/grafana/provisioning/datasources/prometheus.yml
   - Create deployment/docker/grafana/provisioning/dashboards/dashboards.yml
 - Task 9: Grafana dashboard JSON
   - Create deployment/docker/grafana/provisioning/dashboards/azure-free-tier.json
   - Panels: stat panels per metric (green→yellow→red at 80%), bar gauges by category, full table view
 - Task 10: Grafana alert rules
   - Create deployment/docker/grafana/provisioning/alerting/azure-free-tier-alerts.yml
   - Alert: azure_free_tier_usage_ratio > 0.8 for any metric
   - Human-readable message with category + resource name + percentage
 - Task 11: Update root docker-compose.yml
   - Add observability-exporter service (port 5004)
   - Add prometheus service (port 9090)
   - Add grafana service (port 3000, admin/admin)
   - Add grafana_data named volume
 - Task 12: Verify end-to-end
   - dotnet build — solution builds cleanly
   - docker-compose up --build — all 7 services start
   - curl localhost:5004/metrics — Prometheus text with azure_free_tier_* gauges
   - localhost:9090 — Prometheus shows scraped metrics
   - localhost:3000 — Grafana dashboard loads, all panels visible, alert rule present