using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Analyzer.SealedKeyword.Internals;

internal static class AnalyzerExtensions
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