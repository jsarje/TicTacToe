# Research: Server-Authoritative Match State

## Hosting Model

- **Decision**: Rebuild the front end as an ASP.NET Core Blazor Web App with Interactive WebAssembly enabled and a separate client project for the gameplay components.
- **Rationale**: The user explicitly wants the standalone WebAssembly template replaced with a server-capable architecture while still keeping the UI in a client project. The Blazor Web App template gives a server host, a browser-executed client, same-origin API access, and a straightforward migration path for the current components.
- **Alternatives considered**: An ASP.NET Core hosted Blazor WebAssembly solution would also satisfy the requirement, but the Blazor Web App template is the more current .NET direction and better matches the requested interactive WebAssembly mode. Keeping the standalone WASM app was rejected because it cannot own authoritative server state.

## Persistence Strategy

- **Decision**: Persist the authoritative match state in SQLite through EF Core inside the server host.
- **Rationale**: The feature now requires the server to update a database, survive refreshes, and own the official move and outcome state. SQLite keeps the infrastructure light for one active match per browser while still exercising the real persistence boundary the feature depends on.
- **Alternatives considered**: An in-memory cache was rejected because it loses state on application restart and does not satisfy the explicit database update requirement. A heavier database server was rejected because the scope is small and does not justify operational overhead.

## Browser Identity Boundary

- **Decision**: Issue and read a same-site anonymous browser cookie from the server to associate requests with one browser-owned active match.
- **Rationale**: The feature needs browser isolation without sign-in. A server-issued cookie keeps the browser-to-match association stable across refreshes and return visits in the same browser while avoiding client-generated identifiers becoming part of the trust boundary.
- **Alternatives considered**: Local storage or session storage identifiers would work functionally but put more of the identity contract under client control. ASP.NET Core authenticated users were rejected because the feature does not require accounts.

## Concurrency And Stale-State Handling

- **Decision**: Add a revision number to the persisted match and require move requests to include the last known revision.
- **Rationale**: The spec explicitly requires stale-board rejection and protection against near-simultaneous move requests. A simple optimistic concurrency approach lets the server reject outdated requests, return the newest official state, and avoid duplicate or partially applied moves.
- **Alternatives considered**: Blind last-write-wins updates were rejected because they can overwrite or hide concurrent decisions. Server-side locking without a revision contract was rejected because the client still needs a reliable way to understand stale state.

## API Shape

- **Decision**: Expose same-origin Minimal API endpoints for load, move, and restart operations and return full match snapshots after accepted or rejected domain operations.
- **Rationale**: The app only needs three explicit operations, and the client already uses `HttpClient`. Returning the full latest snapshot after a move attempt keeps the client thin and ensures the browser never has to infer official turn, win, or draw state locally.
- **Alternatives considered**: SignalR was rejected because the game is single-browser and turn-based, so persistent real-time transport adds complexity without real benefit. GraphQL and gRPC were rejected because the contract surface is small and fixed.

## Failure Handling Contract

- **Decision**: Distinguish domain rejections from infrastructure failures by returning normal decision payloads for occupied-square, wrong-turn, completed-match, and stale-revision cases, while using problem responses for temporary load or save failures.
- **Rationale**: Domain rejections are expected gameplay outcomes and should still return the official state. Temporary persistence failures are operational faults and should surface as retryable errors without pretending the move was evaluated successfully.
- **Alternatives considered**: Treating every rejection as an HTTP error was rejected because it complicates the client and obscures the fact that many rejected moves still have a valid latest match snapshot to render.

## Test And Performance Verification

- **Decision**: Keep rule-level unit tests in `TicTacToe.Core.Tests`, add server integration coverage with `WebApplicationFactory`, keep bUnit tests for async client behavior, and add timing guards around load and move integration paths.
- **Rationale**: The migration changes the testing pyramid: server transport and persistence are now as important as pure rule logic. The performance budget is modest enough that integration tests can catch obvious regressions while manual validation confirms the real app remains under the stated p95 target.
- **Alternatives considered**: Browser-only testing was rejected because it would make stale-state and failure-path coverage slower and less deterministic. Skipping timing checks was rejected because performance is a constitutional requirement for this repo.

## Deployment Impact

- **Decision**: Plan for an ASP.NET Core deployment target and stop treating GitHub Pages static hosting as the primary deployment model for this feature.
- **Rationale**: Server-owned persistence, cookies, and referee APIs require a live .NET host. The previous GitHub Pages deployment path was valid for the standalone WASM app but cannot satisfy server-authoritative behavior.
- **Alternatives considered**: Preserving GitHub Pages as the runtime target was rejected because static hosting cannot execute the required server validation or database persistence.