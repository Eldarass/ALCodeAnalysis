using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System;
using System.Collections.Immutable;

namespace ALCodeAnalysis.Readability
{
    [DiagnosticAnalyzer]
    public class Rule019UseParenthesisForFunctionCall : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule019UseParenthesisForFunctionCall);


        public override void Initialize(AnalysisContext context) => context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeInvocationExpression), OperationKind.InvocationExpression);

        private void AnalyzeInvocationExpression(OperationAnalysisContext ctx)
        {
            IInvocationExpression operation = (IInvocationExpression)ctx.Operation;
            if (operation.Arguments.Length != 0)
                return;
            IMethodSymbol targetMethod1 = operation.TargetMethod;
            if ((targetMethod1 != null ? (targetMethod1.MethodKind == MethodKind.Property ? 1 : 0) : 0) != 0)
                return;
            IMethodSymbol targetMethod2 = operation.TargetMethod;
            if ((targetMethod2 != null ? (targetMethod2.MethodKind == MethodKind.BuiltInMethod ? 1 : 0) : 0) != 0 && ((IBuiltInMethodTypeSymbol)operation.TargetMethod).IsProperty || operation.Syntax.GetLastToken().IsKind(SyntaxKind.CloseParenToken))
                return;
            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule019UseParenthesisForFunctionCall, ctx.Operation.Syntax.GetLocation(), (object)operation.TargetMethod.Name));
        }
    }
}
