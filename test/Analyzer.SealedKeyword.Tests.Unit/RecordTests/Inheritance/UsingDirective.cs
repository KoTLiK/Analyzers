namespace Analyzer.SealedKeyword.Tests.Unit.RecordTests.Inheritance;

public sealed class UsingDirective : AnalyzerVerifier
{
    [Theory]
    [InlineData("Space")]
    [InlineData("Custom.Space")]
    [InlineData("Longer.Custom.Space")]
    public Task DirectiveInside_SingleSource_BlockScoped_Then_Info(string @namespace)
    {
        /* lang=csharp */
        var source = $$"""
            namespace SealedSpace {
                using {{@namespace}};
                public sealed record Sealed : Subject {}
            }
            namespace {{@namespace}} {
                public record Subject {}
            }
            """;

        var result = Diagnostic(Descriptor.SKA0004)
            .WithSpan(6, 5, 6, 28)
            .WithArguments("Subject");

        return VerifyAnalyzerAsync(source, result);
    }

    [Theory]
    [InlineData("Space")]
    [InlineData("Custom.Space")]
    [InlineData("Longer.Custom.Space")]
    public Task DirectiveOutside_SingleSource_BlockScoped_Then_Info(string @namespace)
    {
        /* lang=csharp */
        var source = $$"""
            using {{@namespace}};
            namespace Sealed {
                public sealed record Sealed : Subject {}
            }
            namespace {{@namespace}} {
                public record Subject {}
            }
            """;

        var result = Diagnostic(Descriptor.SKA0004)
            .WithSpan(6, 5, 6, 28)
            .WithArguments("Subject");

        return VerifyAnalyzerAsync(source, result);
    }
}