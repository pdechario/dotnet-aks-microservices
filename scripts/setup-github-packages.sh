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

# Update nuget.config
cd "$ROOT_DIR"

cat > nuget.config <<EOF
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="github" value="https://nuget.pkg.github.com/${GITHUB_USERNAME}/index.json" protocolVersion="3" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
  </packageSources>
  <packageSourceCredentials>
    <github>
      <add key="Username" value="${GITHUB_USERNAME}" />
      <add key="ClearTextPassword" value="${GITHUB_TOKEN}" />
    </github>
  </packageSourceCredentials>
</configuration>
EOF

echo "✅ GitHub Packages configured!"
echo "   Username: $GITHUB_USERNAME"
echo "   GitHub Packages Feed: https://nuget.pkg.github.com/${GITHUB_USERNAME}/index.json"
echo ""
echo "📝 Your nuget.config has been updated with your GitHub credentials."
echo "   (This file contains your token - keep it secure!)"
