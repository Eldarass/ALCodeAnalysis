using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Collections.Generic;

namespace ALCodeAnalysis.Utilities
{
    internal static class DictionaryHelper
    {
        internal static void AddIdentifierIfNotExist(
          this Dictionary<string, IdentifierNameSyntax> dictionary,
          IdentifierNameSyntax identifier)
        {
            string valueText = identifier.Identifier.ValueText;
            if (dictionary.ContainsKey(valueText))
                return;
            dictionary.Add(valueText, identifier);
        }

        internal static void AddLastRecordModifyInvocationSyntax(
          this Dictionary<string, IdentifierNameSyntax> dictionary,
          IdentifierNameSyntax identifier)
        {
            string valueText = identifier.Identifier.ValueText;
            // ISSUE: variable of a compiler-generated type
            IdentifierNameSyntax identifierNameSyntax;
            if (dictionary.TryGetValue(valueText, out identifierNameSyntax))
            {
                if (identifierNameSyntax.SpanStart >= identifier.SpanStart)
                    return;
                dictionary[valueText] = identifier;
            }
            else
                dictionary.Add(valueText, identifier);
        }
    }
}
