#region Using directives
using System;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Defines designer features supported by a custom report element.
/// </summary>
[Flags]
public enum ReportElementCapabilities
{
    /// <summary>
    /// No optional designer features.
    /// </summary>
    None = 0,

    /// <summary>
    /// The element can be resized.
    /// </summary>
    Resizable = 1,

    /// <summary>
    /// The element can grow vertically while rendering.
    /// </summary>
    CanGrow = 2,

    /// <summary>
    /// The element uses the shared font properties.
    /// </summary>
    TextFormatting = 4,

    /// <summary>
    /// Common capabilities for a visual leaf element.
    /// </summary>
    Default = Resizable,
}