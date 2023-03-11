namespace Analyzer.SealedKeyword.Tests.Unit;

// TODO clean up this messy unit tests
public sealed class RecordTests : AnalyzerVerifier
{
    [Fact]
    public Task NonSealed_Then_Warning()
    {
        // Arrange
        /* lang=csharp */
        const string source = """
            namespace TestNamespace;
            public record TestRecord;
            """;

        var result = Diagnostic(Descriptor.SKA0002)
            .WithSpan(2, 1, 2, 26)
            .WithArguments("TestRecord");

        // Act + Assert
        return VerifyAnalyzerAsync(source, result);
    }

    [Fact]
    public Task NonSealedPartial_Then_Warning()
    {
        // Arrange
        /* lang=csharp */
        const string source = """
            namespace TestNamespace;
            public partial record TestRecord;
            """;

        var result = Diagnostic(Descriptor.SKA0002)
            .WithSpan(2, 1, 2, 34)
            .WithArguments("TestRecord");

        // Act + Assert
        return VerifyAnalyzerAsync(source, result);
    }

    [Fact]
    public Task NonSealed_AsBaseRecord_Then_Warning()
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
    public Task Abstract_Then_NoWarning()
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
    public Task Sealed_Then_NoWarning()
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
    public Task SealedPartial_Then_NoWarning()
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