using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Moq.QuickMock.MoqQuickMockCodeRefactoringProvider
{
    public class MoqSyntaxHelpers
    {
        public static string CreateIsAnyMockString(IParameterSymbol paramSymbol)
        {
            return $"It.IsAny<{paramSymbol}>()";
        }
    }
}
