using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace ALCodeAnalysis.Readability
{
    [DiagnosticAnalyzer]
    public class SystemWordsCaseValidation : DiagnosticAnalyzer
    {
        private static readonly ImmutableArray<SyntaxKind> KeywordsSyntaxKinds = SyntaxFacts.KeywordsSyntaxKinds;
        private static readonly ImmutableArray<SyntaxKind> RootObjectSyntaxKinds = SyntaxFacts.RootObjectKinds;


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule014SystemKeywordsInLowerCase);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(SystemWordsCaseValidation.AnalyzeSystemWordCase), RootObjectSyntaxKinds);
        }

        public static void AnalyzeSystemWordCase(SyntaxNodeAnalysisContext context)
        {
            if (context.Node != null)
            {
                IEnumerable<SyntaxToken> descendantTokens = context.Node.DescendantTokens();
                ValidateSystemWordCase(context, descendantTokens);
            }
        }

        public static void ValidateSystemWordCase(SyntaxNodeAnalysisContext context, IEnumerable<SyntaxToken> descendantTokens)
        {
            foreach (SyntaxToken descendantToken in descendantTokens)
            {
                if (KeywordsSyntaxKinds.Contains(descendantToken.Kind) && !descendantToken.Text.Any(char.IsLower))
                {
                    ReportSystemKeywordsInLowerCase(context, descendantToken.GetLocation(), descendantToken.Text);
                }
            }
        }

        private static void ReportSystemKeywordsInLowerCase(
           SyntaxNodeAnalysisContext syntaxNodeAnalysisContext,
           Location location,
           string valueText)
        {
            if (syntaxNodeAnalysisContext.Node != null)
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule014SystemKeywordsInLowerCase, location, (object)valueText));
        }

    }
}