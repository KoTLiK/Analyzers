using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Analyzer.SealedKeyword.Internals;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

[assembly: InternalsVisibleTo("Analyzer.SealedKeyword.Tests.Unit")]

namespace Analyzer.SealedKeyword;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SealedKeywordAnalyzer : DiagnosticAnalyzer
{
    private Dictionary<string, Value> Types { get; } = new();

    private readonly record struct Value(
        INamedTypeSymbol Type,
        ICollection<TypeDeclarationSyntax> Declarations,
        bool Reported = false);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(Descriptor.SKA0001, Descriptor.SKA0002);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(CompilationStartAnalysis);
    }

    private void CompilationStartAnalysis(CompilationStartAnalysisContext context)
    {
        var walker = new Walker();
        foreach (var syntaxTree in context.Compilation.SyntaxTrees)
        {
            // Toto bude asi najlepsia moznost.
            // Len musim vymysliet ako ziskam namespace a nazov
            // TypeDeclarationSyntax a BaseTypeDeclarationSyntax
            walker.Visit(syntaxTree.GetRoot());
        }

        // This 'SymbolAnalysis' should go away and 'AnalyzeSymbols' could stay (renamed and bit refactored)
        context.RegisterSymbolAction(SymbolAnalysis, SymbolKind.NamedType);
        context.RegisterCompilationEndAction(AnalyzeSymbols);
    }

    private sealed class Walker : CSharpSyntaxWalker
    {
        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var baseTypeSyntax = node.BaseList?.Types.FirstOrDefault()!;
            var metadata = baseTypeSyntax?.GetLocation().MetadataModule;

            // TODO Find namespace via parent recursively, but reuse the code if possible
            var @namespace = ((node.Parent as FileScopedNamespaceDeclarationSyntax)?.Name as IdentifierNameSyntax)?.Identifier.Text;
            var typeName = node.Identifier.Text;
            base.VisitClassDeclaration(node);
        }

        public override void VisitRecordDeclaration(RecordDeclarationSyntax node)
        {
            base.VisitRecordDeclaration(node);
        }
    }
    private void SymbolAnalysis(SymbolAnalysisContext context)
    {
        var type = (INamedTypeSymbol)context.Symbol;
        if (type.TypeKind is not TypeKind.Class)
        {
            return;
        }

        if (type.IsAbstract || type.IsStatic || type.IsSealed)
        {
            return;
        }

        var key = $"{type.ContainingNamespace}.{type.Name}";
        ref var valueOrNew = ref CollectionsMarshal
            .GetValueRefOrAddDefault(Types, key, out var typeExists);

        if (!typeExists)
        {
            var declarations = type.DeclaringSyntaxReferences
                .Select(r => (TypeDeclarationSyntax)r.GetSyntax())
                .ToArray();
            valueOrNew = new Value(type, declarations);
        }

        if (type.BaseType is null || type.BaseType.Name == "Object")
        {
            return;
        }

        if (Types.ContainsKey(key))
        {
            Types.Remove(key);
        }
    }

    private void AnalyzeSymbols(CompilationAnalysisContext context)
    {
        foreach (var (key, value) in Types)
        {
            var (type, declarations, reported) = value;
            if (reported)
            {
                continue;
            }

            ref var valueOrNew = ref CollectionsMarshal.GetValueRefOrAddDefault(Types, key, out _);
            valueOrNew = value with { Reported = true };

            Report(context, type, declarations);
        }
    }

    private static void Report(
        CompilationAnalysisContext context,
        INamedTypeSymbol type,
        ICollection<TypeDeclarationSyntax> declarations)
    {
        var descriptor = type.IsRecord ? Descriptor.SKA0002 : Descriptor.SKA0001;

        var diagnostics = declarations
            .Select(declaration => Diagnostic.Create(descriptor, declaration.GetLocation(), type.Name));
        foreach (var diagnostic in diagnostics)
        {
            context.ReportDiagnostic(diagnostic);
        }
    }
}