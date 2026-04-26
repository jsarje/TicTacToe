# TicTacToe

Standalone Blazor WebAssembly tic-tac-toe game with a small shared core library for game rules and an automated test suite for both rule logic and UI interaction.

## Prerequisites

- .NET 10 SDK
- A modern desktop or mobile browser

## Project Layout

- `TicTacToe.sln` contains the full solution.
- `src/TicTacToe.Core` contains game models and the rule engine.
- `src/TicTacToe.Web` contains the Blazor WebAssembly UI.
- `tests/TicTacToe.Core.Tests` contains xUnit rule tests.
- `tests/TicTacToe.Web.Tests` contains bUnit component tests.

## Restore And Test

```powershell
dotnet restore TicTacToe.sln
dotnet test TicTacToe.sln
```

## Run The App

```powershell
dotnet run --project src/TicTacToe.Web/TicTacToe.Web.csproj
```

Open the local URL printed by the app and validate the game flow.

## Manual Validation Checklist

Use the scenarios from `specs/001-tic-tac-toe-game/quickstart.md`:

- Play alternating X and O moves.
- Confirm occupied spaces do not change state.
- Verify win and draw outcomes.
- Restart from both active and completed games.
- Check keyboard play with Tab, Enter, and Space.
- Confirm the layout stays readable on mobile widths without horizontal scrolling.
- Confirm moves, status updates, and restart actions render immediately.