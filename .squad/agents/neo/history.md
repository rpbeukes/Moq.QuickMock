# Neo — History

## Project Seed

**Project:** Moq.QuickMock — C# Visual Studio 2022 extension for generating Moq mock setup via quick actions.
**Stack:** C#, VSIX, Roslyn, Moq, xUnit
**Owner:** Ruan Beukes
**Joined:** 2026-05-20

## Learnings

### 2026-05-20: Deprecated Roslyn Testing Packages
Audited deprecated NuGet packages across the solution. Found 6 deprecated packages, all in `Moq.QuickMock.Vsix.Tests`:

**C# Testing Packages (actively used):**
- `Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.MSTest` → Replace with `Microsoft.CodeAnalysis.CSharp.Analyzer.Testing`
- `Microsoft.CodeAnalysis.CSharp.CodeFix.Testing.MSTest` → Replace with `Microsoft.CodeAnalysis.CSharp.CodeFix.Testing`
- `Microsoft.CodeAnalysis.CSharp.CodeRefactoring.Testing.MSTest` → Replace with `Microsoft.CodeAnalysis.CSharp.CodeRefactoring.Testing`

**VB Testing Packages (unused — can remove):**
- `Microsoft.CodeAnalysis.VisualBasic.Analyzer.Testing.MSTest`
- `Microsoft.CodeAnalysis.VisualBasic.CodeFix.Testing.MSTest`
- `Microsoft.CodeAnalysis.VisualBasic.CodeRefactoring.Testing.MSTest`

**Migration approach:** The existing verifier code already uses framework-agnostic patterns (`DefaultVerifier`, `CSharpCodeRefactoringTest<T, TVerifier>`). Migration is a drop-in package replacement. VB packages and unused verifier files can be removed entirely — no VB tests exist.

Full plan documented in `.squad/decisions/inbox/neo-deprecated-packages-plan.md`.

### 2026-05-20: DemoProject .NET 10 upgrade
Upgraded the demo solution projects from `netcoreapp3.1` to `net10.0` after confirming the .NET 10 SDK (`10.0.203`) was installed.

`DemoProjectUnitTests` contains intentionally non-compiling example source files used for refactoring demos (`DemoClassOnlyTests.cs`, `DemoForUTests.cs`). To keep the sample files intact while making the solution buildable, they are now excluded from compilation and included as non-build items in the project.
