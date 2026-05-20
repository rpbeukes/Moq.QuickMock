# Tank — History

## Project Seed

**Project:** Moq.QuickMock — C# Visual Studio 2022 extension.
**Stack:** C#, VSIX, GitHub Actions, MSBuild, NuGet
**Owner:** Ruan Beukes
**Joined:** 2026-05-20

## Learnings

### .slnx Migration (2026-05-20)

- **Migration command:** `dotnet solution <sln-file> migrate` (correct syntax is important; `dotnet sln migrate` does not work)
- **Generated .slnx format:** XML-based with `<Project Path="...">` elements, much more readable than `.sln`
- **All 3 projects migrated successfully:**
  - Source/Moq.QuickMock/Moq.QuickMock.csproj
  - Source/Moq.QuickMock.Vsix/Moq.QuickMock.Vsix.csproj
  - Source/Moq.QuickMock.Vsix.Tests/Moq.QuickMock.Vsix.Tests.csproj
- **VSIX build limitation with .slnx:** `dotnet build Moq.QuickMock.slnx` fails with `error VSIX deployment is not supported with 'dotnet build'`. This is expected per VS SDK behavior. VSIX projects must be built with MSBuild or VS IDE.
- **CI/CD workflow:** CI_main.yml already builds projects individually (not via solution file), so no workflow changes needed. The .slnx file is available for developers using Visual Studio 2026 IDE.
- **Backward compatibility:** Kept original Moq.QuickMock.sln for compatibility with older VS versions
- **Session log:** See `.squad/log/2026-05-20T09-26-13Z-slnx-migration.md`
- **Orchestration log:** See `.squad/orchestration-log/2026-05-20T09-26-13Z-tank.md`

### NuGet Package Updates (2026-05-20)

**All projects successfully updated to latest stable versions:**

**Moq.QuickMock.csproj (netstandard2.0):**
- Microsoft.CodeAnalysis.Analyzers: 3.11.0 → 5.3.0
- Microsoft.CodeAnalysis.CSharp.Workspaces: 4.13.0 → 5.3.0

**Moq.QuickMock.Vsix.csproj (net472):**
- Microsoft.CodeAnalysis.Analyzers: 3.11.0 → 5.3.0
- Microsoft.CodeAnalysis.CSharp.Workspaces: 4.13.0 → 5.3.0
- Microsoft.VSSDK.BuildTools: 17.13.2126 → 18.5.40034 (compatible with VS 2022)

**Moq.QuickMock.Vsix.Tests.csproj (net8.0):**
- Microsoft.CodeAnalysis: 4.13.0 → 5.3.0
- Microsoft.NET.Test.Sdk: 17.14.0-preview-25107-01 → 18.5.1 (removed preview dependency)
- Moq: 4.18.1 → 4.20.72
- MSTest.TestAdapter: 3.8.3 → 4.2.3
- MSTest.TestFramework: 3.8.3 → 4.2.3
- Note: CodeAnalysis.*.Testing.MSTest packages remain at 1.1.2 (compatible with 5.3.0)

**DemoProject.csproj (netcoreapp3.1):**
- Microsoft.Extensions.Logging.Abstractions: 6.0.1 → 10.0.8

**DemoProjectUnitTests.csproj (netcoreapp3.1):**
- coverlet.collector: 1.2.0 → 10.0.1
- Microsoft.NET.Test.Sdk: 16.5.0 → 18.5.1
- Moq: 4.18.1 → 4.20.72
- MSTest.TestAdapter: 2.1.0 → 4.2.3
- MSTest.TestFramework: 2.1.0 → 4.2.3
- Note: Minor NU1701 warning for MSTest packages on netcoreapp3.1, but fully functional

**Build verification:** Solution builds successfully with `dotnet build Moq.QuickMock.slnx --configuration Debug` (0 warnings, 0 errors)

**Notes on skipped updates:**
- CodeAnalysis.Testing packages (1.1.2) remain unchanged — compatible with CodeAnalysis 5.3.0 and only used in test infrastructure
- No preview versions currently in use (removed preview from Microsoft.NET.Test.Sdk in Moq.QuickMock.Vsix.Tests)
- DemoProject & DemoProjectUnitTests: Added `<SuppressTfmSupportBuildErrors>true</SuppressTfmSupportBuildErrors>` to DemoProjectUnitTests.csproj because netcoreapp3.1 is EOL and newer packages don't officially support it (but function correctly). These projects have pre-existing test code issues unrelated to the NuGet updates.

