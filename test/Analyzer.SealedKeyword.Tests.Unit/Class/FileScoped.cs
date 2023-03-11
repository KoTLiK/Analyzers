namespace Analyzer.SealedKeyword.Tests.Unit.Class;

public sealed class FileScoped : AnalyzerVerifier
{
    [Theory]
    [InlineData("Space")]
    [InlineData("Custom.Space")]
    [InlineData("Longer.Custom.Space")]
    public Task NonSealed_Then_Warning(string @namespace)
    {
        /* lang=csharp */
        var source = $$"""
            namespace {{@namespace}};
            public class TestClass {}
            """;

        var result = Diagnostic(Descriptor.SKA0001)
            .WithSpan(2, 1, 2, 26)
            .WithArguments("TestClass");

        return VerifyAnalyzerAsync(source, result);
    }

    [Theory]
    [InlineData("Space")]
    [InlineData("Custom.Space")]
    [InlineData("Longer.Custom.Space")]
    public Task Sealed_Then_Ok(string @namespace)
    {
        /* lang=csharp */
        var source = $$"""
            namespace {{@namespace}};
            public sealed class TestClass {}
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Theory]
    [InlineData("Space")]
    [InlineData("Custom.Space")]
    [InlineData("Longer.Custom.Space")]
    public Task Partial_NonSealed_Then_Warning(string @namespace)
    {
        /* lang=csharp */
        var source = $$"""
            namespace {{@namespace}};
            public partial class TestClass {}
            """;

        var result = Diagnostic(Descriptor.SKA0001)
            .WithSpan(2, 1, 2, 34)
            .WithArguments("TestClass");

        return VerifyAnalyzerAsync(source, result);
    }

    [Theory]
    [InlineData("Space")]
    [InlineData("Custom.Space")]
    [InlineData("Longer.Custom.Space")]
    public Task Partial_Sealed_Then_Ok(string @namespace)
    {
        /* lang=csharp */
        var source = $$"""
            namespace {{@namespace}};
            public sealed partial class TestClass {}
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Theory]
    [InlineData("Space")]
    [InlineData("Custom.Space")]
    [InlineData("Longer.Custom.Space")]
    public Task Static_Then_Ok(string @namespace)
    {
        /* lang=csharp */
        var source = $$"""
            namespace {{@namespace}};
            public static class TestClass {}
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Theory]
    [InlineData("Space")]
    [InlineData("Custom.Space")]
    [InlineData("Longer.Custom.Space")]
    public Task Abstract_Then_Ok(string @namespace)
    {
        /* lang=csharp */
        var source = $$"""
            namespace {{@namespace}};
            public abstract class TestClass {}
            """;

        return VerifyAnalyzerAsync(source);
    }
}