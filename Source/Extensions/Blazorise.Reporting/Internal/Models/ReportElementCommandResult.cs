namespace Blazorise.Reporting.Internal;

internal sealed class ReportElementCommandResult
{
    #region Properties

    public bool Changed { get; set; }

    public string PrimaryElementKey { get; set; }

    public System.Collections.Generic.IReadOnlyList<string> SelectedElementKeys { get; set; } = [];

    public int? SelectedSectionIndex { get; set; }

    #endregion
}