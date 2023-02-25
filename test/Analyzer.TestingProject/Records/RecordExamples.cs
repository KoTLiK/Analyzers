namespace Analyzer.TestingProject.Records;

public record NonSealed;

public sealed record Sealed;

public abstract record Abstract;

public partial record NonSealedPartial;

public sealed partial record Partial;

public sealed record InheritsNonSealed : SuggestAbstractOrRemoval;

public record SuggestAbstractOrRemoval;
