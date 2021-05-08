using ALCodeAnalysis.Utilities;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ALCodeAnalysis.Reliability
{
    [DiagnosticAnalyzer]
    public class Rule198GlobalLocalVariablesShouldNotHaveSameName : DiagnosticAnalyzer
    {
        private static readonly SymbolKind[] SymbolKinds = new SymbolKind[8]
        {
      SymbolKind.Codeunit,
      SymbolKind.Page,
      SymbolKind.Report,
      SymbolKind.Table,
      SymbolKind.Query,
      SymbolKind.XmlPort,
      SymbolKind.PageExtension,
      SymbolKind.TableExtension
        };

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule022GlobalLocalVariablesShouldNotHaveSameName, DiagnosticDescriptors.Rule023DoNotNameLocalVarAsFieldOrMethod, DiagnosticDescriptors.Rule024DoNotNameMethodAsField, DiagnosticDescriptors.Rule025DoNotNameGlobalVarAsFieldOrMethod);

        public override void Initialize(AnalysisContext context) => context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(Rule198GlobalLocalVariablesShouldNotHaveSameName.AnalyzeSymbols), Rule198GlobalLocalVariablesShouldNotHaveSameName.SymbolKinds);

        private static void AnalyzeSymbols(SymbolAnalysisContext ctx)
        {
            PooledHashSet<ISymbol> instance1 = PooledHashSet<ISymbol>.GetInstance();
            PooledHashSet<ISymbol> instance2 = PooledHashSet<ISymbol>.GetInstance();
            PooledHashSet<ISymbol> instance3 = PooledHashSet<ISymbol>.GetInstance();
            PooledHashSet<ISymbol> instance4 = PooledHashSet<ISymbol>.GetInstance();
            PooledHashSet<ISymbol> instance5 = PooledHashSet<ISymbol>.GetInstance();
            PooledHashSet<ISymbol> instance6 = PooledHashSet<ISymbol>.GetInstance();
            ImmutableArray<ISymbol> members = ((IContainerSymbol)ctx.Symbol).GetMembers();
            try
            {
                ImmutableArray<ISymbol>.Enumerator enumerator1 = members.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    ISymbol current1 = enumerator1.Current;
                    switch (current1.Kind)
                    {
                        case SymbolKind.GlobalVariable:
                            instance1.ConcurrentAdd<ISymbol>(current1);
                            continue;
                        case SymbolKind.Method:
                            instance2.ConcurrentAdd<ISymbol>(current1);
                            foreach (IVariableSymbol localVariable in ((IMethodSymbol)current1).LocalVariables)
                                instance3.ConcurrentAdd<ISymbol>((ISymbol)localVariable);
                            continue;
                        case SymbolKind.Field:
                            instance4.ConcurrentAdd<ISymbol>(current1);
                            continue;
                        case SymbolKind.Control:
                            Rule198GlobalLocalVariablesShouldNotHaveSameName.AnalyzeControls((IControlSymbol)current1, instance6);
                            continue;
                        case SymbolKind.Action:
                            Rule198GlobalLocalVariablesShouldNotHaveSameName.AnalyzeActions((IActionSymbol)current1, instance5);
                            continue;
                        case SymbolKind.Change:
                            IEnumerable<IControlSymbol> flattenedSymbolsOfKind1 = Rule198GlobalLocalVariablesShouldNotHaveSameName.GetFlattenedSymbolsOfKind<IControlSymbol>((IContainerSymbol)current1, SymbolKind.Control);
                            IEnumerable<IActionSymbol> flattenedSymbolsOfKind2 = Rule198GlobalLocalVariablesShouldNotHaveSameName.GetFlattenedSymbolsOfKind<IActionSymbol>((IContainerSymbol)current1, SymbolKind.Action);
                            foreach (IControlSymbol controlSymbol in flattenedSymbolsOfKind1)
                                instance6.ConcurrentAdd<ISymbol>((ISymbol)controlSymbol);
                            using (IEnumerator<IActionSymbol> enumerator2 = flattenedSymbolsOfKind2.GetEnumerator())
                            {
                                while (enumerator2.MoveNext())
                                {
                                    IActionSymbol current2 = enumerator2.Current;
                                    instance5.ConcurrentAdd<ISymbol>((ISymbol)current2);
                                }
                                continue;
                            }
                        case SymbolKind.QueryDataItem:
                            foreach (IQueryColumnSymbol flattenedQueryColumn in ((IQueryDataItemSymbol)current1).FlattenedQueryColumns)
                            {
                                if (flattenedQueryColumn.Kind == SymbolKind.QueryColumn)
                                    instance4.ConcurrentAdd<ISymbol>((ISymbol)flattenedQueryColumn);
                            }
                            continue;
                        case SymbolKind.RequestPage:
                            foreach (IControlSymbol flattenedControl in ((IPageBaseTypeSymbol)current1).FlattenedControls)
                                Rule198GlobalLocalVariablesShouldNotHaveSameName.AnalyzeControls(flattenedControl, instance4);
                            foreach (IActionSymbol flattenedAction in ((IPageBaseTypeSymbol)current1).FlattenedActions)
                                instance5.ConcurrentAdd<ISymbol>((ISymbol)flattenedAction);
                            continue;
                        case SymbolKind.XmlPortNode:
                            Rule198GlobalLocalVariablesShouldNotHaveSameName.AnalyzeXmlNode((IXmlPortNodeSymbol)current1, instance4);
                            continue;
                        default:
                            continue;
                    }
                }
                ImmutableHashSet<ISymbol> immutableHashSet1 = instance1.ToImmutableHashSet<ISymbol>((IEqualityComparer<ISymbol>)SemanticFacts.SymbolNameEqualityComparer);
                ImmutableHashSet<ISymbol> immutableHashSet2 = instance2.ToImmutableHashSet<ISymbol>((IEqualityComparer<ISymbol>)SemanticFacts.SymbolNameEqualityComparer);
                ImmutableHashSet<ISymbol> immutableHashSet3 = instance3.ToImmutableHashSet<ISymbol>();
                ImmutableHashSet<ISymbol> immutableHashSet4 = instance4.ToImmutableHashSet<ISymbol>((IEqualityComparer<ISymbol>)SemanticFacts.SymbolNameEqualityComparer);
                ImmutableHashSet<ISymbol> immutableHashSet5 = instance5.ToImmutableHashSet<ISymbol>((IEqualityComparer<ISymbol>)SemanticFacts.SymbolNameEqualityComparer);
                ImmutableHashSet<ISymbol> immutableHashSet6 = instance6.ToImmutableHashSet<ISymbol>((IEqualityComparer<ISymbol>)SemanticFacts.SymbolNameEqualityComparer);
                foreach (ISymbol symbol in immutableHashSet3)
                {
                    if (immutableHashSet1.Contains(symbol))
                        Rule198GlobalLocalVariablesShouldNotHaveSameName.ReportDiagnostic(ctx, DiagnosticDescriptors.Rule022GlobalLocalVariablesShouldNotHaveSameName, symbol);
                    if (immutableHashSet2.Contains(symbol) || immutableHashSet4.Contains(symbol) || (immutableHashSet5.Contains(symbol) || immutableHashSet6.Contains(symbol)))
                        Rule198GlobalLocalVariablesShouldNotHaveSameName.ReportDiagnostic(ctx, DiagnosticDescriptors.Rule023DoNotNameLocalVarAsFieldOrMethod, symbol);
                }
                foreach (ISymbol symbol in immutableHashSet2)
                {
                    if (immutableHashSet4.Contains(symbol) || immutableHashSet5.Contains(symbol) || immutableHashSet6.Contains(symbol))
                        Rule198GlobalLocalVariablesShouldNotHaveSameName.ReportDiagnostic(ctx, DiagnosticDescriptors.Rule024DoNotNameMethodAsField, symbol);
                }
                foreach (ISymbol symbol in immutableHashSet1)
                {
                    if (immutableHashSet4.Contains(symbol) || immutableHashSet2.Contains(symbol) || immutableHashSet5.Contains(symbol))
                    { 
                        Rule198GlobalLocalVariablesShouldNotHaveSameName.ReportDiagnostic(ctx, DiagnosticDescriptors.Rule025DoNotNameGlobalVarAsFieldOrMethod, symbol);
                    }
                    else
                    {
                        ISymbol actualValue;
                        if (immutableHashSet6.Contains(symbol) && immutableHashSet6.TryGetValue(symbol, out actualValue) && actualValue.Kind != SymbolKind.Control)
                            Rule198GlobalLocalVariablesShouldNotHaveSameName.ReportDiagnostic(ctx, DiagnosticDescriptors.Rule025DoNotNameGlobalVarAsFieldOrMethod, symbol);
                    }
                }
            }
            finally
            {
                instance1.Free();
                instance2.Free();
                instance3.Free();
                instance4.Free();
                instance5.Free();
                instance6.Free();
            }
        }

        private static void AnalyzeXmlNode(
          IXmlPortNodeSymbol symbol,
          PooledHashSet<ISymbol> fieldPooledHashSet)
        {
            foreach (IXmlPortNodeSymbol flattenedNode in symbol.FlattenedNodes)
            {
                if (flattenedNode.SourceTypeKind == XmlPortSourceTypeKind.Field)
                    fieldPooledHashSet.ConcurrentAdd<ISymbol>((ISymbol)flattenedNode);
                else
                    Rule198GlobalLocalVariablesShouldNotHaveSameName.AnalyzeXmlNode(flattenedNode, fieldPooledHashSet);
            }
        }

        private static void AnalyzeControls(
          IControlSymbol controlSymbol,
          PooledHashSet<ISymbol> controlPooledHashSet)
        {
            foreach (IControlSymbol control in controlSymbol.Controls)
            {
                if (control.Controls.Length == 0)
                {
                    controlPooledHashSet.ConcurrentAdd<ISymbol>((ISymbol)control);
                }
                else
                {
                    Rule198GlobalLocalVariablesShouldNotHaveSameName.AnalyzeControls(control, controlPooledHashSet);
                    controlPooledHashSet.ConcurrentAdd<ISymbol>((ISymbol)control);
                }
            }
        }

        private static void AnalyzeActions(
          IActionSymbol actionSymbol,
          PooledHashSet<ISymbol> actionPooledHashSet)
        {
            foreach (IActionSymbol action in actionSymbol.Actions)
            {
                if (action.Actions.Length == 0)
                {
                    actionPooledHashSet.ConcurrentAdd<ISymbol>((ISymbol)action);
                }
                else
                {
                    Rule198GlobalLocalVariablesShouldNotHaveSameName.AnalyzeActions(action, actionPooledHashSet);
                    actionPooledHashSet.ConcurrentAdd<ISymbol>((ISymbol)action);
                }
            }
        }

        private static void ReportDiagnostic(
          SymbolAnalysisContext ctx,
          DiagnosticDescriptor diagnosticDescriptor,
          ISymbol symbol)
        {
            ctx.ReportDiagnostic(Diagnostic.Create(diagnosticDescriptor, symbol.GetLocation(), (object)symbol.Name, (object)symbol.GetContainingApplicationObjectTypeSymbol().Name));
        }

        private static IEnumerable<T> GetFlattenedSymbolsOfKind<T>(
          IContainerSymbol parent,
          SymbolKind kind)
        {
            ImmutableArray<ISymbol> members = parent.GetMembers();
            List<T> objList = new List<T>();
            Stack<ISymbol> symbolStack = new Stack<ISymbol>();
            foreach (ISymbol symbol1 in members)
            {
                symbolStack.Push(symbol1);
                while (symbolStack.Count > 0)
                {
                    ISymbol symbol2 = symbolStack.Pop();
                    if (symbol2.Kind == kind)
                    {
                        objList.Add((T)symbol2);
                        foreach (ISymbol member in ((IContainerSymbol)symbol2).GetMembers())
                            symbolStack.Push(member);
                    }
                }
            }
            return (IEnumerable<T>)objList;
        }
    }
}
