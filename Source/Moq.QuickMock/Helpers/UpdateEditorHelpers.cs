using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moq.QuickMock.Helpers
{
    public class UpdateEditorHelpers
    {
        public static async Task<DocumentEditor> ReplaceArguments(Document document, ArgumentListSyntax location, List<string> newArgsList)
        {
            var editor = await DocumentEditor.CreateAsync(document);

            var modifiedArguments = new SeparatedSyntaxList<ArgumentSyntax>();

            foreach (var arg in newArgsList)
            {
                modifiedArguments = modifiedArguments.Add(SyntaxFactory.Argument(SyntaxFactory.ParseExpression(arg)));
            }

            var modifiedArgumentList = SyntaxFactory.ArgumentList(modifiedArguments);
            
            editor.ReplaceNode(location, modifiedArgumentList);
            return editor;
        }
    }
}
