using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using System;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace Moq.QuickMock
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(MockToMockOfCodeRefactoringProvider)), Shared]
    internal class MockToMockOfCodeRefactoringProvider : CodeRefactoringProvider
    {
        public sealed override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // Find the node at the selection.
            var node = root.FindNode(context.Span);

            var isActionAvailable =
                  // this refactor is only available in tests files eg: "*Tests.cs"
                  node.SyntaxTree.FilePath.ToLower().Contains("tests.cs")
                   && node is IdentifierNameSyntax;

            if (!isActionAvailable) return;

            var document = context.Document;
            var semanticModel = await document.GetSemanticModelAsync(context.CancellationToken);
            var identifierName = (IdentifierNameSyntax)node;
            var typeInfo = semanticModel.GetTypeInfo(identifierName);
            var varType = typeInfo.ConvertedType as INamedTypeSymbol;

            if (varType is null) return;

            isActionAvailable = varType.OriginalDefinition.ToString() == "Moq.Mock<T>";

            if (isActionAvailable)
            {
                var action = CodeAction.Create("mock.Object to Mock.Of<T> (Moq)", async (c) =>
                {
                    var editor = await DocumentEditor.CreateAsync(document);

                    var changedDoc = editor.GetChangedDocument();
                    return changedDoc;
                });

                // Register this code action.
                context.RegisterRefactoring(action);
            }
        }
    }
}
