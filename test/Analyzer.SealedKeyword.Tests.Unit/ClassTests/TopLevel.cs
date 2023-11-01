namespace Analyzer.SealedKeyword.Tests.Unit.ClassTests;

public sealed class TopLevel : AnalyzerVerifier
{
    [Fact]
    public Task NonSealed_Then_Warning()
    {
        /* lang=csharp */
        const string source = """
            public class Subject {}
            """;

        var result = Diagnostic(Descriptor.SKA0001)
            .WithSpan(1, 1, 1, 24)
            .WithArguments("Subject");

        return VerifyAnalyzerAsync(source, result);
    }

    [Fact]
    public Task Sealed_Then_Ok()
    {
        /* lang=csharp */
        const string source = """
            public sealed class Subject {}
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task Partial_NonSealed_Then_Ok()
    {
        /* lang=csharp */
        const string source = """
            public partial class Subject {}
            """;

        var result = Diagnostic(Descriptor.SKA0001)
            .WithSpan(1, 1, 1, 32)
            .WithArguments("Subject");

        return VerifyAnalyzerAsync(source, result);
    }

    [Fact]
    public Task Partial_Sealed_Then_Ok()
    {
        /* lang=csharp */
        const string source = """
            public sealed partial class Subject {}
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task Static_Then_Ok()
    {
        /* lang=csharp */
        const string source = """
            public static class Subject {}
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task Abstract_Then_Ok()
    {
        /* lang=csharp */
        const string source = """
            public abstract class Subject {}
            """;

        return VerifyAnalyzerAsync(source);
    }
}