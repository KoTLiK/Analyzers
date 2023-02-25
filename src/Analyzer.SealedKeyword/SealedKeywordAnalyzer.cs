using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

[assembly: InternalsVisibleTo("Analyzer.SealedKeyword.Tests.Unit")]

namespace Analyzer.SealedKeyword;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class SealedKeywordAnalyzer : DiagnosticAnalyzer
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
            _ = syntaxTree;
        }

        context.RegisterSyntaxNodeAction(
            ctx => AnalyzeClassDeclaration<ClassDeclarationSyntax>(ctx, Descriptor.SKA0001),
            SyntaxKind.ClassDeclaration);
        context.RegisterSyntaxNodeAction(
            ctx => AnalyzeClassDeclaration<RecordDeclarationSyntax>(ctx, Descriptor.SKA0002),
            SyntaxKind.RecordDeclaration);
    }

    private static void AnalyzeClassDeclaration<TSyntax>(SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor)
        where TSyntax : TypeDeclarationSyntax
    {
        var classDeclaration = (TSyntax)context.Node;
        // var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration)!;
        // var baseList = (classDeclaration.BaseList?.Types ?? new SeparatedSyntaxList<BaseTypeSyntax>()).ToArray();

        foreach (var syntaxToken in classDeclaration.Modifiers)
        {
            var kind = syntaxToken.Kind();
            if (kind is SyntaxKind.AbstractKeyword
                or SyntaxKind.SealedKeyword
                or SyntaxKind.StaticKeyword)
            {
                return;
            }
        }

        var diagnostic = Diagnostic.Create(descriptor, classDeclaration.GetLocation(), classDeclaration.Identifier.Text);
        context.ReportDiagnostic(diagnostic);
    }
}