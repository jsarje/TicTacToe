# Web Application Development Guidelines

## Programming Language: C#
## Framework: Blazor


## Code Style: Clean Code

**Clean Code Principles:**
- Write self-documenting code with meaningful names
- Keep functions small and focused on a single responsibility
- Avoid deep nesting and complex conditional statements
- Use consistent formatting and indentation
- Write code that tells a story and is easy to understand
- Refactor ruthlessly to eliminate code smells

## Testing: xUnit

**Testing Guidelines:**
- Write comprehensive unit tests for all business logic
- Follow the AAA pattern: Arrange, Act, Assert
- Use FluentAssertions for more readable test assertions
- Maintain good test coverage (aim for 80%+ for critical paths)
- Write descriptive test names that explain the expected behavior
- Use test doubles (mocks, stubs, spies) appropriately - utilise NSubstitute for mocking dependencies
- Implement integration tests for API endpoints and user flows
- Keep tests fast, isolated, and deterministic

## AI Code Generation Preferences

When generating code, please:

- Generate complete, working code examples with proper imports
- Include inline comments for complex logic and business rules
- Follow the established patterns and conventions in this project
- Suggest improvements and alternative approaches when relevant
- Consider performance, security, and maintainability
- Include error handling and edge case considerations
- Generate appropriate unit tests when creating new functions
- Follow accessibility best practices for UI components
- Use semantic HTML and proper ARIA attributes when applicable
