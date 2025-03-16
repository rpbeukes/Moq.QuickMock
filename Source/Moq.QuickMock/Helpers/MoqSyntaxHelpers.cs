using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Moq.QuickMock.Helpers
{
    public class MoqSyntaxHelpers
    {
        public static string CreateIsAnyMockString(string typeValue)
        {
            return $"It.IsAny<{typeValue}>()";
        }
    }
}
