# Coding Standards and Practices

This file provides standards for GitHub Copilot to follow when generating code for this project.

## General Coding Standards

- Use clean, readable code with proper indentation
- Follow C# naming conventions (PascalCase for classes/methods, camelCase for variables)
- Add XML documentation to public APIs
- Keep methods small and focused on a single responsibility
- Use nullable reference types appropriately

## Architecture Guidelines

- Follow Onion Architecture principles
- Keep business logic in Core project
- Data access should be isolated in DataAccess
- UI logic should be thin and focused on presentation
- Do not add Nuget packages or project references without approval. 

## Database Practices

- Use Entity Framework for data access
- Follow Commands and Queries and Handlers data access
- Create mapping files for all entities
- Include database schema changes in appropriate scripts

## Testing Standards
- Test-driven development (TDD) approach: write tests before implementing functionality
- All tests use Shouldly framework for assertions

### Testing Frameworks
- **NUnit**: Primary testing framework
- Avoid mocking libraries when possible

### Test Structure
- Follow AAA pattern (Arrange, Act, Assert), but don't add comments
- Use descriptive test names
- Prefix test methods with "Should" or "When"

### Test Categories
1. **Unit Tests**
   - Test a single unit in isolation
   - Fast execution, no infrastructure dependencies
   - Follow TDD approach (test first, then implement)

2. **Integration Tests**
   - Test component integration
   - May use actual database
   - Should run in CI/CD pipeline

3. **UI Tests**
   - Test user interface components
   - Use appropriate testing tools for Blazor components

### Test Naming Convention
- `[MethodName]_[Scenario]_[ExpectedResult]`
- Examples: 
  - `GetWorkOrder_WithValidId_ReturnsWorkOrder`
  - `SaveChurchBulletin_WithMissingTitle_ThrowsValidationException`

## Blazor Guidelines

- Use clean component structure
- Keep component logic in code-behind files when complex
- Follow proper state management practices
- Minimize JavaScript interop when possible

## Performance Considerations

## Response Guidelines

Do not use "I" or "you" or "me" or "us" or "we" in responses. Do not simulate personality. Be a robot. Short, terse responses.  No additional questions.
When generating any code assume that a test was requested to be generated as well and follow the TDD process.
Before acting on a requested code change, generate a test for the change first.

Do not refer to the user of Visual Studio. Do not use 2nd person pronouns. No pronouns. Be terse. Don't say, for example, "Now let's do something" or "Let me do something" or "I'll help you". Just say "Now doing" or "Checking this file"