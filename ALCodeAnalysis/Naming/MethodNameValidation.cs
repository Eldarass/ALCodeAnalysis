using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Text;
using System;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace ALCodeAnalysis.Naming
{
    [DiagnosticAnalyzer]
    public class MethodNameValidation : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule009MethodsNameInPascalCase, DiagnosticDescriptors.Rule010MethodsNameMayNotContainWhiteSpace);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(MethodNameValidation.AnalyzeMethodNameSyntax), SyntaxKind.MethodDeclaration);
        }

        public static void AnalyzeMethodNameSyntax(SyntaxNodeAnalysisContext context)
        {
            if (context.Node != null)
            {
                MethodDeclarationSyntax syntax = GetMethodDeclarationSyntax(context);
                AnalyzeMethodName(context, syntax);
            }
        }

        public static MethodDeclarationSyntax GetMethodDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            MethodDeclarationSyntax syntax = context.Node as MethodDeclarationSyntax;
            return syntax;
        }

        public static void AnalyzeMethodName(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax syntax)
        {
            if (syntax != null)
            {
                IdentifierNameSyntax syntaxName = syntax.Name;
                SyntaxKind syntaxKind = syntax.Kind;
                string name = syntax.GetNameStringValue();
                if (!IsMethodNamePascalCase(name))
                    ReportMethodNameMustBeDeclaratedInPascalCase(context, syntaxName.GetLocation(), name, syntaxKind, syntaxName);
                if (IsMethodContainWhiteSpace(name))
                    ReportMethodNameMayNotContainWhiteSpace(context, syntaxName.GetLocation(), name, syntaxKind, syntaxName);
            }
        }

        public static bool IsMethodNamePascalCase(string name)
        {
            Match match = Regex.Match(name, @"^[A-Z][a-z0-9]+(?:[A-Z][a-z0-9]+)*$");
            return match.Success;
        }

        public static bool IsMethodContainWhiteSpace(string name)
        {
            Match match = Regex.Match(name, @"\s+");
            return match.Success;
        }

        private static void ReportMethodNameMustBeDeclaratedInPascalCase(
            SyntaxNodeAnalysisContext syntaxNodeAnalysisContext,
            Location location,
            string valueText,
            SyntaxKind syntaxKind,
            IdentifierNameSyntax syntaxName)
        {
            syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule009MethodsNameInPascalCase, location, (object)valueText, (object)syntaxKind, (object)syntaxName));
        }

        private static void ReportMethodNameMayNotContainWhiteSpace(
            SyntaxNodeAnalysisContext syntaxNodeAnalysisContext,
            Location location,
            string valueText,
            SyntaxKind syntaxKind,
            IdentifierNameSyntax syntaxName)
        {
            syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule010MethodsNameMayNotContainWhiteSpace, location, (object)valueText, (object)syntaxKind, (object)syntaxName));
        }
    }
}
