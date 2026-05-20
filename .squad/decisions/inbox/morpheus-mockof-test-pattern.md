# Decision: MockOf Test Pattern

**Date:** 2026-05-20  
**Owner:** Morpheus  
**Status:** ✅ Established  

## Summary

Pattern for testing `MockOfToNewMockCodeRefactoringProvider` using the Roslyn `CSharpCodeRefactoringVerifier<T>` harness.

## Key Rules

### 1. Provider visibility
`MockOfToNewMockCodeRefactoringProvider` must be `public` (not `internal`) so the test project can reference it via the generic verifier type parameter.

### 2. Equivalence key required
`CodeAction.Create` must include `equivalenceKey: MockOfTitle`. Without it the Roslyn test framework cannot filter by action title and the refactoring never fires — the code is left unchanged and the test fails on a misleading diff.

```csharp
public static string MockOfTitle = "Mock.Of<T> to new Mock<T> (Moq)";

var action = CodeAction.Create(MockOfTitle, async (c) => { ... }, equivalenceKey: MockOfTitle);
```

### 3. Trigger span
`DiagnosticResult.CompilerError("Refactoring").WithSpan(16, 53, 16, 53)` places the cursor at the `Mock` identifier inside `Mock.Of<IUser>()` on line 16 of the standard test template (column 53, 1-indexed).

Line 16 content:
```
            var systemUnderTest = new DemoForUTests(Mock.Of<IUser>());
```
Column breakdown: 12 spaces + `var ` + `systemUnderTest ` + `= ` + `new ` + `DemoForUTests` + `(` = col 52, then `Mock` starts at col 53.

### 4. startCode / refactoredCode shape
- `startCode` — verbatim string with the full expression already present (no `|{0}|` placeholder).
- `refactoredCode` — built via `startCode.Replace(...)` replacing the entire SUT line with the `var {name}Mock` declaration line + the SUT line using `.Object`.
- Indentation: 12 spaces, line break separator: `\r\n`.

```csharp
var refactoredCode = startCode.Replace(
    "            var systemUnderTest = new DemoForUTests(Mock.Of<IUser>());",
    "            var userMock = new Mock<IUser>();\r\n" +
    "            var systemUnderTest = new DemoForUTests(userMock.Object);");
```

### 5. File path
The verifier's `DefaultFilePathPrefix` (`/0/TheTests`) and `ChangeFileName` helper ensure the file path seen by the provider contains `"tests.cs"`, satisfying the provider's guard.

## Implementation

- Provider: `Source\Moq.QuickMock\MockOfToNewMockCodeRefactoringProvider.cs`
- Test: `Source\Moq.QuickMock.Vsix.Tests\MockOfToNewMockCodeRefactoringProviderTests.cs`
- Test method: `TriggerMockOfToNewMockCodeRefactoring`
- All 3 tests passing. Committed: `6fd46f0`.
