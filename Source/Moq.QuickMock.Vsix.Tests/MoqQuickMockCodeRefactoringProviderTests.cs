using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Moq.QuickMock.Test.CSharpCodeRefactoringVerifier<
    Moq.QuickMock.MoqQuickMockCodeRefactoringProvider>;

namespace Moq.QuickMock.Tests;

[TestClass]
public class MoqQuickMockCodeRefactoringProviderTests
{
    [TestMethod]
    public async Task TriggerCodeRefactoring()
    {
        var codeTemplate = @"
using System;
using Moq;
namespace DemoProject.Tests
{
    public class DemoForUTests
    {
        public DemoForUTests(string stringValue, int intValue, int? nullIntValue, ICurrentUser user, Func<SomeCommand> cmdFactory)
        { }
    }

    public class DemoForUTTests
    {
        public void DemoForUTTests_test()
        {
            var systemUnderTest = |{0}|;
        }
    }

    public interface ICurrentUser
    {
        string Name { get; set; }
    }
   
    public class SomeCommand
    {
        public void DoSomething()
        {

        }
    }
}
";

        var startCode = codeTemplate.Replace("|{0}|", "new DemoForUTests()");
        var refactoredCode = codeTemplate.Replace("|{0}|", "new DemoForUTests(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>(), Mock.Of<ICurrentUser>(), Mock.Of<Func<SomeCommand>>())");

        DiagnosticResult[] expectedDiagnostic =
        [
            // Special diagnostic needed for refactoring
            DiagnosticResult.CompilerError("Refactoring").WithSpan(16, 53, 16, 53),
        ];

        await VerifyCS.VerifyRefactoringAsync(startCode,
                                              refactoredCode,
                                              expectedDiagnostic,
                                              actionTitle: MoqQuickMockCodeRefactoringProvider.QuickMockCtorTitle);
    }
}
