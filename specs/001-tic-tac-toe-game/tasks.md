# Tasks: Tic-Tac-Toe Web Game

**Input**: Design documents from `/specs/001-tic-tac-toe-game/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Tests are REQUIRED for business logic, user-visible behavior, and the Blazor interaction boundary called out in the specification.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Solution**: `TicTacToe.sln` at repository root
- **Source**: `src/TicTacToe.Core/`, `src/TicTacToe.Web/`
- **Tests**: `tests/TicTacToe.Core.Tests/`, `tests/TicTacToe.Web.Tests/`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Scaffold the greenfield .NET solution and establish baseline project configuration.

- [ ] T001 Scaffold the solution and projects in `TicTacToe.sln`, `src/TicTacToe.Core/TicTacToe.Core.csproj`, `src/TicTacToe.Web/TicTacToe.Web.csproj`, `tests/TicTacToe.Core.Tests/TicTacToe.Core.Tests.csproj`, and `tests/TicTacToe.Web.Tests/TicTacToe.Web.Tests.csproj`
- [ ] T002 Add project references and required test packages in `src/TicTacToe.Web/TicTacToe.Web.csproj`, `tests/TicTacToe.Core.Tests/TicTacToe.Core.Tests.csproj`, and `tests/TicTacToe.Web.Tests/TicTacToe.Web.Tests.csproj`
- [ ] T003 [P] Configure shared SDK, nullable, implicit usings, and test defaults in `Directory.Build.props`
- [ ] T004 [P] Add repository-wide test usings and package aliases in `tests/TicTacToe.Core.Tests/Usings.cs` and `tests/TicTacToe.Web.Tests/Usings.cs`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Create the shared domain, service, and UI shells that every user story depends on.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete.

- [ ] T005 Create core domain types in `src/TicTacToe.Core/Models/PlayerMark.cs`, `src/TicTacToe.Core/Models/GameResult.cs`, `src/TicTacToe.Core/Models/BoardSpace.cs`, and `src/TicTacToe.Core/Models/GameSession.cs`
- [ ] T006 Create the game engine contract and placeholder implementation in `src/TicTacToe.Core/Services/IGameEngine.cs` and `src/TicTacToe.Core/Services/GameEngine.cs`
- [ ] T007 [P] Create the single-screen Blazor shells in `src/TicTacToe.Web/Pages/Home.razor`, `src/TicTacToe.Web/Components/GameBoard.razor`, `src/TicTacToe.Web/Components/GameStatus.razor`, and `src/TicTacToe.Web/Components/RestartButton.razor`
- [ ] T008 [P] Create test host helpers for core and component tests in `tests/TicTacToe.Core.Tests/GameSessionTestData.cs` and `tests/TicTacToe.Web.Tests/TestContextFactory.cs`
- [ ] T009 Wire the application shell and core service registration in `src/TicTacToe.Web/Program.cs`, `src/TicTacToe.Web/App.razor`, and `src/TicTacToe.Web/Layout/MainLayout.razor`

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel.

---

## Phase 3: User Story 1 - Play a complete local match (Priority: P1) 🎯 MVP

**Goal**: Deliver a full two-player local tic-tac-toe match with alternating turns, invalid-move rejection, win detection, and draw detection.

**Independent Test**: Launch the app, play alternating moves on the 3x3 board, verify occupied spaces are ignored, then confirm the game ends correctly for both a win and a draw.

### Tests for User Story 1

- [ ] T010 [P] [US1] Add unit tests for turn alternation and occupied-space rejection in `tests/TicTacToe.Core.Tests/GameEngineMoveTests.cs`
- [ ] T011 [P] [US1] Add unit tests for win-line and draw detection in `tests/TicTacToe.Core.Tests/GameEngineCompletionTests.cs`
- [ ] T012 [P] [US1] Add bUnit interaction tests for full-match play and post-game move blocking in `tests/TicTacToe.Web.Tests/Pages/HomeGameplayTests.cs`

### Implementation for User Story 1

- [ ] T013 [US1] Implement move validation, turn switching, win detection, and draw detection in `src/TicTacToe.Core/Services/GameEngine.cs`
- [ ] T014 [P] [US1] Implement the 3x3 interactive board with nine native buttons in `src/TicTacToe.Web/Components/GameBoard.razor` and `src/TicTacToe.Web/Components/GameBoard.razor.css`
- [ ] T015 [US1] Implement page-level gameplay orchestration and board updates in `src/TicTacToe.Web/Pages/Home.razor` and `src/TicTacToe.Web/Pages/Home.razor.cs`
- [ ] T016 [US1] Prevent repeat selections and additional moves after completion in `src/TicTacToe.Web/Pages/Home.razor.cs` and `src/TicTacToe.Web/Components/GameBoard.razor`

**Checkpoint**: User Story 1 should now support a complete playable match and be testable on its own.

---

## Phase 4: User Story 2 - Understand current game status (Priority: P2)

**Goal**: Keep the status region accurate and visible so players always know whose turn it is or whether the match has ended.

**Independent Test**: Observe the status area at game start, after valid moves, after invalid move attempts, and after win and draw outcomes to confirm the message remains correct and stable.

### Tests for User Story 2

- [ ] T017 [P] [US2] Add unit tests for derived status messaging in `tests/TicTacToe.Core.Tests/GameSessionStatusTests.cs`
- [ ] T018 [P] [US2] Add bUnit tests for live status announcements across active and terminal states in `tests/TicTacToe.Web.Tests/Components/GameStatusTests.cs`

### Implementation for User Story 2

- [ ] T019 [US2] Add status-message derivation to the session model in `src/TicTacToe.Core/Models/GameSession.cs`
- [ ] T020 [P] [US2] Implement the persistent live status region in `src/TicTacToe.Web/Components/GameStatus.razor` and `src/TicTacToe.Web/Components/GameStatus.razor.css`
- [ ] T021 [US2] Bind turn, win, and draw messages to the page state in `src/TicTacToe.Web/Pages/Home.razor` and `src/TicTacToe.Web/Pages/Home.razor.cs`

**Checkpoint**: User Story 2 should now communicate active-turn and end-of-game states independently of restart behavior.

---

## Phase 5: User Story 3 - Start a fresh game quickly (Priority: P3)

**Goal**: Let players reset from any state into a clean new game without reloading the page.

**Independent Test**: Start or finish a match, activate restart, and verify the board clears, terminal state disappears, and Player X becomes active again.

### Tests for User Story 3

- [ ] T022 [P] [US3] Add unit tests for restart behavior from active and completed sessions in `tests/TicTacToe.Core.Tests/GameEngineRestartTests.cs`
- [ ] T023 [P] [US3] Add bUnit tests for the always-available restart flow in `tests/TicTacToe.Web.Tests/Pages/HomeRestartTests.cs`

### Implementation for User Story 3

- [ ] T024 [US3] Implement session reset behavior in `src/TicTacToe.Core/Services/GameEngine.cs`
- [ ] T025 [P] [US3] Implement the always-visible restart control in `src/TicTacToe.Web/Components/RestartButton.razor`
- [ ] T026 [US3] Wire restart actions and fresh-session state into `src/TicTacToe.Web/Pages/Home.razor` and `src/TicTacToe.Web/Pages/Home.razor.cs`

**Checkpoint**: User Story 3 should now provide a one-action restart from any game state.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Finish responsive, accessibility, and documentation work that spans multiple stories.

- [ ] T027 [P] Refine responsive layout and no-scroll mobile styling in `src/TicTacToe.Web/Pages/Home.razor.css` and `src/TicTacToe.Web/Components/GameBoard.razor.css`
- [ ] T028 [P] Finalize accessible labels, live-region semantics, and visible focus treatment in `src/TicTacToe.Web/Components/GameBoard.razor`, `src/TicTacToe.Web/Components/GameStatus.razor`, and `src/TicTacToe.Web/Components/RestartButton.razor`
- [ ] T029 [P] Update project bootstrap, test, and run instructions in `README.md`
- [ ] T030 Run the manual validation checklist from `specs/001-tic-tac-toe-game/quickstart.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - start immediately.
- **Foundational (Phase 2)**: Depends on Setup - blocks all user stories.
- **User Story 1 (Phase 3)**: Depends on Foundational - establishes the MVP.
- **User Story 2 (Phase 4)**: Depends on the session and board flow from US1.
- **User Story 3 (Phase 5)**: Depends on the active session flow from US1 and the status model from US2.
- **Polish (Phase 6)**: Depends on all selected user stories being complete.

