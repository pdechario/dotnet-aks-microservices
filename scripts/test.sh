#!/bin/bash

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(dirname "$SCRIPT_DIR")"

SERVICE=${1:-all}

get_layer() {
    [ -d "$ROOT_DIR/platform/$1" ] && echo "platform" || echo "product"
}

test_service() {
    local svc=$1
    local layer=$(get_layer "$svc")
    echo "🧪 Testing $svc service..."
    if [ -d "$ROOT_DIR/$layer/$svc/tests" ]; then
        dotnet test "$ROOT_DIR/$layer/$svc/tests/" --verbosity normal
    else
        echo "⚠️  No tests directory for $svc"
    fi
}

if [ "$SERVICE" = "all" ]; then
    echo "🧪 Running all tests..."
    cd "$ROOT_DIR"
    dotnet test
else
    local layer=$(get_layer "$SERVICE")
    if [ ! -d "$ROOT_DIR/$layer/$SERVICE" ]; then
        echo "❌ Service '$SERVICE' not found"
        echo "Usage: ./scripts/test.sh [gateway|tasks|users|notifications|taskscli|all]"
        exit 1
    fi
    test_service "$SERVICE"
fi

echo "✅ Tests complete"
