# Morpheus — MockCtor test pattern

## Context
`MoqQuickMockCodeRefactoringProvider.MockCtorTitle` differs from `QuickMockCtorTitle` because it inserts `var ...Mock` declarations before the system-under-test line.

## Testing Pattern
- Keep `startCode` identical to the existing ctor refactoring test so the diagnostic span remains `WithSpan(16, 53, 16, 53)`.
- Build `refactoredCode` by replacing the full `            var systemUnderTest = |{0}|;` line, not just `|{0}|`.
- The replacement must include exact method-body indentation and explicit `\r\n` breaks:
  - `var userMock = new Mock<IUser>();`
  - `var cmdFactoryMock = new Mock<Func<SomeCommand>>();`
  - `var systemUnderTest = new DemoForUTests(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>(), userMock.Object, cmdFactoryMock.Object);`

## Gotcha
Roslyn refactoring verification is whitespace-sensitive here; mismatched indentation or line endings will fail the expected output comparison.
