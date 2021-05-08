using ALCodeAnalysis.Utilities;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ALCodeAnalysis.Reliability
{
    [DiagnosticAnalyzer]
    public class VariableInitializationUsageAnalyzer : DiagnosticAnalyzer
    {
        private static readonly HashSet<string> changingBuildInMethods = new HashSet<string>((IEqualityComparer<string>)SemanticFacts.NameEqualityComparer)
    {
      "insert"
    };
        private static readonly HashSet<string> initBuildInMethods = new HashSet<string>((IEqualityComparer<string>)SemanticFacts.NameEqualityComparer)
    {
      "init",
      "clear",
      "copy"
    };
        private static readonly HashSet<NavTypeKind> navTypeKinds = new HashSet<NavTypeKind>()
    {
      NavTypeKind.BigInteger,
      NavTypeKind.Boolean,
      NavTypeKind.Byte,
      NavTypeKind.Char,
      NavTypeKind.Code,
      NavTypeKind.Date,
      NavTypeKind.DateTime,
      NavTypeKind.Decimal,
      NavTypeKind.Integer,
      NavTypeKind.Record,
      NavTypeKind.Time
    };
        private static readonly SyntaxKind[] objectSyntaxKinds = new SyntaxKind[8]
        {
      SyntaxKind.CodeunitObject,
      SyntaxKind.PageObject,
      SyntaxKind.PageExtensionObject,
      SyntaxKind.ReportObject,
      SyntaxKind.TableObject,
      SyntaxKind.TableExtensionObject,
      SyntaxKind.XmlPortObject,
      SyntaxKind.QueryObject
        };

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule021VariablesMustAlwaysBeInitialized, DiagnosticDescriptors.Rule020InitializedVariablesMustAlwaysBeUsed);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(VariableInitializationUsageAnalyzer.InitializeGlobalVariablesAnalyzer), VariableInitializationUsageAnalyzer.objectSyntaxKinds);
            VariableInitializationUsageAnalyzer.InitializeLocalVariablesAnalyzer(context);
        }

        private static void InitializeLocalVariablesAnalyzer(AnalysisContext context)
        {
            context.RegisterCodeBlockStartAction((Action<CodeBlockStartAnalysisContext>)(startCodeBlockContext =>
{
if ((startCodeBlockContext != null ? (startCodeBlockContext.OwningSymbol.Kind != SymbolKind.Method ? 1 : 0) : 1) != 0)
return;
IMethodSymbol owningSymbol = (IMethodSymbol)startCodeBlockContext.OwningSymbol;
if (owningSymbol.IsEvent || owningSymbol.IsObsoleteRemoved || owningSymbol.IsObsoletePending || owningSymbol.LocalVariables.IsEmpty)
return;
VariableInitializationUsageAnalyzer.LocalVariableAnalyzer variableAnalyzer = new VariableInitializationUsageAnalyzer.LocalVariableAnalyzer();
variableAnalyzer.CollectDeclaredLocalVariables(startCodeBlockContext);
startCodeBlockContext.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(variableAnalyzer.CollectLocalVariablesUsage), SyntaxKind.IdentifierName, SyntaxKind.IdentifierNameOrEmpty);
startCodeBlockContext.RegisterCodeBlockEndAction(new Action<CodeBlockAnalysisContext>(variableAnalyzer.ReportUnusedUninitializedVars));
}));
        }

        private static void InitializeGlobalVariablesAnalyzer(
          SyntaxNodeAnalysisContext syntaxNodeAnalysisContext)
        {
            PooledNameObjectDictionary<VariableDeclarationBaseSyntax> instance1 = PooledNameObjectDictionary<VariableDeclarationBaseSyntax>.GetInstance();
            PooledNameObjectDictionary<IdentifierNameSyntax> instance2 = PooledNameObjectDictionary<IdentifierNameSyntax>.GetInstance();
            PooledNameObjectDictionary<IdentifierNameSyntax> instance3 = PooledNameObjectDictionary<IdentifierNameSyntax>.GetInstance();
            PooledNameObjectDictionary<IdentifierNameSyntax> instance4 = PooledNameObjectDictionary<IdentifierNameSyntax>.GetInstance();
            PooledNameObjectDictionary<IdentifierNameSyntax> instance5 = PooledNameObjectDictionary<IdentifierNameSyntax>.GetInstance();
            try
            {
                VariableInitializationUsageAnalyzer.CollectDeclaredGlobalVariables(syntaxNodeAnalysisContext, (Dictionary<string, VariableDeclarationBaseSyntax>)instance1);
                foreach (SyntaxNode descendantNode in syntaxNodeAnalysisContext.Node.DescendantNodes())
                {
                    if (descendantNode.IsKind(SyntaxKind.IdentifierName))
                    {
                        // ISSUE: variable of a compiler-generated type
                        IdentifierNameSyntax identifier = (IdentifierNameSyntax)descendantNode;
                        if (instance1.ContainsKey(identifier.Identifier.ValueText) && !identifier.Parent.IsKind(SyntaxKind.VariableDeclaration))
                            VariableInitializationUsageAnalyzer.AnalyzeGlobalVarUsage(identifier, (Dictionary<string, IdentifierNameSyntax>)instance2, (Dictionary<string, IdentifierNameSyntax>)instance3, (Dictionary<string, IdentifierNameSyntax>)instance4, (Dictionary<string, IdentifierNameSyntax>)instance5, syntaxNodeAnalysisContext, (Dictionary<string, VariableDeclarationBaseSyntax>)instance1);
                    }
                }
                VariableInitializationUsageAnalyzer.ReportInitializedNotUsedNotInitializedUsedVars(syntaxNodeAnalysisContext, (Dictionary<string, IdentifierNameSyntax>)instance3, (Dictionary<string, IdentifierNameSyntax>)instance2, (Dictionary<string, VariableDeclarationBaseSyntax>)instance1);
            }
            finally
            {
                instance1.Free();
                instance2.Free();
                instance3.Free();
                instance4.Free();
                instance5.Free();
            }
        }

        private static void AnalyzeArgumentListIdentifier(
          IdentifierNameSyntax identifier,
          SyntaxNodeAnalysisContext context,
          Dictionary<string, IdentifierNameSyntax> firstVarInitializerSyntax,
          Dictionary<string, IdentifierNameSyntax> firstVarUsageSyntax,
          Dictionary<string, IdentifierNameSyntax> firstRecordChangeSyntax,
          NavTypeKind variableNavTypeKind)
        {
            // ISSUE: variable of a compiler-generated type
            ArgumentListSyntax parent = (ArgumentListSyntax)identifier.Parent;
            if (!parent.Parent.IsKind(SyntaxKind.InvocationExpression))
                return;
            if (((InvocationExpressionSyntax)parent.Parent).Expression.IsKind(SyntaxKind.IdentifierName) && SemanticFacts.IsSameName(((SimpleNameSyntax)((InvocationExpressionSyntax)parent.Parent).Expression).Identifier.ValueText, "evaluate"))
            {
                SeparatedSyntaxList<CodeExpressionSyntax> arguments = parent.Arguments;
                if (arguments[0].IsKind(SyntaxKind.IdentifierName))
                {
                    arguments = parent.Arguments;
                    SyntaxToken identifier1 = ((SimpleNameSyntax)arguments[0]).Identifier;
                    string valueText1 = identifier1.ValueText;
                    identifier1 = identifier.Identifier;
                    string valueText2 = identifier1.ValueText;
                    if (SemanticFacts.IsSameName(valueText1, valueText2))
                    {
                        firstVarInitializerSyntax.AddIdentifierIfNotExist(identifier);
                        SyntaxKind? kind = identifier.Parent?.Parent?.Parent?.Kind;
                        if (!kind.HasValue)
                            return;
                        SyntaxKind valueOrDefault = kind.GetValueOrDefault();
                        if ((uint)valueOrDefault <= 235U)
                        {
                            if (valueOrDefault != SyntaxKind.IfStatement && valueOrDefault != SyntaxKind.ExitStatement)
                                return;
                        }
                        else
                        {
                            switch (valueOrDefault)
                            {
                                case SyntaxKind.LogicalOrExpression:
                                case SyntaxKind.LogicalAndExpression:
                                case SyntaxKind.LogicalXorExpression:
                                case SyntaxKind.UnaryNotExpression:
                                    break;
                                default:
                                    return;
                            }
                        }
                        firstVarUsageSyntax.AddIdentifierIfNotExist(identifier);
                        return;
                    }
                }
                firstVarUsageSyntax.AddIdentifierIfNotExist(identifier);
            }
            else
            {
                IInvocationExpression operation = (IInvocationExpression)context.SemanticModel.GetOperation(parent.Parent);
                if (operation == null)
                {
                    if (parent.Arguments.Count == 0)
                        return;
                    firstVarUsageSyntax.AddIdentifierIfNotExist(identifier);
                }
                else
                {
                    if (operation.TargetMethod.MethodKind == MethodKind.BuiltInMethod && SemanticFacts.IsSameName(operation.TargetMethod.Name, "settable"))
                        firstRecordChangeSyntax.AddIdentifierIfNotExist(identifier);
                    foreach (IArgument obj in operation.Arguments)
                    {
                        if (obj.Value.Syntax == identifier)
                        {
                            if (obj.Parameter != null)
                            {
                                if (obj.Parameter.IsVar)
                                {
                                    firstVarInitializerSyntax.AddIdentifierIfNotExist(identifier);
                                    if (variableNavTypeKind == NavTypeKind.Record)
                                    {
                                        firstRecordChangeSyntax.AddIdentifierIfNotExist(identifier);
                                        break;
                                    }
                                    firstVarUsageSyntax.AddIdentifierIfNotExist(identifier);
                                    break;
                                }
                                if (obj.Parameter.IsMemberReference)
                                    break;
                            }
                            if (variableNavTypeKind == NavTypeKind.Record)
                                break;
                            firstVarUsageSyntax.AddIdentifierIfNotExist(identifier);
                            break;
                        }
                    }
                }
            }
        }

        private static void AnalyzeForEachStatementIdentifier(
          IdentifierNameSyntax identifier,
          Dictionary<string, IdentifierNameSyntax> firstVarInitializerSyntax,
          Dictionary<string, IdentifierNameSyntax> firstVarUsageSyntax)
        {
            SyntaxToken identifier1 = ((ForEachStatementSyntax)identifier.Parent).IterationVariable.Identifier;
            string valueText1 = identifier1.ValueText;
            identifier1 = identifier.Identifier;
            string valueText2 = identifier1.ValueText;
            (SemanticFacts.IsSameName(valueText1, valueText2) ? firstVarInitializerSyntax : firstVarUsageSyntax).AddIdentifierIfNotExist(identifier);
        }

        private static void AnalyzeForStatementIdentifier(
          IdentifierNameSyntax identifier,
          Dictionary<string, IdentifierNameSyntax> firstVarInitializerSyntax,
          Dictionary<string, IdentifierNameSyntax> firstVarUsageSyntax)
        {
            // ISSUE: variable of a compiler-generated type
            IdentifierNameSyntax loopVariable = (IdentifierNameSyntax)((ForStatementSyntax)identifier.Parent).LoopVariable;
            SyntaxToken identifier1;
            string nameA;
            if (loopVariable == null)
            {
                nameA = (string)null;
            }
            else
            {
                identifier1 = loopVariable.Identifier;
                nameA = identifier1.ValueText;
            }
            identifier1 = identifier.Identifier;
            string valueText = identifier1.ValueText;
            if (SemanticFacts.IsSameName(nameA, valueText))
                firstVarInitializerSyntax.AddIdentifierIfNotExist(identifier);
            firstVarUsageSyntax.AddIdentifierIfNotExist(identifier);
        }

        private static void AnalyzeAssignmentStatementIdentifier(
          IdentifierNameSyntax identifier,
          Dictionary<string, IdentifierNameSyntax> firstVarInitializerSyntax,
          Dictionary<string, IdentifierNameSyntax> firstVarUsageSyntax,
          Dictionary<string, IdentifierNameSyntax> firstRecordChangeSyntax,
          NavTypeKind variableNavTypeKind)
        {
            // ISSUE: variable of a compiler-generated type
            CodeExpressionSyntax target = ((AssignmentStatementSyntax)identifier.Parent).Target;
            if (target.IsKind(SyntaxKind.IdentifierName))
            {
                SyntaxToken identifier1 = ((SimpleNameSyntax)target).Identifier;
                string valueText1 = identifier1.ValueText;
                identifier1 = identifier.Identifier;
                string valueText2 = identifier1.ValueText;
                if (SemanticFacts.IsSameName(valueText1, valueText2))
                {
                    firstVarInitializerSyntax.AddIdentifierIfNotExist(identifier);
                    return;
                }
                if (variableNavTypeKind == NavTypeKind.Record)
                {
                    firstRecordChangeSyntax.AddIdentifierIfNotExist((IdentifierNameSyntax)target);
                    return;
                }
            }
            firstVarUsageSyntax.AddIdentifierIfNotExist(identifier);
        }

        private static void AnalyzeCompoundAssignmentStatementIdentifier(
          IdentifierNameSyntax identifier,
          Dictionary<string, IdentifierNameSyntax> firstVarInitializerSyntax,
          Dictionary<string, IdentifierNameSyntax> firstVarUsageSyntax)
        {
            // ISSUE: variable of a compiler-generated type
            CompoundAssignmentStatementSyntax parent = (CompoundAssignmentStatementSyntax)identifier.Parent;
            if (parent.Target.IsKind(SyntaxKind.IdentifierName) && SemanticFacts.IsSameName(((SimpleNameSyntax)parent.Target).Identifier.ValueText, identifier.Identifier.ValueText))
                firstVarInitializerSyntax.AddIdentifierIfNotExist(identifier);
            else
                firstVarUsageSyntax.AddIdentifierIfNotExist(identifier);
        }

        private static void AnalyzeMemberAccessIdentifier(
          IdentifierNameSyntax identifier,
          Dictionary<string, IdentifierNameSyntax> firstVarInitializerSyntax,
          Dictionary<string, IdentifierNameSyntax> firstVarUsageSyntax,
          Dictionary<string, IdentifierNameSyntax> lastRecordModifyInvocationSyntax,
          Dictionary<string, IdentifierNameSyntax> firstRecordChangeSyntax,
          NavTypeKind variableNavTypeKind)
        {
            // ISSUE: variable of a compiler-generated type
            MemberAccessExpressionSyntax parent = (MemberAccessExpressionSyntax)identifier.Parent;
            if (variableNavTypeKind != NavTypeKind.Record)
                return;
            string str = string.Empty;
            switch (parent.Parent.Kind)
            {
                case SyntaxKind.ExpressionStatement:
                    str = ((ExpressionStatementSyntax)parent.Parent).Expression.GetNameStringValue();
                    break;
                case SyntaxKind.InvocationExpression:
                    str = parent.Name.ToString();
                    break;
            }
            if (string.IsNullOrEmpty(str) || SemanticFacts.IsSameName(str, "validate"))
            {
                firstRecordChangeSyntax.AddIdentifierIfNotExist(identifier);
            }
            else
            {
                if (VariableInitializationUsageAnalyzer.changingBuildInMethods.Contains(str))
                    firstVarUsageSyntax.AddIdentifierIfNotExist(identifier);
                if (VariableInitializationUsageAnalyzer.initBuildInMethods.Contains(str))
                    firstVarInitializerSyntax.AddIdentifierIfNotExist(identifier);
                if (!SemanticFacts.IsSameName("modify", str))
                    return;
                lastRecordModifyInvocationSyntax.AddLastRecordModifyInvocationSyntax(identifier);
            }
        }

        private static void AnalyzeArrayIndexExpressionIdentifier(
          IdentifierNameSyntax identifier,
          Dictionary<string, IdentifierNameSyntax> firstVarInitializerSyntax)
        {
            // ISSUE: variable of a compiler-generated type
            ElementAccessExpressionSyntax parent = (ElementAccessExpressionSyntax)identifier.Parent;
            if (!parent.Parent.IsKind(SyntaxKind.AssignmentStatement))
                return;
            switch (((AssignmentStatementSyntax)parent.Parent).Target.Kind)
            {
                case SyntaxKind.ArrayIndexExpression:
                    SyntaxToken identifier1 = ((SimpleNameSyntax)((ElementAccessExpressionSyntax)((AssignmentStatementSyntax)parent.Parent).Target).Expression).Identifier;
                    string valueText1 = identifier1.ValueText;
                    identifier1 = identifier.Identifier;
                    string valueText2 = identifier1.ValueText;
                    if (!SemanticFacts.IsSameName(valueText1, valueText2))
                        break;
                    firstVarInitializerSyntax.AddIdentifierIfNotExist(identifier);
                    break;
                case SyntaxKind.IdentifierName:
                    if (!SemanticFacts.IsSameName(((SimpleNameSyntax)((AssignmentStatementSyntax)parent.Parent).Target).Identifier.ValueText, identifier.Identifier.ValueText))
                        break;
                    firstVarInitializerSyntax.AddIdentifierIfNotExist(identifier);
                    break;
            }
        }

        private static void AnalyzeGlobalVarUsage(
          IdentifierNameSyntax identifier,
          Dictionary<string, IdentifierNameSyntax> firstGlobalVarUsageSyntax,
          Dictionary<string, IdentifierNameSyntax> firstGlobalVarInitializerSyntax,
          Dictionary<string, IdentifierNameSyntax> lastRecordModifyInvocationSyntax,
          Dictionary<string, IdentifierNameSyntax> firstRecordChangeSyntax,
          SyntaxNodeAnalysisContext syntaxNodeAnalysisContext,
          Dictionary<string, VariableDeclarationBaseSyntax> globalVariables)
        {
            if (IdentifierUtilities.IdentifierIsLocalVariable(identifier))
                return;
            SyntaxKind kind = identifier.Parent.Kind;
            switch (kind)
            {
                case SyntaxKind.PageField:
                case SyntaxKind.ReportColumn:
                    firstGlobalVarInitializerSyntax.AddIdentifierIfNotExist(identifier);
                    firstGlobalVarUsageSyntax.AddIdentifierIfNotExist(identifier);
                    break;
                default:
                    if (globalVariables.Count == 0 || !globalVariables.ContainsKey(identifier.Identifier.ValueText))
                        break;
                    // ISSUE: variable of a compiler-generated type
                    VariableDeclarationBaseSyntax declarationBaseSyntax;
                    globalVariables.TryGetValue(identifier.Identifier.ValueText, out declarationBaseSyntax);
                    NavTypeKind navTypeKind = NavTypeExtensions.GetNavTypeKind(declarationBaseSyntax?.Type.DataType.TypeName.ValueText);
                    if ((uint)kind <= 233U)
                    {
                        switch (kind)
                        {
                            case SyntaxKind.AssignmentStatement:
                                VariableInitializationUsageAnalyzer.AnalyzeAssignmentStatementIdentifier(identifier, firstGlobalVarInitializerSyntax, firstGlobalVarUsageSyntax, firstRecordChangeSyntax, navTypeKind);
                                return;
                            case SyntaxKind.CompoundAssignmentStatement:
                                if (navTypeKind == NavTypeKind.Record)
                                    return;
                                VariableInitializationUsageAnalyzer.AnalyzeCompoundAssignmentStatementIdentifier(identifier, firstGlobalVarInitializerSyntax, firstGlobalVarUsageSyntax);
                                return;
                            case SyntaxKind.ForStatement:
                                if (navTypeKind == NavTypeKind.Record)
                                    return;
                                VariableInitializationUsageAnalyzer.AnalyzeForStatementIdentifier(identifier, firstGlobalVarInitializerSyntax, firstGlobalVarUsageSyntax);
                                return;
                        }
                    }
                    else if ((uint)kind <= 262U)
                    {
                        switch (kind)
                        {
                            case SyntaxKind.ForEachStatement:
                                if (navTypeKind == NavTypeKind.Record)
                                    return;
                                VariableInitializationUsageAnalyzer.AnalyzeForEachStatementIdentifier(identifier, firstGlobalVarInitializerSyntax, firstGlobalVarUsageSyntax);
                                return;
                            case SyntaxKind.ArrayIndexExpression:
                                if (navTypeKind != NavTypeKind.Code && navTypeKind != NavTypeKind.Text)
                                    return;
                                VariableInitializationUsageAnalyzer.AnalyzeArrayIndexExpressionIdentifier(identifier, firstGlobalVarInitializerSyntax);
                                return;
                        }
                    }
                    else if (kind != SyntaxKind.MemberAccessExpression)
                    {
                        if (kind == SyntaxKind.ArgumentList)
                        {
                            VariableInitializationUsageAnalyzer.AnalyzeArgumentListIdentifier(identifier, syntaxNodeAnalysisContext, firstGlobalVarInitializerSyntax, firstGlobalVarUsageSyntax, firstRecordChangeSyntax, navTypeKind);
                            break;
                        }
                    }
                    else
                    {
                        VariableInitializationUsageAnalyzer.AnalyzeMemberAccessIdentifier(identifier, firstGlobalVarInitializerSyntax, firstGlobalVarUsageSyntax, lastRecordModifyInvocationSyntax, firstRecordChangeSyntax, navTypeKind);
                        break;
                    }
                    if (navTypeKind == NavTypeKind.Record)
                        break;
                    firstGlobalVarUsageSyntax.AddIdentifierIfNotExist(identifier);
                    break;
            }
        }

        private static void CollectInitializedNotUsedNotInitializedUsedGlobalVars(
          HashSet<IdentifierNameSyntax> syntaxUsedNotInitialized,
          HashSet<IdentifierNameSyntax> syntaxAssignedNotUsed,
          Dictionary<string, IdentifierNameSyntax> firstGlobalVarInitializerSyntax,
          Dictionary<string, IdentifierNameSyntax> firstGlobalVarUsageSyntax,
          Dictionary<string, VariableDeclarationBaseSyntax> globalVariables)
        {
            foreach (string key in globalVariables.Keys)
            {
                // ISSUE: variable of a compiler-generated type
                VariableDeclarationBaseSyntax declarationBaseSyntax;
                globalVariables.TryGetValue(key, out declarationBaseSyntax);
                if (!declarationBaseSyntax.Type.IsKind(SyntaxKind.SimpleTypeReference) || declarationBaseSyntax.Type.Array == null || !declarationBaseSyntax.Type.Array.IsKind(SyntaxKind.Array))
                {
                    NavTypeKind navTypeKind = NavTypeExtensions.GetNavTypeKind(declarationBaseSyntax.Type.DataType.TypeName.ValueText);
                    if (VariableInitializationUsageAnalyzer.navTypeKinds.Contains(navTypeKind))
                    {
                        bool flag1 = firstGlobalVarInitializerSyntax.ContainsKey(key);
                        bool flag2 = firstGlobalVarUsageSyntax.ContainsKey(key);
                        if (flag1 && !flag2 && navTypeKind != NavTypeKind.Record)
                        {
                            // ISSUE: variable of a compiler-generated type
                            IdentifierNameSyntax identifierNameSyntax;
                            firstGlobalVarInitializerSyntax.TryGetValue(key, out identifierNameSyntax);
                            syntaxAssignedNotUsed.Add(identifierNameSyntax);
                        }
                        if (flag2 && !flag1 && navTypeKind != NavTypeKind.Boolean && (!declarationBaseSyntax.Type.IsKind(SyntaxKind.RecordTypeReference) || !((RecordTypeReferenceSyntax)declarationBaseSyntax.Type).Temporary.IsKind(SyntaxKind.TemporaryKeyword)))
                        {
                            // ISSUE: variable of a compiler-generated type
                            IdentifierNameSyntax identifierNameSyntax;
                            firstGlobalVarUsageSyntax.TryGetValue(key, out identifierNameSyntax);
                            syntaxUsedNotInitialized.Add(identifierNameSyntax);
                        }
                    }
                }
            }
        }

        private static void ReportInitializedNotUsedNotInitializedUsedVars(
          SyntaxNodeAnalysisContext context,
          Dictionary<string, IdentifierNameSyntax> firstGlobalVarInitializerSyntax,
          Dictionary<string, IdentifierNameSyntax> firstGlobalVarUsageSyntax,
          Dictionary<string, VariableDeclarationBaseSyntax> globalVariables)
        {
            PooledHashSet<IdentifierNameSyntax> instance1 = PooledHashSet<IdentifierNameSyntax>.GetInstance();
            PooledHashSet<IdentifierNameSyntax> instance2 = PooledHashSet<IdentifierNameSyntax>.GetInstance();
            try
            {
                VariableInitializationUsageAnalyzer.CollectInitializedNotUsedNotInitializedUsedGlobalVars((HashSet<IdentifierNameSyntax>)instance2, (HashSet<IdentifierNameSyntax>)instance1, firstGlobalVarInitializerSyntax, firstGlobalVarUsageSyntax, globalVariables);
                VariableInitializationUsageAnalyzer.ReportInitializedNotUsedVariables(new Action<Diagnostic>(context.ReportDiagnostic), instance1);
                VariableInitializationUsageAnalyzer.ReportNotInitializedVariables(new Action<Diagnostic>(context.ReportDiagnostic), instance2);
            }
            finally
            {
                instance1.Free();
                instance2.Free();
            }
        }

        private static void AddVariableToGlobalVarList(
          Dictionary<string, VariableDeclarationBaseSyntax> globalVariables,
          VariableDeclarationBaseSyntax variable)
        {
            if (!VariableInitializationUsageAnalyzer.navTypeKinds.Contains(NavTypeExtensions.GetNavTypeKind(variable.Type.DataType.TypeName.ValueText)) && variable.Type.Array != null)
                return;
            if (variable.IsKind(SyntaxKind.VariableListDeclaration))
            {
                foreach (VariableDeclarationNameSyntax variableName in ((VariableListDeclarationSyntax)variable).VariableNames)
                    globalVariables.Add(variableName.GetNameStringValue(), variable);
            }
            else
                globalVariables.Add(variable.GetNameStringValue(), variable);
        }

        private static void CollectDeclaredGlobalVariables(
          SyntaxNodeAnalysisContext syntaxNodeAnalysisContext,
          Dictionary<string, VariableDeclarationBaseSyntax> globalVariables)
        {
            // ISSUE: variable of a compiler-generated type
            ApplicationObjectSyntax node = (ApplicationObjectSyntax)syntaxNodeAnalysisContext.Node;
            SyntaxList<MemberSyntax> members = node.Members;
            int index1 = members.IndexOf((Func<MemberSyntax, bool>)(ms => ms.Kind == SyntaxKind.GlobalVarSection));
            if (index1 != -1)
            {
                members = node.Members;
                foreach (VariableDeclarationBaseSyntax variable in ((VarSectionBaseSyntax)members[index1]).Variables)
                    VariableInitializationUsageAnalyzer.AddVariableToGlobalVarList(globalVariables, variable);
            }
            if (!syntaxNodeAnalysisContext.Node.IsKind(SyntaxKind.ReportObject, SyntaxKind.XmlPortObject))
                return;
            switch (syntaxNodeAnalysisContext.Node.Kind)
            {
                case SyntaxKind.ReportObject:
                    // ISSUE: variable of a compiler-generated type
                    RequestPageSyntax requestPage = ((ReportSyntax)syntaxNodeAnalysisContext.Node).RequestPage;
                    int num1;
                    if (requestPage == null)
                    {
                        num1 = -1;
                    }
                    else
                    {
                        members = requestPage.Members;
                        num1 = members.IndexOf((Func<MemberSyntax, bool>)(ms => ms.Kind == SyntaxKind.GlobalVarSection));
                    }
                    int index2 = num1;
                    if (index2 == -1)
                        break;
                    Dictionary<string, VariableDeclarationBaseSyntax> globalVariables1 = globalVariables;
                    members = ((ReportSyntax)syntaxNodeAnalysisContext.Node).RequestPage.Members;
                    SyntaxList<VariableDeclarationBaseSyntax> variables1 = ((VarSectionBaseSyntax)members[index2]).Variables;
                    VariableInitializationUsageAnalyzer.AddVariablesToGlobalVarList(globalVariables1, variables1);
                    break;
                case SyntaxKind.XmlPortObject:
                    // ISSUE: variable of a compiler-generated type
                    RequestPageSyntax xmlPortRequestPage = ((XmlPortSyntax)syntaxNodeAnalysisContext.Node).XmlPortRequestPage;
                    int num2;
                    if (xmlPortRequestPage == null)
                    {
                        num2 = -1;
                    }
                    else
                    {
                        members = xmlPortRequestPage.Members;
                        num2 = members.IndexOf((Func<MemberSyntax, bool>)(ms => ms.Kind == SyntaxKind.GlobalVarSection));
                    }
                    int index3 = num2;
                    if (index3 == -1)
                        break;
                    Dictionary<string, VariableDeclarationBaseSyntax> globalVariables2 = globalVariables;
                    members = ((XmlPortSyntax)syntaxNodeAnalysisContext.Node).XmlPortRequestPage.Members;
                    SyntaxList<VariableDeclarationBaseSyntax> variables2 = ((VarSectionBaseSyntax)members[index3]).Variables;
                    VariableInitializationUsageAnalyzer.AddVariablesToGlobalVarList(globalVariables2, variables2);
                    break;
            }
        }

        private static void AddVariablesToGlobalVarList(
          Dictionary<string, VariableDeclarationBaseSyntax> globalVariables,
          SyntaxList<VariableDeclarationBaseSyntax> variables)
        {
            foreach (VariableDeclarationBaseSyntax variable in variables)
                VariableInitializationUsageAnalyzer.AddVariableToGlobalVarList(globalVariables, variable);
        }

        private static void ReportNotInitializedVariables(
          Action<Diagnostic> action,
          PooledHashSet<IdentifierNameSyntax> uninitializedSyntaxes)
        {
            if (uninitializedSyntaxes.Count == 0)
                return;
            foreach (IdentifierNameSyntax uninitializedSyntax in (HashSet<IdentifierNameSyntax>)uninitializedSyntaxes)
            {
                Diagnostic diagnostic = Diagnostic.Create(DiagnosticDescriptors.Rule021VariablesMustAlwaysBeInitialized, uninitializedSyntax.GetLocation(), (object)uninitializedSyntax.ToString(), (object)uninitializedSyntax.GetContainingObjectSyntax().Name);
                action(diagnostic);
            }
        }

        private static void ReportInitializedNotUsedVariables(
          Action<Diagnostic> action,
          PooledHashSet<IdentifierNameSyntax> initializedSyntaxes)
        {
            if (initializedSyntaxes.Count == 0)
                return;
            foreach (IdentifierNameSyntax initializedSyntax in (HashSet<IdentifierNameSyntax>)initializedSyntaxes)
            {
                Diagnostic diagnostic = Diagnostic.Create(DiagnosticDescriptors.Rule020InitializedVariablesMustAlwaysBeUsed, initializedSyntax.GetLocation(), (object)initializedSyntax.ToString(), (object)initializedSyntax.GetContainingObjectSyntax().Name);
                action(diagnostic);
            }
        }

        private class LocalVariableAnalyzer : IDisposable
        {
            private readonly PooledNameObjectDictionary<VariableDeclarationBaseSyntax> localVariables;
            private readonly PooledNameObjectDictionary<IdentifierNameSyntax> firstLocalVarUsageSyntax;
            private readonly PooledNameObjectDictionary<IdentifierNameSyntax> firstInitializerSyntax;
            private readonly PooledNameObjectDictionary<IdentifierNameSyntax> lastRecordModifyInvocationSyntax;
            private readonly PooledNameObjectDictionary<IdentifierNameSyntax> firstRecordChangeSyntax;

            public LocalVariableAnalyzer()
            {
                this.localVariables = PooledNameObjectDictionary<VariableDeclarationBaseSyntax>.GetInstance();
                this.firstInitializerSyntax = PooledNameObjectDictionary<IdentifierNameSyntax>.GetInstance();
                this.firstLocalVarUsageSyntax = PooledNameObjectDictionary<IdentifierNameSyntax>.GetInstance();
                this.lastRecordModifyInvocationSyntax = PooledNameObjectDictionary<IdentifierNameSyntax>.GetInstance();
                this.firstRecordChangeSyntax = PooledNameObjectDictionary<IdentifierNameSyntax>.GetInstance();
            }

            public void CollectDeclaredLocalVariables(CodeBlockStartAnalysisContext context)
            {
                switch (context.CodeBlock.Kind)
                {
                    case SyntaxKind.TriggerDeclaration:
                        if (((MethodOrTriggerDeclarationSyntax)context.CodeBlock).Variables == null)
                            break;
                        this.AddDeclaredLocalVariables(context);
                        break;
                    case SyntaxKind.MethodDeclaration:
                        this.AddDeclaredLocalVariables(context);
                        break;
                }
            }

            private void AddDeclaredLocalVariables(CodeBlockStartAnalysisContext context)
            {
                foreach (VariableDeclarationBaseSyntax variable in ((MethodOrTriggerDeclarationSyntax)context.CodeBlock).Variables.Variables)
                {
                    if (variable.IsKind(SyntaxKind.VariableListDeclaration))
                    {
                        foreach (VariableDeclarationNameSyntax variableName in ((VariableListDeclarationSyntax)variable).VariableNames)
                            this.localVariables.Add(variableName.GetNameStringValue(), variable);
                        break;
                    }
                    this.localVariables.Add(variable.GetNameStringValue(), variable);
                }
            }

            public void CollectLocalVariablesUsage(SyntaxNodeAnalysisContext context)
            {
                IdentifierNameSyntax identifierNameSyntax = context.GetIdentifierNameSyntax();
                if (identifierNameSyntax == null || this.localVariables.Count == 0 || !this.localVariables.ContainsKey(identifierNameSyntax.Identifier.ValueText))
                    return;
                VariableDeclarationBaseSyntax declarationBaseSyntax;
                this.localVariables.TryGetValue(identifierNameSyntax.Identifier.ValueText, out declarationBaseSyntax);
                NavTypeKind navTypeKind = NavTypeExtensions.GetNavTypeKind(declarationBaseSyntax?.Type.DataType.TypeName.ValueText);
                SyntaxKind kind = identifierNameSyntax.Parent.Kind;
                if ((uint)kind <= 233U)
                {
                    switch (kind)
                    {
                        case SyntaxKind.AssignmentStatement:
                            VariableInitializationUsageAnalyzer.AnalyzeAssignmentStatementIdentifier(identifierNameSyntax, (Dictionary<string, IdentifierNameSyntax>)this.firstInitializerSyntax, (Dictionary<string, IdentifierNameSyntax>)this.firstLocalVarUsageSyntax, (Dictionary<string, IdentifierNameSyntax>)this.firstRecordChangeSyntax, navTypeKind);
                            return;
                        case SyntaxKind.CompoundAssignmentStatement:
                            VariableInitializationUsageAnalyzer.AnalyzeCompoundAssignmentStatementIdentifier(identifierNameSyntax, (Dictionary<string, IdentifierNameSyntax>)this.firstInitializerSyntax, (Dictionary<string, IdentifierNameSyntax>)this.firstLocalVarUsageSyntax);
                            return;
                        case SyntaxKind.ForStatement:
                            VariableInitializationUsageAnalyzer.AnalyzeForStatementIdentifier(identifierNameSyntax, (Dictionary<string, IdentifierNameSyntax>)this.firstInitializerSyntax, (Dictionary<string, IdentifierNameSyntax>)this.firstLocalVarUsageSyntax);
                            return;
                    }
                }
                else
                {
                    switch (kind)
                    {
                        case SyntaxKind.ForEachStatement:
                            VariableInitializationUsageAnalyzer.AnalyzeForEachStatementIdentifier(identifierNameSyntax, (Dictionary<string, IdentifierNameSyntax>)this.firstInitializerSyntax, (Dictionary<string, IdentifierNameSyntax>)this.firstLocalVarUsageSyntax);
                            return;
                        case SyntaxKind.ArrayIndexExpression:
                            if (navTypeKind != NavTypeKind.Code && navTypeKind != NavTypeKind.Text)
                                return;
                            VariableInitializationUsageAnalyzer.AnalyzeArrayIndexExpressionIdentifier(identifierNameSyntax, (Dictionary<string, IdentifierNameSyntax>)this.firstInitializerSyntax);
                            return;
                        case SyntaxKind.MemberAccessExpression:
                            if (navTypeKind != NavTypeKind.List && navTypeKind != NavTypeKind.Record)
                                return;
                            VariableInitializationUsageAnalyzer.AnalyzeMemberAccessIdentifier(identifierNameSyntax, (Dictionary<string, IdentifierNameSyntax>)this.firstInitializerSyntax, (Dictionary<string, IdentifierNameSyntax>)this.firstLocalVarUsageSyntax, (Dictionary<string, IdentifierNameSyntax>)this.lastRecordModifyInvocationSyntax, (Dictionary<string, IdentifierNameSyntax>)this.firstRecordChangeSyntax, navTypeKind);
                            return;
                        case SyntaxKind.OptionAccessExpression:
                            return;
                        case SyntaxKind.ArgumentList:
                            VariableInitializationUsageAnalyzer.AnalyzeArgumentListIdentifier(identifierNameSyntax, context, (Dictionary<string, IdentifierNameSyntax>)this.firstInitializerSyntax, (Dictionary<string, IdentifierNameSyntax>)this.firstLocalVarUsageSyntax, (Dictionary<string, IdentifierNameSyntax>)this.firstRecordChangeSyntax, navTypeKind);
                            return;
                    }
                }
                this.firstLocalVarUsageSyntax.AddIdentifierIfNotExist(identifierNameSyntax);
            }

            public void ReportUnusedUninitializedVars(CodeBlockAnalysisContext context)
            {
                try
                {
                    PooledNameObjectDictionary<VariableDeclarationBaseSyntax> localVariables = this.localVariables;
                    if ((localVariables != null ? ((uint)localVariables.Values.Count > 0U ? 1 : 0) : 1) == 0)
                        return;
                    this.ReportInitializedNotUsedNotInitializedUsedLocalVars(context);
                }
                finally
                {
                    this.Dispose();
                }
            }

            private void ReportInitializedNotUsedNotInitializedUsedLocalVars(
              CodeBlockAnalysisContext context)
            {
                PooledHashSet<IdentifierNameSyntax> instance1 = PooledHashSet<IdentifierNameSyntax>.GetInstance();
                PooledHashSet<IdentifierNameSyntax> instance2 = PooledHashSet<IdentifierNameSyntax>.GetInstance();
                try
                {
                    this.CollectInitializedNotUsedNotInitializedUsedLocalVars((HashSet<IdentifierNameSyntax>)instance1, (HashSet<IdentifierNameSyntax>)instance2);
                    VariableInitializationUsageAnalyzer.ReportInitializedNotUsedVariables(new Action<Diagnostic>(context.ReportDiagnostic), instance1);
                    VariableInitializationUsageAnalyzer.ReportNotInitializedVariables(new Action<Diagnostic>(context.ReportDiagnostic), instance2);
                }
                finally
                {
                    instance1.Free();
                    instance2.Free();
                }
            }

            private void CollectInitializedNotUsedNotInitializedUsedLocalVars(
              HashSet<IdentifierNameSyntax> syntaxAssignedNotUsed,
              HashSet<IdentifierNameSyntax> syntaxUsedNotInitialized)
            {
                foreach (string key in this.localVariables.Keys)
                {

                    VariableDeclarationBaseSyntax declarationBaseSyntax;
                    this.localVariables.TryGetValue(key, out declarationBaseSyntax);
                    switch (declarationBaseSyntax.Type.Kind)
                    {
                        case SyntaxKind.SimpleTypeReference:
                            if (declarationBaseSyntax.Type.Array == null || !declarationBaseSyntax.Type.Array.IsKind(SyntaxKind.Array))
                                break;
                            continue;
                        case SyntaxKind.RecordTypeReference:

                            IdentifierNameSyntax identifierNameSyntax1;
                            if (this.lastRecordModifyInvocationSyntax.TryGetValue(key, out identifierNameSyntax1))
                            {
                                IdentifierNameSyntax identifierNameSyntax2;
                                if (this.firstRecordChangeSyntax.TryGetValue(key, out identifierNameSyntax2))
                                {
                                    continue;
                                }
                                continue;
                            }
                            continue;
                    }
                    NavTypeKind navTypeKind = NavTypeExtensions.GetNavTypeKind(declarationBaseSyntax.Type.DataType.TypeName.ValueText);
                    if (VariableInitializationUsageAnalyzer.navTypeKinds.Contains(navTypeKind))
                    {
                        bool flag1 = this.firstInitializerSyntax.ContainsKey(key);
                        bool flag2 = this.firstLocalVarUsageSyntax.ContainsKey(key);

                        IdentifierNameSyntax identifierNameSyntax2;
                        if (flag1 && !flag2)
                        {
                            this.firstInitializerSyntax.TryGetValue(key, out identifierNameSyntax2);
                            syntaxAssignedNotUsed.Add(identifierNameSyntax2);
                        }
                        if (navTypeKind != NavTypeKind.Boolean)
                        {
                            IdentifierNameSyntax identifierNameSyntax3;
                            if (flag2 && !flag1)
                            {
                                this.firstLocalVarUsageSyntax.TryGetValue(key, out identifierNameSyntax3);
                                syntaxUsedNotInitialized.Add(identifierNameSyntax3);
                            }
                            if (flag2 & flag1)
                            {
                                this.firstLocalVarUsageSyntax.TryGetValue(key, out identifierNameSyntax3);
                                this.firstInitializerSyntax.TryGetValue(key, out identifierNameSyntax2);
                                FileLinePositionSpan lineSpan;
                                int? nullable1;
                                if (identifierNameSyntax2 == null)
                                {
                                    nullable1 = new int?();
                                }
                                else
                                {
                                    lineSpan = identifierNameSyntax2.GetLocation().GetLineSpan();
                                    nullable1 = new int?(lineSpan.StartLinePosition.Line);
                                }
                                int? nullable2 = nullable1;
                                int? nullable3;
                                if (identifierNameSyntax3 == null)
                                {
                                    nullable3 = new int?();
                                }
                                else
                                {
                                    lineSpan = identifierNameSyntax3.GetLocation().GetLineSpan();
                                    nullable3 = new int?(lineSpan.StartLinePosition.Line);
                                }
                                int? nullable4 = nullable3;
                                if (nullable2.GetValueOrDefault() > nullable4.GetValueOrDefault() & (nullable2.HasValue & nullable4.HasValue))
                                    syntaxUsedNotInitialized.Add(identifierNameSyntax3);
                            }
                        }
                    }
                }
            }

            public void Dispose()
            {
                this.firstInitializerSyntax.Free();
                this.firstLocalVarUsageSyntax.Free();
                this.lastRecordModifyInvocationSyntax.Free();
                this.firstRecordChangeSyntax.Free();
                this.localVariables.Free();
            }
        }
    }
}
