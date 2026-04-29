# Tasks: Server-Authoritative Match State

**Input**: Design documents from `/specs/002-server-referee-state/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, quickstart.md, contracts/

**Tests**: Tests are REQUIRED for business logic, persistence, API behavior, and all user-visible client flows because this feature changes the application authority boundary.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no unmet dependencies)
- **[Story]**: Which user story this task belongs to (e.g., `US1`, `US2`, `US3`)
- Every task includes exact file paths for direct execution

## Path Conventions

- **Solution**: `TicTacToe.sln` at repository root
- **Core domain**: `src/TicTacToe.Core/`
- **Server host**: `src/TicTacToe.Web/`
- **Interactive WebAssembly client**: `src/TicTacToe.Web.Client/`
- **Tests**: `tests/TicTacToe.Core.Tests/`, `tests/TicTacToe.Web.Tests/`, `tests/TicTacToe.Web.IntegrationTests/`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Restructure the solution from standalone WASM into a hosted Blazor Web App with a dedicated client project and integration-test project.

- [ ] T001 Replace the standalone web project with a Blazor Web App and client project in `TicTacToe.sln`, `src/TicTacToe.Web/TicTacToe.Web.csproj`, and `src/TicTacToe.Web.Client/TicTacToe.Web.Client.csproj`
- [ ] T002 Add server, client, EF Core SQLite, and integration-test package references in `src/TicTacToe.Web/TicTacToe.Web.csproj`, `src/TicTacToe.Web.Client/TicTacToe.Web.Client.csproj`, `tests/TicTacToe.Web.Tests/TicTacToe.Web.Tests.csproj`, and `tests/TicTacToe.Web.IntegrationTests/TicTacToe.Web.IntegrationTests.csproj`
- [ ] T003 [P] Configure startup settings and SQLite connection defaults in `src/TicTacToe.Web/Program.cs`, `src/TicTacToe.Web/appsettings.json`, and `src/TicTacToe.Web/appsettings.Development.json`
- [ ] T004 [P] Add shared test usings and hosted-app test references in `tests/TicTacToe.Web.Tests/Usings.cs`, `tests/TicTacToe.Web.IntegrationTests/Usings.cs`, and `tests/TicTacToe.Web.IntegrationTests/TestWebApplicationFactory.cs`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Establish the shared persistence, browser identity, transport contracts, and infrastructure required by every story.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete.

- [ ] T005 Create persisted match and browser identity entities in `src/TicTacToe.Web/Data/MatchSessionEntity.cs` and `src/TicTacToe.Web/Data/BrowserIdentityEntity.cs`
- [ ] T006 [P] Create the EF Core context, entity configuration, and repository contract in `src/TicTacToe.Web/Data/TicTacToeDbContext.cs`, `src/TicTacToe.Web/Data/Configurations/MatchSessionEntityConfiguration.cs`, and `src/TicTacToe.Web/Infrastructure/IMatchSessionRepository.cs`
- [ ] T007 [P] Create shared API contracts and rejection enums in `src/TicTacToe.Core/Contracts/MatchSnapshotDto.cs`, `src/TicTacToe.Core/Contracts/MoveRequestDto.cs`, `src/TicTacToe.Core/Contracts/MoveDecisionDto.cs`, `src/TicTacToe.Core/Contracts/RestartDecisionDto.cs`, and `src/TicTacToe.Core/Models/MoveRejectionReason.cs`
- [ ] T008 [P] Create browser cookie identity infrastructure in `src/TicTacToe.Web/Infrastructure/IBrowserIdentityService.cs` and `src/TicTacToe.Web/Infrastructure/BrowserIdentityCookieService.cs`
- [ ] T009 Create the authoritative application service boundary in `src/TicTacToe.Web/Infrastructure/IMatchRefereeService.cs`, `src/TicTacToe.Web/Infrastructure/MatchRefereeService.cs`, and `src/TicTacToe.Web/Infrastructure/MatchMappingExtensions.cs`
- [ ] T010 [P] Configure server middleware, problem details, endpoint registration, and dependency injection in `src/TicTacToe.Web/Program.cs` and `src/TicTacToe.Web/Endpoints/MatchEndpoints.cs`
- [ ] T011 [P] Add the initial SQLite migration and database bootstrap in `src/TicTacToe.Web/Data/Migrations/` and `src/TicTacToe.Web/Program.cs`

**Checkpoint**: Foundation ready. All user stories can now proceed independently from the same persistence and API baseline.

---

## Phase 3: User Story 1 - Resume an active match after refresh (Priority: P1) 🎯 MVP

**Goal**: Load and persist one official match per browser so refresh or reopen returns the same authoritative board, turn, and outcome.

**Independent Test**: Start a match, persist several moves, refresh in the same browser, and verify that the board, turn, and terminal state reload exactly; confirm a second browser receives a different isolated match.

### Tests for User Story 1 ⚠️

> **NOTE: Write these tests first and ensure they fail before implementation.**

- [ ] T012 [P] [US1] Add integration tests for initial match creation, persisted reload, and completed-match reload in `tests/TicTacToe.Web.IntegrationTests/MatchLoadEndpointsTests.cs`
- [ ] T013 [P] [US1] Add integration tests for browser isolation using separate cookie containers in `tests/TicTacToe.Web.IntegrationTests/BrowserIsolationTests.cs`
- [ ] T014 [P] [US1] Add bUnit tests for async initial load, persisted board rendering, and refresh-safe home state in `tests/TicTacToe.Web.Tests/Pages/HomeMatchRestoreTests.cs`

### Implementation for User Story 1

- [ ] T015 [P] [US1] Implement SQLite-backed load/create persistence flow in `src/TicTacToe.Web/Data/MatchSessionRepository.cs` and `src/TicTacToe.Web/Data/TicTacToeDbContext.cs`
- [ ] T016 [P] [US1] Implement browser identity issuance and `GET /api/match` loading in `src/TicTacToe.Web/Infrastructure/BrowserIdentityCookieService.cs` and `src/TicTacToe.Web/Endpoints/MatchEndpoints.cs`
- [ ] T017 [P] [US1] Create the client API loader and result models in `src/TicTacToe.Web.Client/Services/IGameApiClient.cs` and `src/TicTacToe.Web.Client/Services/GameApiClient.cs`
- [ ] T018 [US1] Move the gameplay screen into the client project and hydrate the official match on page load in `src/TicTacToe.Web.Client/Pages/Home.razor`, `src/TicTacToe.Web.Client/Pages/Home.razor.cs`, and `src/TicTacToe.Web.Client/Components/GameStatus.razor`
- [ ] T019 [US1] Render loading, completed-match reload, and initial-load retry states from the last confirmed snapshot in `src/TicTacToe.Web.Client/Pages/Home.razor`, `src/TicTacToe.Web.Client/Components/GameBoard.razor`, and `src/TicTacToe.Web.Client/Components/RestartButton.razor`
- [ ] T020 [US1] Add load-path timing assertions to keep current-match retrieval under the feature budget in `tests/TicTacToe.Web.IntegrationTests/MatchLoadEndpointsTests.cs`

**Checkpoint**: User Story 1 should now preserve one browser-owned match across refreshes and be testable without implementing move submission.

---

## Phase 4: User Story 2 - Submit moves through an authoritative referee (Priority: P2)

**Goal**: Send every move and restart request to the server so turn validation, accepted writes, win detection, draw detection, and restart state are all decided authoritatively.

**Independent Test**: Load a match, submit valid and invalid move attempts, verify only accepted moves mutate the official state, and confirm decisive moves produce wins or draws returned by the server.

### Tests for User Story 2 ⚠️

- [ ] T021 [P] [US2] Extend core rule tests for authoritative move acceptance, turn order, win detection, and draw detection in `tests/TicTacToe.Core.Tests/GameEngineMoveTests.cs`, `tests/TicTacToe.Core.Tests/GameEngineCompletionTests.cs`, and `tests/TicTacToe.Core.Tests/GameEngineRestartTests.cs`
- [ ] T022 [P] [US2] Add API integration tests for `POST /api/match/moves` and `POST /api/match/restart` in `tests/TicTacToe.Web.IntegrationTests/MoveEndpointsTests.cs`
- [ ] T023 [P] [US2] Add bUnit tests for async move submission, server-confirmed board updates, and restart synchronization in `tests/TicTacToe.Web.Tests/Pages/HomeServerGameplayTests.cs`

### Implementation for User Story 2

- [ ] T024 [P] [US2] Refactor the core engine for server-authoritative move evaluation and restart behavior in `src/TicTacToe.Core/Services/IGameEngine.cs` and `src/TicTacToe.Core/Services/GameEngine.cs`
- [ ] T025 [P] [US2] Implement revision-aware move application and restart persistence in `src/TicTacToe.Web/Infrastructure/MatchRefereeService.cs` and `src/TicTacToe.Web/Data/MatchSessionRepository.cs`
- [ ] T026 [US2] Implement `POST /api/match/moves` and `POST /api/match/restart` in `src/TicTacToe.Web/Endpoints/MatchEndpoints.cs`
- [ ] T027 [US2] Wire move and restart requests through the client API service in `src/TicTacToe.Web.Client/Services/GameApiClient.cs` and `src/TicTacToe.Web.Client/Pages/Home.razor.cs`
- [ ] T028 [US2] Update the board, status, and restart components to render only server-confirmed state and disable controls during round-trips in `src/TicTacToe.Web.Client/Components/GameBoard.razor`, `src/TicTacToe.Web.Client/Components/GameStatus.razor`, and `src/TicTacToe.Web.Client/Components/RestartButton.razor`
- [ ] T029 [US2] Add move and restart latency assertions to keep the hot path within the feature budget in `tests/TicTacToe.Web.IntegrationTests/MoveEndpointsTests.cs`

**Checkpoint**: User Story 2 should now provide authoritative move and restart behavior with server-owned win and draw outcomes.

---

## Phase 5: User Story 3 - Understand rejected or delayed move attempts (Priority: P3)

**Goal**: Surface stale-state, invalid-move, and temporary infrastructure failure outcomes clearly while keeping the last confirmed official board visible and retryable.

**Independent Test**: Force stale revisions, wrong-turn requests, completed-match requests, and temporary load or save failures, then verify the client shows a clear reason, preserves the last confirmed board, and allows retry where appropriate.

### Tests for User Story 3 ⚠️

- [ ] T030 [P] [US3] Add API integration tests for stale revision, wrong turn, completed match, and temporary persistence failures in `tests/TicTacToe.Web.IntegrationTests/MoveFailureEndpointsTests.cs`
- [ ] T031 [P] [US3] Add bUnit tests for rejection messaging, loading notices, retry behavior, and assistive-technology announcements in `tests/TicTacToe.Web.Tests/Pages/HomeFailureStateTests.cs`
- [ ] T032 [P] [US3] Add integration coverage for preserving the last confirmed snapshot during transient failures in `tests/TicTacToe.Web.IntegrationTests/MatchLoadEndpointsTests.cs` and `tests/TicTacToe.Web.IntegrationTests/MoveFailureEndpointsTests.cs`

### Implementation for User Story 3

- [ ] T033 [P] [US3] Implement rejection-message mapping and retry-safe problem-details handling in `src/TicTacToe.Web/Infrastructure/MoveDecisionMessageFactory.cs` and `src/TicTacToe.Web/Program.cs`
- [ ] T034 [P] [US3] Add deterministic fault injection hooks for repository failure tests in `src/TicTacToe.Web/Data/MatchSessionRepository.cs` and `tests/TicTacToe.Web.IntegrationTests/TestWebApplicationFactory.cs`
- [ ] T035 [US3] Implement stale-state refresh, last-confirmed-state preservation, and retry commands in `src/TicTacToe.Web.Client/Pages/Home.razor` and `src/TicTacToe.Web.Client/Pages/Home.razor.cs`
- [ ] T036 [US3] Add accessible loading, rejection, and failure-state presentation in `src/TicTacToe.Web.Client/Components/GameStatus.razor`, `src/TicTacToe.Web.Client/Components/GameBoard.razor`, `src/TicTacToe.Web.Client/Components/RestartButton.razor`, and `src/TicTacToe.Web.Client/Pages/Home.razor.css`
- [ ] T037 [US3] Verify stale-state and failure-path UX consistency against the contract in `tests/TicTacToe.Web.Tests/Pages/HomeFailureStateTests.cs` and `specs/002-server-referee-state/contracts/server-referee-api.md`

**Checkpoint**: User Story 3 should now explain rejected or delayed actions clearly without losing the authoritative game snapshot.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Clean up migration leftovers, document the new hosted architecture, and complete cross-story validation.

- [ ] T038 [P] Remove obsolete standalone hosting artifacts and align the hosted app shell in `src/TicTacToe.Web/wwwroot/`, `src/TicTacToe.Web.Client/wwwroot/`, `src/TicTacToe.Web/App.razor`, and `src/TicTacToe.Web.Client/App.razor`
- [ ] T039 [P] Update developer setup, runtime expectations, and deployment guidance in `README.md` and `specs/002-server-referee-state/quickstart.md`
- [ ] T040 [P] Add final regression coverage for hosted app startup and shared wiring in `tests/TicTacToe.Web.IntegrationTests/HostedAppSmokeTests.cs`
- [ ] T041 Run the full validation checklist and record manual performance and UX evidence from `specs/002-server-referee-state/quickstart.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies. Start immediately.
- **Foundational (Phase 2)**: Depends on Setup. Blocks all user stories.
- **User Story 1 (Phase 3)**: Depends on Foundational only. This is the MVP.
- **User Story 2 (Phase 4)**: Depends on Foundational only. It can proceed independently once the shared persistence and API baseline exists.
- **User Story 3 (Phase 5)**: Depends on Foundational only. It can be built and tested against the same baseline without waiting for another story to finish.
- **Polish (Phase 6)**: Depends on the user stories selected for delivery.

