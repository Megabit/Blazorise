namespace Blazorise.Reporting.Internal;

/// <summary>
/// Defines the current report designer ruler marker bounds.
/// </summary>
public sealed class ReportDesignerRulerMarker
{
    #region Properties

    internal double X { get; set; }

    internal double Y { get; set; }

    internal double Width { get; set; }

    internal double Height { get; set; }

    internal bool Active { get; set; }

    #endregion
}