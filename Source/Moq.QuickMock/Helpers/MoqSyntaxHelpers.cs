using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Moq.QuickMock.Helpers
{
    public class MoqSyntaxHelpers
    {
        public static string CreateIsAnyMockString(IParameterSymbol paramSymbol)
        {
            return $"It.IsAny<{paramSymbol}>()";
        }
    }
}
