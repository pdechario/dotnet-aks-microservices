#!/bin/bash

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(dirname "$SCRIPT_DIR")"

echo "📦 Packing NuGet packages..."
cd "$ROOT_DIR"
dotnet pack -o ./packages --configuration Release

echo "🔨 Building entire solution..."
dotnet build

echo "✅ Build complete"
