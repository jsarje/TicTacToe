# Quickstart: Tic-Tac-Toe Web Game

## Prerequisites

- .NET 10 SDK installed
- A browser supported by standalone Blazor WebAssembly

## 1. Scaffold The Solution

From the repository root:

```powershell
dotnet new sln -n TicTacToe
dotnet new blazorwasm -n TicTacToe.Web -o src/TicTacToe.Web -f net10.0
dotnet new classlib -n TicTacToe.Core -o src/TicTacToe.Core -f net10.0
dotnet new xunit -n TicTacToe.Core.Tests -o tests/TicTacToe.Core.Tests -f net10.0
dotnet new xunit -n TicTacToe.Web.Tests -o tests/TicTacToe.Web.Tests -f net10.0
dotnet sln add src/TicTacToe.Web/TicTacToe.Web.csproj
dotnet sln add src/TicTacToe.Core/TicTacToe.Core.csproj
dotnet sln add tests/TicTacToe.Core.Tests/TicTacToe.Core.Tests.csproj
dotnet sln add tests/TicTacToe.Web.Tests/TicTacToe.Web.Tests.csproj
dotnet add src/TicTacToe.Web/TicTacToe.Web.csproj reference src/TicTacToe.Core/TicTacToe.Core.csproj
dotnet add tests/TicTacToe.Core.Tests/TicTacToe.Core.Tests.csproj reference src/TicTacToe.Core/TicTacToe.Core.csproj
dotnet add tests/TicTacToe.Web.Tests/TicTacToe.Web.Tests.csproj reference src/TicTacToe.Web/TicTacToe.Web.csproj
dotnet add tests/TicTacToe.Core.Tests/TicTacToe.Core.Tests.csproj package FluentAssertions
dotnet add tests/TicTacToe.Web.Tests/TicTacToe.Web.Tests.csproj package FluentAssertions
dotnet add tests/TicTacToe.Web.Tests/TicTacToe.Web.Tests.csproj package bunit
```

## 2. Implement The Feature

- Add the core game session model and move-evaluation service under `src/TicTacToe.Core`.
- Build the single-screen board UI under `src/TicTacToe.Web` with native buttons, a live status region, and a restart control.
- Keep styling responsive so the full interaction fits on common mobile widths without horizontal scrolling.

## 3. Run Tests

```powershell
dotnet test
```

The test suite should include unit coverage for rule evaluation and bUnit coverage for board rendering, status updates, invalid move handling, and restart behavior.

## 4. Run The App

```powershell
dotnet run --project src/TicTacToe.Web/TicTacToe.Web.csproj
```

Open the local URL emitted by the Blazor app and manually validate:

- Turn alternation for X then O
- Occupied-space clicks are ignored
- All win conditions and draw completion
- Restart from active and completed games
- Moves, status updates, and restart actions render without perceptible delay
- Keyboard play with Tab, Enter, and Space
- Mobile-width layout without horizontal scrolling