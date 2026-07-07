#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportClipboardResult
{
    #region Properties

    public bool Changed { get; set; }

    public List<ReportElementDefinition> ClipboardElements { get; set; } = [];

    public string ClipboardSectionId { get; set; }

    public int? SelectedSectionIndex { get; set; }

    public string PrimaryElementKey { get; set; }

    public IReadOnlyList<string> SelectedElementKeys { get; set; } = [];

    public string SelectedCellKey { get; set; }

    #endregion
}