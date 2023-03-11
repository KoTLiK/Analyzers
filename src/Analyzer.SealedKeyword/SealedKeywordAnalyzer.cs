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

        var type = ctx.SemanticModel.GetDeclaredSymbol(node);
        if (type is null)
        {
            return;
        }

        if (type.IsSealed || type.IsAbstract || type.IsStatic)
        {
            return;
        }

        if (baseTypes.TryGetValue(nodeName, out var baseNamespace)
            && baseNamespace == nodeNamespace)
        {
            var infoDescriptor = type.IsRecord ? Descriptor.SKA0004 : Descriptor.SKA0003;
            var infoDiagnostic = Diagnostic.Create(infoDescriptor, node.GetLocation(), nodeName);
            ctx.ReportDiagnostic(infoDiagnostic);
            return;
        }

        var warnDescriptor = type.IsRecord ? Descriptor.SKA0002 : Descriptor.SKA0001;
        var warnDiagnostic = Diagnostic.Create(warnDescriptor, node.GetLocation(), nodeName);
        ctx.ReportDiagnostic(warnDiagnostic);
    }
}

public sealed class Walker : CSharpSyntaxWalker
{
    // <Namespace, List<Types>>
    public Dictionary<string, List<string>> Types { get; } = new();

    // <Type, (Namespace|Using|...)>
    public Dictionary<string, string> BaseTypes { get; } = new();

    public Walker Visit(SyntaxTree syntaxTree)
    {
        Visit(syntaxTree.GetRoot());
        return this;
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        AddVisitedNodeType(node);
        TryAddBaseType(node);
    }

    private void TryAddBaseType(TypeDeclarationSyntax node)
    {
        if (node.BaseList?.Types.FirstOrDefault()?.Type is not { } baseType)
        {
            return;
        }

        var identifiers = baseType.GetIdentifier().ToArray();
        var identifier = identifiers.Last();

        ref var valueOrAdd = ref CollectionsMarshal.GetValueRefOrAddDefault(BaseTypes, identifier, out var exists);
        if (exists)
        {
            return;
        }

        if (identifiers.Length == 1)
        {
            valueOrAdd = baseType.GetNamespaceName();
            return;
        }

        valueOrAdd = identifiers[..^1].Join(".");
    }

    private void AddVisitedNodeType(TypeDeclarationSyntax node)
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
        return @namespace?.Name.GetIdentifier().Join(".") ?? TopLevelNamespaceWildcard;
    }

    public static IEnumerable<string> GetIdentifier(this TypeSyntax? typeSyntax)
    {
        static IEnumerable<string> SingleIdentifier(SimpleNameSyntax identifier)
        {
            yield return identifier.Identifier.Text;
        }

        return typeSyntax switch
        {
            IdentifierNameSyntax identifier => SingleIdentifier(identifier),
            QualifiedNameSyntax qualifier => qualifier.Left.GetIdentifier().Concat(qualifier.Right.GetIdentifier()),
            _ => Enumerable.Empty<string>()
        };
    }

    public static string Join(this IEnumerable<string> source, string separator)
        => string.Join(separator, source);
}