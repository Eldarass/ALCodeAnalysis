using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ALCodeAnalysis.Readability;

namespace ALCodeAnalysisTests.Readability
{
    [TestClass]
    public class ExtensionFieldsIDValidationTests
    {
        [TestMethod]
        public void TestAnalyzeTableExtFieldsIDs()
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            TableExtensionSyntax tableExtensionSyntax = (SyntaxTree.ParseObjectText(GenerateFakeTableExt()).GetRoot(token) as ObjectCompilationUnitSyntax).Objects.FirstOrDefault() as TableExtensionSyntax;
            SyntaxNodeAnalysisContext context = new SyntaxNodeAnalysisContext();
            if (tableExtensionSyntax != null)
                ExtensionsFieldsIDValidation.AnalyzeTableExtFieldsIDs(context, tableExtensionSyntax, "[50000..99999]");
        }

        [TestMethod]
        public void TestAnalyzeEnumExtValuesIDs()
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            EnumExtensionTypeSyntax enumExtensionTypeSyntax = (SyntaxTree.ParseObjectText(GenerateFakeEnumExt()).GetRoot(token) as ObjectCompilationUnitSyntax).Objects.FirstOrDefault() as EnumExtensionTypeSyntax;
            SyntaxNodeAnalysisContext context = new SyntaxNodeAnalysisContext();
            if (enumExtensionTypeSyntax != null)
                ExtensionsFieldsIDValidation.AnalyzeEnumExtValuesIDs(context, enumExtensionTypeSyntax, "[50000..99999]");
        }

        public string GenerateFakeTableExt()
        {
            string objectHeader = "tableextension 50100 MyTable extends \"Acc. Sched. Cell Value\"";
            string fieldHeader = "field(500; MyField; Integer)";
            string fieldsToken = "fields";
            string openBracketsToken = "{";
            string closeBracketsToken = "}";

            StringBuilder generatedObject = new StringBuilder();
            generatedObject.AppendFormat("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}", objectHeader, openBracketsToken, fieldsToken, openBracketsToken, fieldHeader, openBracketsToken, closeBracketsToken, closeBracketsToken, closeBracketsToken);
            return generatedObject.ToString();
        }

        public string GenerateFakeEnumExt()
        {
            string objectHeader = "enumextension 50100 MyEnumExtension extends status";
            string enumValue = "value(500; MyValue)";
            string openBracketsToken = "{";
            string closeBracketsToken = "}";

            StringBuilder generatedObject = new StringBuilder();
            generatedObject.AppendFormat("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", objectHeader, openBracketsToken, enumValue, openBracketsToken, closeBracketsToken, closeBracketsToken);
            return generatedObject.ToString();
        }
    }
}
