namespace Analyzer.SealedKeyword.Tests.Unit.ClassTests;

public sealed class SameClass : AnalyzerVerifier
{
    [Fact]
    public Task SingleSource_OneNonSealed_Then_Warning()
    {
        /* lang=csharp */
        const string source = """
            namespace Sealed {
                public sealed class Subject {}
            }
            namespace NonSealed {
                public class Subject {}
            }
            """;

        var result = Diagnostic(Descriptor.SKA0001)
            .WithSpan(5, 5, 5, 28)
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
                public class Subject {}
            }
            """,
            /* lang=csharp */"""
            namespace Sealed {
                public sealed class Subject {}
            }
            """,
        };

        var result = Diagnostic(Descriptor.SKA0001)
            .WithSpan(2, 5, 2, 28)
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
            public class Subject {}
            """,
            /* lang=csharp */"""
            namespace Sealed;
            public sealed class Subject {}
            """,
        };

        var result = Diagnostic(Descriptor.SKA0001)
            .WithSpan(2, 1, 2, 24)
            .WithArguments("Subject");

        return VerifyAnalyzerAsync(sources, result);
    }

    [Fact]
    public Task SingleSource_BothNonSealed_Then_Warning()
    {
        /* lang=csharp */
        const string source = """
            namespace NonSealed1 {
                public class Subject {}
            }
            namespace NonSealed2 {
                public class Subject {}
            }
            """;

        var result1 = Diagnostic(Descriptor.SKA0001)
            .WithSpan(2, 5, 2, 28)
            .WithArguments("Subject");

        var result2 = Diagnostic(Descriptor.SKA0001)
            .WithSpan(5, 5, 5, 28)
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
                public class Subject {}
            }
            """,
            /* lang=csharp */"""
            namespace NonSealed2 {
                public class Subject {}
            }
            """,
        };

        var result1 = Diagnostic(Descriptor.SKA0001)
            .WithSpan(2, 5, 2, 28)
            .WithArguments("Subject");

        var result2 = Diagnostic(Descriptor.SKA0001)
            .WithSpan("/0/Test1.cs", 2, 5, 2, 28)
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
            public class Subject {}
            """,
            /* lang=csharp */"""
            namespace NonSealed2;
            public class Subject {}
            """,
        };

        var result1 = Diagnostic(Descriptor.SKA0001)
            .WithSpan(2, 1, 2, 24)
            .WithArguments("Subject");

        var result2 = Diagnostic(Descriptor.SKA0001)
            .WithSpan("/0/Test1.cs", 2, 1, 2, 24)
            .WithArguments("Subject");

        return VerifyAnalyzerAsync(sources, result1, result2);
    }
}