### User Story Dependencies

- **US1 (P1)**: No dependency on other user stories after Foundational.
- **US2 (P2)**: No dependency on other user stories after Foundational; it consumes the shared load and persistence infrastructure but owns move and restart behavior.
- **US3 (P3)**: No dependency on other user stories after Foundational; it focuses on rejection, stale-state, and failure-path handling over the same contracts.

### Within Each User Story

- Tests must be written and fail before the corresponding implementation tasks.
- Shared data and transport changes come before endpoint and UI wiring.
- Server behavior must exist before client behavior that depends on it.
- Performance verification is part of story completion, not a later optional step.

### Parallel Opportunities

- `T003` and `T004` can run in parallel after project restructuring begins.
- `T006`, `T007`, `T008`, `T010`, and `T011` can run in parallel during Foundation once the basic hosted structure exists.
- `T012`, `T013`, and `T014` can run in parallel for US1.
- `T021`, `T022`, and `T023` can run in parallel for US2.
- `T030`, `T031`, and `T032` can run in parallel for US3.
- `T038`, `T039`, and `T040` can run in parallel during Polish.

---

## Parallel Example: User Story 1

```text
Task: "T012 [US1] Add integration tests for initial match creation, persisted reload, and completed-match reload in tests/TicTacToe.Web.IntegrationTests/MatchLoadEndpointsTests.cs"
Task: "T013 [US1] Add integration tests for browser isolation using separate cookie containers in tests/TicTacToe.Web.IntegrationTests/BrowserIsolationTests.cs"
Task: "T014 [US1] Add bUnit tests for async initial load, persisted board rendering, and refresh-safe home state in tests/TicTacToe.Web.Tests/Pages/HomeMatchRestoreTests.cs"
```

