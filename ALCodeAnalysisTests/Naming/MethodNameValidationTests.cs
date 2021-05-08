using Microsoft.VisualStudio.TestTools.UnitTesting;
using ALCodeAnalysis.Naming;
using System.Threading;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Collections.Generic;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Text;

namespace ALCodeAnalysisTests.Naming
{
    [TestClass]
    public class MethodNameValidationTests
    {
        [TestMethod]
        public void TestAnalyzeMethodSyntax()
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            MethodDeclarationSyntax methodDeclarationSyntax = null;

            IEnumerable<SyntaxNode> objectNodes = (SyntaxTree.ParseObjectText(GenerateFakeObjectWithVarForCodeLines("test: Integer;", "test += 1;")).GetRoot(token) as ObjectCompilationUnitSyntax).Objects.FirstOrDefault().DescendantNodes();
            foreach (SyntaxNode syntax in objectNodes)
            {
                if (syntax.Kind == SyntaxKind.MethodDeclaration)
                {
                    methodDeclarationSyntax = syntax as MethodDeclarationSyntax;
                }
            }
            SyntaxNodeAnalysisContext context = new SyntaxNodeAnalysisContext();
            CodeBlockStartAnalysisContext analysisContext;
            MethodNameValidation.AnalyzeMethodNameSyntax(context);
            if (methodDeclarationSyntax != null)
                MethodNameValidation.AnalyzeMethodName(context, methodDeclarationSyntax);
        }

        [TestMethod]
        public void IsMethodNamePascalCase_RetrunsFalse()
        {
            string methodName = "methodName";

            bool actual = MethodNameValidation.IsMethodNamePascalCase(methodName);

            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsMethodNamePascalCase_RetrunsTrue()
        {
            string methodName = "MethodName";

            bool actual = MethodNameValidation.IsMethodNamePascalCase(methodName);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void IsMethodNameContainsWhiteSpace_ReturnsTrue()
        {
            string methodName = "meth od";

            bool actual = MethodNameValidation.IsMethodContainWhiteSpace(methodName);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void IsMethodNameContainsWhiteSpace_ReturnsFalse()
        {
            string methodName = "method";

            bool actual = MethodNameValidation.IsMethodContainWhiteSpace(methodName);

            Assert.IsFalse (actual);
        }

        public string GenerateFakeObjectWithVarForCodeLines(string variables, string codeLines)
        {
            string objectHeader = "codeunit 50000 GeneratedObject";
            string procedureName = "local procedure GeneratedProcedure()";
            string varToken = "var";
            string beginToken = "begin";
            string endToken = "end";
            string semicolonToken = ";";
            string openBracketsToken = "{";
            string closeBracketsToken = "}";

            StringBuilder generatedObject = new StringBuilder();
            generatedObject.AppendFormat("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}{8}\n{9}", objectHeader, openBracketsToken, procedureName, varToken, variables, beginToken, codeLines, endToken, semicolonToken, closeBracketsToken);
            return generatedObject.ToString();
        }
    }
}
