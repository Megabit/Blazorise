#region Using directives
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders the report designer properties editor.
/// </summary>
public partial class _ReportDesignerPropertiesPanel
{
    #region Members

    private const double SelectedElementMoveStep = 8;

    private const double SelectedElementHeightResizeStep = 8;

    private const double SelectedElementWidthResizeStep = 16;

    private const int DefaultTableColumnCount = 2;

    private const int DefaultTableRowCount = 2;

    private static readonly decimal TableCountStep = 1m;

    private _ReportDesignerDataSourceDialog dataSourceDialogRef;

    private _ReportDesignerFormulaDialog formulaDialogRef;

    private Func<string, Task> formulaConfirmed;

    private static readonly (ReportPageSize Value, string Text)[] PageSizeOptions =
    [
        ( ReportPageSize.Custom, "Custom" ),
        ( ReportPageSize.A3, "A3" ),
        ( ReportPageSize.A4, "A4" ),
        ( ReportPageSize.A5, "A5" ),
        ( ReportPageSize.Letter, "Letter" ),
        ( ReportPageSize.Legal, "Legal" ),
    ];

    private static readonly (ReportOrientation Value, string Text)[] PageOrientationOptions =
    [
        ( ReportOrientation.Portrait, "Portrait" ),
        ( ReportOrientation.Landscape, "Landscape" ),
    ];

    private static readonly (ReportMeasurementUnit Value, string Text)[] PageMeasurementUnitOptions =
    [
        ( ReportMeasurementUnit.Centimeter, "Centimeters" ),
        ( ReportMeasurementUnit.Millimeter, "Millimeters" ),
        ( ReportMeasurementUnit.Inch, "Inches" ),
        ( ReportMeasurementUnit.Point, "Points" ),
    ];

    private static readonly (string Value, string Text)[] ElementSnapToGridOptions =
    [
        ( string.Empty, "Default" ),
        ( "true", "True" ),
        ( "false", "False" ),
    ];

    private static readonly (VerticalAlignment Value, string Text)[] TextVerticalAlignmentOptions =
    [
        ( VerticalAlignment.Default, "Default" ),
        ( VerticalAlignment.Top, "Top" ),
        ( VerticalAlignment.Middle, "Middle" ),
        ( VerticalAlignment.Bottom, "Bottom" ),
    ];

    #endregion

    #region Methods

    private bool HasSelection => ReportSelected || SelectedSection is not null || SelectedElement is not null || SelectedCell is not null;

    private bool IsSelectedElementLine => SelectedElement?.Type == ReportElementType.Line;

    private double? GetSelectedLineThickness()
    {
        return SelectedElement?.Type == ReportElementType.Line
            ? SelectedElement.Thickness ?? ReportLayoutGeometry.DefaultLineThickness
            : null;
    }

    private ReportAppearanceDefinition EnsureSelectedSectionAppearance()
    {
        return EnsureSectionAppearance( SelectedSection );
    }

    private ReportBorderDefinition EnsureSelectedSectionBorder()
    {
        return EnsureSectionBorder( SelectedSection );
    }

    private static ReportAppearanceDefinition EnsureSectionAppearance( ReportSectionDefinition section )
    {
        return section.Appearance ??= new();
    }

    private static ReportBorderDefinition EnsureSectionBorder( ReportSectionDefinition section )
    {
        return section.Border ??= new();
    }

    private static ReportPageMarginsDefinition EnsurePageMargins( ReportPageDefinition page )
    {
        return page.Margins ??= new();
    }

    private ReportMeasurementUnit GetPageMeasurementUnit()
    {
        return Definition?.Page?.MeasurementUnit ?? ReportMeasurementUnit.Centimeter;
    }

    private decimal? GetMeasurementStep()
    {
        return ReportMeasurementConverter.GetEditorStep( GetPageMeasurementUnit() );
    }

    private double FromPoints( double value )
    {
        return ReportMeasurementConverter.RoundForDisplay( ReportMeasurementConverter.FromPoints( value, GetPageMeasurementUnit() ), GetPageMeasurementUnit() );
    }

    private double FromPoints( double? value )
    {
        return ReportMeasurementConverter.RoundForDisplay( ReportMeasurementConverter.FromPoints( value, GetPageMeasurementUnit() ), GetPageMeasurementUnit() );
    }

    private double ToPoints( double value )
    {
        return ReportMeasurementConverter.ToPoints( value, GetPageMeasurementUnit() );
    }