### User Story Dependencies

- **US1 (P1)**: No dependency on other user stories after Foundational.
- **US2 (P2)**: Depends on US1 because status content is driven by live gameplay state.
- **US3 (P3)**: Depends on US1 for resettable session state and on US2 for full status reset verification.

### Within Each User Story

- Tests must be written before the corresponding implementation tasks.
- Core/domain changes precede UI integration.
- Component work can run in parallel when it touches different files.
- Each story ends with an independently runnable validation checkpoint.

### Parallel Opportunities

- T003 and T004 can run together after project scaffolding.
- T007 and T008 can run together after the core placeholders exist.
- T010, T011, and T012 can run together before US1 implementation.
- T017 and T018 can run together before US2 implementation.
- T022 and T023 can run together before US3 implementation.
- T027, T028, and T029 can run together during polish.

---

## Parallel Example: User Story 1

```text
Task: "T010 [US1] Add unit tests for turn alternation and occupied-space rejection in tests/TicTacToe.Core.Tests/GameEngineMoveTests.cs"
Task: "T011 [US1] Add unit tests for win-line and draw detection in tests/TicTacToe.Core.Tests/GameEngineCompletionTests.cs"
Task: "T012 [US1] Add bUnit interaction tests for full-match play and post-game move blocking in tests/TicTacToe.Web.Tests/Pages/HomeGameplayTests.cs"
```

