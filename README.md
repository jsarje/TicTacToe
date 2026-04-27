# TicTacToe

Standalone Blazor WebAssembly tic-tac-toe game with a small shared core library for game rules and an automated test suite for both rule logic and UI interaction.

## Approach

- Pulled in instructions for C# and Blazor from https://github.com/github/awesome-copilot to help with code styling and best practices
- Initialised spec-kit https://github.com/github/spec-kit
- /speckit.constitution with their recommended prompt
- /speckit.spec Build a web application that hosts a playable tic-tac-toe game. It should include two-player local gameplay, win and draw detection, a simple and clean user interface, the ability to restart the game, and clear indication of which players turn it is.
- /speckit.plan The application uses NET10, C#, Blazor (standalone WASM mode), with minimal number of libraries
- /speckit.tasks
- /speckit.implement
- Manually verified functionality was as expected but noticed layout was a bit off.
- Raised issue for the layout and assigned to copilot cloud agent