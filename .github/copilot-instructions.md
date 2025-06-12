# Testing Standards

This file provides testing standards for GitHub Copilot to follow when generating test code for this project.

## Testing Frameworks

- **NUnit**: Primary testing framework
- No mocking libraries

## Test Structure

- Follow the AAA pattern (Arrange, Act, Assert)
- Use descriptive test names that describe the scenario and expected outcome
- Prefix test methods with "Should" or "When" to clearly indicate purpose

## Test Categories

1. **Unit Tests**
   - Test a single unit of functionality in isolation
   - Stub all external dependencies
   - Fast execution, no infrastructure dependencies
	- When generating code, using Test-driven development. Write the test that should pass first. Then run the test to make sure that it fails because the needed production code doesn't exist yet. You can generate skeleton code just to enable the solution to compile, but leave a NotImplementedException("TDD") in the method as you run the test to ensure it fails. Check for the "TDD" in the test failture to make sure the failure is the right failure. Then generate the code to make the test pass. Do this iteratively with each needed change to each class. Write one test at a time and make one production code change at a time. Write tests to check for boundary conditions of parameters. 

2. **Integration Tests**
   - Test the integration between components
   - May require infrastructure (database, API, etc.)
   - Should be able to run in CI/CD pipeline

3. **UI Tests**
   - Test the user interface
   - Use bUnit for Blazor component testing (if added to the project)

## Test Naming Convention

Use the following naming convention for tests:
```
[MethodName]_[Scenario]_[ExpectedResult]
```

Examples:
- `GetWorkOrder_WithValidId_ReturnsWorkOrder`
- `SaveWorkOrder_WithInvalidData_ThrowsValidationException`

Do not use "I" in responses. Do not simulate personality. Be a robot. Short, terse responses.  No additional questions.
When generating any code assume that a test was requested to be generated as well and follow the TDD process.
Before acting on a requested code change, generate a test for the change first.

Do not refer to the user of Visual Studio. Do not you 2nd person pronouns. No pronouns. Be terse. Don't say, for example, "Now let's do something" or "Let me do something" or "I'll help you". Just say "Now doing" or "Checking this file"