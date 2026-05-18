#!/bin/bash

cat << 'EOF'
📚 Dotnet AKS Microservices - Build Scripts

BUILD COMMANDS:
  ./scripts/build.sh                 Build entire solution + Common package
  ./scripts/build-service.sh NAME    Build specific service [gateway|tasks|users|notifications]

TEST COMMANDS:
  ./scripts/test.sh                  Run all tests
  ./scripts/test.sh SERVICE          Run tests for specific service [gateway|tasks|users|notifications]

RUN COMMANDS:
  ./scripts/run.sh                   Start all services with Docker Compose

UTILITY COMMANDS:
  ./scripts/restore.sh               Restore NuGet packages
  ./scripts/clean.sh                 Clean build artifacts
  ./scripts/help.sh                  Show this help message

EXAMPLES:
  ./scripts/build.sh                 # Build everything
  ./scripts/build-service.sh tasks   # Build only Tasks service
  ./scripts/test.sh gateway          # Test only Gateway service
  ./scripts/test.sh                  # Run all tests
  ./scripts/run.sh                   # Start services with docker-compose

EOF
