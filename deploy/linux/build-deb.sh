#!/bin/bash
# Script para empacotamento .deb do TaskFlow NoteManager
# Requer: dotnet publish na pasta publish/

set -e

APP_NAME="taskflow-notemanager"
VERSION="1.0.0"
BUILD_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECT_DIR="$BUILD_DIR/../../src/TaskFlow.Presentation"
PUBLISH_DIR="$PROJECT_DIR/bin/Release/net8.0/linux-x64/publish"
DEB_DIR="$BUILD_DIR/${APP_NAME}_${VERSION}"

echo "=== Building .deb package for $APP_NAME v$VERSION ==="

# Publish the app
echo "Publishing the application..."
dotnet publish "$PROJECT_DIR" -c Release -r linux-x64 --self-contained true -o "$PUBLISH_DIR"

# Create .deb directory structure
echo "Creating .deb structure..."
rm -rf "$DEB_DIR"
mkdir -p "$DEB_DIR/DEBIAN"
mkdir -p "$DEB_DIR/usr/local/bin/taskflow"
mkdir -p "$DEB_DIR/usr/share/applications"
mkdir -p "$DEB_DIR/usr/share/icons/hicolor/128x128/apps"

# Copy files
cp "$BUILD_DIR/DEBIAN/control" "$DEB_DIR/DEBIAN/"
cp -r "$PUBLISH_DIR/"* "$DEB_DIR/usr/local/bin/taskflow/"
cp "$BUILD_DIR/taskflow.desktop" "$DEB_DIR/usr/share/applications/"

# Set permissions
chmod -R 755 "$DEB_DIR/DEBIAN"
chmod 755 "$DEB_DIR/usr/local/bin/taskflow/TaskFlow.Presentation"

# Build the package
echo "Building .deb package..."
mkdir -p "$BUILD_DIR/../../dist/linux"
dpkg-deb --build "$DEB_DIR" "$BUILD_DIR/../../dist/linux/${APP_NAME}_${VERSION}_amd64.deb"

echo "=== Done: dist/linux/${APP_NAME}_${VERSION}_amd64.deb ==="
rm -rf "$DEB_DIR"
