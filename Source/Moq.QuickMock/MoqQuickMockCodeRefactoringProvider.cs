using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeRefactoring1
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(MoqQuickMockCodeRefactoringProvider)), Shared]
    internal class MoqQuickMockCodeRefactoringProvider : CodeRefactoringProvider
    {
        public sealed override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
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
                        var syntaxRoot = await document.GetSyntaxRootAsync(context.CancellationToken);
                        var semanticModel = await document.GetSemanticModelAsync(context.CancellationToken);
                        var editor = await DocumentEditor.CreateAsync(document);
                        var objectCreationExpressionSyntax = argumentList.Parent as ObjectCreationExpressionSyntax;
                        var ctorSymbol = semanticModel.GetSymbolInfo(objectCreationExpressionSyntax);

                        if (ctorSymbol.CandidateSymbols.Any())
                        {
                            var ctorMethodSymbols = ctorSymbol.CandidateSymbols.OfType<IMethodSymbol>()
                                                              .Where(x => x.Parameters.Length > 0);
                            if (ctorMethodSymbols.Any())
                            {
                                var action = CodeAction.Create("Quick mock ctor (Moq)", c => QuickMockCtor(context.Document, ctorMethodSymbols, argumentList, c));

                                // Register this code action.
                                context.RegisterRefactoring(action);
                            }
                        }
                    }
                }
            }
            return;
        }

        private async Task<Document> QuickMockCtor(Document document, IEnumerable<IMethodSymbol> ctorMethodSymbols, ArgumentListSyntax argumentList, CancellationToken cancellationToken)
        {
            // phase one, mock the biggest ctor (can be improved later)
            // also, one should validate if the parameters are all reference types because
            //       Moq does not allow eg: `new Hero(Mock.Of<string>(), Mock.Of<bool>());`
            var ctorWithMostParameters = ctorMethodSymbols.Select(x => x.Parameters).OrderByDescending(x => x.Length).First();

            var changedList = new List<string>();
            foreach (var paramSymbol in ctorWithMostParameters)
            {
                var moqString = $"Mock.Of<{paramSymbol}>()";
                changedList.Add(moqString);
            }

            if (changedList.Any())
            {
                var editor = await DocumentEditor.CreateAsync(document);

                var modifiedArguments = new SeparatedSyntaxList<ArgumentSyntax>().AddRange
                (
                    new ArgumentSyntax[]
                    {
                        SyntaxFactory.Argument(SyntaxFactory.ParseExpression($"{string.Join($", ", changedList)}")),
                    }
                );

                var modifiedArgumentList = SyntaxFactory.ArgumentList(modifiedArguments);
                editor.ReplaceNode(argumentList, modifiedArgumentList);
                var changedDoc = editor.GetChangedDocument();
                return changedDoc;
            }

            // no change detected
            return document;

            //var r = typeDecl;
            //var fileText = typeDecl.Parent.SyntaxTree.GetText().ToString();
            //var tree = CSharpSyntaxTree.ParseText(fileText);
            //CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            //var identi = ((IdentifierNameSyntax)((ObjectCreationExpressionSyntax)((SyntaxNode)typeDecl).Parent).Type).Identifier;
            //var item = document.Project.Solution.GetDocumentIdsWithFilePath();
            // Get the symbol representing the type to be renamed.

            //var objectCreationExpressionSyntax = argumentList.Parent as ObjectCreationExpressionSyntax;
            //var classToFind = (objectCreationExpressionSyntax.Type as IdentifierNameSyntax).Identifier.ValueText;

            //var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken);
            //var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            //var decendantsIdentifierNameSyntax = syntaxRoot.DescendantNodes().OfType<IdentifierNameSyntax>();

            //var specificIdentifiers = decendantsIdentifierNameSyntax.Where(x => x.Identifier.Text == classToFind);

            //var classDefinition = specificIdentifiers.Where(x => semanticModel.GetTypeInfo(x).Type != null).FirstOrDefault();

            //if (classDefinition is null)
            //    return document; // class not found in the solution, no change.

            //var classSyntaxToFindCtorParamters = semanticModel.GetTypeInfo(classDefinition)
            //                                                   .Type
            //                                                   ?.DeclaringSyntaxReferences
            //                                                   .FirstOrDefault()
            //                                                   .GetSyntax();

            //// phase one, mock the biggest ctor (can be improve later)
            //var ctorWithMostParameters = classSyntaxToFindCtorParamters
            //                                                .DescendantNodes()
            //                                                .OfType<ConstructorDeclarationSyntax>()
            //                                                // find the biggest constructor
            //                                                .OrderByDescending(x => x.ParameterList.Parameters.Count)
            //                                                .FirstOrDefault();

            //var ctorParameters = ctorWithMostParameters?.ParameterList.Parameters;
            //if (ctorParameters is null)
            //    return document;

            //var changedList = new List<string>();
            //foreach (var ctorParameter in ctorParameters)
            //{
            //    var paramLine = ctorParameter.GetText().Container.CurrentText.ToString();
            //    var paramType = paramLine.Split(" ".ToArray()).FirstOrDefault();
            //    var moqString = $"Mock.Of<{paramType}>()";
            //    changedList.Add(moqString);
            //}

            //if (changedList.Any())
            //{
            //    var editor = await DocumentEditor.CreateAsync(document);

            //    var modifiedArguments = new SeparatedSyntaxList<ArgumentSyntax>().AddRange
            //    (
            //        new ArgumentSyntax[]
            //        {
            //            SyntaxFactory.Argument(SyntaxFactory.ParseExpression($"{string.Join($", ", changedList)}")),
            //        }
            //    );

            //    var modifiedArgumentList = SyntaxFactory.ArgumentList(modifiedArguments);
            //    editor.ReplaceNode(argumentList, modifiedArgumentList);
            //    var changedDoc = editor.GetChangedDocument();
            //    return changedDoc;
            //}

            //// no change detected
            //return document;

            //var rewriter = new AttributeStatementChanger();

            //foreach (var id in identifiers)
            //{
            //    var eeee = semanticModel.GetTypeInfo(id);
            //    var correctCode = eeee.Type.DeclaringSyntaxReferences.ElementAt(0);
            //    var decendantNodesOfCorrectCode = correctCode.GetSyntax().DescendantNodes().OfType<ConstructorDeclarationSyntax>()
            //                                                                               .OrderByDescending(x => x.ParameterList.Parameters.Count)
            //                                                                               .FirstOrDefault();
            //    var ctorParameters = decendantNodesOfCorrectCode.ParameterList.Parameters;
            //    var changedList = new List<string>();
            //    foreach (var ctorParameter in ctorParameters)
            //    {
            //        var paramLine = ctorParameter.GetText().Container.CurrentText.ToString();
            //        var paramType = paramLine.Split(" ".ToArray()).FirstOrDefault();
            //        var moqString = $"Mock.Of<{paramType}>()";
            //        changedList.Add(moqString);
            //    }

            //    //var actualType = eeee.Type.GetType();
            //}

            //var originalSolution = document.Project.Solution;
            //var optionSet = originalSolution.Workspace.Options;

            //var newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, typeSymbol, newName, optionSet, cancellationToken).ConfigureAwait(false);


            //var classes = syntaxRoot.DescendantNodes().OfType<ClassDeclarationSyntax>();
            //foreach (var classDeclarationSyntax in classes)
            //{
            //    var symbol = semanticModel.GetDeclaredSymbol(classDeclarationSyntax);
            //    var objectCreationExpressionSyntaxs = classDeclarationSyntax.DescendantNodes().OfType<ObjectCreationExpressionSyntax>();
            //}
            //var typeSymbol = semanticModel.GetDeclaredSymbol(typeDecl, cancellationToken);

            // return originalSolution;
        }
    }
}
