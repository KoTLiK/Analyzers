namespace Analyzer.TestingProject.Classes;

public class NonSealed {}

public sealed class Sealed {}

public abstract class Abstract {}

public partial class NonSealedPartial {}

public sealed partial class Partial {}

public static class Static {}

public sealed class InheritsNonSealed : SuggestAbstractOrRemoval {}

public class SuggestAbstractOrRemoval {}
