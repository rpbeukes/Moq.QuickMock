using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMockOfCS = Moq.QuickMock.Test.CSharpCodeRefactoringVerifier<
    Moq.QuickMock.MockOfToNewMockCodeRefactoringProvider>;

namespace Moq.QuickMock.Tests;

[TestClass]
public class MockOfToNewMockCodeRefactoringProviderTests
{
    [TestMethod]
    public async Task TriggerMockOfToNewMockCodeRefactoring()
    {
        var startCode = @"
using System;
using Moq;
namespace DemoProject.Tests
{
    public class DemoForUTests
    {
        public DemoForUTests(IUser user)
        { }
    }

    public class DemoForUTTests
    {
        public void DemoForUTTests_test()
        {
            var systemUnderTest = new DemoForUTests(Mock.Of<IUser>());
        }
    }

    public interface IUser
    {
        string Name { get; set; }
    }
}
";

        var refactoredCode = startCode.Replace(
            "            var systemUnderTest = new DemoForUTests(Mock.Of<IUser>());",
            "            var userMock = new Mock<IUser>();\r\n" +
            "            var systemUnderTest = new DemoForUTests(userMock.Object);");

        DiagnosticResult[] expectedDiagnostic =
        [
            // Cursor placed on `Mock` in Mock.Of<IUser>() — line 16, col 53
            DiagnosticResult.CompilerError("Refactoring").WithSpan(16, 53, 16, 53),
        ];

        await VerifyMockOfCS.VerifyRefactoringAsync(startCode,
                                                    refactoredCode,
                                                    expectedDiagnostic,
                                                    actionTitle: MockOfToNewMockCodeRefactoringProvider.MockOfTitle);
    }
}
