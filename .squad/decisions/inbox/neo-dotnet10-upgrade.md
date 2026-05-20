# Decision: DemoProject .NET 10 Upgrade

**Date:** 2026-05-20  
**Owner:** Neo  
**Status:** Proposed

## Summary
Upgrade the `DemoProject` solution from `netcoreapp3.1` to `net10.0`, and preserve intentionally broken demo inputs by excluding them from compilation rather than altering the example code.

## Context
`DemoProject` is a sample/demo solution used to exercise and demonstrate Moq.QuickMock refactorings. Two files in `DemoProjectUnitTests` (`DemoClassOnlyTests.cs` and `DemoForUTests.cs`) intentionally contain constructor calls that do not compile, because they are example inputs for the extension's refactorings.

After the TFM upgrade, a clean build exposed that these files prevent the test project from compiling. Changing the sample code would defeat the purpose of the demos.

## Decision
- Change both demo projects to `net10.0`
- Keep the broken demo inputs unchanged
- Mark the demo input `.cs` files as non-compiling project items (`<Compile Remove=... />` + `<None Include=... />`) so the solution remains buildable

## Consequences
- `dotnet build DemoProject\DemoProject.sln` succeeds on .NET 10
- The demo files remain available in the project for manual refactoring demos
- `dotnet test` reports zero discovered tests in `DemoProjectUnitTests`, which is acceptable because the project serves as demo input, not an automated test suite
