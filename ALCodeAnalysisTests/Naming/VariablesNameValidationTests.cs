using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ALCodeAnalysisTests.Naming
{
    [TestClass]
    public class VariablesNameValidationTests
    {
        [TestMethod]
        public void TestAnalyzeVariableSyntax()
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            VariableDeclarationSyntax variableDeclarationSyntax = null;

            IEnumerable<SyntaxNode> objectNodes = (SyntaxTree.ParseObjectText(GenerateFakeObjectWithVarForCodeLines("test: Integer;", "test += 1;")).GetRoot(token) as ObjectCompilationUnitSyntax).Objects.FirstOrDefault().DescendantNodes();
            foreach (SyntaxNode syntax in objectNodes)
            {
                if (syntax.Kind == SyntaxKind.VariableDeclaration)
                {
                    variableDeclarationSyntax = syntax as VariableDeclarationSyntax;
                }
            }
            SyntaxNodeAnalysisContext context = new SyntaxNodeAnalysisContext();
            ALCodeAnalysis.Naming.VariablesNameValidation.AnalyzeVariableSyntax(context);
            Assert.IsInstanceOfType(context, context.GetType());

            if (variableDeclarationSyntax != null)
            ALCodeAnalysis.Naming.VariablesNameValidation.AnalyzeVariable(context, variableDeclarationSyntax);
        }

        [TestMethod]
        public void IsVariableNamePascalCase_ReturnsFalse()
        {
            string variableName = "namenotInPascalCase";

            bool actual = ALCodeAnalysis.Naming.VariablesNameValidation.IsVariableNamePascalCase(variableName);

            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsVariableNamePascalCase_ReturnsTrue()
        {
            string variableName = "NameInPascalCase";

            bool actual = ALCodeAnalysis.Naming.VariablesNameValidation.IsVariableNamePascalCase(variableName);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void IsVaraibleContainWhiteSpace_ReturnsTrue()
        {
            string variableName = "NameWith White Spaces";

            bool actual = ALCodeAnalysis.Naming.VariablesNameValidation.IsVaraibleContainWhiteSpace(variableName);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void IsVaraibleContainWhiteSpace_ReturnsFalse()
        {
            string variableName = "NameWithoutWhiteSpace";

            bool actual = ALCodeAnalysis.Naming.VariablesNameValidation.IsVaraibleContainWhiteSpace(variableName);

            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsVaraibleContainWildcardSymbols_ReturnsTrue()
        {
            string variableName = "$NameWithWildCard%";

            bool actual = ALCodeAnalysis.Naming.VariablesNameValidation.IsVaraibleContainWildcardSymbols(variableName);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void IsVaraibleContainWildcardSymbols_ReturnsFalse()
        {
            string variableName = "NameWithoutWildCard@";

            bool actual = ALCodeAnalysis.Naming.VariablesNameValidation.IsVaraibleContainWildcardSymbols(variableName);

            Assert.IsFalse(actual);
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
