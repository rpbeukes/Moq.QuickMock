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
