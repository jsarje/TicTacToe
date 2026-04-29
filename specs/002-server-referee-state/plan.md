# Implementation Plan: Server-Authoritative Match State

**Branch**: `[002-server-referee-state]` | **Date**: 2026-04-29 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/002-server-referee-state/spec.md`

## Summary

Replace the standalone Blazor WebAssembly front end with an ASP.NET Core Blazor Web App that uses Interactive WebAssembly, move the gameplay UI into the generated client project, and move move-validation plus win/draw evaluation behind server-owned APIs. The server will persist one official match per browser in SQLite through EF Core, identify the browser through a same-site anonymous cookie, reject stale or invalid moves using revision-aware validation, and return the latest confirmed state for every load, move, rejection, and restart path.

## Technical Context

**Language/Version**: C# 14 on .NET 10  
**Primary Dependencies**: ASP.NET Core Blazor Web App with Interactive WebAssembly, ASP.NET Core Minimal APIs, Entity Framework Core with SQLite, xUnit, FluentAssertions, bUnit, ASP.NET Core integration testing via `WebApplicationFactory`  
**Storage**: SQLite database in the server host for persisted browser-owned match state  
**Testing**: xUnit for domain and persistence unit tests, FluentAssertions for assertions, bUnit for client component tests, ASP.NET Core integration tests for HTTP endpoints and persistence flows  
**Target Platform**: ASP.NET Core-hosted Blazor Web App for modern desktop and mobile browsers; requires deploy target capable of running .NET rather than static GitHub Pages hosting  
**Project Type**: Web application with server host, Interactive WebAssembly client, shared core library, and persistence layer  
**Performance Goals**: Current-match load and move confirmation complete within 1 second for 95% of requests under normal operating conditions  
**Constraints**: Preserve the existing one-screen gameplay model, keep the board keyboard operable, return the last confirmed server state after accepted or rejected moves, persist one isolated active match per browser, and support recoverable load/save failure states  
**Scale/Scope**: One active match per browser, low write concurrency, one primary page, three UI regions, and a small authoritative state model

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Initial Gate Review: PASS**

- **Code Quality**: Keep game rules in `src/TicTacToe.Core`, introduce a narrow server application layer in `src/TicTacToe.Web` for browser identity, persistence, and API endpoints, and move the existing gameplay Razor components into `src/TicTacToe.Web.Client`. This removes rule decisions from `Home.razor.cs` instead of duplicating logic on both sides.
- **Testing**: Preserve the existing core-rule tests, add unit tests for persistence and stale-state handling, add API integration tests for load, move, restart, and failure paths, and keep bUnit coverage for async client rendering, loading indicators, rejection messaging, and accessibility behavior. No critical path is intentionally left untested.
- **UX Consistency**: Reuse the existing single-screen board, status, and restart interaction model. The only approved additions are loading and failure feedback that preserve the last confirmed official state and communicate rejected moves clearly.
- **Performance**: The feature has an explicit budget of less than 1 second p95 for match load and move confirmation. Verification will combine automated integration-test timing guards against the hot path with manual validation against the real SQLite-backed app.
- **Increment Size**: Deliver in reviewable slices: scaffold the hosted app and client split, introduce persistence plus browser identity, add server referee endpoints, convert the client to API-driven gameplay, then finish tests and performance validation.

## Project Structure

### Documentation (this feature)

```text
specs/002-server-referee-state/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── server-referee-api.md
└── tasks.md
```

### Source Code (repository root)

```text
src/
├── TicTacToe.Core/
│   ├── Models/
│   └── Services/
├── TicTacToe.Web/
│   ├── Components/
│   ├── Data/
│   ├── Endpoints/
│   ├── Infrastructure/
│   └── Program.cs
└── TicTacToe.Web.Client/
    ├── Components/
    ├── Pages/
    ├── Services/
    └── wwwroot/

tests/
├── TicTacToe.Core.Tests/
├── TicTacToe.Web.Tests/
└── TicTacToe.Web.IntegrationTests/
```

**Structure Decision**: Use a hosted Blazor Web App layout so the server can own persistence and rule enforcement while the browser remains an Interactive WebAssembly client. `src/TicTacToe.Core` remains the shared domain and referee logic library, `src/TicTacToe.Web` becomes the server host plus persistence boundary, and `src/TicTacToe.Web.Client` becomes the only place that renders the board, status, and restart experience. A dedicated integration-test project is warranted because HTTP contract, persistence, and concurrency behavior are now a first-class regression surface.

## Post-Design Constitution Check

**Re-check After Phase 1 Design: PASS**

- **Code Quality**: The design keeps a clean boundary between domain rules, persistence, transport contracts, and client rendering. Browser identity and revision checks are explicit design elements rather than ad hoc state hidden in components.
- **Testing**: The data model and API contract preserve unit, integration, and component coverage for every required acceptance path, including refresh restoration, stale-state rejection, and temporary persistence failures.
- **UX Consistency**: The quickstart and API contract keep the existing one-page interaction model intact while defining consistent loading, rejection, and recoverable failure states.
- **Performance**: SQLite plus a compact match row and small payload responses keep the hot path small; the design includes automated and manual verification against the defined latency budget.
- **Increment Size**: The artifact set supports small implementation slices with clear checkpoints around scaffolding, persistence, API behavior, client migration, and validation.

## Complexity Tracking

No constitution violations or waivers are required for this feature.
