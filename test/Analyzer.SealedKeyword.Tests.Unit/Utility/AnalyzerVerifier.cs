using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.CSharp.Testing.XUnit;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Analyzer.SealedKeyword.Tests.Unit.Utility;

public abstract class AnalyzerVerifier : AnalyzerVerifier<SealedKeywordAnalyzer>
{
    protected static Task VerifyAnalyzerAsync(IEnumerable<string> sources, params DiagnosticResult[] expected)
    {
        var test = new CSharpAnalyzerTest<SealedKeywordAnalyzer, XUnitVerifier>();
        foreach (var source in sources)
        {
            test.TestCode = source;
        }

        test.ExpectedDiagnostics.AddRange(expected);
        return test.RunAsync(CancellationToken.None);
    }
}