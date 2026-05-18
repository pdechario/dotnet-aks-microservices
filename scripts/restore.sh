#!/bin/bash

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(dirname "$SCRIPT_DIR")"

echo "📦 Restoring NuGet packages..."
cd "$ROOT_DIR"
dotnet restore

echo "✅ Restore complete"
