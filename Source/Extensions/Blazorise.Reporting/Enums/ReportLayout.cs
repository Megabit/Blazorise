namespace Blazorise.Reporting;

/// <summary>
/// Defines layout behavior for elements inside a report section.
/// </summary>
public enum ReportLayout
{
    /// <summary>
    /// Positions elements by absolute X and Y coordinates.
    /// </summary>
    Absolute,

    /// <summary>
    /// Arranges elements in document order.
    /// </summary>
    Stack,

    /// <summary>
    /// Arranges elements using flex layout rules.
    /// </summary>
    Flex,

    /// <summary>
    /// Arranges elements using grid layout rules.
    /// </summary>
    Grid
}