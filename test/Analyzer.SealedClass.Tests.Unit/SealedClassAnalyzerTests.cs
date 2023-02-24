using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

#pragma warning disable xUnit1026 // Theory method on test class does not use parameter 'sourceName'. Use the parameter, or remove the parameter and associated data.

namespace Analyzer.SealedClass.Tests.Unit;

public class SealedClassAnalyzerTestsV2
{
    public void Foo()
    {

    }
}

public class SealedClassAnalyzerTests
{
    [Theory]
    [InlineData(nameof(Sources.NonSealedClass), Sources.NonSealedClass)]
    [InlineData(nameof(Sources.NonSealedPartialClass), Sources.NonSealedPartialClass)]
    public Task NonSealedClass_With_Warning(string sourceName, string source)
    {
        var expectedDiagnostic = Diagnostic.Create(
            Descriptor.SCA0001,
            Location.Create(CSharpSyntaxTree.ParseText(source), new TextSpan(0, 0)),
            "TestClass"
        );

        return Verifier.VerifyDiagnostics<SealedClassAnalyzer>(source, expectedDiagnostic);
    }

    [Theory]
    [InlineData(nameof(Sources.AbstractClass), Sources.AbstractClass)]
    [InlineData(nameof(Sources.SealedClass), Sources.SealedClass)]
    [InlineData(nameof(Sources.StaticClass), Sources.StaticClass)]
    [InlineData(nameof(Sources.InheritedNonSealedClass), Sources.InheritedNonSealedClass)]
    public Task Classes_Without_Warning(string sourceName, string source)
        => Verifier.VerifyDiagnostics<SealedClassAnalyzer>(source);
}