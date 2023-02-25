using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

[assembly: InternalsVisibleTo("Analyzer.SealedClass.Tests.Unit")]

namespace Analyzer.SealedClass;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class SealedClassAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(Descriptor.SCA0001, Descriptor.SCA0002);

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
            ctx => AnalyzeClassDeclaration<ClassDeclarationSyntax>(ctx, Descriptor.SCA0001),
            SyntaxKind.ClassDeclaration);
        context.RegisterSyntaxNodeAction(
            ctx => AnalyzeClassDeclaration<RecordDeclarationSyntax>(ctx, Descriptor.SCA0002),
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