## Parallel Example: User Story 2

```text
Task: "T021 [US2] Extend core rule tests for authoritative move acceptance, turn order, win detection, and draw detection in tests/TicTacToe.Core.Tests/GameEngineMoveTests.cs, tests/TicTacToe.Core.Tests/GameEngineCompletionTests.cs, and tests/TicTacToe.Core.Tests/GameEngineRestartTests.cs"
Task: "T022 [US2] Add API integration tests for POST /api/match/moves and POST /api/match/restart in tests/TicTacToe.Web.IntegrationTests/MoveEndpointsTests.cs"
Task: "T023 [US2] Add bUnit tests for async move submission, server-confirmed board updates, and restart synchronization in tests/TicTacToe.Web.Tests/Pages/HomeServerGameplayTests.cs"
```

## Parallel Example: User Story 3

```text
Task: "T030 [US3] Add API integration tests for stale revision, wrong turn, completed match, and temporary persistence failures in tests/TicTacToe.Web.IntegrationTests/MoveFailureEndpointsTests.cs"
Task: "T031 [US3] Add bUnit tests for rejection messaging, loading notices, retry behavior, and assistive-technology announcements in tests/TicTacToe.Web.Tests/Pages/HomeFailureStateTests.cs"
Task: "T032 [US3] Add integration coverage for preserving the last confirmed snapshot during transient failures in tests/TicTacToe.Web.IntegrationTests/MatchLoadEndpointsTests.cs and tests/TicTacToe.Web.IntegrationTests/MoveFailureEndpointsTests.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup.
2. Complete Phase 2: Foundational.
3. Complete Phase 3: User Story 1.
4. Stop and validate refresh restoration, completed-match reload, and browser isolation.

### Incremental Delivery

1. Deliver hosted app restructuring and shared persistence infrastructure.
2. Deliver US1 as the first user-visible migration outcome.
3. Deliver US2 to move authoritative gameplay and restart decisions to the server.
4. Deliver US3 to complete rejection, stale-state, and failure handling.
5. Finish polish, documentation, and final regression validation.

### Parallel Team Strategy

1. One developer handles project restructuring while another prepares hosted-test infrastructure.
2. After Foundational completes, different developers can take US1, US2, and US3 in parallel.
3. Polish work can be split between documentation, startup cleanup, and regression validation.

---

## Notes

- Tasks marked `[P]` touch different files and have no unmet dependencies.
- Every user story includes explicit tests, required failure-state work, and performance validation.
- Suggested MVP scope: Phase 3 only, User Story 1.
- User stories intentionally depend on Foundational work rather than chaining to lower-priority stories.