# DemoProject .slnx Migration

- **Date:** 2026-05-20
- **Owner:** Tank (DevOps)
- **Status:** Proposed

## Summary
Migrate the demo-only sub-solution from `DemoProject.sln` to `DemoProject.slnx` and remove the legacy `.sln` after validating builds on the .NET 10 toolchain.

## Why this matters
- The installed SDK supports native `.slnx` migration and build workflows.
- The demo solution is internal to this repository and does not need dual-format compatibility.
- Keeping only one solution file avoids drift between `DemoProject.sln` and `DemoProject.slnx`.

## Verification
- `dotnet solution DemoProject.sln migrate` generated `DemoProject.slnx`
- `dotnet build DemoProject\DemoProject.slnx --configuration Debug` succeeded
- `Moq.QuickMock.slnx` does not reference `DemoProject\DemoProject.sln`
