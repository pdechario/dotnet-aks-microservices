#!/bin/bash

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(dirname "$SCRIPT_DIR")"

SERVICE=${1:?Error: Service name required. Usage: ./scripts/build-service.sh [gateway|tasks|users|notifications|taskscli]}

get_layer() {
    [ -d "$ROOT_DIR/platform/$1" ] && echo "platform" || echo "product"
}

LAYER=$(get_layer "$SERVICE")

# Map service names to PascalCase project names
case "$SERVICE" in
    gateway)      PASCAL="Gateway" ;;
    tasks)        PASCAL="Tasks" ;;
    users)        PASCAL="Users" ;;
    notifications) PASCAL="Notifications" ;;
    taskscli)     PASCAL="TasksCli" ;;
    *)            echo "❌ Unknown service '$SERVICE'"; exit 1 ;;
esac

if [ ! -d "$ROOT_DIR/$LAYER/$SERVICE" ]; then
    echo "❌ Service '$SERVICE' not found"
    echo "Available services: gateway (platform), tasks, users, notifications, taskscli (product)"
    exit 1
fi

echo "🔨 Building $SERVICE service..."
dotnet build "$ROOT_DIR/$LAYER/$SERVICE/$PASCAL.csproj"

echo "✅ Build complete"
