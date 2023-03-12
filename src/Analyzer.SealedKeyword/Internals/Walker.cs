using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Analyzer.SealedKeyword.Internals;

internal sealed class Walker : CSharpSyntaxWalker
{
    // <Namespace, List<Types>>
    public Dictionary<string, List<string>> Types { get; } = new();

    // <(Type|TypeAlias), (Namespace|Alias|null)> - null => The same namespace as visited syntax tree or top-level
    public Dictionary<string, string?> BaseTypes { get; } = new();

    // <Using, (Alias|null)>
    public Dictionary<string, string?> UsingDirectives { get; } = new();

    /*
     * Pre kazdy using z walkera, najdi namespace zo VSETKYCH typov a spojim si base type zo vsetkych typov.
     * Rozbije to top-level? Pre walkera upravim vytahovanie namespace?
     */
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

    public override void VisitRecordDeclaration(RecordDeclarationSyntax node)
    {
        AddVisitedNodeType(node);
        TryAddBaseType(node);
    }

    public override void VisitUsingDirective(UsingDirectiveSyntax node)
    {
        // using Custom.Namespace;
        // using Alias = Custom.Namespace;
        // using TypeAlias = Custom.Namespace.Type;
        // TODO may work only for first type so far
        var identifier = node.Name.GetIdentifier().Join(".");

        ref var valueOrAdd = ref CollectionsMarshal.GetValueRefOrAddDefault(UsingDirectives, identifier, out var exists);
        if (exists)
        {
            return;
        }

        valueOrAdd = node.Alias?.Name.Identifier.Text;
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

    private void TryAddBaseType(TypeDeclarationSyntax node)
    {
        if (node.BaseList?.Types.FirstOrDefault()?.Type is not { } baseType)
        {
            return;
        }

        var identifiers = baseType.GetIdentifier().ToArray();
        var identifier = identifiers.Last();

        ref var valueOrAdd = ref CollectionsMarshal.GetValueRefOrAddDefault(BaseTypes, identifier, out var exists);
        if (exists || identifiers.Length == 1)
        {
            return;
        }

        // length 1 => name of the Type, cannot use as fully qualified namespace to the type
        valueOrAdd = identifiers[..^1].Join(".");
    }
}