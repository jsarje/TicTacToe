# Feature Specification: Server Referee State

**Feature Branch**: `[002-server-referee-state]`  
**Created**: 2026-04-29  
**Status**: Draft  
**Input**: User description: "We want to introduce a paradigm shift from client to server. Right now our Blazor WebAssembly app holds the entire game state in the browser's memory, so refreshing the page resets the game. Migrate the backend logic so the client asks the server to validate and apply a move, the system persists the new state, checks for a win, and keeps the match available after refresh."

## Clarifications

### Session 2026-04-29

- Q: What persisted match should the app load on refresh? -> A: Each browser gets its own persisted match state, and refresh reloads that browser's official match rather than a shared global game.
- Q: What does "reopen in the same browser" mean for persistence? -> A: The same browser should restore its active match after the browser is closed and reopened for normal return visits unless the user clears site data or the browser identity has expired.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Resume an active match after refresh (Priority: P1)

A player can refresh or reopen the game in the same browser and continue that browser's in-progress match instead of losing the board state and starting over.

**Why this priority**: Preserving the current match across refreshes is the clearest user-visible outcome of moving game authority out of browser memory. Without it, the migration does not solve the main problem.

**Independent Test**: Can be fully tested by starting a match, making several valid moves, refreshing the page, closing and reopening the same browser, and verifying that the board, current turn, and game status return exactly as they were before the refresh.

**Acceptance Scenarios**:

1. **Given** a browser has a match in progress with persisted moves already applied, **When** the player refreshes the page in that same browser, **Then** the game reloads that browser's board state, active player, and match status before any new move can be made.
2. **Given** a browser has a completed match with a win or draw, **When** the player reloads the game in that same browser, **Then** the completed board and final result are shown and additional moves remain unavailable until that browser starts a new match.
3. **Given** a browser closes and reopens without clearing site data, **When** the player returns to the game in that same browser, **Then** the browser reloads its existing official match rather than starting a new shared or empty game.
4. **Given** two different browsers each have their own persisted matches, **When** one browser refreshes or plays a move, **Then** the other browser's match state remains unchanged.

---

### User Story 2 - Submit moves through an authoritative referee (Priority: P2)

A player can choose a square, send that move for validation, and receive the updated board only after the system confirms the move is legal and applies it to the official match state.

**Why this priority**: The core business change is that the browser no longer decides whether a move is valid or whether the game has been won. The authoritative referee must protect turn order, move legality, and final outcomes.

**Independent Test**: Can be fully tested by starting a match, submitting valid and invalid moves, and verifying that only legal moves change the official board, advance the turn, and update the match outcome.

**Acceptance Scenarios**:

1. **Given** an active match and the correct player's turn, **When** that player selects an empty square, **Then** the system validates the request, records the move, updates the board, and returns the next turn or final result.
2. **Given** an active match, **When** a move is requested for an occupied square, **Then** the move is rejected, the official board remains unchanged, and the current turn stays the same.
3. **Given** an active match, **When** a move request would complete a winning line or fill the final open square without a winner, **Then** the system records the move, marks the match as won or drawn, and blocks further moves.

---

### User Story 3 - Understand rejected or delayed move attempts (Priority: P3)

A player receives clear feedback when a requested move cannot be applied immediately because the match is no longer in the expected state or the system cannot confirm the update.

**Why this priority**: Server-authoritative play introduces loading, stale-state, and failure conditions that did not exist in a purely in-browser game. The experience must remain understandable when a move is not accepted.

**Independent Test**: Can be tested by forcing rejected move conditions and temporary retrieval or save failures, then verifying that the player sees the current official state and a clear explanation of what happened.

**Acceptance Scenarios**:

1. **Given** the displayed board is no longer current, **When** the player submits a move based on outdated information, **Then** the move is not applied and the interface refreshes to the latest official match state with a clear explanation.
2. **Given** the system cannot load or save the match state temporarily, **When** the player opens the game or submits a move, **Then** the interface shows that the action could not be completed, preserves the last confirmed board state, and allows the player to retry.

---

### Edge Cases

