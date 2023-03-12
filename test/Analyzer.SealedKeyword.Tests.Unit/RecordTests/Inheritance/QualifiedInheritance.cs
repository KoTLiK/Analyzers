namespace Analyzer.SealedKeyword.Tests.Unit.RecordTests.Inheritance;

public sealed class QualifiedInheritance : AnalyzerVerifier
{
    [Fact]
    public Task TopLevel_Then_Info()
    {
        /* lang=csharp */
        const string source = """
            public sealed record Sealed : Subject {}
            public record Subject {}
            """;

        var result = Diagnostic(Descriptor.SKA0004)
            .WithSpan(2, 1, 2, 25)
            .WithArguments("Subject");

        return VerifyAnalyzerAsync(source, result);
    }

    [Theory]
    [InlineData("Space")]
    [InlineData("Custom.Space")]
    [InlineData("Longer.Custom.Space")]
    public Task FileScoped_Then_Info(string @namespace)
    {
        /* lang=csharp */
        var source = $$"""
            namespace {{@namespace}};
            public sealed record Sealed : Subject {}
            public record Subject {}
            """;

        var result = Diagnostic(Descriptor.SKA0004)
            .WithSpan(3, 1, 3, 25)
            .WithArguments("Subject");

        return VerifyAnalyzerAsync(source, result);
    }

    [Theory]
    [InlineData("Space")]
    [InlineData("Custom.Space")]
    [InlineData("Longer.Custom.Space")]
    public Task SingleSource_BlockScoped_Qualified_Then_Info(string @namespace)
    {
        /* lang=csharp */
        var source = $$"""
            namespace SealedSpace {
                public sealed record Sealed : {{@namespace}}.Subject {}
            }
            namespace {{@namespace}} {
                public record Subject {}
            }
            """;

        var result = Diagnostic(Descriptor.SKA0004)
            .WithSpan(5, 5, 5, 29)
            .WithArguments("Subject");

        return VerifyAnalyzerAsync(source, result);
    }

    [Theory]
    [InlineData("Space")]
    [InlineData("Custom.Space")]
    [InlineData("Longer.Custom.Space")]
    public Task MultiSource_BlockScoped_Qualified_Then_Info(string @namespace)
    {
        var sources = new []
        {
            /* lang=csharp */$$"""
            namespace SealedSpace {
                public sealed record Sealed : {{@namespace}}.Subject {}
            }
            """,
            /* lang=csharp */$$"""
            namespace {{@namespace}} {
                public record Subject {}
            }
            """,
        };

        var result = Diagnostic(Descriptor.SKA0004)
            .WithSpan("/0/Test1.cs", 2, 5, 2, 29)
            .WithArguments("Subject");

        return VerifyAnalyzerAsync(sources, result);
    }
}