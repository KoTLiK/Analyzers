namespace TestNamespace1 {
    using TestNamespace2;
    using X = TestNamespace2.TestClass1;
    public sealed class SealedClass1 : X {}
    public sealed class SealedClass2 : TestClass2 {}
    public sealed class SealedClass3 : TestNamespace3.TestClass2 {}
}

namespace TestNamespace2 {
    public class TestClass1 {}
    public class TestClass2 {}
}

namespace TestNamespace3 {
    public sealed class TestClass1 {}
    public class TestClass2 {}
}