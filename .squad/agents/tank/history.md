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

### GitHub Release V1.0.8.94 Draft (2026-05-20)

- **Comparison:** V1.0.8.85 → V1.0.8.94 (9 commits since last release)
- **Release URL:** https://github.com/rpbeukes/Moq.QuickMock/releases/tag/untagged-e6464481ac0fb5b38ee0
- **Status:** Draft (ready for review before publishing)
- **Release notes grouped by PR:**
  - PR #11: New unit tests for Mock.Object refactoring + NuGet updates + deprecated package migration
  - PR #12: DemoProject .NET 10 upgrade + .slnx format migration + csproj fix + README updates
- **Format:** Matched existing release notes style with contributor mentions and changelog link
- **Command used:** `gh release create V1.0.8.94 --draft --title "V1.0.8.94" --notes "..."`

### VS Marketplace Auto-Deploy Investigation (2026-05-21)

- `CI_main.yml` already provides the right release input: it rewrites `source.extension.vsixmanifest` version from base `1.0.8` to `1.0.8.<github.run_number>`, builds the VSIX with MSBuild, and uploads `Moq.QuickMock.<version>.vsix` as the CI artifact.
- Current workflow gap is release integrity metadata: CI does **not** yet emit a raw-file SHA256 for the VSIX, so the release/deploy pipeline should add `Moq.QuickMock.<version>.vsix.sha256` and upload it alongside the VSIX in the same artifact bundle.
- The release-side lookup can be done without rebuilding by using the GitHub REST API: query `GET /repos/{owner}/{repo}/actions/artifacts?name=Moq.QuickMock.<version>.vsix`, filter for non-expired artifacts from `main`, then download `GET /repos/{owner}/{repo}/actions/artifacts/{artifact_id}/zip` with `GITHUB_TOKEN`.
- For Marketplace publishing, the correct GitHub Actions approach is **not** VS Code tooling (`vsce`, `cschleiden/vscode-marketplace-publish`, `microsoft/vscode-extension-test-runner`); it is `VsixPublisher.exe` discovered on `windows-latest` via `vswhere` and invoked with a Marketplace PAT plus a publish manifest.
- Key operational constraints: current repo tags are uppercase `V...` while Ruan's desired flow uses lowercase `v...`; the workflow should accept both. Also, GitHub Actions artifact retention is the hard limit for this integrity model—once the CI artifact expires, the exact original VSIX cannot be deployed later without violating the no-rebuild requirement.

### VS Marketplace Pipeline Implementation (2026-05-21)

- **Modified:** `.github\workflows\CI_main.yml`
- **Created:** `.github\workflows\CD_release.yml`
- **Created:** `.squad\decisions\inbox\tank-marketplace-pipeline-implemented.md`
- CI now prepares a release bundle under `artifacts\<version>`, computes `Moq.QuickMock.<version>.vsix.sha256`, exposes the VSIX SHA256 via `GITHUB_ENV`, and uploads the bundle with 90-day retention.
- CD now triggers on `release: published`, parses tags in `v1.0.8.95` format while accepting both lowercase `v` and uppercase `V`, resolves the matching non-expired CI artifact from `main`, verifies the extracted VSIX checksum, uploads the VSIX plus `.sha256` to the GitHub Release, and publishes the same VSIX with `VsixPublisher.exe`.
- **Confirmed decisions applied:** publisher `Rpbeukes`, Marketplace internal name `MoqQuickMock2022`, manual tagging/release publication strategy (Option B), and 90-day artifact expiry accepted.

### Developer Guide (README-DEV.md) (2026-05-21)

- **Created:** `README-DEV.md` at repo root
- Comprehensive developer guide covering:
  - Prerequisites and project setup (Visual Studio 2022/2026, .NET SDK 8.0, MSBuild)
  - Project structure (three projects: core library, VSIX, tests)
  - Local build instructions (dotnet restore/build/test, MSBuild for VSIX)
  - Versioning model (base version + run number = 4-part version)
  - CI pipeline (CI_main.yml triggers, build steps, artifact retention, SHA256)
  - Release process (full step-by-step: merge → CI → tag → publish → CD)
  - SHA256 verification guide (for users downloading the VSIX)
  - VS Marketplace PAT setup (dev.azure.com, GitHub Secrets, scope requirements)
  - Troubleshooting and quick reference commands
- Document provides accurate technical details extracted from CI/CD workflows and source manifest

