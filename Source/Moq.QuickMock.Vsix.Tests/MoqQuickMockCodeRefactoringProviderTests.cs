﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Moq.QuickMock.Test.CSharpCodeRefactoringVerifier<
    Moq.QuickMock.MoqQuickMockCodeRefactoringProvider>;

namespace Moq.QuickMock.Tests;

[TestClass]
public class MoqQuickMockCodeRefactoringProviderTests
{
    // This test still fails with syntax node differences which is under investigation
    // all testing is done manually :(
    [TestMethod]
    public async Task TriggerCodeRefactoring()
    {
        var test = @"
using System;

namespace DemoProject.Tests
{
    public class DemoForUTests
    {
        public DemoForUTests(string stringValue, int intValue)
        { }
    }

    public class DemoForUTTests
    {
        public void DemoForUTTests_test()
        {
            var systemUnderTest = new DemoForUTests();
        }
    }
}
";

        var fixtest = @"
using System;

namespace DemoProject.Tests
{
    public class DemoForUTests
    {
        public DemoForUTests(string stringValue, int intValue)
        { }
    }

    public class DemoForUTTests
    {
        public void DemoForUTTests_test()
        {
            var systemUnderTest = new DemoForUTests(It.IsAny<string>(), It.IsAny<int>());
        }
    }
}
";

        var expectedDiagnostic = DiagnosticResult
                                    .CompilerError("CS7036")
                                    .WithSpan(16, 39, 16, 52)
                                    .WithArguments("stringValue", "DemoProject.Tests.DemoForUTests.DemoForUTests(string, int)")
                                    ;

        var expectedRefactoring = DiagnosticResult
                                    .CompilerError("Refactoring")
                                    .WithSpan(16, 53, 16, 53)
                                    ;

        await VerifyCS.VerifyRefactoringAsync(test,
                                              fixtest,
                                              [expectedDiagnostic, expectedRefactoring],
                                              actionTitle: MoqQuickMockCodeRefactoringProvider.QuickMockCtorTitle,
                                              testBehaviors: TestBehaviors.SkipGeneratedCodeCheck);
    }
}
