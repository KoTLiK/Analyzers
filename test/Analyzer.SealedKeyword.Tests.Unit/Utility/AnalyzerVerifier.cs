using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.CSharp.Testing.XUnit;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Analyzer.SealedKeyword.Tests.Unit.Utility;

public class AnalyzerVerifier : AnalyzerVerifier<SealedKeywordAnalyzer>
{
    public static Task VerifyAnalyzerAsync(IEnumerable<string> sources, IEnumerable<DiagnosticResult> expected)
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