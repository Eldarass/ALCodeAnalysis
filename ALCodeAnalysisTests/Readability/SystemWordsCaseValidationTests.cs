using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ALCodeAnalysis.Readability;

namespace ALCodeAnalysisTests.Readability
{
    [TestClass]
    public class SystemWordsCaseValidationTests
    {
        [TestMethod]
        public void TestValidateSystemWordCase()
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            IEnumerable<SyntaxToken> objectNodes = (SyntaxTree.ParseObjectText(GenerateFakeObjectWithCodeLines("EXIT;")).GetRoot(token) as ObjectCompilationUnitSyntax).Objects.FirstOrDefault().DescendantTokens();
            SyntaxNodeAnalysisContext context = new SyntaxNodeAnalysisContext();
            SystemWordsCaseValidation.AnalyzeSystemWordCase(context);
            if (objectNodes != null)
                SystemWordsCaseValidation.ValidateSystemWordCase(context, objectNodes);
        }

        public string GenerateFakeObjectWithCodeLines(string codeLines)
        {
            string objectHeader = "codeunit 50000 GeneratedObject";
            string procedureName = "local procedure GeneratedProcedure()";
            string beginToken = "begin";
            string endToken = "end;";
            string openBracketsToken = "{";
            string closeBracketsToken = "}";

            StringBuilder generatedObject = new StringBuilder();
            generatedObject.AppendFormat("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}", objectHeader, openBracketsToken, procedureName, beginToken, codeLines, endToken, closeBracketsToken);
            return generatedObject.ToString();
        }
    }
}
