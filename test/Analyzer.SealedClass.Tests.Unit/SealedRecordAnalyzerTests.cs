using Microsoft.CodeAnalysis.CSharp.Testing.XUnit;
using Microsoft.CodeAnalysis.Testing;

namespace Analyzer.SealedClass.Tests.Unit;

public sealed class SealedRecordAnalyzerTests : AnalyzerVerifier<SealedClassAnalyzer>
{
    [Fact]
    public Task When_NonSealed_Then_Warning()
    {
        // Arrange
        /* lang=csharp */
        const string source = """
            namespace TestNamespace;
            public record TestRecord;
            """;

        var result = Diagnostic(Descriptor.SCA0002)
            .WithSpan(2, 1, 2, 26)
            .WithArguments("TestRecord");

        // Act + Assert
        return VerifyAnalyzerAsync(source, result);
    }

    [Fact]
    public Task When_NonSealedPartial_Then_Warning()
    {
        // Arrange
        /* lang=csharp */
        const string source = """
            namespace TestNamespace;
            public partial record TestRecord;
            """;

        var result = Diagnostic(Descriptor.SCA0002)
            .WithSpan(2, 1, 2, 34)
            .WithArguments("TestRecord");

        // Act + Assert
        return VerifyAnalyzerAsync(source, result);
    }

    [Fact]
    public Task When_NonSealed_AsBaseRecord_Then_Warning()
    {
        // Arrange
        /* lang=csharp */
        const string source = """
            namespace TestNamespace;
            public sealed record SealedRecord : TestRecord;
            public record TestRecord;
            """;

        // Act + Assert
        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task When_Abstract_Then_NoWarning()
    {
        // Arrange
        /* lang=csharp */
        const string source = """
            namespace TestNamespace;
            public abstract record TestRecord;
            """;

        // Act + Assert
        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task When_Sealed_Then_NoWarning()
    {
        // Arrange
        /* lang=csharp */
        const string source = """
            namespace TestNamespace;
            public sealed record TestRecord;
            """;

        // Act + Assert
        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task When_SealedPartial_Then_NoWarning()
    {
        // Arrange
        /* lang=csharp */
        const string source = """
            namespace TestNamespace;
            public sealed partial record TestRecord;
            """;

        // Act + Assert
        return VerifyAnalyzerAsync(source);
    }
}