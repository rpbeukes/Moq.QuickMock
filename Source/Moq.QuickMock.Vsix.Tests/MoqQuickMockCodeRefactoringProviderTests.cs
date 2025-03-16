using System.Linq;
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

    [TestMethod]
    public async Task TriggerBothDiagnosticsAndCodeFix()
    {
        var test = @"
using System;

namespace DemoProject.Tests
{
    /// <summary>
    /// Demo for uni tests
    /// </summary>
    public class DemoForUTests
    {
        public DemoForUTests(string stringValue, int intValue)
        {

        }
    }

    public class DemoForUTTests
    {
        public void DemoForUT_Quick_mock()
        {
            var systemUnderTest = new DemoForUTests({|#0:|});
        }
    }
}
";

        var fixtest = @"
using System;

namespace DemoProject.Tests
{
    /// <summary>
    /// Demo for uni tests
    /// </summary>
    public class DemoForUTests
    {
        public DemoForUTests(string stringValue, int intValue)
        {

        }
    }

    public class DemoForUTTests
    {
        public void DemoForUT_Quick_mock()
        {
            var systemUnderTest = new DemoForUTests(It.IsAny<string>(), It.IsAny<int>());
        }
    }
}
";

        //var systemUnderTest = new DemoForUTests(It.IsAny<string>(), It.IsAny<int>());
        //var systemUnderTest = new DemoForUTests(It.IsAny<string stringValue>(), It.IsAny<int intValue>());

        var expectedDiagnostic = DiagnosticResult
                                    .CompilerError("CS7036")
                                    .WithSpan(21, 39, 21, 52)
                                    .WithArguments("stringValue", "DemoProject.Tests.DemoForUTests.DemoForUTests(string, int)")
                                    ;

        var expectedRefactoring = DiagnosticResult
                                    .CompilerError("Refactoring")
                                    .WithSpan(21, 53, 21, 53)
                                    ;


        //var rrr = new DiagnosticResultSpan
        //{
        //    
        //}
        await VerifyCS.VerifyRefactoringAsync(test,
                                              fixtest,
                                              [expectedDiagnostic, expectedRefactoring],
                                              actionTitle: "Quick mock ctor (Moq)",
                                              testBehaviors: TestBehaviors.SkipGeneratedCodeCheck);
    }
}
