# UI Contract: Tic-Tac-Toe Game Screen

## Screen Composition

- The application exposes one visible game screen with three persistent regions: status, board, and restart control.
- No navigation is required to access or replay the game.

## Board Contract

- The board renders exactly nine interactive spaces in a 3x3 layout.
- Each space is implemented as a native button in the normal tab order.
- Each space exposes an accessible label that identifies the position and current value.
- Activating an empty space during an active game writes the current player's mark into that space.
- Activating an occupied space or any space after game completion leaves the visible board and status unchanged.

## Status Contract

- At the start of every session, the status region announces that Player X goes first.
- After each valid non-terminal move, the status region announces the next player's turn.
- After a winning move, the status region announces the winning player.
- After a draw, the status region announces the draw outcome.
- The status region remains visible in the same position across all game states and should be suitable for screen-reader announcement.

## Restart Contract

- A restart control remains visible and operable throughout the session.
- Activating restart clears all board marks, removes any terminal result, and starts a new game with Player X.

## Interaction Constraints

- Keyboard users can reach every board button and the restart control through normal tab navigation.
- Board buttons support activation through Enter or Space through native button behavior.
- Focus indication must remain visible for board spaces and the restart control.

## Responsive Contract

- On common desktop and mobile viewport sizes, the full board, status, and restart control remain visible without horizontal scrolling.
- The contract does not require animation, persistence, or alternate screen layouts.