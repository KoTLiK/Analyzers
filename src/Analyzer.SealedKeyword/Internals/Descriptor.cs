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
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    /// <summary>
    /// Record should be marked as 'sealed'.
    /// </summary>
    public static readonly DiagnosticDescriptor SKA0002 = new(
        Constants.SKA0002.Id,
        Constants.SKA0002.Title,
        Constants.SKA0002.MessageFormat,
        Constants.Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    /// <summary>
    /// Class could be marked as 'abstract'.
    /// </summary>
    public static readonly DiagnosticDescriptor SKA0003 = new(
        Constants.SKA0003.Id,
        Constants.SKA0003.Title,
        Constants.SKA0003.MessageFormat,
        Constants.Category,
        DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    /// <summary>
    /// Record could be marked as 'abstract'.
    /// </summary>
    public static readonly DiagnosticDescriptor SKA0004 = new(
        Constants.SKA0004.Id,
        Constants.SKA0004.Title,
        Constants.SKA0004.MessageFormat,
        Constants.Category,
        DiagnosticSeverity.Info,
        isEnabledByDefault: true);
}