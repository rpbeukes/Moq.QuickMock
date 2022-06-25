using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Simplification;
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

            var isActionAvailable =
                  // this refactor is only available in tests files eg: "*Tests.cs"
                  node.SyntaxTree.FilePath.ToLower().Contains("tests.cs")
                    && node is IdentifierNameSyntax identifierName
                    && identifierName?.Identifier.ValueText == "Mock"
                    && identifierName.Ancestors().OfType<ArgumentListSyntax>().FirstOrDefault() is ArgumentListSyntax argtList
                    && argtList.Parent is ObjectCreationExpressionSyntax
            ;

            if (isActionAvailable)
            {
                var objectCreationExpression = node.Ancestors().OfType<ObjectCreationExpressionSyntax>().First();
                var argumentList = node.Ancestors().OfType<ArgumentListSyntax>().FirstOrDefault();

                var document = context.Document;
                var semanticModel = await document.GetSemanticModelAsync(context.CancellationToken);
                var typeInfo = semanticModel.GetTypeInfo(objectCreationExpression);
                var classDefinition = typeInfo.ConvertedType as INamedTypeSymbol;

                if (classDefinition is null) return;

                if (classDefinition.TypeKind == TypeKind.Class
                    // does the number of args match the constructors args
                    && classDefinition.Constructors.Any(x => x.Parameters.Length == argumentList.ChildNodes().Count())
                )
                {
                    var action = CodeAction.Create("Mock.Of<T> to new Mock<T> (Moq)", async (c) =>
                    {
                        var theCorrectConstructor = classDefinition.Constructors.First(x => x.Parameters.Length == argumentList.ChildNodes().Count());
                        var currentArg = node.Ancestors().OfType<ArgumentSyntax>().FirstOrDefault();
                        var argPosition = argumentList.Arguments.IndexOf(a => a.Span == currentArg.Span);

                        var paramSymbol = theCorrectConstructor.Parameters[argPosition];


                        var newVarName = $"{paramSymbol.Name}Mock";
                        var declaringStatement = $"var {newVarName} = new Mock<{paramSymbol.Type}>();{Environment.NewLine}";
                        var newVar = SyntaxFactory.ParseStatement(declaringStatement)
                                         // this seems to be the magic to remove Full Qualified Names
                                         .WithAdditionalAnnotations(Formatter.Annotation, Microsoft.CodeAnalysis.Simplification.Simplifier.Annotation);

                        var newVarList = new List<StatementSyntax>() { newVar };

                        var editor = await DocumentEditor.CreateAsync(document);

                        var location = node.Ancestors().OfType<LocalDeclarationStatementSyntax>().First();
                        editor.InsertBefore(location, newVarList);
                        
                        var newExpression = SyntaxFactory.ParseExpression($"{newVarName}.Object")
                                             // this seems to be the magic to remove Full Qualified Names
                                             .WithAdditionalAnnotations(Formatter.Annotation, Simplifier.Annotation)
                                             .WithLeadingTrivia(currentArg.GetLeadingTrivia());

                        var newArg = SyntaxFactory.Argument(newExpression);

                        editor.ReplaceNode(currentArg, newArg);

                        var changedDoc = editor.GetChangedDocument();
                        return changedDoc;
                    });

                    // Register this code action.
                    context.RegisterRefactoring(action);
                }
            }
            return;
        }
    }
}
