using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeActions;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeActions.Mef;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeFixes;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Workspaces;
using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace ALCodeAnalysis.CodeFixes
{
    [CodeFixProvider("Fix019UseParenthesisForFunctionCallCodeFixProvider")]
    public sealed class Fix019UseParenthesisForFunctionCallCodeFixProvider : ICodeFixProvider
    {
        public ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create<string>(DiagnosticDescriptors.Rule019UseParenthesisForFunctionCall.Id);

        public async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            Document document = context.Document;
            TextSpan span = context.Span;
            SyntaxNode node = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false)).FindNode(span);
            if (node == null)
                return;
            else
                await Task.Run((Action)(() => context.RegisterCodeFix((CodeAction)new Fix019UseParenthesisForFunctionCallCodeFixProvider.UseParenthesisForFunctionCallCodeAction("Add paranthesis", (Func<CancellationToken, Task<Document>>)(c => this.AddParenthesisForFunction(document, node, c)), (string)null), context.Diagnostics)));
        }

        private async Task<Document> AddParenthesisForFunction(
          Document document,
          SyntaxNode oldNode,
          CancellationToken cancellationToken)
        {
            SyntaxNode newNode;
            switch (oldNode.Kind)
            {
                case SyntaxKind.MemberAccessExpression:
                    // ISSUE: reference to a compiler-generated method
                    newNode = (SyntaxNode)SyntaxFactory.InvocationExpression((CodeExpressionSyntax)oldNode);
                    break;
                case SyntaxKind.IdentifierName:
                    // ISSUE: reference to a compiler-generated method
                    // ISSUE: reference to a compiler-generated method
                    newNode = (SyntaxNode)SyntaxFactory.InvocationExpression((CodeExpressionSyntax)SyntaxFactory.IdentifierName(oldNode.GetIdentifierOrLiteralValue())).WithoutTrivia<InvocationExpressionSyntax>();
                    break;
                default:
                    return document;
            }
            return document.WithSyntaxRoot((await document.GetSyntaxRootAsync(cancellationToken)).ReplaceNode<SyntaxNode>(oldNode, newNode));
        }

        private class UseParenthesisForFunctionCallCodeAction : CodeAction.DocumentChangeAction
        {
            public override CodeActionKind Kind => CodeActionKind.QuickFix;

            public UseParenthesisForFunctionCallCodeAction(
              string title,
              Func<CancellationToken, Task<Document>> createChangedDocument,
              string equivalenceKey)
              : base(title, createChangedDocument, equivalenceKey)
            {
            }
        }
    }
}
