namespace Analyzer.SealedKeyword.Tests.Unit.Class;

public class ClassTests : AnalyzerVerifier
{
    [Theory]
    [InlineData("Space")]
    [InlineData("Custom.Space")]
    [InlineData("Longer.Custom.Space")]
    public Task Simple_Sealed_Then_Ok(string @namespace)
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
    public Task Simple_NonSealed_Then_Warning(string @namespace)
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

    [Fact]
    public Task TopLevel_NonSealed_Then_Warning()
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
    public Task SingleSource_DifferentNamespace_SameClass_OneIsNonSealed_Then_Warning()
    {
        /* lang=csharp */
        const string source = """
            namespace Sealed {
                public sealed class TestClass {}
            }
            namespace NonSealed {
                public class TestClass {}
            }
            """;

        var result = Diagnostic(Descriptor.SKA0001)
            .WithSpan(5, 5, 5, 30)
            .WithArguments("TestClass");

        return VerifyAnalyzerAsync(source, result);
    }

    [Fact]
    public Task SingleSource_DifferentNamespace_SameClass_NonSealed_Then_Warning()
    {
        /* lang=csharp */
        const string source = """
            namespace NonSealed1 {
                public class TestClass {}
            }
            namespace NonSealed2 {
                public class TestClass {}
            }
            """;

        var result1 = Diagnostic(Descriptor.SKA0001)
            .WithSpan(2, 5, 2, 30)
            .WithArguments("TestClass");

        var result2 = Diagnostic(Descriptor.SKA0001)
            .WithSpan(5, 5, 5, 30)
            .WithArguments("TestClass");

        return VerifyAnalyzerAsync(source, result1, result2);
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

        var result = Diagnostic(Descriptor.SKA0001)
            .WithSpan(2, 5, 2, 30)
            .WithArguments("TestClass");

        return VerifyAnalyzerAsync(sources, result);
    }

    [Fact]
    public Task MultiSource_DifferentNamespace_SameClass_NonSealed_Then_Warning()
    {
        var sources = new []
        {
            /* lang=csharp */"""
            namespace NonSealed1 {
                public class TestClass {}
            }
            """,
            /* lang=csharp */"""
            namespace NonSealed2 {
                public class TestClass {}
            }
            """,
        };

        var result1 = Diagnostic(Descriptor.SKA0001)
            .WithSpan(2, 5, 2, 30)
            .WithArguments("TestClass");

        var result2 = Diagnostic(Descriptor.SKA0001)
            .WithSpan("/0/Test1.cs", 2, 5, 2, 30)
            .WithArguments("TestClass");

        return VerifyAnalyzerAsync(sources, result1, result2);
    }
}