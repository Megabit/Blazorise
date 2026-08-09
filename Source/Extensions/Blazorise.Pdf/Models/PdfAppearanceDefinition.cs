#region Using directives
using System;
using System.Collections.Generic;
using Blazorise;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Describes PDF fill styling.
/// </summary>
public sealed class PdfAppearanceDefinition
{
    #region Properties

    /// <summary>
    /// Fill color as a hex color value.
    /// </summary>
    public string BackgroundColor { get; set; }

    #endregion
}