using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Text;
using System;
using System.Collections.Immutable;

namespace ALCodeAnalysis.Readability
{
    [DiagnosticAnalyzer]
    public class GlobalVariablesPlacement : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule004GlobalVariablesMayBeAboveTriggersProcedures);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(AnalyzeGlobalVariables), SyntaxKind.GlobalVarSection);
        }

        public static void AnalyzeGlobalVariables(SyntaxNodeAnalysisContext context)
        {
            GlobalVarSectionSyntax syntax = context.Node as GlobalVarSectionSyntax;
            AnalyzeGlobalVariablesPlacement(context, syntax);

        }

        public static void AnalyzeGlobalVariablesPlacement(SyntaxNodeAnalysisContext context, GlobalVarSectionSyntax syntax)
        {
            if (syntax != null)
            {
                int globalVarSectionPosition = 0;
                int triggersSectionPosition = 0;
                int MethodSectionPosition = 0;
                SyntaxNode parent = syntax.Parent;
                SyntaxKind kind = parent.Kind;
                dynamic parentSyntax = parent;
                if (parentSyntax != null)
                    foreach (MemberSyntax member in parentSyntax.Members)
                    {
                        if (member.Kind == SyntaxKind.GlobalVarSection)
                        {
                            globalVarSectionPosition = member.Position;
                        }
                        if (member.Kind == SyntaxKind.TriggerDeclaration && triggersSectionPosition == 0)
                        {
                            triggersSectionPosition = member.Position;
                        }
                        if (member.Kind == SyntaxKind.MethodDeclaration && MethodSectionPosition == 0)
                        {
                            MethodSectionPosition = member.Position;
                        }
                    }
                if ((globalVarSectionPosition > triggersSectionPosition) || globalVarSectionPosition > MethodSectionPosition)
                {
                    if (context.Node != null)
                        ReportGlobalVariablesShouldbeBeforeTriggersProcedures(context, context.Node.GetLocation(), syntax.Kind);
                }
            }
        }

        private static void ReportGlobalVariablesShouldbeBeforeTriggersProcedures(
            SyntaxNodeAnalysisContext syntaxNodeAnalysisContext,
            Location location,
            SyntaxKind syntaxKind)
        {
            syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule004GlobalVariablesMayBeAboveTriggersProcedures, location, "", (object)syntaxKind));
        }
    }
}
