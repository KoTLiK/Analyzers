using System.Collections.Immutable;
using Analyzer.SealedKeyword.Internals;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzer.SealedKeyword;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SealedKeywordAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(
            Descriptor.SKA0001,
            Descriptor.SKA0002,
            Descriptor.SKA0003,
            Descriptor.SKA0004);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(CompilationStartAnalysis);
    }

    private static void CompilationStartAnalysis(CompilationStartAnalysisContext context)
    {
        var walkers = context.Compilation.SyntaxTrees
            .Select(tree => new Walker().Visit(tree))
            .Select(walker => walker.CreateUsingConnections())
            .ToArray();

        var types = walkers
            .SelectMany(w => w.Types)
            .GroupBy(pair => pair.Key)
            .ToDictionary(
                group => group.Key,
                group => group.SelectMany(g => g.Value).ToHashSet());

        var baseTypes = walkers
            .SelectMany(w => w.BaseTypes)
            .GroupBy(pair => pair.Key)
            .ToDictionary(
                group => group.Key,
                group => group.Select(g => g.Value).ToHashSet().SingleOrDefault());

        context.RegisterSyntaxNodeAction(ctx => TypeDeclaration(ctx, types, baseTypes), SyntaxKind.ClassDeclaration);
        context.RegisterSyntaxNodeAction(ctx => TypeDeclaration(ctx, types, baseTypes), SyntaxKind.RecordDeclaration);
    }

    private static void TypeDeclaration(SyntaxNodeAnalysisContext ctx,
        IReadOnlyDictionary<string, HashSet<string>> types,
        IReadOnlyDictionary<string, string?> baseTypes)
    {
        var node = (TypeDeclarationSyntax)ctx.Node;
        var nodeNamespace = node.GetNamespaceName();
        var nodeName = node.Identifier.Text;

        if (!types[nodeNamespace].Contains(nodeName))
        {
            return;
        }

        if (ctx.SemanticModel.GetDeclaredSymbol(node) is not { } type)
        {
            return;
        }

        if (type.IsSealed || type.IsStatic || type.IsAbstract )
        {
            return;
        }

        var isBaseType = baseTypes.TryGetValue(nodeName, out var baseNamespace) && baseNamespace == nodeNamespace;
        var descriptor = GetDescriptor(isBaseType, type.IsRecord);
        ctx.ReportDiagnostic(Diagnostic.Create(descriptor, node.GetLocation(), nodeName));
    }

    private static DiagnosticDescriptor GetDescriptor(bool isBaseType, bool isRecord)
        => (isBaseType, isRecord) switch
        {
            (true, true) => Descriptor.SKA0004,
            (true, false) => Descriptor.SKA0003,
            (false, true) => Descriptor.SKA0002,
            (false, false) => Descriptor.SKA0001,
        };
}