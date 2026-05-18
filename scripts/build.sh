#!/bin/bash

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(dirname "$SCRIPT_DIR")"

cd "$ROOT_DIR"

echo "📦 Building Common and creating local package..."
mkdir -p packages
dotnet build platform/common/src/Common.csproj -c Release
dotnet pack platform/common/src/Common.csproj -c Release -o packages --no-build

echo "🔄 Restoring packages..."
dotnet restore

echo "🔨 Building entire solution..."
dotnet build --configuration Release

echo "✅ Build complete"
echo ""
echo "📦 Local packages available in ./packages/"
echo "   When you push to main, GitHub Actions will automatically publish to GitHub Packages."
