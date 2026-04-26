# Research: Tic-Tac-Toe Web Game

## Project Structure

- **Decision**: Use a greenfield solution with `src/TicTacToe.Web` for the standalone Blazor WebAssembly app, `src/TicTacToe.Core` for pure game logic, and separate `tests/TicTacToe.Core.Tests` and `tests/TicTacToe.Web.Tests` projects.
- **Rationale**: The repository currently has no .NET source, so the feature must establish the baseline structure. Keeping rule evaluation outside the Razor component reduces UI complexity and enables fast, focused unit tests.
- **Alternatives considered**: A single Blazor project with all logic inside a page component would be smaller initially, but it would couple rendering and rules too tightly and make regression testing less precise.

## Dependency Strategy

- **Decision**: Keep implementation dependencies to framework-provided Blazor WebAssembly features only, and use `xUnit`, `FluentAssertions`, and `bUnit` as the only planned external test libraries.
- **Rationale**: The game is local, stateful only in memory, and has no forms, backend, or persistence. Additional state management, validation, mapping, or UI libraries would not add meaningful value.
- **Alternatives considered**: Fluxor, FluentValidation, or local-storage packages were rejected because they add surface area without solving a real requirement in this scope.

## Testing Approach

- **Decision**: Combine unit tests for game-rule evaluation with bUnit component tests for rendering and interaction behavior.
- **Rationale**: The most regression-prone path includes both rule calculation and UI updates. Unit tests cover all win, draw, and invalid-move logic quickly, while component tests prove that the board, status text, and restart control stay in sync with the game state.
- **Alternatives considered**: Browser E2E automation was rejected for initial scope because it adds substantial setup cost to validate a local-only interaction flow that component tests already cover well.

## Interface Contract

- **Decision**: Document a UI interaction contract rather than an API contract, because the application exposes no backend or network interface.
- **Rationale**: The feature is a user-facing standalone SPA. The meaningful contract is the board, status, and restart behavior that must remain stable across game states.
- **Alternatives considered**: OpenAPI or service-endpoint contracts were rejected because there is no external API. An internal C# service interface remains an implementation detail, not a cross-system contract.

## Accessibility And Performance

- **Decision**: Represent each board space as a native button, expose status changes through a live region, and rely on standard Blazor state updates without custom rendering optimizations.
- **Rationale**: Native buttons satisfy keyboard activation requirements with the least custom logic, and the board is small enough that additional render tuning is unnecessary. This keeps the experience accessible and aligned with the minimal-library constraint.
- **Alternatives considered**: Custom focus-management widgets and manual render suppression hooks were rejected because they add complexity without measurable benefit for a nine-cell board.