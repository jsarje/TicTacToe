# Data Model: Tic-Tac-Toe Web Game

## Entity: GameSession

**Purpose**: Represents a single playable round and all state required to render and validate the board.

| Field | Type | Description | Validation |
|-------|------|-------------|------------|
| Board | `BoardSpace[9]` or equivalent immutable collection | Ordered set of the nine board positions | Must always contain exactly 9 entries |
| CurrentPlayer | `PlayerMark` | The player allowed to make the next valid move while the game is active | Must be `X` or `O` when `Result` is `InProgress` |
| Result | `GameResult` | Current game outcome | Must be one of `InProgress`, `XWins`, `OWins`, or `Draw` |
| WinningLine | `int[]?` | Optional indexes for the winning row, column, or diagonal | Present only when `Result` is `XWins` or `OWins` |

### State Transitions

| From | Event | To |
|------|-------|----|
| New session | Start game | `InProgress` with Player X active |
| In progress | Valid move without completion | `InProgress` with the other player active |
| In progress | Valid move that forms a winning line | `XWins` or `OWins` |
| In progress | Valid move that fills final empty cell without a winner | `Draw` |
| Any state | Restart | New session |

## Entity: BoardSpace

**Purpose**: Represents one position on the 3x3 board.

| Field | Type | Description | Validation |
|-------|------|-------------|------------|
| Index | `int` | Zero-based board position from 0 to 8 | Must remain within 0 through 8 |
| Mark | `PlayerMark?` | Current occupant of the space | Must be `null`, `X`, or `O` |

### Rules

- A move is valid only when `Mark` is empty and the session result is `InProgress`.
- Once a mark is applied, it cannot be changed until the session is restarted.

## Entity: PlayerMark

**Purpose**: Identifies the active or recorded player mark.

| Value | Meaning |
|-------|---------|
| `X` | Starting player for every new session |
| `O` | Second player |

## Entity: GameResult

**Purpose**: Defines the session outcome used by the UI status region and move validation.

| Value | Meaning |
|-------|---------|
| `InProgress` | Game can still accept moves |
| `XWins` | Player X completed a winning line |
| `OWins` | Player O completed a winning line |
| `Draw` | All spaces filled with no winner |

## Derived Values

| Name | Description |
|------|-------------|
| AvailableMoves | Set of board indexes where `Mark` is empty |
| StatusMessage | User-facing text derived from `CurrentPlayer` and `Result` |
| IsBoardInteractive | Boolean indicating whether the board should still accept input |

## Validation Summary

- The board must always remain nine cells long.
- Turn changes only after a valid move during an active session.
- Occupied-cell selection must leave the session unchanged.
- No move may be recorded once the result is `XWins`, `OWins`, or `Draw`.
- Restart always resets the board to empty, clears `WinningLine`, sets `Result` to `InProgress`, and sets `CurrentPlayer` to `X`.