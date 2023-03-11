namespace Analyzer.SealedKeyword.Tests.Unit;

public class ClassInheritanceTests : AnalyzerVerifier
{
    [Fact]
    public Task TopLevel_Then_Info()
    {
        /* lang=csharp */
        const string source = """
            public sealed class Sealed : TestClass {}
            public class TestClass {}
            """;

        var result = Diagnostic(Descriptor.SKA0003)
            .WithSpan(2, 1, 2, 26)
            .WithArguments("TestClass");

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
            public sealed class Sealed : TestClass {}
            public class TestClass {}
            """;

        var result = Diagnostic(Descriptor.SKA0003)
            .WithSpan(3, 1, 3, 26)
            .WithArguments("TestClass");

        return VerifyAnalyzerAsync(source, result);
    }

    [Theory]
    [InlineData("Space")]
    [InlineData("Custom.Space")]
    [InlineData("Longer.Custom.Space")]
    public Task SingleSource_DifferentNamespace_QualifiedInheritance_Then_Info(string @namespace)
    {
        /* lang=csharp */
        var source = $$"""
            namespace Sealed {
                public sealed class Sealed : {{@namespace}}.TestClass {}
            }
            namespace {{@namespace}} {
                public class TestClass {}
            }
            """;

        var result = Diagnostic(Descriptor.SKA0003)
            .WithSpan(5, 5, 5, 30)
            .WithArguments("TestClass");

        return VerifyAnalyzerAsync(source, result);
    }

    [Theory]
    [InlineData("Space")]
    [InlineData("Custom.Space")]
    [InlineData("Longer.Custom.Space")]
    public Task MultiSource_DifferentNamespace_QualifiedInheritance_Then_Info(string @namespace)
    {
        var sources = new []
        {
            /* lang=csharp */$$"""
            namespace Sealed {
                public sealed class Sealed : {{@namespace}}.TestClass {}
            }
            """,
            /* lang=csharp */$$"""
            namespace {{@namespace}} {
                public class TestClass {}
            }
            """,
        };

        var result = Diagnostic(Descriptor.SKA0003)
            .WithSpan("/0/Test1.cs", 2, 5, 2, 30)
            .WithArguments("TestClass");

        return VerifyAnalyzerAsync(sources, result);
    }
}