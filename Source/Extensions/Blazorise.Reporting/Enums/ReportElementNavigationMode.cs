#region Using directives
using System;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Defines how report elements participate in keyboard navigation.
/// </summary>
[Flags]
public enum ReportElementNavigationMode
{
    /// <summary>
    /// Excludes report elements and table cells from keyboard navigation.
    /// </summary>
    None = 0,

    /// <summary>
    /// Allows keyboard navigation between report elements.
    /// </summary>
    Element = 1,

    /// <summary>
    /// Allows keyboard navigation between table cells.
    /// </summary>
    Cell = 2,

    /// <summary>
    /// Allows keyboard navigation between report elements and table cells.
    /// </summary>
    ElementAndCell = Element | Cell,
}