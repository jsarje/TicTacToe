# Contract: Server Referee API

## Overview

- The client never decides whether a move is valid, whether a move wins the game, or whether the match is complete.
- The server owns the official match state and returns the latest snapshot after every load, accepted move, rejected move, and restart.
- The browser-to-match association is established by a same-site anonymous cookie managed by the server.

## Identity Boundary

- Cookie name: `ttt-browser`
- Scope: Same-site requests to the TicTacToe server host
- Persistence: Persistent cookie with a 30-day lifetime so the same browser can close and reopen without losing its browser association during normal return visits
- Responsibility: The server issues the cookie when a browser without identity first loads the app or calls an API endpoint, and renews its expiry on successful requests
- Client behavior: The client does not send a browser identifier in the JSON payload; it relies on the browser cookie automatically
- Expiry behavior: If the cookie is missing, cleared, or expired, the server creates a new browser identity and may create a new official match

## DTOs

### MatchSnapshotDto

```json
{
  "matchId": "3a62ce6f-0ab4-4a2f-bd20-8dc353dfd620",
  "revision": 4,
  "board": ["X", "O", null, null, "X", null, "O", null, null],
  "currentPlayer": "X",
  "result": "InProgress",
  "winningLine": null,
  "isBoardInteractive": true,
  "statusMessage": "Player X to move.",
  "lastUpdatedUtc": "2026-04-29T12:00:00Z"
}
```

### MoveRequestDto

```json
{
  "spaceIndex": 4,
  "actingPlayer": "X",
  "expectedRevision": 4
}
```

### MoveDecisionDto

```json
{
  "accepted": false,
  "rejectionReason": "OccupiedSquare",
  "userMessage": "Square 4 is already occupied.",
  "match": {
    "matchId": "3a62ce6f-0ab4-4a2f-bd20-8dc353dfd620",
    "revision": 4,
    "board": ["X", "O", null, null, "X", null, "O", null, null],
    "currentPlayer": "X",
    "result": "InProgress",
    "winningLine": null,
    "isBoardInteractive": true,
    "statusMessage": "Player X to move.",
    "lastUpdatedUtc": "2026-04-29T12:00:00Z"
  }
}
```

### RestartDecisionDto

```json
{
  "userMessage": "New game started. Player X to move.",
  "match": {
    "matchId": "f7c84c74-e253-46c7-b2ea-659028e2f7fd",
    "revision": 0,
    "board": [null, null, null, null, null, null, null, null, null],
    "currentPlayer": "X",
    "result": "InProgress",
    "winningLine": null,
    "isBoardInteractive": true,
    "statusMessage": "Player X to move.",
    "lastUpdatedUtc": "2026-04-29T12:01:00Z"
  }
}
```

## Endpoints

### `GET /api/match`

- Purpose: Load the latest official match for the current browser or create a fresh one when none exists
- Success: `200 OK` with `MatchSnapshotDto`
- Failure: `503 Service Unavailable` with problem details when the server cannot load or create the match state

### `POST /api/match/moves`

- Purpose: Validate and apply a requested move against the official match state
- Request body: `MoveRequestDto`
- Success: `200 OK` with `MoveDecisionDto`
- Domain rejection cases returned in the normal decision body:
  - `OccupiedSquare`
  - `WrongTurn`
  - `MatchComplete`
  - `StaleRevision`
- Infrastructure failure: `503 Service Unavailable` with problem details when the server cannot persist or retrieve the match

### `POST /api/match/restart`

- Purpose: Replace the current browser-owned match with a fresh official session
- Request body: none
- Success: `200 OK` with `RestartDecisionDto`
- Failure: `503 Service Unavailable` with problem details when the server cannot persist the replacement match

## Behavioral Rules

- The server must persist an accepted move before returning an updated snapshot.
- Rejected domain move attempts do not change the persisted `revision`.
- A stale move request returns the newest official match snapshot so the client can immediately refresh its UI.
- A load request that arrives while another move is still being confirmed returns the last committed official snapshot rather than a partially applied or duplicated move.
- Concurrent move requests for the same browser-owned match may result in only one accepted write for a given revision; later conflicting requests must be rejected against the updated official state.
- Once the result is `XWins`, `OWins`, or `Draw`, additional move requests remain rejected until restart succeeds.
- Restart always returns a fresh empty board with Player X active.

## Error Contract

- Temporary load or save failures use RFC 9457 problem details.
- Problem responses should include a retry-friendly detail message suitable for conversion into UI copy.
- When an infrastructure failure happens after the client already has a confirmed snapshot, the client continues showing that last confirmed snapshot until a retry succeeds.