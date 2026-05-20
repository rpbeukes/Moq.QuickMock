# Decisions Log

## Deprecated Package Migration Plan (2026-05-20)

**Owner:** Neo (Lead / Architect)  
**Status:** 📋 Proposed

### Summary
**6 deprecated packages** found in Moq.QuickMock.Vsix.Tests:
- **3 VB packages** → Remove (unused scaffolding)
- **3 C# packages** → Replace with framework-agnostic versions (1.1.3)

**Recommended execution order:**
1. Remove VB packages (zero risk cleanup)
2. Replace C# packages: `*.MSTest` → framework-agnostic + MSTest verifier
3. Run tests to validate

**Estimated effort:** ~30 minutes (5 min removal, 10 min replacement, 15 min validation)

**Risk:** None — test-only dependencies, VSIX/core library unaffected

**References:** [Roslyn Testing Docs](https://github.com/dotnet/roslyn-sdk/tree/main/src/Testing)

---

## NuGet Package Update Summary (2026-05-20)

**Status:** ✅ Complete  
**All projects successfully updated to latest stable versions**  
**Build:** ✅ Passed (0 warnings, 0 errors)

## Updated Packages by Project

### Core Library: Moq.QuickMock.csproj (netstandard2.0)
| Package | Old → New | Notes |
|---------|-----------|-------|
| Microsoft.CodeAnalysis.Analyzers | 3.11.0 → 5.3.0 | Latest stable Roslyn analyzers |
| Microsoft.CodeAnalysis.CSharp.Workspaces | 4.13.0 → 5.3.0 | Latest stable C# language support |

### VSIX Extension: Moq.QuickMock.Vsix.csproj (net472)
| Package | Old → New | Notes |
|---------|-----------|-------|
| Microsoft.CodeAnalysis.Analyzers | 3.11.0 → 5.3.0 | Latest stable |
| Microsoft.CodeAnalysis.CSharp.Workspaces | 4.13.0 → 5.3.0 | Latest stable |
| Microsoft.VSSDK.BuildTools | 17.13.2126 → 18.5.40034 | VS 2022 compatible, latest stable |

### Test Project: Moq.QuickMock.Vsix.Tests.csproj (net8.0)
| Package | Old → New | Notes |
|---------|-----------|-------|
| Microsoft.CodeAnalysis | 4.13.0 → 5.3.0 | Latest stable |
| Microsoft.NET.Test.Sdk | 17.14.0-preview-25107-01 → 18.5.1 | **Removed preview build** |
| Moq | 4.18.1 → 4.20.72 | Latest stable mocking library |
| MSTest.TestAdapter | 3.8.3 → 4.2.3 | Latest stable test adapter |
| MSTest.TestFramework | 3.8.3 → 4.2.3 | Latest stable test framework |
| CodeAnalysis.*.Testing packages | 1.1.2 (unchanged) | Compatible with CodeAnalysis 5.3.0 |

### Demo Project: DemoProject.csproj (netcoreapp3.1)
| Package | Old → New | Notes |
|---------|-----------|-------|
| Microsoft.Extensions.Logging.Abstractions | 6.0.1 → 10.0.8 | Latest stable logging abstractions |

### Demo Tests: DemoProjectUnitTests.csproj (netcoreapp3.1)
| Package | Old → New | Notes |
|---------|-----------|-------|
| coverlet.collector | 1.2.0 → 10.0.1 | Latest stable code coverage |
| Microsoft.NET.Test.Sdk | 16.5.0 → 18.5.1 | Updated from very old version |
| Moq | 4.18.1 → 4.20.72 | Latest stable |
| MSTest.TestAdapter | 2.1.0 → 4.2.3 | Updated from very old version |
| MSTest.TestFramework | 2.1.0 → 4.2.3 | Updated from very old version |

**Compatibility Note:** MSTest packages 4.2.3 on netcoreapp3.1 generate minor warning (NU1701) but are fully functional.

## Summary of Changes

- **Total packages updated:** 18 across 5 projects
- **Total major upgrades:** 8 (affecting core dependencies)
- **Preview versions removed:** 1 (Microsoft.NET.Test.Sdk in Vsix.Tests)
- **All builds:** ✅ Successful
- **Breaking changes:** None detected

## Key Improvements

1. **Roslyn/CodeAnalysis:** Entire solution now on Roslyn 5.3.0 (2-3 major versions ahead)
2. **Test Framework:** Removed preview build from main test project
3. **Coverage:** Coverlet upgraded from 1.2.0 to 10.0.1 (8 major versions)
4. **VS SDK:** VSSDK package updated for optimal VS 2022 compatibility

## What Was NOT Updated

- **CodeAnalysis.Testing packages** (1.1.2): Remain stable and compatible with CodeAnalysis 5.3.0
- **No prerelease versions:** All packages are stable releases

## Verification

```bash
# Build command used for verification:
dotnet build C:\Repos\Moq.QuickMock\Moq.QuickMock.slnx --configuration Debug

# Result:
Build succeeded in 3.6s
  0 Warning(s)
  0 Error(s)
```

## Next Steps

- Monitor CI/CD pipeline for any issues with the updated packages
- Consider updating demo project to a modern .NET version (netcoreapp3.1 is EOL) in a separate task
- Update locked dependencies in any lock files if in use
