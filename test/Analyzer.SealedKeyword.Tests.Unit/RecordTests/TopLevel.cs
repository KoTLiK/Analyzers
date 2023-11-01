namespace Analyzer.SealedKeyword.Tests.Unit.RecordTests;

public sealed class TopLevel : AnalyzerVerifier
{
    [Fact]
    public Task NonSealed_Then_Warning()
    {
        /* lang=csharp */
        const string source = """
            public record Subject {}
            """;

        var result = Diagnostic(Descriptor.SKA0002)
            .WithSpan(1, 1, 1, 25)
            .WithArguments("Subject");

        return VerifyAnalyzerAsync(source, result);
    }

    [Fact]
    public Task Sealed_Then_Ok()
    {
        /* lang=csharp */
        const string source = """
            public sealed record Subject {}
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task Partial_NonSealed_Then_Ok()
    {
        /* lang=csharp */
        const string source = """
            public partial record Subject {}
            """;

        var result = Diagnostic(Descriptor.SKA0002)
            .WithSpan(1, 1, 1, 33)
            .WithArguments("Subject");

        return VerifyAnalyzerAsync(source, result);
    }

    [Fact]
    public Task Partial_Sealed_Then_Ok()
    {
        /* lang=csharp */
        const string source = """
            public sealed partial record Subject {}
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task Abstract_Then_Ok()
    {
        /* lang=csharp */
        const string source = """
            public abstract record Subject {}
            """;

        return VerifyAnalyzerAsync(source);
    }
}