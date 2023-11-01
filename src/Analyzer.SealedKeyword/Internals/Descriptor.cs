using Microsoft.CodeAnalysis;

namespace Analyzer.SealedKeyword.Internals;

// ReSharper disable file InconsistentNaming
internal static class Descriptor
{
    /// <summary>
    /// Class should be marked as 'sealed'.
    /// </summary>
    public static readonly DiagnosticDescriptor SKA0001 = new(
        Constants.SKA0001.Id,
        Constants.SKA0001.Title,
        Constants.SKA0001.MessageFormat,
        Constants.Category,
        DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    /// <summary>
    /// Record should be marked as 'sealed'.
    /// </summary>
    public static readonly DiagnosticDescriptor SKA0002 = new(
        Constants.SKA0002.Id,
        Constants.SKA0002.Title,
        Constants.SKA0002.MessageFormat,
        Constants.Category,
        DiagnosticSeverity.Info,
        isEnabledByDefault: true);
}
