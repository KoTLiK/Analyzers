namespace Analyzer.SealedClass.Tests.Unit;

public static partial class Sources
{
    /* lang=csharp */
    public const string SealedClass = """
            using System;
            namespace TestNamespace;
            public sealed class TestClass {}
            """;

    /* lang=csharp */
    public const string AbstractClass = """
            using System;
            namespace TestNamespace;
            public abstract class TestClass {}
            """;

    /* lang=csharp */
    public const string StaticClass = """
            using System;
            namespace TestNamespace;
            public static class TestClass {}
            """;

    public static class Static {}
    public sealed class Sealed {}
    public abstract class Abstract {}
    public class Class {}
    public sealed partial class Partial {}
}