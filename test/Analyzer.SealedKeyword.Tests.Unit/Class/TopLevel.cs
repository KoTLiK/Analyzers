namespace Analyzer.SealedKeyword.Tests.Unit.Class;

public class TopLevel : AnalyzerVerifier
{
    [Fact]
    public Task NonSealed_Then_Warning()
    {
        /* lang=csharp */
        const string source = """
            public class TestClass {}
            """;

        var result = Diagnostic(Descriptor.SKA0001)
            .WithSpan(1, 1, 1, 26)
            .WithArguments("TestClass");

        return VerifyAnalyzerAsync(source, result);
    }

    [Fact]
    public Task Sealed_Then_Ok()
    {
        /* lang=csharp */
        const string source = """
            public sealed class TestClass {}
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task Partial_NonSealed_Then_Ok()
    {
        /* lang=csharp */
        const string source = """
            public partial class TestClass {}
            """;

        var result = Diagnostic(Descriptor.SKA0001)
            .WithSpan(1, 1, 1, 34)
            .WithArguments("TestClass");

        return VerifyAnalyzerAsync(source, result);
    }

    [Fact]
    public Task Partial_Sealed_Then_Ok()
    {
        /* lang=csharp */
        const string source = """
            public sealed partial class TestClass {}
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task Static_Then_Ok()
    {
        /* lang=csharp */
        const string source = """
            public static class TestClass {}
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task Abstract_Then_Ok()
    {
        /* lang=csharp */
        const string source = """
            public abstract class TestClass {}
            """;

        return VerifyAnalyzerAsync(source);
    }
}