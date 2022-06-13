using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq.QuickMock.MoqQuickMockCodeRefactoringProviderActions;
using System;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace Moq.QuickMock
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(MoqQuickMockCodeRefactoringProvider)), Shared]
    public class MoqQuickMockCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // Find the node at the selection.
            var node = root.FindNode(context.Span);

            // this refactor is only available in tests files eg: "*Tests.cs"
            if (node.SyntaxTree.FilePath.ToLower().Contains("tests.cs"))
            {
                if (node is ArgumentListSyntax argumentList)
                {
                    var isCreatingNewObject = argumentList.Parent.IsKind(SyntaxKind.ObjectCreationExpression);
                    if (isCreatingNewObject)
                    {
                        var document = context.Document;
                        var semanticModel = await document.GetSemanticModelAsync(context.CancellationToken);
                        var objectCreationExpressionSyntax = argumentList.Parent as ObjectCreationExpressionSyntax;
                        var ctorSymbol = semanticModel.GetSymbolInfo(objectCreationExpressionSyntax);
                        
                        if (ctorSymbol.Symbol.ContainingType.Constructors.Any())
                        {
                            var ctorMethodSymbols = ctorSymbol.Symbol.ContainingType.Constructors.OfType<IMethodSymbol>()
                                                                                 .Where(x => x.Parameters.Length > 0);
                            if (ctorMethodSymbols.Any())
                            {
                                var quickMockCtorAction = CodeAction.Create("Quick mock ctor (Moq)", c => MoqActions.QuickMockCtor(context.Document, ctorMethodSymbols, argumentList, c));
                                var mockCtorAction = CodeAction.Create("Mock ctor (Moq)", c => MoqActions.MockCtor(context.Document, ctorMethodSymbols, argumentList, c));

                                // Register this code action.
                                context.RegisterRefactoring(quickMockCtorAction);
                                context.RegisterRefactoring(mockCtorAction);
                            }
                        }
                    }
                }
            }
            return;
        }
    }
}
