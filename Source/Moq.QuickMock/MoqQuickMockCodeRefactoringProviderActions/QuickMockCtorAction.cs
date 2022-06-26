using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq.QuickMock.Extensions;
using Moq.QuickMock.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Moq.QuickMock.MoqQuickMockCodeRefactoringProviderActions
{
    public partial class MoqActions
    {
        public static async Task<Document> QuickMockCtor(Document document, IEnumerable<IMethodSymbol> ctorMethodSymbols, ArgumentListSyntax argumentList, CancellationToken cancellationToken)
        {
            // phase one, mock the biggest ctor (can be improved later)
            var ctorWithMostParameters = ctorMethodSymbols.Select(x => x.Parameters).OrderByDescending(x => x.Length).First();

            var newVarList = new List<StatementSyntax>();
            var newArgsList = new List<string>();

            ctorWithMostParameters.FindReferenceAndValueTypes(
                onFoundReferenceType: paramSymbol =>
                {
                    var paramSymbolWithoutFullyQualifiedNames = paramSymbol.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                    var moqString = $"Mock.Of<{paramSymbolWithoutFullyQualifiedNames}>()";
                    newArgsList.Add(moqString);
                },
                onFoundValueType: (paramSymbol, suggestedArgumentText) =>
                {
                    newArgsList.Add(suggestedArgumentText);
                },
                onTypeNotIdentified: (paramSymbol, suggestedArgumentText) =>
                {
                    // dont know what to do here, user should look closer and fix.
                    newArgsList.Add(suggestedArgumentText);
                });

            if (newArgsList.Any())
            {
                var editor = await UpdateEditorHelpers.ReplaceArguments(document, argumentList, newArgsList);
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