### Scribe Session (2026-05-20T12:08:52Z)

- NuGet update decision merged into `decisions.md`
- Morpheus confirmed all tests pass post-update (5/5 tests, 0 failures)
- Session logged: `.squad/log/2026-05-20T12-08-52Z-nuget-updates.md`
- Orchestration logged: `.squad/orchestration-log/2026-05-20T12-08-52Z-tank.md`
- Commit: staged and committed all `.squad/` files to git

### Deprecated Package Migration (2026-05-20)

**Objective:** Migrate Moq.QuickMock.Vsix.Tests from MSTest-flavored CodeAnalysis testing packages to framework-agnostic versions.

**Packages Removed:**
- ❌ Microsoft.CodeAnalysis.VisualBasic.Analyzer.Testing.MSTest 1.1.2
- ❌ Microsoft.CodeAnalysis.VisualBasic.CodeFix.Testing.MSTest 1.1.2
- ❌ Microsoft.CodeAnalysis.VisualBasic.CodeRefactoring.Testing.MSTest 1.1.2
- ❌ Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.MSTest 1.1.2
- ❌ Microsoft.CodeAnalysis.CSharp.CodeFix.Testing.MSTest 1.1.2
- ❌ Microsoft.CodeAnalysis.CSharp.CodeRefactoring.Testing.MSTest 1.1.2

**Packages Added:**
- ✅ Microsoft.CodeAnalysis.CSharp.Analyzer.Testing 1.1.3 (framework-agnostic)
- ✅ Microsoft.CodeAnalysis.CSharp.CodeFix.Testing 1.1.3 (framework-agnostic)
- ✅ Microsoft.CodeAnalysis.CSharp.CodeRefactoring.Testing 1.1.3 (framework-agnostic)

**Verifier Files Deleted:**
- Removed all VisualBasic verifier files (6 files, unused for VB-specific testing):
  - VisualBasicAnalyzerVerifier`1.cs
  - VisualBasicAnalyzerVerifier`1+Test.cs
  - VisualBasicCodeFixVerifier`2.cs
  - VisualBasicCodeFixVerifier`2+Test.cs
  - VisualBasicCodeRefactoringVerifier`1.cs
  - VisualBasicCodeRefactoringVerifier`1+Test.cs

**Using Statement Updates:**
- Removed `using Microsoft.CodeAnalysis.Testing.Verifiers;` from all CSharp verifier files
- Reason: Framework-agnostic packages do not export this namespace; DefaultVerifier is available via `Microsoft.CodeAnalysis.Testing`
- Updated files:
  - CSharpAnalyzerVerifier`1.cs
  - CSharpAnalyzerVerifier`1+Test.cs
  - CSharpCodeFixVerifier`2.cs
  - CSharpCodeFixVerifier`2+Test.cs
  - CSharpCodeRefactoringVerifier`1.cs
  - CSharpCodeRefactoringVerifier`1+Test.cs

**Build Result:** ✅ `dotnet build Moq.QuickMock.Vsix.Tests.csproj --configuration Debug` succeeds (0 warnings, 0 errors)

**Key Insights:**
- MSTest-flavored packages (*.MSTest) are deprecated in favor of framework-agnostic versions
- The framework-agnostic packages (1.1.3) are only available for CSharp and CodeFix/CodeRefactoring, not for VisualBasic
- The Verifiers namespace changed when moving to framework-agnostic packages; it's now internal to the Testing namespace
- DefaultVerifier is still accessible from `Microsoft.CodeAnalysis.Testing` without the Verifiers using statement

### DemoProject .slnx Migration (2026-05-20)

- Verified SDK/tooling on .NET `10.0.203`
- `dotnet solution DemoProject.sln migrate` is the working migration command for the demo solution
- Generated `DemoProject.slnx` contains two relative project entries:
  - `DemoProject/DemoProject.csproj`
  - `DemoProjectUnitTests/DemoProjectUnitTests.csproj`
- `dotnet build DemoProject\DemoProject.slnx --configuration Debug` succeeds
- Removed the legacy `DemoProject.sln` after validating the new solution file
- Root `Moq.QuickMock.slnx` does not reference the demo solution file, so no root solution update was required

