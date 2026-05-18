#!/bin/bash

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(dirname "$SCRIPT_DIR")"

SERVICE=${1:-all}

test_service() {
    local svc=$1
    echo "🧪 Testing $svc service..."
    if [ -d "$ROOT_DIR/services/$svc/tests" ]; then
        dotnet test "$ROOT_DIR/services/$svc/tests/" --verbosity normal
    else
        echo "⚠️  No tests directory for $svc"
    fi
}

if [ "$SERVICE" = "all" ]; then
    echo "🧪 Running all tests..."
    cd "$ROOT_DIR"
    dotnet test
else
    if [ ! -d "$ROOT_DIR/services/$SERVICE" ]; then
        echo "❌ Service '$SERVICE' not found"
        echo "Usage: ./scripts/test.sh [gateway|tasks|users|notifications|all]"
        exit 1
    fi
    test_service "$SERVICE"
fi

echo "✅ Tests complete"
