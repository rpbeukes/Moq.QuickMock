using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMockToMockOfCS = Moq.QuickMock.Test.CSharpCodeRefactoringVerifier<
    Moq.QuickMock.MockToMockOfCodeRefactoringProvider>;

namespace Moq.QuickMock.Tests;

[TestClass]
public class MockToMockOfCodeRefactoringProviderTests
{
    [TestMethod]
    public async Task TriggerMockObjectToMockOf_ReplacesArgAndRemovesLocalVar()
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
            var userMock = new Mock<IUser>();
            var systemUnderTest = new DemoForUTests(userMock.Object);
        }
    }

    public interface IUser
    {
        string Name { get; set; }
    }
}
";

        var refactoredCode = startCode.Replace(
            "            var userMock = new Mock<IUser>();\r\n            var systemUnderTest = new DemoForUTests(userMock.Object);",
            "            var systemUnderTest = new DemoForUTests(Mock.Of<IUser>());");

        DiagnosticResult[] expectedDiagnostic =
        [
            // Cursor placed on `userMock` in userMock.Object — line 17, col 53
            DiagnosticResult.CompilerError("Refactoring").WithSpan(17, 53, 17, 53),
        ];

        await VerifyMockToMockOfCS.VerifyRefactoringAsync(startCode,
                                                          refactoredCode,
                                                          expectedDiagnostic,
                                                          actionTitle: MockToMockOfCodeRefactoringProvider.MockObjectTitle);
    }

    [TestMethod]
    public async Task TriggerMockObjectToMockOf_ReplacesArg_DoesNotRemoveVar_WhenDeclaredSeparately()
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
            Mock<IUser> userMock;
            userMock = new Mock<IUser>();
            var systemUnderTest = new DemoForUTests(userMock.Object);
        }
    }

    public interface IUser
    {
        string Name { get; set; }
    }
}
";

        var refactoredCode = startCode.Replace("userMock.Object", "Mock.Of<IUser>()");

        DiagnosticResult[] expectedDiagnostic =
        [
            // Cursor placed on `userMock` in userMock.Object — line 18, col 53
            DiagnosticResult.CompilerError("Refactoring").WithSpan(18, 53, 18, 53),
        ];

        await VerifyMockToMockOfCS.VerifyRefactoringAsync(startCode,
                                                          refactoredCode,
                                                          expectedDiagnostic,
                                                          actionTitle: MockToMockOfCodeRefactoringProvider.MockObjectTitle);
    }
}
