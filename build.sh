#!/bin/bash

# Build script for the microservices solution
# This script builds the Common package and places it in the local packages directory

set -e

echo "🔨 Building Common package..."

cd ./shared/src/Common
dotnet pack -o ../../packages --configuration Release

cd ../../../

echo "✅ Common package built and placed in ./packages"
echo "📦 To restore packages, run: dotnet restore"
