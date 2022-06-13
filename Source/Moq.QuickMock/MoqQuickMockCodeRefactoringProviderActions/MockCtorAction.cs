using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
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
        public static async Task<Document> MockCtor(Document document, IEnumerable<IMethodSymbol> ctorMethodSymbols, ArgumentListSyntax argumentList, CancellationToken cancellationToken)
        {
            var ctorWithMostParameters = ctorMethodSymbols.Select(x => x.Parameters).OrderByDescending(x => x.Length).First();

            var newVarList = new List<StatementSyntax>();
            var newArgsList = new List<string>();

            ctorWithMostParameters.FindReferenceAndValueTypes(
                onFoundReferenceType: paramSymbol =>
                {
                    var newVarName = $"{paramSymbol.Name}Mock";
                    var declaringStatement = $"var {newVarName} = new Mock<{paramSymbol.Type}>();{Environment.NewLine}";
                    var newVar = SyntaxFactory.ParseStatement(declaringStatement)
                                     // this seems to be the magic to remove Full Qualified Names
                                     .WithAdditionalAnnotations(Formatter.Annotation, Simplifier.Annotation);
                    newVarList.Add(newVar);
                    newArgsList.Add($"{newVarName}.Object");
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

            var editor = await UpdateEditorHelpers.ReplaceArguments(document, argumentList, newArgsList);

            var location = argumentList.Ancestors().OfType<LocalDeclarationStatementSyntax>().First();
            editor.InsertBefore(location, newVarList);

            var changedDoc = editor.GetChangedDocument();
            return changedDoc;
        }
    }
}
