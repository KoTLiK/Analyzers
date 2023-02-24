namespace Analyzer.SealedClass;

// ReSharper disable file InconsistentNaming
internal static class Constants
{
    public const string Category = "Design";

    public static class SCA0001
    {
        public const string Id = "SCA0001";
        public const string Title = "Classes should be marked as 'sealed'";
        public const string MessageFormat = "Class '{0}' should be marked as 'sealed'";
    }
}