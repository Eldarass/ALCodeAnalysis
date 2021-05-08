
using Microsoft.Dynamics.Nav.Analyzers.Common;
using Microsoft.Dynamics.Nav.Analyzers.Common.AppSourceCopConfiguration;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Packaging;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Text;
using Microsoft.Dynamics.Nav.CodeAnalysis.Translation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;


namespace ALCodeAnalysis.Security
{
    [DiagnosticAnalyzer]
    public class EmailAndPhoneNoMustNotBePresentInTheSource : DiagnosticAnalyzer
    {
        private static readonly SymbolKind[] AppObjects = new SymbolKind[15]
        {
      SymbolKind.Page,
      SymbolKind.Codeunit,
      SymbolKind.ControlAddIn,
      SymbolKind.Enum,
      SymbolKind.EnumExtension,
      SymbolKind.Interface,
      SymbolKind.PageCustomization,
      SymbolKind.PageExtension,
      SymbolKind.Profile,
      SymbolKind.ProfileExtension,
      SymbolKind.Query,
      SymbolKind.Report,
      SymbolKind.Table,
      SymbolKind.TableExtension,
      SymbolKind.XmlPort
        };
        private static readonly Lazy<Regex> EmailPatternLazy = new Lazy<Regex>((Func<Regex>)(() => new Regex("([\\w\\-\\.]+)@((\\[([0-9]{1,3}\\.){3}[0-9]{1,3}\\])|(([\\w\\-]+\\.)+)([a-zA-Z]{2,4}))", RegexOptions.Compiled)));
        private static readonly Lazy<Regex> PhoneNoPatternLazy = new Lazy<Regex>((Func<Regex>)(() => new Regex("(([+][(]?[0-9]{1,3}[)]?)|([(]?[0-9]{4}[)]?))\\s*[)]?[-\\s\\.]?[(]?[0-9]{1,3}[)]?([-\\s\\.]?[0-9]{3})([-\\s\\.]?[0-9]{3,4})", RegexOptions.Compiled)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule026EmailAndPhoneNoMustNotBePresentInTheSource);

        private static Regex EmailPattern => EmailAndPhoneNoMustNotBePresentInTheSource.EmailPatternLazy.Value;

        private static Regex PhoneNoPattern => EmailAndPhoneNoMustNotBePresentInTheSource.PhoneNoPatternLazy.Value;

        private static bool IsContainsEmail(string input) => EmailAndPhoneNoMustNotBePresentInTheSource.EmailPattern.IsMatch(input);

        private static bool IsContainsPhoneNo(string input) => EmailAndPhoneNoMustNotBePresentInTheSource.PhoneNoPattern.IsMatch(input);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(EmailAndPhoneNoMustNotBePresentInTheSource.CheckApplicationObjects), EmailAndPhoneNoMustNotBePresentInTheSource.AppObjects);
            context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(EmailAndPhoneNoMustNotBePresentInTheSource.CheckSyntax), SyntaxKind.TextConstDataType, SyntaxKind.LabelDataType, SyntaxKind.PageField, SyntaxKind.PageAction, SyntaxKind.Field, SyntaxKind.FieldModification, SyntaxKind.ControlModifyChange, SyntaxKind.ActionModifyChange);
            context.RegisterCompilationAction(new Action<CompilationAnalysisContext>(EmailAndPhoneNoMustNotBePresentInTheSource.CheckAppManifest));
        }

        private static void CheckApplicationObjects(SymbolAnalysisContext symbolAnalysisContext)
        {
            ITypeSymbol symbol = (ITypeSymbol)symbolAnalysisContext.Symbol;
            EmailAndPhoneNoMustNotBePresentInTheSource.CheckObjectName(symbolAnalysisContext, symbol);
            EmailAndPhoneNoMustNotBePresentInTheSource.CheckObjectCaptions(symbolAnalysisContext, symbol);
        }

        private static void CheckObjectName(
          SymbolAnalysisContext symbolAnalysisContext,
          ITypeSymbol applicationObject)
        {
            EmailAndPhoneNoMustNotBePresentInTheSource.VerifyValue(new Action<Diagnostic>(symbolAnalysisContext.ReportDiagnostic), applicationObject.Name, applicationObject.GetLocation(), applicationObject.Kind.ToString(), "Name");
        }

        private static void CheckObjectCaptions(
          SymbolAnalysisContext symbolAnalysisContext,
          ITypeSymbol applicationObject)
        {
            EmailAndPhoneNoMustNotBePresentInTheSource.CheckCaptions(new Action<Diagnostic>(symbolAnalysisContext.ReportDiagnostic), applicationObject.DeclaringSyntaxReference.GetSyntax());
        }

        private static void CheckSyntax(
          SyntaxNodeAnalysisContext syntaxNodeAnalysisContext)
        {
            SyntaxKind kind = syntaxNodeAnalysisContext.Node.Kind;
            if ((uint)kind <= 321U)
            {
                if ((uint)kind <= 307U)
                {
                    if (kind != SyntaxKind.TextConstDataType)
                    {
                        if (kind != SyntaxKind.LabelDataType)
                            return;
                        EmailAndPhoneNoMustNotBePresentInTheSource.CheckLabel(syntaxNodeAnalysisContext, (LabelDataTypeSyntax)syntaxNodeAnalysisContext.Node);
                        return;
                    }
                    EmailAndPhoneNoMustNotBePresentInTheSource.CheckTextConst(syntaxNodeAnalysisContext, (TextConstDataTypeSyntax)syntaxNodeAnalysisContext.Node);
                    return;
                }
                if (kind != SyntaxKind.Field)
                {
                    if (kind != SyntaxKind.FieldModification)
                        return;
                }
                else
                {
                    SyntaxNode node = syntaxNodeAnalysisContext.Node;
                    EmailAndPhoneNoMustNotBePresentInTheSource.CheckCaptions(new Action<Diagnostic>(syntaxNodeAnalysisContext.ReportDiagnostic), node);
                    EmailAndPhoneNoMustNotBePresentInTheSource.CheckFieldName(syntaxNodeAnalysisContext, node);
                    return;
                }
            }
            else if ((uint)kind <= 337U)
            {
                if (kind != SyntaxKind.PageField && kind != SyntaxKind.PageAction)
                    return;
            }
            else if (kind != SyntaxKind.ActionModifyChange && kind != SyntaxKind.ControlModifyChange)
                return;
            SyntaxNode node1 = syntaxNodeAnalysisContext.Node;
            EmailAndPhoneNoMustNotBePresentInTheSource.CheckCaptions(new Action<Diagnostic>(syntaxNodeAnalysisContext.ReportDiagnostic), node1);
            EmailAndPhoneNoMustNotBePresentInTheSource.CheckPageFieldTooltips(new Action<Diagnostic>(syntaxNodeAnalysisContext.ReportDiagnostic), node1);
            EmailAndPhoneNoMustNotBePresentInTheSource.CheckFieldName(syntaxNodeAnalysisContext, node1);
        }

        private static void CheckTextConst(
          SyntaxNodeAnalysisContext syntaxNodeAnalysisContext,
          TextConstDataTypeSyntax textConst)
        {
            foreach (IdentifierEqualsStringSyntax equalsStringSyntax in textConst.Multilanguage.Values)
                EmailAndPhoneNoMustNotBePresentInTheSource.VerifyValue(new Action<Diagnostic>(syntaxNodeAnalysisContext.ReportDiagnostic), equalsStringSyntax.StringLiteral.ToString(), equalsStringSyntax.GetLocation(), NavTypeKind.TextConst.ToString(), ((VariableDeclarationSyntax)textConst.Parent.Parent).Name.ToString());
        }

        private static void CheckLabel(
          SyntaxNodeAnalysisContext syntaxNodeAnalysisContext,
          LabelDataTypeSyntax label)
        {
            EmailAndPhoneNoMustNotBePresentInTheSource.VerifyValue(new Action<Diagnostic>(syntaxNodeAnalysisContext.ReportDiagnostic), label.Label.LabelText.Value.ValueText, label.GetLocation(), NavTypeKind.Label.ToString(), ((VariableDeclarationSyntax)label.Parent.Parent).Name.ToString());
        }

        private static void CheckCaptions(Action<Diagnostic> reportDiagnostic, SyntaxNode syntaxNode) => EmailAndPhoneNoMustNotBePresentInTheSource.CheckCaptionsTooltips(reportDiagnostic, syntaxNode.GetPropertyValue(PropertyKind.Caption) ?? syntaxNode.GetPropertyValue(PropertyKind.CaptionML), PropertyKind.Caption, syntaxNode.GetNameStringValue());

        private static void CheckPageFieldTooltips(
          Action<Diagnostic> reportDiagnostic,
          SyntaxNode syntaxNode)
        {
            EmailAndPhoneNoMustNotBePresentInTheSource.CheckCaptionsTooltips(reportDiagnostic, syntaxNode.GetPropertyValue(PropertyKind.ToolTip) ?? syntaxNode.GetPropertyValue(PropertyKind.ToolTipML), PropertyKind.ToolTip, syntaxNode.GetNameStringValue());
        }

        private static void CheckFieldName(
          SyntaxNodeAnalysisContext syntaxNodeAnalysisContext,
          SyntaxNode syntaxNode)
        {
            string nameStringValue = syntaxNode.GetNameStringValue();
            EmailAndPhoneNoMustNotBePresentInTheSource.VerifyValue(new Action<Diagnostic>(syntaxNodeAnalysisContext.ReportDiagnostic), nameStringValue, syntaxNode.GetLocation(), syntaxNode.Kind.ToString(), nameStringValue);
        }

        private static void CheckCaptionsTooltips(
          Action<Diagnostic> reportDiagnostic,
          PropertyValueSyntax propValue,
          PropertyKind propKind,
          string name)
        {
            if (propValue == null)
                return;
            switch (propValue.Kind)
            {
                case SyntaxKind.MultilanguagePropertyValue:
                    EmailAndPhoneNoMustNotBePresentInTheSource.CheckPropertyML(reportDiagnostic, propValue, name, propKind.ToString());
                    break;
                case SyntaxKind.LabelPropertyValue:
                    EmailAndPhoneNoMustNotBePresentInTheSource.CheckProperty(reportDiagnostic, propValue, name, propKind.ToString());
                    break;
            }
        }

        private static void CheckProperty(
          Action<Diagnostic> reportDiagnostic,
          PropertyValueSyntax propValue,
          string name,
          string kindName)
        {
            EmailAndPhoneNoMustNotBePresentInTheSource.VerifyValue(reportDiagnostic, propValue.ToString(), propValue.GetLocation(), kindName, name);
        }

        private static void CheckPropertyML(
          Action<Diagnostic> reportDiagnostic,
          PropertyValueSyntax propValue,
          string name,
          string kindName)
        {
            // ISSUE: variable of a compiler-generated type
            CommaSeparatedIdentifierEqualsStringListSyntax stringListSyntax = ((MultilanguagePropertyValueSyntax)propValue).Value;
            if (stringListSyntax.Values.Count <= 0)
                return;
            foreach (IdentifierEqualsStringSyntax equalsStringSyntax in stringListSyntax.Values)
            {
                Action<Diagnostic> reportDiagnostic1 = reportDiagnostic;
                // ISSUE: variable of a compiler-generated type
                StringLiteralValueSyntax stringLiteral = equalsStringSyntax.StringLiteral;
                string input = stringLiteral != null ? SyntaxNodeExtensions.GetLiteralValue(stringLiteral).ToString() : (string)null;
                Location location = propValue.GetLocation();
                string kindName1 = kindName;
                string name1 = name;
                EmailAndPhoneNoMustNotBePresentInTheSource.VerifyValue(reportDiagnostic1, input, location, kindName1, name1);
            }
        }

        private static void CheckAppManifest(
          CompilationAnalysisContext compilationAnalysisContext)
        {
            NavAppManifest manifest = AppSourceCopConfigurationProvider.GetManifest(compilationAnalysisContext.Compilation);
            if (manifest == null)
                return;
            EmailAndPhoneNoMustNotBePresentInTheSource.VerifyValue(compilationAnalysisContext, manifest, manifest.AppBrief, "brief");
            EmailAndPhoneNoMustNotBePresentInTheSource.VerifyValue(compilationAnalysisContext, manifest, manifest.AppDescription, "description");
            EmailAndPhoneNoMustNotBePresentInTheSource.VerifyValue(compilationAnalysisContext, manifest, manifest.AppName, "name");
            EmailAndPhoneNoMustNotBePresentInTheSource.VerifyValue(compilationAnalysisContext, manifest, manifest.AppPublisher, "publisher");
            EmailAndPhoneNoMustNotBePresentInTheSource.CheckTranslations(compilationAnalysisContext, manifest);
        }

        private static void CheckTranslations(
          CompilationAnalysisContext compilationAnalysisContext,
          NavAppManifest manifest)
        {
            if (!manifest.CompilerFeatures.ShouldGenerateTranslationFile())
                return;
            List<string> stringList = new List<string>();
            foreach (string supportedCountry in AppSourceCopConfigurationProvider.GetSupportedCountries(compilationAnalysisContext.Compilation))
            {
                if (LocalizationHelper.CountryCodeToLanguageCodes.ContainsKey(supportedCountry))
                    stringList.Add(supportedCountry);
            }
            if (stringList.Count == 0 || EmailAndPhoneNoMustNotBePresentInTheSource.GetLanguageCodesForSupportedCountries((IEnumerable<string>)stringList).Count == 0)
                return;
            foreach (string xliffLanguageFile in LanguageFileUtilities.GetXliffLanguageFiles(compilationAnalysisContext.Compilation.FileSystem, manifest.AppName))
            {
                try
                {
                    MemoryStream memoryStream = new MemoryStream(compilationAnalysisContext.Compilation.FileSystem.ReadBytes(xliffLanguageFile));
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load((Stream)memoryStream);
                    Location propertyLocation = AppSourceCopConfigurationProvider.GetAppSourceCopConfigurationPropertyLocation(compilationAnalysisContext.Compilation, "SupportedCountries");
                    foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName("trans-unit", "urn:oasis:names:tc:xliff:document:1.2"))
                    {
                        string str = xmlNode.Attributes["id"].Value;
                        foreach (XmlNode childNode in xmlNode.ChildNodes)
                        {
                            if (SemanticFacts.IsSameName("target", childNode.LocalName))
                                EmailAndPhoneNoMustNotBePresentInTheSource.VerifyValue(new Action<Diagnostic>(compilationAnalysisContext.ReportDiagnostic), childNode.InnerText, propertyLocation, xliffLanguageFile, string.Format("{0}:{1}", (object)str, (object)childNode.LocalName));
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobalLogger.LogException(ex);
                }
            }
        }

        private static ISet<string> GetLanguageCodesForSupportedCountries(
          IEnumerable<string> countries)
        {
            PooledHashSet<string> instance = PooledHashSet<string>.GetInstance();
            try
            {
                foreach (string country in countries)
                    instance.UnionWith(EmailAndPhoneNoMustNotBePresentInTheSource.GetSupportedLocales(country));
                return (ISet<string>)instance.ToImmutableHashSet<string>();
            }
            finally
            {
                instance.Free();
            }
        }

        private static IEnumerable<string> GetSupportedLocales(string countryCode)
        {
            List<string> stringList;
            return LocalizationHelper.CountryCodeToLanguageCodes.TryGetValue(countryCode, out stringList) ? (IEnumerable<string>)stringList : (IEnumerable<string>)Array.Empty<string>();
        }

        private static void VerifyValue(
          Action<Diagnostic> reportDiagnostic,
          string input,
          Location location,
          string kindName,
          string name)
        {
            if (string.IsNullOrWhiteSpace(input))
                return;
            input = input.Trim();
            if (!EmailAndPhoneNoMustNotBePresentInTheSource.IsContainsEmail(input) && !EmailAndPhoneNoMustNotBePresentInTheSource.IsContainsPhoneNo(input))
                return;
            reportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule026EmailAndPhoneNoMustNotBePresentInTheSource, location, (object)kindName, (object)name));
        }

        private static void VerifyValue(
          CompilationAnalysisContext context,
          NavAppManifest manifest,
          string input,
          string propertyName)
        {
            EmailAndPhoneNoMustNotBePresentInTheSource.VerifyValue(new Action<Diagnostic>(context.ReportDiagnostic), input, manifest.GetDiagnosticLocation(propertyName), "app.json", propertyName);
        }
    }
}
