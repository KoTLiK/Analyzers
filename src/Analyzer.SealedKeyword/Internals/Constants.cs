namespace Analyzer.SealedKeyword.Internals;

// ReSharper disable file InconsistentNaming
internal static class Constants
{
    public const string Category = "Design";

    public static class SKA0001
    {
        public const string Id = "SKA0001";
        public const string Title = "Classes should be marked as 'sealed'";
        public const string MessageFormat = "Class '{0}' should be marked as 'sealed'";
    }

    public static class SKA0002
    {
        public const string Id = "SKA0002";
        public const string Title = "Records should be marked as 'sealed'";
        public const string MessageFormat = "Record '{0}' should be marked as 'sealed'";
    }
}
