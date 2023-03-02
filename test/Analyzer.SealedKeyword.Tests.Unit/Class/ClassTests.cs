namespace Analyzer.SealedKeyword.Tests.Unit.Class;

public class ClassTests : AnalyzerVerifier
{
    [Fact]
    public Task NonSealed_Then_Warning()
    {
        /* lang=csharp */
        const string source = """
            namespace TestNamespace;
            public class TestClass {}
            """;

        var result = Diagnostic(Descriptor.SKA0001)
            .WithSpan(2, 1, 2, 26)
            .WithArguments("TestClass");

        return VerifyAnalyzerAsync(source, result);
    }

    [Fact]
    public Task Sealed_Then_NoWarning()
    {
        /* lang=csharp */
        const string source = """
            namespace TestNamespace;
            public sealed class TestClass {}
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task SingleSource_DifferentNamespace_SameClass_OneIsNonSealed_Then_Warning()
    {
        /* lang=csharp */
        const string source = """
            namespace NonSealed {
                public class TestClass {}
            }
            namespace Sealed {
                public sealed class TestClass {}
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task MultiSource_DifferentNamespace_SameClass_OneIsNonSealed_Then_Warning()
    {
        var sources = new []
        {
            /* lang=csharp */"""
            namespace NonSealed {
                public class TestClass {}
            }
            """,
            /* lang=csharp */"""
            namespace Sealed {
                public sealed class TestClass {}
            }
            """,
        };

        return VerifyAnalyzerAsync(sources, ArraySegment<DiagnosticResult>.Empty);
    }
}