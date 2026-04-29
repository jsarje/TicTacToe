# Data Model: Server-Authoritative Match State

## Entity: BrowserMatchSession

**Purpose**: Represents the authoritative persisted match owned by one browser and returned by the server after every load, move, rejection, and restart flow.

| Field | Type | Description | Validation |
|-------|------|-------------|------------|
| MatchId | `Guid` | Unique identifier for the current persisted match instance | Must be regenerated when a browser starts a replacement match through restart |
| BrowserId | `string` | Stable browser-scoped identifier sourced from the server-issued cookie | Required and unique per active browser-owned match |
| Board | `BoardSpace[9]` or equivalent serialized representation | Ordered set of the nine current board positions | Must always contain exactly 9 entries |
| CurrentPlayer | `PlayerMark` | Player allowed to make the next valid move while the match is active | Must be `X` or `O` when `Result` is `InProgress` |
| Result | `GameResult` | Official outcome of the persisted match | Must be one of `InProgress`, `XWins`, `OWins`, or `Draw` |
| WinningLine | `int[]?` | Winning indexes when a player has won | Present only when `Result` is `XWins` or `OWins` |
| Revision | `int` | Monotonic version used for stale-state detection | Must increment after every accepted move and restart |
| CreatedUtc | `DateTimeOffset` | Match creation timestamp | Required |
| LastUpdatedUtc | `DateTimeOffset` | Timestamp of the last accepted mutation | Must update after every accepted move and restart |

### State Transitions

| From | Event | To |
|------|-------|----|
| No persisted match | Initial load for browser | New `InProgress` session with Player X active |
| `InProgress` | Valid move with no terminal outcome | `InProgress` with switched `CurrentPlayer` and incremented `Revision` |
| `InProgress` | Valid move that creates a winning line | `XWins` or `OWins` with stored `WinningLine` and incremented `Revision` |
| `InProgress` | Valid move that fills the final empty square | `Draw` with incremented `Revision` |
| Any persisted match | Restart | Replacement `InProgress` session with empty board, Player X active, and new `MatchId` |
| Any persisted match | Invalid or stale move request | Unchanged persisted state |

## Value Object: BrowserIdentity

**Purpose**: Captures the anonymous browser association used by the server to load or create the correct official match without requiring authentication.

| Field | Type | Description | Validation |
|-------|------|-------------|------------|
| BrowserId | `string` | Same-site cookie value that identifies the browser's active match | Required, opaque to the client, stable until cookie replacement |
| IssuedUtc | `DateTimeOffset` | When the browser identity was first issued | Required |
| LastSeenUtc | `DateTimeOffset` | Most recent time the server observed the browser identity | Updates on successful requests |

### Rules

- The client does not generate or mutate `BrowserId`.
- The browser identity is not shared across different browsers.
- If no cookie is present, the server creates a new identity before loading or creating a match.

## Command: MoveRequest

**Purpose**: Represents the client request to apply one move against the latest known official state.

| Field | Type | Description | Validation |
|-------|------|-------------|------------|
| SpaceIndex | `int` | Zero-based board position requested by the player | Must be within 0 through 8 |
| ActingPlayer | `PlayerMark` | Player mark the client believes is making the move | Must be `X` or `O` |
| ExpectedRevision | `int` | Last revision observed by the client | Must be greater than or equal to 0 |

### Validation Rules

- The request is valid only when the browser has an active match, the match result is `InProgress`, `ActingPlayer` matches `CurrentPlayer`, the target square is empty, and `ExpectedRevision` equals the persisted `Revision`.
- The server performs win and draw evaluation only after a move passes all validation rules and is persisted.

## Result: MoveDecision

**Purpose**: Describes the server outcome for a move attempt together with the latest official state the client must render.

| Field | Type | Description | Validation |
|-------|------|-------------|------------|
| Accepted | `bool` | Indicates whether the move changed the persisted match | Required |
| RejectionReason | `MoveRejectionReason?` | Domain reason when a move is not accepted | Present only when `Accepted` is `false` |
| UserMessage | `string` | Status or error message suitable for the UI status region | Required |
| Match | `BrowserMatchSession` snapshot DTO | Latest official state after evaluating the request | Required for accepted and domain-rejected move attempts |

## Enum: MoveRejectionReason

**Purpose**: Normalizes the client-visible rejection categories required by the specification.

| Value | Meaning |
|-------|---------|
| `OccupiedSquare` | The requested square already contains a mark |
| `WrongTurn` | The acting player does not match the official current player |
| `MatchComplete` | The match is already won or drawn |
| `StaleRevision` | The client requested a move using an outdated board revision |

## Derived Values

| Name | Description |
|------|-------------|
| IsBoardInteractive | `true` only when `Result` is `InProgress` |
| AvailableMoves | Board indexes where no mark is present |
| StatusMessage | User-facing text derived from the latest official match state and any recent decision |

## Validation Summary

- Every persisted board must remain nine cells long.
- Only the server may decide turn changes, wins, draws, or whether a move should be rejected.
- `Revision` must change only after accepted writes, never after rejected move attempts.
- Restart replaces the browser's previous official match with a fresh `InProgress` session.
- Infrastructure failures do not mutate persisted state and must not fabricate a new revision.