namespace Analyzer.SealedKeyword.Tests.Unit.RecordTests;

public sealed class BlockScoped : AnalyzerVerifier
{
    [Theory]
    [InlineData("Space")]
    [InlineData("Custom.Space")]
    [InlineData("Longer.Custom.Space")]
    public Task NonSealed_Then_Warning(string @namespace)
    {
        /* lang=csharp */
        var source = $$"""
            namespace {{@namespace}} {
                public record Subject {}
            }
            """;

        var result = Diagnostic(Descriptor.SKA0002)
            .WithSpan(2, 5, 2, 29)
            .WithArguments("Subject");

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
            namespace {{@namespace}} {
                public sealed record Subject {}
            }
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
            namespace {{@namespace}} {
                public partial record Subject {}
            }
            """;

        var result = Diagnostic(Descriptor.SKA0002)
            .WithSpan(2, 5, 2, 37)
            .WithArguments("Subject");

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
            namespace {{@namespace}} {
                public sealed partial record Subject {}
            }
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
            namespace {{@namespace}} {
                public abstract record Subject {}
            }
            """;

        return VerifyAnalyzerAsync(source);
    }
}