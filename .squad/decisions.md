# Squad Decisions

## Active Decisions

### Decision: .slnx Migration (VS V18 / Visual Studio 2026)
**Date:** 2026-05-20  
**Owner:** Tank (DevOps)  
**Status:** ✅ Completed

**Summary:** Migrated Moq.QuickMock solution from legacy `.sln` format (VS V17) to new `.slnx` format (VS V18 / Visual Studio 2026).

**Details:**
- Generated `Moq.QuickMock.slnx` alongside existing `Moq.QuickMock.sln`
- New file uses XML format with simplified project references
- All 3 projects included: Moq.QuickMock, Moq.QuickMock.Vsix, Moq.QuickMock.Vsix.Tests

**Build Impact:**
- ✅ Non-VSIX projects: Build successfully with `dotnet build` via .slnx
- ⚠️ VSIX project: Requires MSBuild (not `dotnet build`) due to VSIX SDK limitations
- ✅ CI/CD: No changes needed — workflows already build projects individually, not via solution file
- ✅ Legacy compatibility: Original .sln remains for developers on older VS versions

**Developer Experience:**
- Visual Studio 2026 users can now open `Moq.QuickMock.slnx`
- Older VS versions continue to use `Moq.QuickMock.sln`
- No breaking changes

**Rationale:**
1. **Forward compatibility:** .slnx format is the new standard for VS 2026+
2. **Non-disruptive:** Keeping both files allows gradual migration without breaking older tooling
3. **Minimal overhead:** Single XML file, easy to maintain
4. **No CI/CD changes required:** Existing workflow already handles individual project builds

**Risks & Mitigations:**
| Risk | Mitigation |
|------|-----------|
| Developer confusion (two solution files) | Documented in squad history; IDE will default to latest format |
| .slnx format instability | MS maintains both formats; rollback to .sln if needed |

**Next Steps:**
- ✅ Migration complete and committed
- Developers can opt-in to using .slnx in VS 2026
- Monitor for any compatibility issues in coming releases

### Decision: MockCtor Test Pattern
**Date:** 2026-05-20  
**Owner:** Morpheus  
**Status:** ✅ Established

**Summary:** `MoqQuickMockCodeRefactoringProvider.MockCtorTitle` generates `var ...Mock` declarations before the system-under-test line, distinct from `QuickMockCtorTitle`.

**Testing Pattern:**
- Keep `startCode` identical to existing ctor refactoring test (diagnostic span: `WithSpan(16, 53, 16, 53)`)
- Build `refactoredCode` by replacing the full `            var systemUnderTest = |{0}|;` line (not just the placeholder)
- Replacement must include exact method-body indentation and explicit `\r\n` breaks:
  - `var userMock = new Mock<IUser>();`
  - `var cmdFactoryMock = new Mock<Func<SomeCommand>>();`
  - `var systemUnderTest = new DemoForUTests(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>(), userMock.Object, cmdFactoryMock.Object);`

**Implementation Notes:**
- Roslyn refactoring verification is whitespace-sensitive
- Mismatched indentation or line endings will fail expected output comparison
- Test added to `MoqQuickMockCodeRefactoringProviderTests.cs`, verifies Mock ctor action generates var {name}Mock declarations

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction
