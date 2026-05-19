#!/bin/bash

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(dirname "$SCRIPT_DIR")"

cd "$ROOT_DIR"

echo "🔄 Restoring packages..."
dotnet restore

echo "🔨 Building entire solution..."
dotnet build --configuration Release

echo "📦 Publishing services for Docker..."
dotnet publish product/tasks/src/Tasks.csproj -c Release
dotnet publish product/users/src/Users.csproj -c Release
dotnet publish product/notifications/src/Notifications.csproj -c Release
dotnet publish platform/gateway/src/Gateway.csproj -c Release

echo "✅ Build complete"
echo ""
echo "To run services with Docker: docker-compose up --build"
echo "To run tests:                ./scripts/test.sh"
