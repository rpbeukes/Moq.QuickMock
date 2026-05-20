# Morpheus — History

## Project Seed

**Project:** Moq.QuickMock — C# Visual Studio 2022 extension for generating Moq mock setup via quick actions.
**Stack:** C#, VSIX, Roslyn, xUnit
**Owner:** Ruan Beukes
**Joined:** 2026-05-20

## Learnings

_No learnings recorded yet. Append new entries below as work progresses._

### 2026-05-20 — `.slnx` migration regression check

**Run command:** `dotnet test Source\Moq.QuickMock.Vsix.Tests\Moq.QuickMock.Vsix.Tests.csproj --configuration Debug --verbosity normal`

**Results:**
- Total: 1
- Passed: 1
- Failed: 0
- Skipped: 0
- Duration: 4.3s
- Build: succeeded (7.0s)

**Verdict:** ✅ All tests pass. No regressions introduced by the `.sln` → `.slnx` migration (branch: `upgrade`).

**Session log:** See `.squad/log/2026-05-20T09-26-13Z-slnx-migration.md`
**Orchestration log:** See `.squad/orchestration-log/2026-05-20T09-26-13Z-morpheus.md`

### 2026-05-20 — `Mock ctor (Moq)` refactoring test pattern

- `MoqQuickMockCodeRefactoringProvider.MockCtorTitle` needs an explicit `refactoredCode` string replacement for the full `var systemUnderTest = |{0}|;` line because the action inserts `var ...Mock` declarations *before* the SUT line.
- Reuse the same refactoring cursor diagnostic span as the existing ctor test: `DiagnosticResult.CompilerError("Refactoring").WithSpan(16, 53, 16, 53)`.
- Whitespace matters: the expected replacement must preserve the 12-space indentation in the method body and use exact `\r\n` line breaks between the inserted mock declarations and the SUT line.

**Completion:** Test `TriggerMockCtorCodeRefactoring` added to `MoqQuickMockCodeRefactoringProviderTests.cs`. 2/2 tests passing. Committed 8233b61. Decision recorded in `.squad/decisions.md`.
