namespace Blazorise.Reporting.Internal;

internal sealed class ReportClipboardResult
{
    #region Properties

    public bool Changed { get; set; }

    public ReportElementDefinition ClipboardElement { get; set; }

    public string ClipboardSectionId { get; set; }

    public int? SelectedSectionIndex { get; set; }

    public string SelectedElementKey { get; set; }

    public string SelectedCellKey { get; set; }

    #endregion
}