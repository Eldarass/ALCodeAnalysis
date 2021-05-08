using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Text;
using Microsoft.Dynamics.Nav.CodeAnalysis.Translation;
using System;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace ALCodeAnalysis.Naming
{
    [DiagnosticAnalyzer]
    public class VariablesNameValidation : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule001VariablesNameInPascalCase, DiagnosticDescriptors.Rule002VariableMayNotContainWhiteSpace, DiagnosticDescriptors.Rule003VariableMayNotContainWildcardSymbols);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(VariablesNameValidation.AnalyzeVariableSyntax, SyntaxKind.VariableDeclaration);
        }

        public static void AnalyzeVariableSyntax(SyntaxNodeAnalysisContext context)
        {
            if (context.Node != null)
            {
                VariableDeclarationSyntax syntax = GetVariableDeclarationSyntax(context);
                AnalyzeVariable(context, syntax);
            }
        }
        public static VariableDeclarationSyntax GetVariableDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            VariableDeclarationSyntax syntax = context.Node as VariableDeclarationSyntax;
            return syntax;
        }

        public static void AnalyzeVariable(SyntaxNodeAnalysisContext context, VariableDeclarationSyntax syntax)
        {
            if (syntax == null)
                return;
            IdentifierNameSyntax syntaxName = syntax.Name;
            SyntaxKind syntaxKind = syntax.Kind;
            string name = syntax.GetNameStringValue();
            if (!IsVariableNamePascalCase(name))
                ReportVariableNameMustBeDeclaratedInPascalCase(context, syntaxName.GetLocation(), name, syntaxKind, syntaxName);
            if (IsVaraibleContainWhiteSpace(name))
                ReportVariableNameMayNotContainWhiteSpace(context, syntaxName.GetLocation(), name, syntaxKind, syntaxName);
            if (IsVaraibleContainWildcardSymbols(name))
                ReportVariableNameMayNotContainWildcardSymbols(context, syntaxName.GetLocation(), name, syntaxKind, syntaxName);
        }

        public static bool IsVariableNamePascalCase(string name)
        {
            Match match = Regex.Match(name, @"^[A-Z][a-z0-9]+(?:[A-Z][a-z0-9]+)*$");
            return match.Success;
        }

        public static bool IsVaraibleContainWhiteSpace(string name)
        {
            Match match = Regex.Match(name, @"\s+");
            return match.Success;
        }

        public static bool IsVaraibleContainWildcardSymbols(string name)
        {
            Match match = Regex.Match(name, @"[\%\&]+");
            return match.Success;
        }

        private static void ReportVariableNameMustBeDeclaratedInPascalCase(
            SyntaxNodeAnalysisContext syntaxNodeAnalysisContext,
            Location location,
            string valueText,
            SyntaxKind syntaxKind,
            IdentifierNameSyntax syntaxName)
        {
            if (syntaxNodeAnalysisContext.Node != null && location != null && syntaxName != null)
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule001VariablesNameInPascalCase, location, (object)valueText, (object)syntaxKind, (object)syntaxName));
        }

        private static void ReportVariableNameMayNotContainWhiteSpace(
            SyntaxNodeAnalysisContext syntaxNodeAnalysisContext,
            Location location,
            string valueText,
            SyntaxKind syntaxKind,
            IdentifierNameSyntax syntaxName)
        {
            syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule002VariableMayNotContainWhiteSpace, location, (object)valueText, (object)syntaxKind, (object)syntaxName));
        }

        private static void ReportVariableNameMayNotContainWildcardSymbols(
            SyntaxNodeAnalysisContext syntaxNodeAnalysisContext,
            Location location,
            string valueText,
            SyntaxKind syntaxKind,
            IdentifierNameSyntax syntaxName)
        {
            syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule003VariableMayNotContainWildcardSymbols, location, (object)valueText, (object)syntaxKind, (object)syntaxName));
        }
    }
}
