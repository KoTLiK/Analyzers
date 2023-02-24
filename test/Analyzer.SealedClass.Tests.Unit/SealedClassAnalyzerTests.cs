using Microsoft.CodeAnalysis.CSharp.Testing.XUnit;

namespace Analyzer.SealedClass.Tests.Unit;

public class SealedClassAnalyzerTests : AnalyzerVerifier<SealedClassAnalyzer>
{
    [Fact]
    public Task When_NonSealedClass_Then_Warning()
    {
        // Arrange
        /* lang=csharp */
        const string source = """
            namespace TestNamespace;
            public class TestClass {}
            """;

        var result = Diagnostic(Descriptor.SCA0001)
            .WithSpan(2, 1, 2, 26)
            .WithArguments("TestClass");

        // Act + Assert
        return VerifyAnalyzerAsync(source, result);
    }

    [Fact]
    public Task When_NonSealedPartialClass_Then_Warning()
    {
        // Arrange
        /* lang=csharp */
        const string source = """
            namespace TestNamespace;
            public partial class TestClass {}
            """;

        var result = Diagnostic(Descriptor.SCA0001)
            .WithSpan(2, 1, 2, 34)
            .WithArguments("TestClass");

        // Act + Assert
        return VerifyAnalyzerAsync(source, result);
    }

    [Fact]
    public Task When_NonSealedClass_AsBaseClass_Then_Warning()
    {
        // Arrange
        /* lang=csharp */
        const string source = """
            namespace TestNamespace;
            public sealed class SealedClass : TestClass {}
            public class TestClass {}
            """;

        // Act + Assert
        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task When_AbstractClass_Then_NoWarning()
    {
        // Arrange
        /* lang=csharp */
        const string source = """
            namespace TestNamespace;
            public abstract class TestClass {}
            """;

        // Act + Assert
        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task When_SealedClass_Then_NoWarning()
    {
        // Arrange
        /* lang=csharp */
        const string source = """
            namespace TestNamespace;
            public sealed class TestClass {}
            """;

        // Act + Assert
        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task When_StaticClass_Then_NoWarning()
    {
        // Arrange
        /* lang=csharp */
        const string source = """
            namespace TestNamespace;
            public static class TestClass {}
            """;

        // Act + Assert
        return VerifyAnalyzerAsync(source);
    }
}