- If a player refreshes while a move is still being confirmed, the next screen load must show the latest confirmed official state rather than a duplicate or partially applied move.
- If a move request targets an occupied square, arrives after the match is complete, or is no longer valid for the current turn, the match state remains unchanged.
- If two move requests are submitted close together for the same browser's match, only the first valid request may change that official state and later conflicting requests must be rejected clearly.
- If the game cannot retrieve the current match on initial load, the player sees a recoverable failure state rather than an empty or silently reset board.
- If the player restarts a match after completion, the new match begins from a clean board and does not reuse the previous completed outcome.
- If the browser's site data is cleared or its browser identity has expired, the next visit may start a new official match because the prior browser association no longer exists.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST maintain the official tic-tac-toe match state outside the browser so that a page refresh does not discard the current match.
- **FR-002**: System MUST maintain a distinct persisted official match state for each browser so different users can have different game progress without affecting one another.
- **FR-003**: System MUST load the latest official match state associated with the current browser whenever that browser opens or refreshes the game.
- **FR-004**: System MUST prevent the browser from finalizing moves, turn changes, wins, or draws without confirmation from the authoritative match state.
- **FR-005**: System MUST accept move requests that identify the intended square and the acting player for the current browser-associated match.
- **FR-006**: System MUST validate each requested move against the official match state, including whether the match is still active, whether the requested square is empty, and whether it is that player's turn.
- **FR-007**: System MUST reject invalid move requests without changing the official board state, current turn, or recorded outcome.
- **FR-008**: System MUST persist every accepted move before confirming the updated board state back to the client.
- **FR-009**: System MUST evaluate every accepted move for win and draw outcomes using the official match state and persist the resulting status.
- **FR-010**: System MUST block any further move requests after a match has been won or drawn until that browser starts a new match.
- **FR-011**: System MUST allow the player to start a new match that clears the previous board, resets the turn to Player X, and becomes that browser's new official match state.
- **FR-012**: System MUST present the latest confirmed board state, current turn, and match outcome after every load, refresh, accepted move, rejected move, and restart.
- **FR-013**: System MUST provide a clear user-visible message when a move is rejected, including whether the reason is an occupied square, incorrect turn, completed match, stale board, or temporary save/load failure.
- **FR-014**: System MUST provide an explicit loading state while retrieving the official match state or awaiting move confirmation.
- **FR-015**: System MUST preserve the existing single-screen gameplay model with one visible board, one status area, and one restart action.
- **FR-016**: System MUST define automated test coverage for match restoration after refresh, browser isolation between concurrent users, valid move submission, rejected move handling, win detection, draw detection, restart behavior, and temporary retrieval or save failures.
- **FR-017**: System MUST keep the board controls and restart action keyboard operable and ensure status changes are perceivable to assistive technologies.
- **FR-018**: System MUST keep primary gameplay interactions within the defined latency budget for loading the current match and confirming a move.

## Experience Consistency *(mandatory for user-facing changes)*

- **Existing Patterns**: The feature must preserve the current Home page gameplay pattern: a single visible board, a prominent game-status message, and one restart control. Server authority may add loading and failure feedback, but it must not introduce extra navigation steps for the primary play loop.
- **States**: The experience must clearly represent loading the current browser's match, active turn, invalid move rejection, stale-state refresh, win, draw, save/load failure, and restart completion. The board must always reflect the last confirmed official state for that browser.
- **Accessibility**: Each square must remain a standard keyboard-operable control with visible focus indication. Status updates, loading notices, and rejection messages must be announced clearly to screen reader users. Color alone must not be used to distinguish success, failure, or turn state.

## Performance Requirements *(mandatory)*

- **Budget**: In normal operating conditions, the current match should load within 1 second for 95% of requests, and a submitted move should return a confirmed outcome within 1 second for 95% of requests.
- **Hot Path**: The most regression-prone workflow is loading the official match, submitting a move, validating the request, and returning the updated confirmed state without duplicating or losing moves.
- **Validation**: Performance will be validated through automated checks around move-processing latency and manual verification that load, move, and restart flows stay within the stated response budget under normal conditions.

### Key Entities *(include if feature involves data)*

- **Match Session**: The authoritative record of one browser-isolated tic-tac-toe game, including the board, active player, current outcome, and whether further moves are allowed.
- **Browser Match Identity**: The stable browser-associated identifier used to reload the correct persisted match for that browser without exposing or merging another browser's state.
- **Move Request**: A player's attempt to place a specific mark in a specific square for that browser's current match.
- **Move Decision**: The outcome of validating a requested move, including whether it was accepted, why it was rejected if not accepted, and the latest official match state returned for that browser.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% of tested in-progress matches survive a page refresh in the same browser without losing the last confirmed board state, current turn, or recorded outcome.
- **SC-002**: 100% of tested invalid move requests leave the official match state unchanged and provide a visible rejection reason.
- **SC-003**: 100% of tested wins and draws are identified from the official match state immediately after the decisive accepted move.
- **SC-004**: 95% of current-match loads and move confirmations complete within 1 second under normal operating conditions.
- **SC-005**: Required automated tests cover every critical acceptance path for refresh restoration, browser isolation, move validation, outcome detection, restart behavior, and temporary failure handling.
- **SC-006**: Players can restart from any completed or in-progress match in a single action and begin a fresh game with an empty board.

## Assumptions

- The feature continues to support one active local tic-tac-toe match per browser rather than introducing online matchmaking or shared cross-browser multiplayer state.
- Player X remains the starting player for every new match unless a later feature changes the game rules.
- Match state is retained until that browser's active match is replaced by a restart or the browser identity expires, ensuring normal page refreshes and short return visits restore the same official state for that browser.
- Existing board, status, and restart interactions remain the baseline user experience, with only the minimum additions needed for loading and failure feedback.
