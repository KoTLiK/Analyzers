namespace Analyzer.SealedKeyword.Tests.Unit.ClassTests;

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
                public class Subject {}
            }
            """;

        var result = Diagnostic(Descriptor.SKA0001)
            .WithSpan(2, 5, 2, 28)
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
                public sealed class Subject {}
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
                public partial class Subject {}
            }
            """;

        var result = Diagnostic(Descriptor.SKA0001)
            .WithSpan(2, 5, 2, 36)
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
                public sealed partial class Subject {}
            }
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
            namespace {{@namespace}} {
                public static class Subject {}
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
                public abstract class Subject {}
            }
            """;

        return VerifyAnalyzerAsync(source);
    }
}