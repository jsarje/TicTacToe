# TicTacToe

Hosted Blazor tic-tac-toe game with a server-authoritative referee, SQLite-backed browser-scoped match persistence, a WebAssembly client, and automated unit, bUnit, and integration coverage.

## Run

Use .NET 10 preview SDK.

```powershell
dotnet test TicTacToe.sln
dotnet run --project src/TicTacToe.Web/TicTacToe.Web.csproj
```

The app now requires an ASP.NET Core host and no longer targets static GitHub Pages hosting.

## Approach

Note used GPT-5.4 model for all interactions with Github Copilot

- Pulled in instructions for C# and Blazor from https://github.com/github/awesome-copilot to help with code styling and best practices
- Initialised spec-kit https://github.com/github/spec-kit
- /speckit.constitution with their recommended prompt
- /speckit.spec Build a web application that hosts a playable tic-tac-toe game. It should include two-player local gameplay, win and draw detection, a simple and clean user interface, the ability to restart the game, and clear indication of which players turn it is.
- /speckit.plan The application uses NET10, C#, Blazor, with minimal number of libraries
- /speckit.tasks
- /speckit.implement
- Manually verified functionality was as expected but noticed layout was a bit off.
- Raised issue for the layout and assigned to copilot cloud agent
- Reviewed and merged the PR to fix layout
- Migrated the app to a hosted architecture so the server can own validation, persistence, and browser identity.

## Enhancements

- Introduce playwright tests covering main paths
- Look at implementing remote multiplayer functionality