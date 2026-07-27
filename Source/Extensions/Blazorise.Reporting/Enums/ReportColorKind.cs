namespace Blazorise.Reporting;

/// <summary>
/// Identifies how a report color stores its value.
/// </summary>
public enum ReportColorKind
{
    /// <summary>
    /// No color is explicitly applied.
    /// </summary>
    Default,

    /// <summary>
    /// A named report color that is resolved by a report color resolver.
    /// </summary>
    Named,

    /// <summary>
    /// An explicit red, green, blue, and alpha color value.
    /// </summary>
    Rgb,

    /// <summary>
    /// A fully transparent color.
    /// </summary>
    Transparent,
}