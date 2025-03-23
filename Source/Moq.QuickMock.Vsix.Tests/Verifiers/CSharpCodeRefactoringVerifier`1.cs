using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
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

        // This is custom code not part of the original templated created by Microsoft
        public static async Task VerifyRefactoringAsync(string source,
                                                        string fixedSource,
                                                        DiagnosticResult[] expected = null,
                                                        string actionTitle = null)
        {
            var test = new Test
            {
                TestCode = source,
                FixedCode = fixedSource,

                CompilerDiagnostics = CompilerDiagnostics.None,
            };

            if (expected != null && expected.Any())
            {
                test.ExpectedDiagnostics.AddRange(expected);
            }

            if (actionTitle != null)
            {
                test.CodeActionEquivalenceKey = actionTitle;
            }

            // HACK: the refactoring only works on files that follow naming convention:
            //              `~/SomeFeatureTests.cs`
            // Example of the file name received here
            // "/0/TheTests0.cs";
            // remove the 0 to make the actual verification happen.
            ChangeFileName(test.TestState);
            ChangeFileName(test.FixedState);

            await test.RunAsync();
        }

        private static void ChangeFileName(SolutionState state)
        {
            if (state.Sources.Any())
            {
                for (int i = 0; i < state.Sources.Count; i++)
                {
                    var srce = state.Sources[i];
                    srce.filename = srce.filename.Replace("Tests0", "Tests");
                    state.Sources[i] = srce;
                }
            }
        }
    }
}
