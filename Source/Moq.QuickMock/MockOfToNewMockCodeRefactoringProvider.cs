using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Moq.QuickMock
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(MockOfToNewMockCodeRefactoringProvider)), Shared]
    internal class MockOfToNewMockCodeRefactoringProvider : CodeRefactoringProvider
    {
        public sealed override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // Find the node at the selection.
            var node = root.FindNode(context.Span);

            // this refactor is only available in tests files eg: "*Tests.cs"
            if (node.SyntaxTree.FilePath.ToLower().Contains("tests.cs"))
            {
                var action = CodeAction.Create("Mock.Of<T> to new Mock<T> (Moq)", c => MockOfToNewMock(context.Document, null, null, c));
                
                // Register this code action.
                context.RegisterRefactoring(action);
            }
            return;
        }

        public static async Task<Document> MockOfToNewMock(Document document, IEnumerable<IMethodSymbol> ctorMethodSymbols, ArgumentListSyntax argumentList, CancellationToken cancellationToken)
        {
            return document;
        }
}
}
