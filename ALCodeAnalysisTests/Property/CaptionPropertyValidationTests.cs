using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading;
using ALCodeAnalysis.Property;
using System.Text;

namespace ALCodeAnalysisTests.Property
{
    [TestClass]
    public class CaptionPropertyValidationTests
    {
        [TestMethod]
        public void TestObjectCaptionPropertyValidation()
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            ObjectSyntax objectSyntax = (SyntaxTree.ParseObjectText(GenerateFakeTableObject()).GetRoot(token) as ObjectCompilationUnitSyntax).Objects.FirstOrDefault();
            SyntaxNodeAnalysisContext context = new SyntaxNodeAnalysisContext();
            CaptionPropertyValidation.AnalyzeObjectCaption(context);
            if (objectSyntax != null)
                CaptionPropertyValidation.AnalyzeObjectCaptions(context,objectSyntax);
        }

        [TestMethod]
        public void TestFieldCaptionPropertyValidation()
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            FieldSyntax fieldSyntax = null;

            IEnumerable<SyntaxNode> objectNodes = (SyntaxTree.ParseObjectText(GenerateFakeTableObject()).GetRoot(token) as ObjectCompilationUnitSyntax).Objects.FirstOrDefault().DescendantNodes();
            foreach (SyntaxNode syntax in objectNodes)
            {
                if (syntax.Kind == SyntaxKind.Field)
                {
                    fieldSyntax = syntax as FieldSyntax;
                }
            }

            SyntaxNodeAnalysisContext context = new SyntaxNodeAnalysisContext();
            CaptionPropertyValidation.AnalyzeTableFieldsCaption(context);
            if (fieldSyntax != null)
                CaptionPropertyValidation.AnalyzeTableFieldsCaptions(context, fieldSyntax);
        }

        [TestMethod]
        public void TestEnumValueCaptionPropertyValidation()
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            EnumValueSyntax enumValueSyntax = null;

            IEnumerable<SyntaxNode> objectNodes = (SyntaxTree.ParseObjectText(GenerateFakeEnumObject()).GetRoot(token) as ObjectCompilationUnitSyntax).Objects.FirstOrDefault().DescendantNodes();
            foreach (SyntaxNode syntax in objectNodes)
            {
                if (syntax.Kind == SyntaxKind.EnumValue)
                {
                    enumValueSyntax = syntax as EnumValueSyntax;
                }
            }

            SyntaxNodeAnalysisContext context = new SyntaxNodeAnalysisContext();
            CaptionPropertyValidation.AnalyzeEnumValuesCaption(context);
            if (enumValueSyntax != null)
                CaptionPropertyValidation.AnalyzeEnumValuesCaptions(context, enumValueSyntax);
        }

        [TestMethod]
        public void TestPagePartCaptionPropertyValidation()
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            PageGroupSyntax pageGroupSyntax = null;

            IEnumerable<SyntaxNode> objectNodes = (SyntaxTree.ParseObjectText(GeneratePageObject()).GetRoot(token) as ObjectCompilationUnitSyntax).Objects.FirstOrDefault().DescendantNodes();
            foreach (SyntaxNode syntax in objectNodes)
            {
                if (syntax.Kind == SyntaxKind.PageGroup)
                {
                    pageGroupSyntax = syntax as PageGroupSyntax;
                }
            }

            SyntaxNodeAnalysisContext context = new SyntaxNodeAnalysisContext();
            CaptionPropertyValidation.AnalyzePagePartsCaption(context);
            if (pageGroupSyntax != null)
                CaptionPropertyValidation.AnalyzePagePartsCaptions(context, pageGroupSyntax);
        }

        public string GenerateFakeTableObject()
        {
            string objectHeader = "table 50000 GeneratedObject";
            string caption = " Caption = 'test';";
            string fieldsToken = "fields";
            string fieldDeclaration = "field(1;MyField; Integer)";
            string openBracketsToken = "{";
            string closeBracketsToken = "}";

            StringBuilder generatedObject = new StringBuilder();
            generatedObject.AppendFormat("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}\n{9}\n{10}", objectHeader, openBracketsToken,caption, fieldsToken, openBracketsToken, fieldDeclaration, openBracketsToken, caption, closeBracketsToken, closeBracketsToken, closeBracketsToken);
            return generatedObject.ToString();
        }

        public string GenerateFakeEnumObject()
        {
            string objectHeader = "enum 50000 GeneratedObject";
            string caption = "Caption = 'test';";
            string enumValue = "value(0; MyValue)";
            string openBracketsToken = "{";
            string closeBracketsToken = "}";

            StringBuilder generatedObject = new StringBuilder();
            generatedObject.AppendFormat("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}", objectHeader, openBracketsToken, enumValue, openBracketsToken, caption, closeBracketsToken, closeBracketsToken);
            return generatedObject.ToString();
        }

        public string GeneratePageObject()
        {
            string objectHeader = "page 50000 GeneratedObject";
            string caption = " Caption = 'test';";
            string layoutToken = "layout";
            string area = "area(Content)";
            string group = "group(GroupName)";
            string openBracketsToken = "{";
            string closeBracketsToken = "}";

            StringBuilder generatedObject = new StringBuilder();
            generatedObject.AppendFormat("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}\n{9}\n{10}\n{11}\n{12}", objectHeader, openBracketsToken, layoutToken, openBracketsToken, area, openBracketsToken, group, openBracketsToken, caption, closeBracketsToken, closeBracketsToken, closeBracketsToken, closeBracketsToken);
            return generatedObject.ToString();
        }
    }
}
