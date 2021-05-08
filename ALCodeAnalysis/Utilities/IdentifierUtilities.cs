using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

namespace ALCodeAnalysis.Utilities
{
    internal static class IdentifierUtilities
    {
        private static SyntaxNode TryGetParentMethod(SyntaxNode syntaxNode)
        {
            if (syntaxNode?.Parent == null)
                return (SyntaxNode)null;
            switch (syntaxNode.Parent.Kind)
            {
                case SyntaxKind.TriggerDeclaration:
                case SyntaxKind.MethodDeclaration:
                    return syntaxNode.Parent;
                default:
                    return IdentifierUtilities.TryGetParentMethod(syntaxNode.Parent);
            }
        }

        internal static bool IdentifierIsLocalVariable(IdentifierNameSyntax identifier)
        {
            SyntaxNode parentMethod = IdentifierUtilities.TryGetParentMethod(identifier.Parent);
            SyntaxKind? kind = parentMethod?.Kind;
            if (kind.HasValue)
            {
                switch (kind.GetValueOrDefault())
                {
                    case SyntaxKind.TriggerDeclaration:
                        return IdentifierUtilities.VerifyIdentifierIsInMethodTriggerVarParamList<TriggerDeclarationSyntax>((TriggerDeclarationSyntax)parentMethod, identifier);
                    case SyntaxKind.MethodDeclaration:
                        return IdentifierUtilities.VerifyIdentifierIsInMethodTriggerVarParamList<MethodDeclarationSyntax>((MethodDeclarationSyntax)parentMethod, identifier);
                }
            }
            return false;
        }

        internal static bool VerifyIdentifierIsInMethodTriggerVarParamList<T>(
          T methodOrTriggerDeclarationSyntax,
          IdentifierNameSyntax identifier)
          where T : MethodOrTriggerDeclarationSyntax
        {
            if (methodOrTriggerDeclarationSyntax.ParameterList != null)
            {
                SeparatedSyntaxList<ParameterSyntax> parameters = methodOrTriggerDeclarationSyntax.ParameterList.Parameters;
                if (parameters.Count != 0)
                {
                    parameters = methodOrTriggerDeclarationSyntax.ParameterList.Parameters;
                    foreach (ParameterSyntax parameterSyntax in parameters)
                    {
                        if (SemanticFacts.IsSameName(parameterSyntax.Name.Identifier.ValueText, identifier.Identifier.ValueText))
                            return true;
                    }
                }
            }
            if (methodOrTriggerDeclarationSyntax.Variables != null)
            {
                SyntaxList<VariableDeclarationBaseSyntax> variables = methodOrTriggerDeclarationSyntax.Variables.Variables;
                if (variables.Count != 0)
                {
                    variables = methodOrTriggerDeclarationSyntax.Variables.Variables;
                    foreach (SyntaxNode syntaxNode in variables)
                    {
                        if (SemanticFacts.IsSameName(syntaxNode.GetNameStringValue(), identifier.Identifier.ValueText))
                            return true;
                    }
                }
            }
            return false;
        }
    }
}
