using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzer.SealedClass.Tests.Unit;

public static class Verifier
{
    public static async Task VerifyDiagnostics<TAnalyzer>(string source, params Diagnostic[] expectedDiagnostics)
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        var analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new TAnalyzer());
        var tree = CSharpSyntaxTree.ParseText(source);
        var compilation = CSharpCompilation
            .Create("Test", syntaxTrees: new[] { tree })
            .WithAnalyzers(analyzers);

        var actualDiagnostics = await compilation.GetAnalyzerDiagnosticsAsync();

        using (new AssertionScope())
        {
            actualDiagnostics.Length.Should().Be(expectedDiagnostics.Length);

            for (var i = 0; i < expectedDiagnostics.Length; i++)
            {
                var expected = expectedDiagnostics[i];
                var actual = actualDiagnostics[i];

                actual.Id.Should().BeEquivalentTo(expected.Id);
                actual.GetMessage().Should().BeEquivalentTo(expected.GetMessage());
            }
        }
    }
}