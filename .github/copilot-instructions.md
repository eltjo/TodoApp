# Copilot Instructions — TodoApp

## Project

.NET MAUI desktop todo app for macOS. Uses `gh` CLI for GitHub integrations (issues, releases, workflow status). Local todo data is persisted on device; GitHub is used for developer workflow, not as a data backend.

## Build & Run

```bash
# Build for macOS (Mac Catalyst)
dotnet build -f net10.0-maccatalyst

# Run on macOS
dotnet run -f net10.0-maccatalyst

# Build release
dotnet publish -f net10.0-maccatalyst -c Release
```

## gh CLI Integrations

This project uses `gh` as a first-class tool. Key patterns used throughout the codebase:

```bash
# Check auth status (used in app startup diagnostic)
gh auth status

# Create a bug report issue
gh issue create --label bug --title "..." --body "..."

# List open issues
gh issue list --state open

# View CI status
gh run list --limit 5

# Create a release
gh release create v1.0.0 --generate-notes
```

When adding new GitHub integrations, prefer `gh` CLI commands (via `Process.Start`) over direct REST/GraphQL API calls.

## Architecture

- **`MainPage.xaml` / `MainPage.xaml.cs`** — Main UI: todo list view and add/complete/delete actions
- **`AppShell.xaml`** — Shell navigation (single-page for now, extend here for multi-page)
- **`MauiProgram.cs`** — App bootstrap, DI registration
- **`Platforms/MacCatalyst/`** — macOS-specific entry point and entitlements

Todo items are stored locally (JSON file in `AppDataDirectory`). The `TodoService` (to be created under `Services/`) owns all persistence logic — UI never reads/writes storage directly.

## Conventions

- Target framework for macOS is `net10.0-maccatalyst` — always specify `-f net10.0-maccatalyst` when building/running
- XAML for layout, code-behind only for event wiring; business logic goes in `Services/`
- New pages go in `Pages/`, new models in `Models/`, services in `Services/`
- Use `Shell.Current.GoToAsync("//routename")` for navigation
- `gh` CLI calls from C# use `System.Diagnostics.Process` with `RedirectStandardOutput = true`
