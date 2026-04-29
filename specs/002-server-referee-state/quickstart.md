# Quickstart: Server-Authoritative Match State

## Prerequisites

- .NET 10 SDK installed
- A local runtime capable of hosting ASP.NET Core and SQLite
- A browser supported by Blazor Web Apps with Interactive WebAssembly

## 1. Restructure The Web Front End

From the repository root, replace the standalone WebAssembly app with a Blazor Web App that enables Interactive WebAssembly:

```powershell
dotnet new blazor -n TicTacToe.Web -o src/TicTacToe.Web --interactivity WebAssembly -f net10.0
```

Then align the solution to the hosted structure:

- Keep `src/TicTacToe.Core` as the shared rules library.
- Use the generated client project as `src/TicTacToe.Web.Client` and move the existing gameplay components, page logic, and related styles into that client project.
- Keep `src/TicTacToe.Web` as the server host and API entry point.
- Add a new integration test project for server endpoints and persistence.

Suggested solution updates:

```powershell
dotnet new xunit -n TicTacToe.Web.IntegrationTests -o tests/TicTacToe.Web.IntegrationTests -f net10.0
dotnet sln add src/TicTacToe.Web/TicTacToe.Web.csproj
dotnet sln add src/TicTacToe.Web.Client/TicTacToe.Web.Client.csproj
dotnet sln add tests/TicTacToe.Web.IntegrationTests/TicTacToe.Web.IntegrationTests.csproj
dotnet add tests/TicTacToe.Web.IntegrationTests/TicTacToe.Web.IntegrationTests.csproj package Microsoft.AspNetCore.Mvc.Testing
```

## 2. Add Persistence And Server Referee Endpoints

Add the server-side packages needed for persistence and migrations:

```powershell
dotnet add src/TicTacToe.Web/TicTacToe.Web.csproj package Microsoft.EntityFrameworkCore.Sqlite
dotnet add src/TicTacToe.Web/TicTacToe.Web.csproj package Microsoft.EntityFrameworkCore.Design
```

Implement the feature in four layers:

- Keep pure move and win/draw evaluation in `src/TicTacToe.Core`.
- Add SQLite-backed match persistence, browser cookie handling, and stale-revision checks in `src/TicTacToe.Web`.
- Expose load, move, and restart endpoints from the server host.
- Replace direct `IGameEngine` calls in the client page with async API calls that load the current official match, submit move requests, and render loading or failure states.

## 3. Create The Database And Run Tests

Create the first migration and execute the test suite:

```powershell
dotnet ef migrations add InitialServerMatchState --project src/TicTacToe.Web/TicTacToe.Web.csproj
dotnet test
```

The automated suite should cover:

- Game-rule unit tests in `tests/TicTacToe.Core.Tests`
- Persistence and API integration tests in `tests/TicTacToe.Web.IntegrationTests`
- Async component behavior, accessibility states, and rejection messaging in `tests/TicTacToe.Web.Tests`

## 4. Run The Hosted App

```powershell
dotnet run --project src/TicTacToe.Web/TicTacToe.Web.csproj
```

Open the local URL emitted by the server host and manually validate:

- Refresh restores the same browser's official board state, turn, and outcome
- A valid move is confirmed only after the server responds
- Occupied-square, wrong-turn, completed-match, and stale-state attempts show clear feedback and keep the latest official board visible
- Restart replaces the browser's prior match with a fresh board for Player X
- Load, move, and restart remain within the stated 1 second budget under normal conditions

## 5. Deployment Note

This feature no longer targets static GitHub Pages hosting. The application now requires an ASP.NET Core deployment target that can run the server host, set cookies, and access the SQLite database.