    private Task OnPageMeasurementUnitChanged( ReportMeasurementUnit value )
    {
        return UpdateReportPage( page => page.MeasurementUnit = value );
    }

    private Task OnPageSizeChanged( ReportPageSize value )
    {
        return UpdateReportPage( page => ReportPageDefinitionHelper.ApplySize( page, value ) );
    }

    private Task OnPageOrientationChanged( ReportOrientation value )
    {
        return UpdateReportPage( page => ReportPageDefinitionHelper.ApplyOrientation( page, value ) );
    }

    private Task OnPageWidthChanged( double value )
    {
        return UpdateReportPage( page =>
        {
            page.Size = ReportPageSize.Custom;
            page.Width = Math.Max( 1, ToPoints( value ) );
        } );
    }

    private Task OnPageHeightChanged( double value )
    {
        return UpdateReportPage( page =>
        {
            page.Size = ReportPageSize.Custom;
            page.Height = Math.Max( 1, ToPoints( value ) );
        } );
    }

    private Task OnPageMarginLeftChanged( double value )
    {
        return UpdateReportPage( page => EnsurePageMargins( page ).Left = Math.Max( 0, ToPoints( value ) ) );
    }

    private Task OnPageMarginTopChanged( double value )
    {
        return UpdateReportPage( page => EnsurePageMargins( page ).Top = Math.Max( 0, ToPoints( value ) ) );
    }

    private Task OnPageMarginRightChanged( double value )
    {
        return UpdateReportPage( page => EnsurePageMargins( page ).Right = Math.Max( 0, ToPoints( value ) ) );
    }

    private Task OnPageMarginBottomChanged( double value )
    {
        return UpdateReportPage( page => EnsurePageMargins( page ).Bottom = Math.Max( 0, ToPoints( value ) ) );
    }

    private Task OnSelectedSectionHeightChanged( double value )
    {
        return UpdateSelectedSection( section => section.Height = Math.Max( GetMinimumSectionHeight?.Invoke( section ) ?? ReportLayoutGeometry.DefaultMinimumElementSize, ToPoints( value ) ) );
    }

    private Task OnSelectedElementXChanged( double value )
    {
        return UpdateSelectedElement( element => element.X = ToPoints( value ) );
    }

    private Task OnSelectedElementYChanged( double value )
    {
        return UpdateSelectedElement( element => element.Y = ToPoints( value ) );
    }

    private Task OnSelectedElementWidthChanged( double value )
    {
        return UpdateSelectedElement( element => element.Width = ToPoints( value ) );
    }

    private Task OnSelectedElementHeightChanged( double value )
    {
        return UpdateSelectedElement( element => element.Height = ToPoints( value ) );
    }

    private Task OnSelectedLineThicknessChanged( double? value )
    {
        return UpdateSelectedElement( element => element.Thickness = ReportElementDefinitionHelper.NormalizeNullablePositiveNumber( value ) );
    }

    private double GetSelectedTableRowCount()
    {
        return Math.Max( 1, SelectedElement?.Rows?.Count > 0 ? SelectedElement.Rows.Count : DefaultTableRowCount );
    }

    private double GetSelectedTableColumnCount()
    {
        return Math.Max( 1, SelectedElement?.Columns?.Count > 0 ? SelectedElement.Columns.Count : DefaultTableColumnCount );
    }

    private Task OnSelectedTableRowCountChanged( double value )
    {
        int rowCount = Math.Max( 1, Convert.ToInt32( Math.Round( value ) ) );

        return UpdateSelectedElement( element => ReportDefinitionHelper.EnsureTableLayout( element, rowCount, Convert.ToInt32( GetSelectedTableColumnCount() ) ) );
    }

    private Task OnSelectedTableColumnCountChanged( double value )
    {
        int columnCount = Math.Max( 1, Convert.ToInt32( Math.Round( value ) ) );

        return UpdateSelectedElement( element => ReportDefinitionHelper.EnsureTableLayout( element, Convert.ToInt32( GetSelectedTableRowCount() ), columnCount ) );
    }

    private string GetSelectedElementSnapToGridValue()
    {
        return SelectedElement?.SnapToGrid?.Value switch
        {
            true => "true",
            false => "false",
            _ => string.Empty,
        };
    }

