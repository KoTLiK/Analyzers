namespace Analyzer.SealedKeyword.Tests.Unit.RecordTests;

public sealed class SameClass : AnalyzerVerifier
{
    [Fact]
    public Task SingleSource_OneNonSealed_Then_Warning()
    {
        /* lang=csharp */
        const string source = """
            namespace Sealed {
                public sealed record Subject {}
            }
            namespace NonSealed {
                public record Subject {}
            }
            """;

        var result = Diagnostic(Descriptor.SKA0002)
            .WithSpan(5, 5, 5, 29)
            .WithArguments("Subject");

        return VerifyAnalyzerAsync(source, result);
    }

    [Fact]
    public Task MultiSource_OneNonSealed_Then_Warning()
    {
        var sources = new []
        {
            /* lang=csharp */"""
            namespace NonSealed {
                public record Subject {}
            }
            """,
            /* lang=csharp */"""
            namespace Sealed {
                public sealed record Subject {}
            }
            """,
        };

        var result = Diagnostic(Descriptor.SKA0002)
            .WithSpan(2, 5, 2, 29)
            .WithArguments("Subject");

        return VerifyAnalyzerAsync(sources, result);
    }

    [Fact]
    public Task MultiSource_FileScoped_OneNonSealed_Then_Warning()
    {
        var sources = new []
        {
            /* lang=csharp */"""
            namespace NonSealed;
            public record Subject {}
            """,
            /* lang=csharp */"""
            namespace Sealed;
            public sealed record Subject {}
            """,
        };

        var result = Diagnostic(Descriptor.SKA0002)
            .WithSpan(2, 1, 2, 25)
            .WithArguments("Subject");

        return VerifyAnalyzerAsync(sources, result);
    }

    [Fact]
    public Task SingleSource_BothNonSealed_Then_Warning()
    {
        /* lang=csharp */
        const string source = """
            namespace NonSealed1 {
                public record Subject {}
            }
            namespace NonSealed2 {
                public record Subject {}
            }
            """;

        var result1 = Diagnostic(Descriptor.SKA0002)
            .WithSpan(2, 5, 2, 29)
            .WithArguments("Subject");

        var result2 = Diagnostic(Descriptor.SKA0002)
            .WithSpan(5, 5, 5, 29)
            .WithArguments("Subject");

        return VerifyAnalyzerAsync(source, result1, result2);
    }

    [Fact]
    public Task MultiSource_BothNonSealed_Then_Warning()
    {
        var sources = new []
        {
            /* lang=csharp */"""
            namespace NonSealed1 {
                public record Subject {}
            }
            """,
            /* lang=csharp */"""
            namespace NonSealed2 {
                public record Subject {}
            }
            """,
        };

        var result1 = Diagnostic(Descriptor.SKA0002)
            .WithSpan(2, 5, 2, 29)
            .WithArguments("Subject");

        var result2 = Diagnostic(Descriptor.SKA0002)
            .WithSpan("/0/Test1.cs", 2, 5, 2, 29)
            .WithArguments("Subject");

        return VerifyAnalyzerAsync(sources, result1, result2);
    }

    [Fact]
    public Task MultiSource_FileScoped_BothNonSealed_Then_Warning()
    {
        var sources = new []
        {
            /* lang=csharp */"""
            namespace NonSealed1;
            public record Subject {}
            """,
            /* lang=csharp */"""
            namespace NonSealed2;
            public record Subject {}
            """,
        };

        var result1 = Diagnostic(Descriptor.SKA0002)
            .WithSpan(2, 1, 2, 25)
            .WithArguments("Subject");

        var result2 = Diagnostic(Descriptor.SKA0002)
            .WithSpan("/0/Test1.cs", 2, 1, 2, 25)
            .WithArguments("Subject");

        return VerifyAnalyzerAsync(sources, result1, result2);
    }
}