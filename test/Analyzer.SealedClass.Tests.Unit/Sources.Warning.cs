namespace Analyzer.SealedClass.Tests.Unit;

public static partial class Sources
{
    /* lang=csharp */
    public const string NonSealedClass = """
            using System;
            namespace TestNamespace;
            public class TestClass {}
            """;

    /* lang=csharp */
    public const string NonSealedPartialClass = """
            using System;
            namespace TestNamespace;
            public partial class TestClass {}
            """;

    /* lang=csharp */
    public const string InheritedNonSealedClass = """
            using System;
            namespace TestNamespace;
            public sealed class SealedClass : TestClass {}
            public class TestClass {}
            """;
}