    private Task OnSelectedElementSnapToGridChanged( string value )
    {
        bool? snapToGrid = value switch
        {
            "true" => true,
            "false" => false,
            _ => null,
        };

        return UpdateSelectedElement( element => element.SnapToGrid = ReportValue.Create( snapToGrid, element.SnapToGrid?.Formula ) );
    }

    private Task UpdateSelectedElementCanGrowAsync( bool value )
    {
        return UpdateSelectedElement( element => element.CanGrow = ReportValue.Create( value, element.CanGrow?.Formula ) );
    }

    private Task UpdateSelectedElementSuppressAsync( bool value )
    {
        return UpdateSelectedElement( element => element.Suppress = ReportValue.Create( value, element.Suppress?.Formula ) );
    }

    private Task UpdateSelectedSectionKeepTogetherAsync( bool value )
    {
        return UpdateSelectedSection( section => section.KeepTogether = ReportValue.Create( value, section.KeepTogether?.Formula ) );
    }

    private Task UpdateSelectedSectionNewPageBeforeAsync( bool value )
    {
        return UpdateSelectedSection( section => section.NewPageBefore = ReportValue.Create( value, section.NewPageBefore?.Formula ) );
    }

    private Task UpdateSelectedSectionNewPageAfterAsync( bool value )
    {
        return UpdateSelectedSection( section => section.NewPageAfter = ReportValue.Create( value, section.NewPageAfter?.Formula ) );
    }

    private Task OpenSelectedSectionSuppressFormulaAsync()
    {
        return OpenFormulaDialogAsync(
            "Suppress",
            SelectedSection?.Suppress?.Formula,
            formula => UpdateSelectedSection( section => section.Suppress = ReportValue.Create( section.Suppress?.Value ?? false, formula ) ) );
    }

    private Task OpenSelectedSectionKeepTogetherFormulaAsync()
    {
        return OpenFormulaDialogAsync(
            "Keep together",
            SelectedSection?.KeepTogether?.Formula,
            formula => UpdateSelectedSection( section => section.KeepTogether = ReportValue.Create( section.KeepTogether?.Value ?? false, formula ) ) );
    }

    private Task OpenSelectedSectionNewPageBeforeFormulaAsync()
    {
        return OpenFormulaDialogAsync(
            "New page before",
            SelectedSection?.NewPageBefore?.Formula,
            formula => UpdateSelectedSection( section => section.NewPageBefore = ReportValue.Create( section.NewPageBefore?.Value ?? false, formula ) ) );
    }

    private Task OpenSelectedSectionNewPageAfterFormulaAsync()
    {
        return OpenFormulaDialogAsync(
            "New page after",
            SelectedSection?.NewPageAfter?.Formula,
            formula => UpdateSelectedSection( section => section.NewPageAfter = ReportValue.Create( section.NewPageAfter?.Value ?? false, formula ) ) );
    }

    private Task OpenSelectedElementCanGrowFormulaAsync()
    {
        return OpenFormulaDialogAsync(
            "Can grow",
            SelectedElement?.CanGrow?.Formula,
            formula => UpdateSelectedElement( element => element.CanGrow = ReportValue.Create( element.CanGrow?.Value ?? false, formula ) ) );
    }

    private Task OpenSelectedElementSuppressFormulaAsync()
    {
        return OpenFormulaDialogAsync(
            "Suppress",
            SelectedElement?.Suppress?.Formula,
            formula => UpdateSelectedElement( element => element.Suppress = ReportValue.Create( element.Suppress?.Value ?? false, formula ) ) );
    }

    private Task OpenSelectedElementSnapToGridFormulaAsync()
    {
        return OpenFormulaDialogAsync(
            "Snap to grid",
            SelectedElement?.SnapToGrid?.Formula,
            formula => UpdateSelectedElement( element => element.SnapToGrid = ReportValue.Create( element.SnapToGrid?.Value, formula ) ) );
    }

    private Task OpenFormulaDialogAsync( string propertyName, string formula, Func<string, Task> confirmed )
    {
        formulaConfirmed = confirmed;

        return formulaDialogRef?.ShowAsync( propertyName, formula ) ?? Task.CompletedTask;
    }

    private async Task OnFormulaDialogConfirmedAsync( string formula )
    {
        if ( formulaConfirmed is not null )
            await formulaConfirmed.Invoke( formula );
    }

    private void OnSnapToGridChanged( bool value )
    {
        SnapToGridChanged?.Invoke( value );
    }