## Parallel Example: User Story 2

```text
Task: "T017 [US2] Add unit tests for derived status messaging in tests/TicTacToe.Core.Tests/GameSessionStatusTests.cs"
Task: "T018 [US2] Add bUnit tests for live status announcements across active and terminal states in tests/TicTacToe.Web.Tests/Components/GameStatusTests.cs"
```

## Parallel Example: User Story 3

```text
Task: "T022 [US3] Add unit tests for restart behavior from active and completed sessions in tests/TicTacToe.Core.Tests/GameEngineRestartTests.cs"
Task: "T023 [US3] Add bUnit tests for the always-available restart flow in tests/TicTacToe.Web.Tests/Pages/HomeRestartTests.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup.
2. Complete Phase 2: Foundational.
3. Complete Phase 3: User Story 1.
4. Validate the playable match flow before expanding scope.

### Incremental Delivery

1. Deliver the scaffold and core game engine baseline.
2. Ship US1 as the first playable MVP.
3. Add US2 to complete player-facing status feedback.
4. Add US3 to support repeated play sessions.
5. Finish responsive, accessibility, and documentation polish.

### Parallel Team Strategy

1. One developer completes solution scaffolding while another prepares shared test infrastructure.
2. After Foundational is complete, one developer can drive core-rule work while another builds the Blazor components.
3. Polish tasks can be split across styling, accessibility, and documentation without file conflicts.

---

## Notes

- [P] tasks touch different files and have no unmet dependencies.
- Every user-story task includes an exact target file path for direct execution.
- Tests are explicitly included because the specification requires automated coverage for critical behaviors.
- The suggested MVP scope is Phase 3 only: User Story 1.