using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq.QuickMock.MoqQuickMockCodeRefactoringProviderActions;
using System;
using System.Composition;
using System.Diagnostics;
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
                Debug.WriteLine($"node: {node}");
                Debug.WriteLine($"node: {node.GetType()}");

                if (node is ArgumentListSyntax argumentList)
                {
                    var isCreatingNewObject = argumentList.Parent.IsKind(SyntaxKind.ObjectCreationExpression);
                    if (isCreatingNewObject)
                    {
                        var document = context.Document;
                        var semanticModel = await document.GetSemanticModelAsync(context.CancellationToken);
                        var objectCreationExpressionSyntax = argumentList.Parent as ObjectCreationExpressionSyntax;
                        
                        if (objectCreationExpressionSyntax is null) return;
                        
                        var typeInfo = semanticModel.GetTypeInfo(objectCreationExpressionSyntax);
                        var classDefinition = typeInfo.ConvertedType as INamedTypeSymbol;
                        
                        if (classDefinition is null) return;
                        
                        if (classDefinition.TypeKind == TypeKind.Class && classDefinition.Constructors.Any())
                        {
                            var ctorMethodSymbols = classDefinition.Constructors.Where(x => x.Parameters.Length > 0);
                            if (ctorMethodSymbols.Any())
                            {
                                var title = "Quick mock ctor (Moq)";
                                var quickMockCtorAction = CodeAction.Create(title,
                                                                            c => MoqActions.QuickMockCtor(context.Document, ctorMethodSymbols, argumentList, c),
                                                                            equivalenceKey: title);
                                
                                title = "Mock ctor (Moq)";
                                var mockCtorAction = CodeAction.Create(title,
                                                                       c => MoqActions.MockCtor(context.Document, ctorMethodSymbols, argumentList, c),
                                                                       equivalenceKey: title);

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
