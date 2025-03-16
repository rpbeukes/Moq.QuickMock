using Microsoft.CodeAnalysis;
using Moq.QuickMock.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Moq.QuickMock.Extensions
{
    public static class ParameterSymbolExtensions
    {
        public static void FindReferenceAndValueTypes(this ImmutableArray<IParameterSymbol> ctorWithMostParameters,
                                                      Action<IParameterSymbol> onFoundReferenceType,
                                                      Action<IParameterSymbol, string> onFoundValueType,
                                                      Action<IParameterSymbol, string> onTypeNotIdentified = null)
        {
            foreach (var paramSymbol in ctorWithMostParameters)
            {
                var paramType = paramSymbol.Type;
                var isString = paramType.Name.Equals("string", StringComparison.OrdinalIgnoreCase);

                if (!isString && paramType.IsReferenceType)
                    onFoundReferenceType(paramSymbol);
                else if (isString || paramType.IsValueType)
                {
                    var suggestedArgumentText = MoqSyntaxHelpers.CreateIsAnyMockString(paramType.ToString());
                    onFoundValueType(paramSymbol, suggestedArgumentText);
                }
                else
                    // dont know what to do here, user should look closer and fix.
                    onTypeNotIdentified?.Invoke(paramSymbol, " ");
            }
        }
    }
}
