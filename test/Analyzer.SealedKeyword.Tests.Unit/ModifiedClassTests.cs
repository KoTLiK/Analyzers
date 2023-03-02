namespace Analyzer.SealedKeyword.Tests.Unit;

// TODO clean up this messy unit tests
public sealed class ModifiedClassTests : AnalyzerVerifier
{
    [Fact]
    public Task When_NonSealed_Then_Warning()
    {
        // Arrange
        /* lang=csharp */
        const string source = """
            namespace TestNamespace;
            public class TestClass {}
            """;

        var result = Diagnostic(Descriptor.SKA0001)
            .WithSpan(2, 1, 2, 26)
            .WithArguments("TestClass");

        // Act + Assert
        return VerifyAnalyzerAsync(source, result);
    }

    [Fact]
    public Task When_NonSealedPartial_Then_Warning()
    {
        // Arrange
        /* lang=csharp */
        const string source = """
            namespace TestNamespace;
            public partial class TestClass {}
            """;

        var result = Diagnostic(Descriptor.SKA0001)
            .WithSpan(2, 1, 2, 34)
            .WithArguments("TestClass");

        // Act + Assert
        return VerifyAnalyzerAsync(source, result);
    }

    [Fact]
    public Task When_NonSealed_AsBaseClass_Then_Warning()
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
    public Task When_Abstract_Then_NoWarning()
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
    public Task When_Sealed_Then_NoWarning()
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
    public Task When_SealedPartial_Then_NoWarning()
    {
        // Arrange
        /* lang=csharp */
        const string source = """
            namespace TestNamespace;
            public sealed partial class TestClass {}
            """;

        // Act + Assert
        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task When_Static_Then_NoWarning()
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