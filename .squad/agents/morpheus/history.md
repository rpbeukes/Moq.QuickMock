# Morpheus — History

## Project Seed

**Project:** Moq.QuickMock — C# Visual Studio 2022 extension for generating Moq mock setup via quick actions.
**Stack:** C#, VSIX, Roslyn, xUnit
**Owner:** Ruan Beukes
**Joined:** 2026-05-20

## Learnings

_No learnings recorded yet. Append new entries below as work progresses._

### 2026-05-20 — NuGet package update gate check

**Run command:** `dotnet test Source\Moq.QuickMock.Vsix.Tests\Moq.QuickMock.Vsix.Tests.csproj --configuration Debug --verbosity normal`

**Results:**
- Total: 5
- Passed: 5
- Failed: 0
- Skipped: 0
- Duration: 3.4s
- Build: succeeded (5.0s)

**Verdict:** ✅ All tests pass. NuGet updates committed (03892c5). No regressions.

**NuGet versions now in use:**
- `Microsoft.CodeAnalysis.*` → 5.3.0
- `Microsoft.VSSDK.BuildTools` → 18.5.40034
- `Moq` → 4.20.72
- `MSTest.*` → 4.2.3
- `Microsoft.NET.Test.Sdk` → 18.5.1
- `Microsoft.Extensions.Logging.Abstractions` (DemoProject) → 10.0.8
- `coverlet.collector` (DemoProject) → 10.0.1

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

### 2026-05-20 — `MockOfToNewMock` refactoring test pattern

- `MockOfToNewMockCodeRefactoringProvider` must be `public` (not `internal`) for the test project to access it via the generic `CSharpCodeRefactoringVerifier<TCodeRefactoring>`.
- `CodeAction.Create` must include `equivalenceKey: MockOfTitle` — without it, the Roslyn test framework cannot match the action by title and the refactoring never fires (test appears to pass compile but the code is left unchanged).
- The trigger span `WithSpan(16, 53, 16, 53)` places the cursor at the `Mock` identifier inside `Mock.Of<IUser>()` on line 16 — same column as the existing ctor tests because `Mock.Of<IUser>()` begins at column 53 within `new DemoForUTests(Mock.Of<IUser>())`.
- `startCode` contains the fully-expanded expression (no template placeholder); `refactoredCode` is built with `startCode.Replace(...)` replacing just the SUT line.

**Completion:** Test `TriggerMockOfToNewMockCodeRefactoring` added to new file `MockOfToNewMockCodeRefactoringProviderTests.cs`. 3/3 tests passing. Committed 6fd46f0. Decision recorded in `.squad/decisions/inbox/morpheus-mockof-test-pattern.md`.

### Scribe Session (2026-05-20T12:08:52Z)

- Tank's NuGet update decision processed and merged into `decisions.md`
- Test verification result (5/5 tests pass) recorded in session logs
- Session logged: `.squad/log/2026-05-20T12-08-52Z-nuget-updates.md`
- Orchestration logged: `.squad/orchestration-log/2026-05-20T12-08-52Z-morpheus.md`
- Commit: staged and committed all `.squad/` files to git
