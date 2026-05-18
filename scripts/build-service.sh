#!/bin/bash

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(dirname "$SCRIPT_DIR")"

SERVICE=${1:?Error: Service name required. Usage: ./scripts/build-service.sh [gateway|tasks|users|notifications]}

if [ ! -d "$ROOT_DIR/services/$SERVICE" ]; then
    echo "❌ Service '$SERVICE' not found"
    echo "Available services: gateway, tasks, users, notifications"
    exit 1
fi

echo "🔨 Building $SERVICE service..."
dotnet build "$ROOT_DIR/services/$SERVICE/src/$(echo $SERVICE | sed 's/./\U&/')/$(echo $SERVICE | sed 's/./\U&/').csproj"

echo "✅ Build complete"
