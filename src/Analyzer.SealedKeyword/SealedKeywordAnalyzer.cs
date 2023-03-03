using System.Collections.Immutable;
using System.Runtime.InteropServices;
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
        => ImmutableArray.Create(Descriptor.SKA0001, Descriptor.SKA0002);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(CompilationStartAnalysis);
    }

    private static void CompilationStartAnalysis(CompilationStartAnalysisContext context)
    {
        foreach (var syntaxTree in context.Compilation.SyntaxTrees)
        {
            new Walker().Visit(syntaxTree);
        }

        var types = context.Compilation.SyntaxTrees
            .SelectMany(tree => new Walker().Visit(tree).Types)
            .GroupBy(pair => pair.Key)
            .ToDictionary(
                group => group.Key,
                group => group.SelectMany(g => g.Value).ToHashSet());

        context.RegisterSyntaxNodeAction(ctx => TypeDeclaration(ctx, types), SyntaxKind.ClassDeclaration);
    }

    private static void TypeDeclaration(SyntaxNodeAnalysisContext ctx, IReadOnlyDictionary<string, HashSet<string>> dictionary)
    {
        var node = (TypeDeclarationSyntax)ctx.Node;
        var nodeNamespace = node.GetNamespaceName();
        var nodeName = node.Identifier.Text;

        if (!dictionary[nodeNamespace].Contains(nodeName))
        {
            return;
        }

        var type = ctx.SemanticModel.GetDeclaredSymbol(node);
        if (type is null)
        {
            return;
        }

        if (type.IsSealed || type.IsAbstract || type.IsStatic)
        {
            return;
        }

        var descriptor = type.IsRecord ? Descriptor.SKA0002 : Descriptor.SKA0001;
        var diagnostic = Diagnostic.Create(descriptor, node.GetLocation(), nodeName);
        ctx.ReportDiagnostic(diagnostic);
    }
}

public sealed class Walker : CSharpSyntaxWalker
{
    public Dictionary<string, List<string>> Types { get; } = new();

    public Walker Visit(SyntaxTree syntaxTree)
    {
        Visit(syntaxTree.GetRoot());
        return this;
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        var nodeNamespace = node.GetNamespaceName();
        var nodeName = node.Identifier.Text;

        ref var valueOrAdd = ref CollectionsMarshal.GetValueRefOrAddDefault(Types, nodeNamespace, out var exists);
        if (exists)
        {
            valueOrAdd!.Add(nodeName);
            return;
        }

        valueOrAdd = new List<string> { nodeName };
    }
}

public static class AnalyzerExtensions
{
    public const string TopLevelNamespaceWildcard = "<top-level-namespace>";

    public static string GetNamespaceName(this SyntaxNode typeDeclaration)
    {
        var @namespace = typeDeclaration.FirstAncestorOrSelf<BaseNamespaceDeclarationSyntax>();
        return ((IdentifierNameSyntax?)@namespace?.Name)?.Identifier.Text
               ?? TopLevelNamespaceWildcard;
    }
}