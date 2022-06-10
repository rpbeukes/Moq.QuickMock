using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Moq.QuickMock.MoqQuickMockCodeRefactoringProvider
{
    public partial class MoqActions
    {
        public static async Task<Document> MockMoq(Document document, IEnumerable<IMethodSymbol> ctorMethodSymbols, ArgumentListSyntax argumentList, CancellationToken cancellationToken)
        {
            var ctorWithMostParameters = ctorMethodSymbols.Select(x => x.Parameters).OrderByDescending(x => x.Length).First();

            var newVarList = new List<StatementSyntax>();
            var newArgsList = new List<string>();
            foreach (var paramSymbol in ctorWithMostParameters)
            {
                var paramType = paramSymbol.Type;
                var isString = paramType.Name.ToLowerInvariant() == "string";
                if (!isString && paramType.IsReferenceType)
                {
                    var newVarName = $"{paramSymbol.Name}Mock";
                    var declaringStatement = $"var {newVarName} = new Mock<{paramSymbol.Type}>();{Environment.NewLine}";
                    var newVar = SyntaxFactory.ParseStatement(declaringStatement)
                                     // this seems to be the magic to remove Full Qualified Names
                                     .WithAdditionalAnnotations(Formatter.Annotation, Simplifier.Annotation);
                    newVarList.Add(newVar);
                    newArgsList.Add($"{newVarName}.Object");
                }
                else if (isString || paramType.IsValueType)
                {
                    newArgsList.Add($"It.IsAny<{paramSymbol}>()");
                }
                else
                {
                    newArgsList.Add($" "); // dont know what to do here, user should look closer and fix.
                }
            }

            var editor = await DocumentEditor.CreateAsync(document);

            var modifiedArguments = new SeparatedSyntaxList<ArgumentSyntax>().AddRange
            (
                new ArgumentSyntax[]
                {
                        SyntaxFactory.Argument(SyntaxFactory.ParseExpression($"{string.Join($", ", newArgsList)}"))
                }
            );

            var modifiedArgumentList = SyntaxFactory.ArgumentList(modifiedArguments);
            editor.ReplaceNode(argumentList, modifiedArgumentList);

            var location = argumentList.Ancestors().OfType<LocalDeclarationStatementSyntax>().First();
            editor.InsertBefore(location, newVarList);

            var changedDoc = editor.GetChangedDocument();
            return changedDoc;
        }
    }
}
