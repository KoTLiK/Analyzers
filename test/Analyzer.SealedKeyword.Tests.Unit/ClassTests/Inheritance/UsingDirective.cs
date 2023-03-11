namespace Analyzer.SealedKeyword.Tests.Unit.ClassTests.Inheritance;

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
            namespace Sealed {
                using {{@namespace}};
                public sealed class Sealed : Subject {}
            }
            namespace {{@namespace}} {
                public class Subject {}
            }
            """;

        var result = Diagnostic(Descriptor.SKA0003)
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
                public sealed class Sealed : Subject {}
            }
            namespace {{@namespace}} {
                public class Subject {}
            }
            """;

        var result = Diagnostic(Descriptor.SKA0003)
            .WithSpan(6, 5, 6, 28)
            .WithArguments("Subject");

        return VerifyAnalyzerAsync(source, result);
    }
}