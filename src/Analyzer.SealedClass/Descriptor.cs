using Microsoft.CodeAnalysis;

namespace Analyzer.SealedClass;

// ReSharper disable file InconsistentNaming
internal static class Descriptor
{
    public static readonly DiagnosticDescriptor SCA0001 = new(
        Constants.SCA0001.Id,
        Constants.SCA0001.Title,
        Constants.SCA0001.MessageFormat,
        Constants.Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor SCA0002 = new(
        Constants.SCA0002.Id,
        Constants.SCA0002.Title,
        Constants.SCA0002.MessageFormat,
        Constants.Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
}