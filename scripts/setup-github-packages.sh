#!/bin/bash

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(dirname "$SCRIPT_DIR")"

echo "🔐 Configuring GitHub Packages authentication..."

# Get GitHub token from gh CLI
GITHUB_TOKEN=$(gh auth token)
if [ -z "$GITHUB_TOKEN" ]; then
  echo "❌ Could not get GitHub token. Make sure you're authenticated with 'gh auth login'"
  exit 1
fi

# Get GitHub username
GITHUB_USERNAME=$(gh api user -q .login)
if [ -z "$GITHUB_USERNAME" ]; then
  echo "❌ Could not get GitHub username"
  exit 1
fi

cd "$ROOT_DIR"

sed -e "s/GITHUB_USERNAME_PLACEHOLDER/${GITHUB_USERNAME}/g" \
    -e "s/GITHUB_TOKEN_PLACEHOLDER/${GITHUB_TOKEN}/g" \
    nuget.template.config > nuget.config

echo "✅ GitHub Packages configured!"
echo "   Username: $GITHUB_USERNAME"
echo "   GitHub Packages Feed: https://nuget.pkg.github.com/${GITHUB_USERNAME}/index.json"
echo ""
echo "📝 For Docker builds, export your token:"
echo "   export NUGET_TOKEN=\$(gh auth token)"
echo "   docker-compose up --build"
