# Copilot Instructions — TodoApp

## Project

.NET 10 Blazor Server todo app, running locally in the browser. Uses `gh` CLI for GitHub integrations (list issues, create issues, releases, workflow status). Local todo data is persisted on disk; GitHub is used for developer workflow features.

## Build & Run

```bash
# Run locally (opens on http://localhost:5291)
dotnet run --launch-profile http

# Build
dotnet build
```

## Architecture

```
Components/
  Pages/          # Blazor pages (@page "/route")
    Issues.razor  # GitHub Issues viewer + create form
    Home.razor    # Landing page
  Layout/
    MainLayout.razor  # Nav bar + page wrapper
Models/
  GitHubIssue.cs  # Issue model returned by GhService
Services/
  GhService.cs    # All gh CLI calls via System.Diagnostics.Process
```

- **`GhService`** is registered as a singleton. All `gh` commands go through `RunGhAsync()` — never call `Process.Start` directly in a component.
- **`GhService.CreateIssueAsync(title, body)`** runs `gh issue create` and returns the new issue URL.
- **`GhService.GetIssuesAsync(state)`** runs `gh issue list --json` and deserializes to `List<GitHubIssue>`.
- Components inject `GhService` and call it directly — no intermediate ViewModel layer.

## gh CLI Integrations

When adding new `gh` integrations, add a method to `GhService` and call `RunGhAsync()`:

```bash
# Patterns already in use:
gh issue list --repo owner/repo --state open --limit 50 --json number,title,state,url,body
gh issue create --repo owner/repo --title "..." --body "..."

# Useful patterns to add:
gh run list --limit 5 --json name,status,conclusion
gh release create v1.0.0 --generate-notes
gh issue close <number>
```

Always use `--json` flags when output needs to be parsed. Use `EscapeArg()` in `GhService` when interpolating user input into CLI arguments.

## Conventions

- New pages go in `Components/Pages/`, new models in `Models/`, services in `Services/`
- `@rendermode InteractiveServer` is required on every interactive page
- Loading/error/success state pattern in components: use `bool Loading`, `string? ErrorMessage`, `string? SuccessMessage` fields
- After a mutation (create/close/etc.), always call `LoadIssues()` to refresh the list
- String escaping for `gh` args: use `GhService.EscapeArg()` — never interpolate raw user input
