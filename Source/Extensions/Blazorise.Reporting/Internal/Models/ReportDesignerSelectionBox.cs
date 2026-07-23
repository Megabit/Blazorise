#region Using directives
using System;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportDesignerSelectionBox
{
    #region Properties

    internal int SectionIndex { get; set; }

    internal double StartX { get; set; }

    internal double StartY { get; set; }

    internal double CurrentX { get; set; }

    internal double CurrentY { get; set; }

    internal double StartClientX { get; set; }

    internal double StartClientY { get; set; }

    internal bool Additive { get; set; }

    internal bool HasMoved { get; set; }

    internal double X => Math.Min( StartX, CurrentX );

    internal double Y => Math.Min( StartY, CurrentY );

    internal double Width => Math.Abs( CurrentX - StartX );

    internal double Height => Math.Abs( CurrentY - StartY );

    #endregion
}