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
        => ImmutableArray.Create(Descriptor.SCA0001);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // context.RegisterCompilationStartAction(); // Ulozit si dictionary vsetkych typov a postupne ich odstranit ak najdem nejake derived typy a potom reportovat diagnostiku
        context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
    }

    private static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;
        var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration)!;
        var baseList = (classDeclaration.BaseList?.Types ?? new SeparatedSyntaxList<BaseTypeSyntax>()).ToArray();

        foreach (var syntaxToken in classDeclaration.Modifiers)
        {
            var kind = syntaxToken.Kind();
            switch (kind)
            {
                case SyntaxKind.AbstractKeyword:
                case SyntaxKind.SealedKeyword:
                case SyntaxKind.StaticKeyword:
                    return;
            }
        }

        var diagnostic = Diagnostic.Create(Descriptor.SCA0001, classDeclaration.GetLocation(), classDeclaration.Identifier.Text);
        context.ReportDiagnostic(diagnostic);
    }
}