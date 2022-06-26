using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
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

            isActionAvailable = varType.OriginalDefinition.ToString().Equals("Moq.Mock<T>", StringComparison.OrdinalIgnoreCase);

            if (!isActionAvailable) return;

            if (isActionAvailable)
            {
                var action = CodeAction.Create("mock.Object to Mock.Of<T> (Moq)", async (c) =>
                {
                    var localVarsInMethod = identifierName.Ancestors()
                                                          .OfType<MethodDeclarationSyntax>()
                                                          .FirstOrDefault()
                                                          ?.DescendantNodes()
                                                          .OfType<LocalDeclarationStatementSyntax>();

                    var editor = await DocumentEditor.CreateAsync(document);

                    var objectCreationExpression = node.Ancestors().OfType<ObjectCreationExpressionSyntax>().First();
                    var argumentList = node.Ancestors().OfType<ArgumentListSyntax>().FirstOrDefault();
                    var typeInfoOfClass = semanticModel.GetTypeInfo(objectCreationExpression);
                    var classDefinition = typeInfoOfClass.ConvertedType as INamedTypeSymbol;
                    var theCorrectConstructor = classDefinition.Constructors.First(x => x.Parameters.Length == argumentList.ChildNodes().Count());
                    var currentArg = node.Ancestors().OfType<ArgumentSyntax>().FirstOrDefault();
                    var argPosition = argumentList.Arguments.IndexOf(a => a.Span == currentArg.Span);
                    var paramSymbol = theCorrectConstructor.Parameters[argPosition];
                    var paramSymbolWithoutFullyQualifiedNames = paramSymbol.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                    var newExpression = SyntaxFactory.ParseExpression($"Mock.Of<{paramSymbolWithoutFullyQualifiedNames}>()")
                                            .WithAdditionalAnnotations(Formatter.Annotation, Simplifier.Annotation)
                                            .WithLeadingTrivia(currentArg.GetLeadingTrivia());

                    var newArg = SyntaxFactory.Argument(newExpression);

                    editor.ReplaceNode(currentArg, newArg);

                    var localVarMatchingArgumentName = localVarsInMethod?.FirstOrDefault(x => x.Declaration.Variables.FirstOrDefault()?.Identifier.ToString().Equals(identifierName.Identifier.ToString(), StringComparison.OrdinalIgnoreCase)
                                                                                        ?? false);
                    if (localVarMatchingArgumentName != null)
                    {
                        var thisIsANewMockInstance = localVarMatchingArgumentName
                                                         .DescendantNodes()
                                                         .OfType<ObjectCreationExpressionSyntax>()
                                                         .FirstOrDefault(x => x.Parent.ToString().Contains($"= new Mock<{paramSymbolWithoutFullyQualifiedNames}>"))
                                                         ;

                        // will only remove local declared variables
                        // if a member variable, don't touch
                        if (thisIsANewMockInstance != null)
                        {
                            // found the variable declaration, remove it.
                            editor.RemoveNode(localVarMatchingArgumentName);
                        }
                    }

                    var changedDoc = editor.GetChangedDocument();
                    return changedDoc;
                });

                // Register this code action.
                context.RegisterRefactoring(action);
            }
        }
    }
}
