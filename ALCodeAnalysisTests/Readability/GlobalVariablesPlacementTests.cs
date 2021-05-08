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
    public class GlobalVariablesPlacementTests
    {
        [TestMethod]
        public void TestAnalyzeGlobalVariables()
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            GlobalVarSectionSyntax globalVarSectionSyntax = null;

            IEnumerable<SyntaxNode> objectNodes = (SyntaxTree.ParseObjectText(GenerateFakeObjectWithVarForCodeLines("test: Integer;", "test += 1;")).GetRoot(token) as ObjectCompilationUnitSyntax).Objects.FirstOrDefault().DescendantNodes();
            foreach (SyntaxNode syntax in objectNodes)
            {
                if (syntax.Kind == SyntaxKind.GlobalVarSection)
                {
                    globalVarSectionSyntax = syntax as GlobalVarSectionSyntax;
                }
            }
            SyntaxNodeAnalysisContext context = new SyntaxNodeAnalysisContext();
            GlobalVariablesPlacement.AnalyzeGlobalVariables(context);
            if (globalVarSectionSyntax != null)
                GlobalVariablesPlacement.AnalyzeGlobalVariablesPlacement(context, globalVarSectionSyntax);
        }

        public string GenerateFakeObjectWithVarForCodeLines(string variables, string codeLines)
        {
            string objectHeader = "codeunit 50000 GeneratedObject";
            string procedureName = "local procedure GeneratedProcedure()";
            string varToken = "var";
            string beginToken = "begin";
            string endToken = "end;";
            string openBracketsToken = "{";
            string closeBracketsToken = "}";

            StringBuilder generatedObject = new StringBuilder();
            generatedObject.AppendFormat("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}", objectHeader, openBracketsToken, procedureName, beginToken, codeLines, endToken, varToken, variables, closeBracketsToken);
            return generatedObject.ToString();
        }
    }
}
