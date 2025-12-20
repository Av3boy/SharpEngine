# SharpEngine Monorepo

This repository is an npm workspace that hosts:
- AssetStore UI app: AssetStore/asset-store-ui
- Shared UI library: Shared/sharpengine-ui-shared

The shared library is consumed by the app via the workspace so both resolve a single React instance and the library builds automatically on install.

## Quick Start

```bash
# From repo root
npm install
npm run start
```

## Common Commands

```bash
# Build both packages
npm run build

# Run the app only
npm run start

# Run tests (app)
npm run test
```

## Project Structure

- App: AssetStore/asset-store-ui
  - Depends on the shared library via file/workspace: "sharpengine-ui-shared": "file:../../Shared/sharpengine-ui-shared"
- Library: Shared/sharpengine-ui-shared
  - Emits dist/ via TypeScript (`npm run build`)
  - Has `prepare` script to build on install
  - Declares React-related packages as peerDependencies to avoid duplicates

## Why Workspaces

Workspaces ensure:
- A single React/ReactDOM instance across app and library
- The library is symlinked and built automatically (`prepare`)
- Clean local development without `npm link`

## Troubleshooting

- If you see duplicate React or context state issues:
  1. Remove nested node_modules folders in the library
     ```bash
     # Windows PowerShell
     rd /s /q Shared\sharpengine-ui-shared\node_modules
     ```
  2. Reinstall from the repo root
     ```bash
     npm install
     ```

- If the app cannot import from the shared library, ensure the dependency is set to the package root (not `dist/`) and the library's `prepare` script exists.

## Notes

- The app renders a global error alert by subscribing to the shared `ErrorEventsProvider`. Components publish errors via `useErrorEvents().publish(...)`.
- The library exposes: `ErrorEventsProvider`, `useErrorEvents` from `sharpengine-ui-shared`.