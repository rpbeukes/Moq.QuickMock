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
        }
    }
}
