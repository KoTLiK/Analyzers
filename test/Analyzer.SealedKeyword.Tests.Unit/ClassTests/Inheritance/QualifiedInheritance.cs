namespace Analyzer.SealedKeyword.Tests.Unit.ClassTests.Inheritance;

public sealed class QualifiedInheritance : AnalyzerVerifier
{
    [Fact]
    public Task TopLevel_Then_Info()
    {
        /* lang=csharp */
        const string source = """
            public sealed class Sealed : Subject {}
            public class Subject {}
            """;

        var result = Diagnostic(Descriptor.SKA0003)
            .WithSpan(2, 1, 2, 24)
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
            public sealed class Sealed : Subject {}
            public class Subject {}
            """;

        var result = Diagnostic(Descriptor.SKA0003)
            .WithSpan(3, 1, 3, 24)
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
                public sealed class Sealed : {{@namespace}}.Subject {}
            }
            namespace {{@namespace}} {
                public class Subject {}
            }
            """;

        var result = Diagnostic(Descriptor.SKA0003)
            .WithSpan(5, 5, 5, 28)
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
                public sealed class Sealed : {{@namespace}}.Subject {}
            }
            """,
            /* lang=csharp */$$"""
            namespace {{@namespace}} {
                public class Subject {}
            }
            """,
        };

        var result = Diagnostic(Descriptor.SKA0003)
            .WithSpan("/0/Test1.cs", 2, 5, 2, 28)
            .WithArguments("Subject");

        return VerifyAnalyzerAsync(sources, result);
    }
}