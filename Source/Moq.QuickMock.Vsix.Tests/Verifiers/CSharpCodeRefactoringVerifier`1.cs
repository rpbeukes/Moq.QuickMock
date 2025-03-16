using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Moq.QuickMock.Test
{
    public static partial class CSharpCodeRefactoringVerifier<TCodeRefactoring>
        where TCodeRefactoring : CodeRefactoringProvider, new()
    {
        /// <inheritdoc cref="CodeRefactoringVerifier{TCodeRefactoring, TTest, TVerifier}.VerifyRefactoringAsync(string, string)"/>
        public static async Task VerifyRefactoringAsync(string source, string fixedSource)
        {
            await VerifyRefactoringAsync(source, DiagnosticResult.EmptyDiagnosticResults, fixedSource);
        }

        /// <inheritdoc cref="CodeRefactoringVerifier{TCodeRefactoring, TTest, TVerifier}.VerifyRefactoringAsync(string, DiagnosticResult, string)"/>
        public static async Task VerifyRefactoringAsync(string source, DiagnosticResult expected, string fixedSource)
        {
            await VerifyRefactoringAsync(source, new[] { expected }, fixedSource);
        }

        /// <inheritdoc cref="CodeRefactoringVerifier{TCodeRefactoring, TTest, TVerifier}.VerifyRefactoringAsync(string, DiagnosticResult[], string)"/>
        public static async Task VerifyRefactoringAsync(string source, DiagnosticResult[] expected, string fixedSource)
        {
            var test = new Test
            {
                TestCode = source,
                FixedCode = fixedSource,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            await test.RunAsync(CancellationToken.None);
        }

        public static async Task VerifyRefactoringAsync(string source,
                                                        string fixedSource,
                                                        DiagnosticResult[] expected = null,
                                                        string actionTitle = null,
                                                        TestBehaviors testBehaviors = TestBehaviors.None)
        {
            var test = new Test
            {
                TestCode = source,
                FixedCode = fixedSource,
                TestBehaviors = testBehaviors,
                //CompilerDiagnostics = CompilerDiagnostics.Errors
            };

            //test.TestCode = source;
            //test.FixedCode = fixedSource;

            if (expected != null && expected.Any())
            {
                test.ExpectedDiagnostics.AddRange(expected);
            }

            if (actionTitle != null)
            {
                test.CodeActionEquivalenceKey = actionTitle;
            }

            // Example of the file name received here
            // "/0/TheTests0.cs";
            // remove the 0 to make the actual verification happen.
            if (test.TestState.Sources.Any())
            {
                for (int i = 0; i < test.TestState.Sources.Count; i++)
                {
                    var srce = test.TestState.Sources[i];
                    srce.filename = srce.filename.Replace("Tests0", "Tests");
                    test.TestState.Sources[i] = srce;
                }
            }

            if (test.FixedState.Sources.Any())
            {
                for (int i = 0; i < test.FixedState.Sources.Count; i++)
                {
                    var srce = test.FixedState.Sources[i];
                    srce.filename = srce.filename.Replace("Tests0", "Tests");
                    test.FixedState.Sources[i] = srce;
                }
            }

            await test.RunAsync();
        }
    }
}
