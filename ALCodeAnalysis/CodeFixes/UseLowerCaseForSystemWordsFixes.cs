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
    [CodeFixProvider("Rule014SystemKeywordsInLowerCase")]
    public sealed class Rule014SystemKeywordsInLowerCase : ICodeFixProvider
    {
        public ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create<string>(DiagnosticDescriptors.Rule014SystemKeywordsInLowerCase.Id);

        public async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            Document document = context.Document;
            TextSpan span = context.Span;
            SyntaxToken token = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false)).FindToken(span.Start);
            await Task.Run((Action)(() => context.RegisterCodeFix((CodeAction)new Rule014SystemKeywordsInLowerCase.UseLowercaseForLanguageKeywordsCodeAction("To lowercase.", (Func<CancellationToken, Task<Document>>)(c => this.KeywordToLowercase(document, token, c)), (string)null), context.Diagnostics)));
        }

        private async Task<Document> KeywordToLowercase(
          Document document,
          SyntaxToken oldToken,
          CancellationToken cancellationToken)
        {
            SyntaxToken newToken = SyntaxFactory.Token(oldToken.Kind).WithTriviaFrom(oldToken);
            return document.WithSyntaxRoot((await document.GetSyntaxRootAsync(cancellationToken)).ReplaceToken<SyntaxNode>(oldToken, newToken));
        }

        private class UseLowercaseForLanguageKeywordsCodeAction : CodeAction.DocumentChangeAction
        {
            public override CodeActionKind Kind => CodeActionKind.QuickFix;

            public UseLowercaseForLanguageKeywordsCodeAction(
              string title,
              Func<CancellationToken, Task<Document>> createChangedDocument,
              string equivalenceKey)
              : base(title, createChangedDocument, equivalenceKey)
            {
            }
        }
    }
}
