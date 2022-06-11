﻿using Microsoft.CodeAnalysis;
using Moq.QuickMock.MoqQuickMockCodeRefactoringProvider;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Moq.QuickMock
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
                var isString = paramType.Name.ToLowerInvariant() == "string";

                if (!isString && paramType.IsReferenceType)
                    onFoundReferenceType(paramSymbol);
                else if (isString || paramType.IsValueType)
                {
                    var suggestedArgumentText = MoqSyntaxHelpers.CreateIsAnyMockString(paramSymbol);
                    onFoundValueType(paramSymbol, suggestedArgumentText);
                }
                else
                    // dont know what to do here, user should look closer and fix.
                    onTypeNotIdentified?.Invoke(paramSymbol, " ");
            }
        }
    }
}
