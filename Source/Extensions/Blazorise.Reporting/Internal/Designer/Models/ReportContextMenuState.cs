#region Using directives
using Blazorise.Reporting;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportContextMenuState
{
    #region Properties

    internal bool Visible { get; set; }

    internal ReportContextMenuTarget Target { get; set; }

    internal int SectionIndex { get; set; } = -1;

    internal string ElementKey { get; set; }

    internal bool HasPastePosition { get; set; }

    internal double PasteX { get; set; }

    internal double PasteY { get; set; }

    internal double ClientX { get; set; }

    internal double ClientY { get; set; }

    #endregion
}