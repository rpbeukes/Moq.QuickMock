using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Moq.QuickMock.Test
{
    public static partial class CSharpCodeRefactoringVerifier<TCodeRefactoring>
        where TCodeRefactoring : CodeRefactoringProvider, new()
    {
        public class Test : CSharpCodeRefactoringTest<TCodeRefactoring, DefaultVerifier>
        {
            public Test()
            {
                SolutionTransforms.Add((solution, projectId) =>
                {
                    var compilationOptions = solution.GetProject(projectId).CompilationOptions;
                    compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(
                        compilationOptions.SpecificDiagnosticOptions.SetItems(CSharpVerifierHelper.NullableWarnings));
                    solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);
                    
                    return solution;
                });

                //DefaultFilePathPrefix
                //DefaultFilePath
            }

            // custom code added for this solution
            protected override string DefaultFilePathPrefix => "/0/TheTests";
            protected override string DefaultFilePath => DefaultFilePathPrefix + "." + DefaultFileExt;

            protected override string DefaultFileExt => base.DefaultFileExt;
            protected override string DefaultTestProjectName => base.DefaultTestProjectName;
        }
    }
}
