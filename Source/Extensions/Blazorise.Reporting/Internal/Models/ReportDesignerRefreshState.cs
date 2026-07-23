namespace Blazorise.Reporting.Internal;

/// <summary>
/// Tracks targeted refresh requests for report designer panes.
/// </summary>
public readonly record struct ReportDesignerRefreshState( int Surface, int SelectedPanel, int ElementSelection, int FieldsExplorer, int Toolbar );