    private string GetSelectedSectionDataSourceDisplayName()
    {
        if ( string.IsNullOrWhiteSpace( SelectedSection?.DataSource ) )
            return null;

        ReportDesignerDataSourceOption dataSource = ReportDataSourceExplorer.ResolveBindableDataSources( Definition ).FirstOrDefault( option =>
            string.Equals( option.Value, SelectedSection.DataSource, StringComparison.OrdinalIgnoreCase )
            || string.Equals( option.DisplayName, SelectedSection.DataSource, StringComparison.OrdinalIgnoreCase ) );

        return dataSource?.DisplayName ?? SelectedSection.DataSource;
    }

    private Task OpenDataSourceDialogAsync()
    {
        return dataSourceDialogRef?.ShowAsync( SelectedSection?.DataSource ) ?? Task.CompletedTask;
    }

    private Task UpdateSelectedSectionDataSource( string value )
    {
        return UpdateSelectedSection( section => section.DataSource = string.IsNullOrWhiteSpace( value ) ? null : value );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Report definition whose page settings are edited.
    /// </summary>
    [Parameter] public ReportDefinition Definition { get; set; }

    /// <summary>
    /// Report data used when validating formula expressions.
    /// </summary>
    [Parameter] public object Data { get; set; }

    /// <summary>
    /// Indicates that the root report node is selected.
    /// </summary>
    [Parameter] public bool ReportSelected { get; set; }

    /// <summary>
    /// Selected band definition, when a band is selected.
    /// </summary>
    [Parameter] public ReportSectionDefinition SelectedSection { get; set; }

    /// <summary>
    /// Band context used when validating formula expressions.
    /// </summary>
    [Parameter] public ReportSectionDefinition FormulaSection { get; set; }

    /// <summary>
    /// Selected element definition, when an element is selected.
    /// </summary>
    [Parameter] public ReportElementDefinition SelectedElement { get; set; }

    /// <summary>
    /// Selected table cell definition, when a layout table cell is selected.
    /// </summary>
    [Parameter] public ReportTableCellDefinition SelectedCell { get; set; }

    /// <summary>
    /// Indicates that the selected element belongs to a suppressed band.
    /// </summary>
    [Parameter] public bool SelectedElementSuppressed { get; set; }

    /// <summary>
    /// Indicates that designer movement snaps to the grid.
    /// </summary>
    [Parameter] public bool SnapToGrid { get; set; }

    /// <summary>
    /// Raised when snap-to-grid is toggled.
    /// </summary>
    [Parameter] public Action<bool> SnapToGridChanged { get; set; }

    /// <summary>
    /// Updates the report page definition.
    /// </summary>
    [Parameter] public Func<Action<ReportPageDefinition>, Task> UpdateReportPage { get; set; }

    /// <summary>
    /// Updates the selected band definition.
    /// </summary>
    [Parameter] public Func<Action<ReportSectionDefinition>, Task> UpdateSelectedSection { get; set; }

    /// <summary>
    /// Updates whether the selected band is suppressed.
    /// </summary>
    [Parameter] public Func<bool, Task> UpdateSelectedSectionSuppression { get; set; }

    /// <summary>
    /// Calculates the smallest height that can contain the band elements.
    /// </summary>
    [Parameter] public Func<ReportSectionDefinition, double> GetMinimumSectionHeight { get; set; }

    /// <summary>
    /// Indicates that a band can be inserted before or after the selected band.
    /// </summary>
    [Parameter] public bool CanInsertSection { get; set; }

    /// <summary>
    /// Inserts a band before or after the selected band.
    /// </summary>
    [Parameter] public Func<bool, Task> InsertSection { get; set; }

    /// <summary>
    /// Indicates that a group can be inserted around the selected detail band.
    /// </summary>
    [Parameter] public bool CanInsertGroup { get; set; }

    /// <summary>
    /// Opens the group insertion workflow for the selected detail band.
    /// </summary>
    [Parameter] public Func<Task> InsertGroup { get; set; }

    /// <summary>
    /// Deletes the currently selected band.
    /// </summary>
    [Parameter] public Func<Task> DeleteSelectedSection { get; set; }

    /// <summary>
    /// Updates the selected element definition.
    /// </summary>
    [Parameter] public Func<Action<ReportElementDefinition>, Task> UpdateSelectedElement { get; set; }

    /// <summary>
    /// Moves or resizes the selected element by a delta.
    /// </summary>
    [Parameter] public Func<double, double, double, double, Task> MoveSelectedElement { get; set; }

    #endregion
}