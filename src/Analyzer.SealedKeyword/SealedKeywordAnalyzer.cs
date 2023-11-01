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
            Descriptor.SKA0002);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(CompilationStartAnalysis);
    }

    private static void CompilationStartAnalysis(CompilationStartAnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(ctx => TypeDeclaration(ctx, false), SyntaxKind.ClassDeclaration);
        context.RegisterSyntaxNodeAction(ctx => TypeDeclaration(ctx, true), SyntaxKind.RecordDeclaration);
    }

    private static void TypeDeclaration(SyntaxNodeAnalysisContext ctx, bool isRecord)
    {
        var node = (TypeDeclarationSyntax)ctx.Node;
        var nodeName = node.Identifier.Text;

        if (ModelExtensions.GetDeclaredSymbol(ctx.SemanticModel, node) is not { } type)
        {
            return;
        }

        if (type.IsSealed || type.IsStatic || type.IsAbstract )
        {
            return;
        }

        var descriptor = isRecord ? Descriptor.SKA0002 : Descriptor.SKA0001;
        var diagnostic = Diagnostic.Create(descriptor, node.GetLocation(), nodeName);
        ctx.ReportDiagnostic(diagnostic);
    }
}
