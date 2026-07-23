#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders the report designer properties editor.
/// </summary>
public partial class _ReportDesignerPropertiesPanel
{
    #region Members

    private const int DefaultTableColumnCount = 2;

    private const int DefaultTableRowCount = 2;

    private const string MixedSelectValue = "__b_report_mixed__";

    private static readonly decimal TableCountStep = 1m;

    private _ReportDesignerDataSourceDialog dataSourceDialogRef;

    private _ReportDesignerFormulaDialog formulaDialogRef;

    private _ReportDesignerFormatDialog formatDialogRef;

    private _ReportDesignerImageUploadDialog imageUploadDialogRef;

    private Func<string, Task> formulaConfirmed;

    private Func<string, Task> dataSourceConfirmed;

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

    private static readonly (Orientation Value, string Text)[] LineOrientationOptions =
    [
        ( Orientation.Horizontal, "Horizontal" ),
        ( Orientation.Vertical, "Vertical" ),
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

    private static readonly (ReportBandMode Value, string Text)[] BandModeOptions =
    [
        ( ReportBandMode.Rail, "Rail" ),
        ( ReportBandMode.Separator, "Separator" ),
        ( ReportBandMode.Compact, "Compact" ),
        ( ReportBandMode.Classic, "Classic" ),
    ];

    private static readonly (VerticalAlignment Value, string Text)[] TextVerticalAlignmentOptions =
    [
        ( VerticalAlignment.Default, "Default" ),
        ( VerticalAlignment.Top, "Top" ),
        ( VerticalAlignment.Middle, "Middle" ),
        ( VerticalAlignment.Bottom, "Bottom" ),
    ];

    private static readonly (TextAlignment Value, string Text)[] TextAlignmentOptions =
    [
        ( TextAlignment.Default, "Default" ),
        ( TextAlignment.Start, "Start" ),
        ( TextAlignment.End, "End" ),
        ( TextAlignment.Center, "Center" ),
        ( TextAlignment.Justified, "Justified" ),
    ];

    private static readonly (ReportBorderStyle Value, string Text)[] BorderStyleOptions =
    [
        ( ReportBorderStyle.Default, "Default" ),
        ( ReportBorderStyle.Solid, "Solid" ),
        ( ReportBorderStyle.Dashed, "Dashed" ),
        ( ReportBorderStyle.Dotted, "Dotted" ),
    ];

    private static readonly (ReportImageFit Value, string Text)[] ImageFitOptions =
    [
        ( ReportImageFit.Default, "Default" ),
        ( ReportImageFit.Contain, "Contain" ),
        ( ReportImageFit.Cover, "Cover" ),
        ( ReportImageFit.Fill, "Fill" ),
        ( ReportImageFit.None, "None" ),
        ( ReportImageFit.ScaleDown, "Scale down" ),
    ];

    #endregion

    #region Methods

    private bool HasSelection => ReportSelected || SelectedSection is not null || SelectedElement is not null || SelectedCell is not null;

    private bool MultipleElementsSelected => SelectedElements?.Count > 1;

    private bool AllSelectedElementsSupportCanGrow => AllSelectedElementsMatch( static element => element is not ReportPanelElementDefinition );

    private bool AllSelectedElementsSupportTextFormatting => AllSelectedElementsMatch( static element => ReportElementDefinitionHelper.SupportsTextFormatting( element.Type ) );

    private bool AllSelectedElementsSuppressed => AllSelectedElementsMatch( static element => element.Suppress?.Value == true );

    private bool AnySelectedElementIsLine => SelectedElements?.Any( static element => element is ReportLineElementDefinition ) == true;

    private bool AllSelectedElementsAre<TElement>()
        where TElement : ReportElementDefinition
        => AllSelectedElementsMatch( static element => element is TElement );

    private bool AllSelectedElementsMatch( Func<ReportElementDefinition, bool> predicate )
    {
        if ( SelectedElements is not { Count: > 0 } )
            return false;

        foreach ( ReportElementDefinition element in SelectedElements )
        {
            if ( !predicate( element ) )
                return false;
        }

        return true;
    }

    private TValue GetSharedSelectedElementValue<TValue>( Func<ReportElementDefinition, TValue> valueSelector )
    {
        ( TValue value, bool mixed ) = GetSelectedElementValue( valueSelector );

        return mixed ? default : value;
    }

    private bool IsSelectedElementValueMixed<TValue>( Func<ReportElementDefinition, TValue> valueSelector )
        => GetSelectedElementValue( valueSelector ).Mixed;

    private (TValue Value, bool Mixed) GetSelectedElementValue<TValue>( Func<ReportElementDefinition, TValue> valueSelector )
    {
        if ( SelectedElements is not { Count: > 0 } )
            return default;

        TValue value = valueSelector( SelectedElements[0] );
        EqualityComparer<TValue> comparer = EqualityComparer<TValue>.Default;

        for ( int i = 1; i < SelectedElements.Count; i++ )
        {
            if ( !comparer.Equals( value, valueSelector( SelectedElements[i] ) ) )
                return ( value, true );
        }

        return ( value, false );
    }

    private double? GetSharedMeasurementValue( Func<ReportElementDefinition, double> valueSelector )
    {
        ( double value, bool mixed ) = GetSelectedElementValue( valueSelector );

        return mixed ? null : FromPoints( value );
    }

    private static double? GetLineThickness( ReportElementDefinition element )
    {
        return element is ReportLineElementDefinition lineElement
            ? lineElement.Thickness ?? ReportLayoutGeometry.DefaultLineThickness
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

    private static ReportAppearanceDefinition EnsureSectionAppearance( ReportBandDefinition section )
    {
        return section.Appearance ??= new();
    }

    private static ReportBorderDefinition EnsureSectionBorder( ReportBandDefinition section )
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

    private Task OnSelectedElementXChanged( double? value )
    {
        return value.HasValue
            ? UpdateSelectedElement( element =>
            {
                ( double containerWidth, _ ) = GetElementContainerSize( element );
                element.X = ReportLayoutGeometry.Clamp( ToPoints( value.Value ), 0, Math.Max( 0, containerWidth - element.Width ) );
            } )
            : Task.CompletedTask;
    }

    private Task OnSelectedElementNameChanged( string value )
    {
        return UpdateSelectedElement( element =>
        {
            element.Name = value;

            if ( element is ReportSubreportElementDefinition subreportElement && subreportElement.Report is not null )
                subreportElement.Report.Name = value;
        } );
    }

    private Task OnSelectedElementYChanged( double? value )
    {
        return value.HasValue
            ? UpdateSelectedElement( element =>
            {
                ( _, double containerHeight ) = GetElementContainerSize( element );
                element.Y = ReportLayoutGeometry.Clamp( ToPoints( value.Value ), 0, Math.Max( 0, containerHeight - element.Height ) );
            } )
            : Task.CompletedTask;
    }

    private Task OnSelectedElementWidthChanged( double? value )
    {
        return value.HasValue
            ? UpdateSelectedElement( element =>
            {
                ( double containerWidth, _ ) = GetElementContainerSize( element );
                element.Width = ReportLayoutGeometry.Clamp( ToPoints( value.Value ),
                    ReportLayoutGeometry.DefaultMinimumElementSize,
                    Math.Max( ReportLayoutGeometry.DefaultMinimumElementSize, containerWidth - element.X ) );
            } )
            : Task.CompletedTask;
    }

    private Task OnSelectedElementFontFamilyChanged( string value )
    {
        if ( value == MixedSelectValue )
            return Task.CompletedTask;

        return UpdateSelectedElement( element => ReportElementDefinitionHelper.EnsureFont( element ).Family = string.IsNullOrWhiteSpace( value ) ? null : value );
    }

    private static string GetFontFamilyValue( string family )
    {
        return string.IsNullOrWhiteSpace( family ) ? string.Empty : family;
    }

    private IReadOnlyList<FontFamily> GetVisibleFontFamilies()
    {
        List<FontFamily> fonts = [];

        foreach ( FontFamily font in Definition?.Fonts ?? [] )
        {
            AddVisibleFontFamily( fonts, font );
        }

        foreach ( FontFamily font in FontProvider?.GetFonts() ?? [] )
        {
            AddVisibleFontFamily( fonts, font );
        }

        return fonts;
    }

    private static void AddVisibleFontFamily( List<FontFamily> fonts, FontFamily font )
    {
        if ( font?.Visible != true || string.IsNullOrWhiteSpace( font.Name ) )
            return;

        if ( fonts.Any( existing => string.Equals( existing.Name, font.Name, StringComparison.OrdinalIgnoreCase ) ) )
            return;

        fonts.Add( font );
    }

    private Task OnSelectedElementHeightChanged( double? value )
    {
        return value.HasValue
            ? UpdateSelectedElement( element =>
            {
                ( _, double containerHeight ) = GetElementContainerSize( element );
                double minimumHeight = ReportLayoutGeometry.GetMinimumElementHeight( element );
                element.Height = ReportLayoutGeometry.Clamp( ToPoints( value.Value ),
                    minimumHeight,
                    Math.Max( minimumHeight, containerHeight - element.Y ) );
            } )
            : Task.CompletedTask;
    }

    private (double Width, double Height) GetElementContainerSize( ReportElementDefinition element )
    {
        double width = Definition?.Page?.Width ?? element.Width;
        double height = Definition?.Page?.Height ?? element.Height;

        if ( !ReportDefinitionHelper.TryFindElementLocation( Definition, ReportDefinitionHelper.EnsureElementId( element ), out ReportElementLocation location ) )
            return ( width, height );

        if ( location.ParentPanel is not null )
            return ( location.ParentPanel.Width, location.ParentPanel.Height );

        if ( location.ParentCell is not null && location.ParentTable is ReportTableElementDefinition table )
        {
            return ( ReportDefinitionHelper.GetTableCellWidth( table, location.ParentCell ),
                ReportDefinitionHelper.GetTableCellHeight( table, location.ParentCell ) );
        }

        height = Definition.Bands[location.SectionIndex].Height;

        return ( width, height );
    }

    private Task OnSelectedLineThicknessChanged( double? value )
    {
        return UpdateSelectedElement( element =>
        {
            if ( element is ReportLineElementDefinition lineElement )
                lineElement.Thickness = ReportElementDefinitionHelper.NormalizeNullablePositiveNumber( value );
        } );
    }

    private Task OnSelectedLineOrientationChanged( Orientation value )
    {
        return UpdateSelectedElement( element =>
        {
            if ( element is not ReportLineElementDefinition lineElement || lineElement.Orientation == value )
                return;

            lineElement.Orientation = value;
            ( lineElement.Width, lineElement.Height ) = ( lineElement.Height, lineElement.Width );
        } );
    }

    private static double GetTableRowCount( ReportElementDefinition element )
    {
        return Math.Max( 1, element is ReportTableElementDefinition tableElement && tableElement.Rows?.Count > 0 ? tableElement.Rows.Count : DefaultTableRowCount );
    }

    private static double GetTableColumnCount( ReportElementDefinition element )
    {
        return Math.Max( 1, element is ReportTableElementDefinition tableElement && tableElement.Columns?.Count > 0 ? tableElement.Columns.Count : DefaultTableColumnCount );
    }

    private Task OnSelectedTableRowCountChanged( double? value )
    {
        if ( !value.HasValue )
            return Task.CompletedTask;

        int rowCount = Math.Max( 1, Convert.ToInt32( Math.Round( value.Value ) ) );

        return UpdateSelectedElement( element =>
        {
            if ( element is ReportTableElementDefinition tableElement )
                ReportDefinitionHelper.EnsureTableLayout( element, rowCount, Math.Max( 1, tableElement.Columns?.Count ?? DefaultTableColumnCount ) );
        } );
    }

    private Task OnSelectedTableColumnCountChanged( double? value )
    {
        if ( !value.HasValue )
            return Task.CompletedTask;

        int columnCount = Math.Max( 1, Convert.ToInt32( Math.Round( value.Value ) ) );

        return UpdateSelectedElement( element =>
        {
            if ( element is ReportTableElementDefinition tableElement )
                ReportDefinitionHelper.EnsureTableLayout( element, Math.Max( 1, tableElement.Rows?.Count ?? DefaultTableRowCount ), columnCount );
        } );
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

    private Task UpdateSelectedElementCanGrow( bool value )
    {
        return UpdateSelectedElement( element => element.CanGrow = ReportValue.Create( value, element.CanGrow?.Formula ) );
    }

    private Task UpdateSelectedElementSuppress( bool value )
    {
        return UpdateSelectedElement( element => element.Suppress = ReportValue.Create( value, element.Suppress?.Formula ) );
    }

    private Task UpdateSelectedSectionKeepTogether( bool value )
    {
        return UpdateSelectedSection( section => section.KeepTogether = ReportValue.Create( value, section.KeepTogether?.Formula ) );
    }

    private Task UpdateSelectedSectionNewPageBefore( bool value )
    {
        return UpdateSelectedSection( section => section.NewPageBefore = ReportValue.Create( value, section.NewPageBefore?.Formula ) );
    }

    private Task UpdateSelectedSectionNewPageAfter( bool value )
    {
        return UpdateSelectedSection( section => section.NewPageAfter = ReportValue.Create( value, section.NewPageAfter?.Formula ) );
    }

    private Task OpenSelectedSectionSuppressFormula()
    {
        return OpenFormulaDialog(
            "Suppress",
            SelectedSection?.Suppress?.Formula,
            formula => UpdateSelectedSection( section => section.Suppress = ReportValue.Create( section.Suppress?.Value ?? false, formula ) ) );
    }

    private Task OpenSelectedSectionKeepTogetherFormula()
    {
        return OpenFormulaDialog(
            "Keep together",
            SelectedSection?.KeepTogether?.Formula,
            formula => UpdateSelectedSection( section => section.KeepTogether = ReportValue.Create( section.KeepTogether?.Value ?? false, formula ) ) );
    }

    private Task OpenSelectedSectionNewPageBeforeFormula()
    {
        return OpenFormulaDialog(
            "New page before",
            SelectedSection?.NewPageBefore?.Formula,
            formula => UpdateSelectedSection( section => section.NewPageBefore = ReportValue.Create( section.NewPageBefore?.Value ?? false, formula ) ) );
    }

    private Task OpenSelectedSectionNewPageAfterFormula()
    {
        return OpenFormulaDialog(
            "New page after",
            SelectedSection?.NewPageAfter?.Formula,
            formula => UpdateSelectedSection( section => section.NewPageAfter = ReportValue.Create( section.NewPageAfter?.Value ?? false, formula ) ) );
    }

    private Task OpenSelectedElementCanGrowFormula()
    {
        return OpenFormulaDialog(
            "Can grow",
            GetSharedSelectedElementValue( element => element.CanGrow?.Formula ),
            formula => UpdateSelectedElement( element => element.CanGrow = ReportValue.Create( element.CanGrow?.Value ?? false, formula ) ) );
    }

    private Task OpenSelectedElementSuppressFormula()
    {
        return OpenFormulaDialog(
            "Suppress",
            GetSharedSelectedElementValue( element => element.Suppress?.Formula ),
            formula => UpdateSelectedElement( element => element.Suppress = ReportValue.Create( element.Suppress?.Value ?? false, formula ) ) );
    }

    private Task OpenSelectedElementSnapToGridFormula()
    {
        return OpenFormulaDialog(
            "Snap to grid",
            GetSharedSelectedElementValue( element => element.SnapToGrid?.Formula ),
            formula => UpdateSelectedElement( element => element.SnapToGrid = ReportValue.Create( element.SnapToGrid?.Value, formula ) ) );
    }

    private Task OpenFormulaDialog( string propertyName, string formula, Func<string, Task> confirmed )
    {
        formulaConfirmed = confirmed;

        return formulaDialogRef?.Show( propertyName, formula ) ?? Task.CompletedTask;
    }

    private async Task OnFormulaDialogConfirmed( string formula )
    {
        if ( formulaConfirmed is not null )
            await formulaConfirmed.Invoke( formula );
    }

    private Task OnSnapToGridChanged( bool value )
        => SnapToGridChanged.InvokeAsync( value );

    private Task OnGridSizeChanged( double value )
        => GridSizeChanged.InvokeAsync( Math.Max( 1, ToPoints( value ) ) );

    private Task OnInsertSectionBeforeClicked( MouseEventArgs eventArgs )
        => InsertSection( false );

    private Task OnInsertSectionAfterClicked( MouseEventArgs eventArgs )
        => InsertSection( true );

    private Task OnInsertGroupClicked( MouseEventArgs eventArgs )
        => InsertGroup();

    private Task OnDeleteSelectedSectionClicked( MouseEventArgs eventArgs )
        => DeleteSelectedSection();

    private string GetSelectedSectionDataSourceDisplayName()
    {
        if ( string.IsNullOrWhiteSpace( SelectedSection?.DataSource ) )
            return null;

        ReportDesignerDataSourceOption dataSource = ReportDataSourceExplorer.ResolveBindableDataSources( Definition ).FirstOrDefault( option =>
            string.Equals( option.Value, SelectedSection.DataSource, StringComparison.OrdinalIgnoreCase )
            || string.Equals( option.DisplayName, SelectedSection.DataSource, StringComparison.OrdinalIgnoreCase ) );

        return dataSource?.DisplayName ?? SelectedSection.DataSource;
    }

    private Task OpenDataSourceDialog()
    {
        return OpenDataSourceDialog( SelectedSection?.DataSource, UpdateSelectedSectionDataSource );
    }

    private Task OpenDataSourceDialog( string dataSource, Func<string, Task> confirmed )
    {
        dataSourceConfirmed = confirmed;

        return dataSourceDialogRef?.Show( dataSource ) ?? Task.CompletedTask;
    }

    private async Task OnDataSourceDialogConfirmed( string value )
    {
        if ( dataSourceConfirmed is not null )
            await dataSourceConfirmed.Invoke( value );
    }

    private Task UpdateSelectedSectionDataSource( string value )
    {
        return UpdateSelectedSection( section => section.DataSource = string.IsNullOrWhiteSpace( value ) ? null : value );
    }

    private string GetSelectedSubreportDataSourceDisplayName( ReportSubreportElementDefinition subreportElement )
    {
        if ( string.IsNullOrWhiteSpace( subreportElement?.DataSource ) )
            return null;

        ReportDesignerDataSourceOption dataSource = ReportDataSourceExplorer.ResolveBindableDataSources( Definition ).FirstOrDefault( option =>
            string.Equals( option.Value, subreportElement.DataSource, StringComparison.OrdinalIgnoreCase )
            || string.Equals( option.DisplayName, subreportElement.DataSource, StringComparison.OrdinalIgnoreCase ) );

        return dataSource?.DisplayName ?? subreportElement.DataSource;
    }

    private Task OpenSelectedSubreportDataSourceDialog()
    {
        return OpenDataSourceDialog(
            GetSharedSelectedElementValue( element => ( element as ReportSubreportElementDefinition )?.DataSource ),
            value => UpdateSelectedElement( element =>
            {
                if ( element is ReportSubreportElementDefinition subreportElement )
                    subreportElement.DataSource = string.IsNullOrWhiteSpace( value ) ? null : value;
            } ) );
    }

    private Task OpenImageUploadDialog()
    {
        return imageUploadDialogRef?.Show() ?? Task.CompletedTask;
    }

    private Task OpenFormatDialog( MouseEventArgs eventArgs )
    {
        bool mixed = IsSelectedElementValueMixed( element => ReportFormatResolver.GetDisplayText( ( element as ReportFieldElementDefinition )?.Format ) );

        return formatDialogRef?.Show( mixed ? null : ( SelectedElement as ReportFieldElementDefinition )?.Format ) ?? Task.CompletedTask;
    }

    private Task OnFormatDialogConfirmed( ReportFormatDefinition format )
    {
        return UpdateSelectedElement( element =>
        {
            if ( element is ReportFieldElementDefinition fieldElement )
                fieldElement.Format = ReportFormats.Clone( format );
        } );
    }

    private Task OnImageUploadConfirmed( string source )
    {
        if ( string.IsNullOrWhiteSpace( source ) )
            return Task.CompletedTask;

        return UpdateSelectedElement( element =>
        {
            if ( element is ReportImageElementDefinition imageElement )
                imageElement.Source = source;
        } );
    }

    #endregion

    #region Properties

    private ReportElementDefinition SelectedElement => SelectedElements is { Count: > 0 } ? SelectedElements[0] : null;

    /// <summary>
    /// Gets or sets the Blazorise font provider.
    /// </summary>
    [Inject] public IFontProvider FontProvider { get; set; }

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
    [Parameter] public ReportBandDefinition SelectedSection { get; set; }

    /// <summary>
    /// Band context used when validating formula expressions.
    /// </summary>
    [Parameter] public ReportBandDefinition FormulaSection { get; set; }

    /// <summary>
    /// Selected element definitions, with the primary element first.
    /// </summary>
    [Parameter] public IReadOnlyList<ReportElementDefinition> SelectedElements { get; set; }

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
    [Parameter] public EventCallback<bool> SnapToGridChanged { get; set; }

    /// <summary>
    /// Grid size used by designer movement and resizing, in points.
    /// </summary>
    [Parameter] public double GridSize { get; set; }

    /// <summary>
    /// Raised when the designer grid size changes.
    /// </summary>
    [Parameter] public EventCallback<double> GridSizeChanged { get; set; }

    /// <summary>
    /// Band presentation used by the designer.
    /// </summary>
    [Parameter] public ReportBandMode BandMode { get; set; }

    /// <summary>
    /// Raised when the designer band presentation changes.
    /// </summary>
    [Parameter] public EventCallback<ReportBandMode> BandModeChanged { get; set; }

    /// <summary>
    /// Indicates that rulers are visible around the report designer page.
    /// </summary>
    [Parameter] public bool ShowRulers { get; set; }

    /// <summary>
    /// Raised when designer ruler visibility changes.
    /// </summary>
    [Parameter] public EventCallback<bool> ShowRulersChanged { get; set; }

    /// <summary>
    /// Indicates that fine-grained ruler ticks are visible around the report designer page.
    /// </summary>
    [Parameter] public bool ShowFineRulerTicks { get; set; }

    /// <summary>
    /// Raised when fine-grained ruler tick visibility changes.
    /// </summary>
    [Parameter] public EventCallback<bool> ShowFineRulerTicksChanged { get; set; }

    /// <summary>
    /// Indicates that cursor guides are visible across the report designer page.
    /// </summary>
    [Parameter] public bool ShowCursorGuides { get; set; }

    /// <summary>
    /// Raised when cursor guide visibility changes.
    /// </summary>
    [Parameter] public EventCallback<bool> ShowCursorGuidesChanged { get; set; }

    /// <summary>
    /// Indicates that overlapping report elements are highlighted in the designer.
    /// </summary>
    [Parameter] public bool ShowCollisionWarnings { get; set; }

    /// <summary>
    /// Raised when collision warning visibility changes.
    /// </summary>
    [Parameter] public EventCallback<bool> ShowCollisionWarningsChanged { get; set; }

    /// <summary>
    /// Updates the report page definition.
    /// </summary>
    [Parameter] public Func<Action<ReportPageDefinition>, Task> UpdateReportPage { get; set; }

    /// <summary>
    /// Updates the selected band definition.
    /// </summary>
    [Parameter] public Func<Action<ReportBandDefinition>, Task> UpdateSelectedSection { get; set; }

    /// <summary>
    /// Updates whether the selected band is suppressed.
    /// </summary>
    [Parameter] public Func<bool, Task> UpdateSelectedSectionSuppression { get; set; }

    /// <summary>
    /// Calculates the smallest height that can contain the band elements.
    /// </summary>
    [Parameter] public Func<ReportBandDefinition, double> GetMinimumSectionHeight { get; set; }

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
    /// Updates the selected element definitions.
    /// </summary>
    [Parameter] public Func<Action<ReportElementDefinition>, Task> UpdateSelectedElement { get; set; }

    /// <summary>
    /// Enables image upload from the Image element source property.
    /// </summary>
    [Parameter] public bool UploadImage { get; set; } = true;

    /// <summary>
    /// A comma-separated list of image MIME types accepted by the image upload dialog.
    /// </summary>
    [Parameter] public string ImageAccept { get; set; } = "image/png, image/jpeg, image/webp, image/svg+xml";

    /// <summary>
    /// Maximum image size in bytes.
    /// </summary>
    [Parameter] public long ImageMaxSize { get; set; } = 1024 * 1024 * 5;

    /// <summary>
    /// Specifies the max chunk size when uploading the image.
    /// </summary>
    [Parameter] public int MaxUploadImageChunkSize { get; set; } = 20 * 1024;

    /// <summary>
    /// Specifies the segment fetch timeout when uploading the image.
    /// </summary>
    [Parameter] public TimeSpan ImageUploadSegmentFetchTimeout { get; set; } = TimeSpan.FromMinutes( 1 );

    /// <summary>
    /// Disables image upload progress callbacks.
    /// </summary>
    [Parameter] public bool DisableImageUploadProgressReport { get; set; }

    /// <summary>
    /// Raised when the selected image changes.
    /// </summary>
    [Parameter] public EventCallback<FileChangedEventArgs> ImageUploadChanged { get; set; }

    /// <summary>
    /// Raised when reading an image starts.
    /// </summary>
    [Parameter] public EventCallback<FileStartedEventArgs> ImageUploadStarted { get; set; }

    /// <summary>
    /// Raised when reading an image ends.
    /// </summary>
    [Parameter] public EventCallback<FileEndedEventArgs> ImageUploadEnded { get; set; }

    /// <summary>
    /// Raised when an image chunk is read.
    /// </summary>
    [Parameter] public EventCallback<FileWrittenEventArgs> ImageUploadWritten { get; set; }

    /// <summary>
    /// Raised when image read progress changes.
    /// </summary>
    [Parameter] public EventCallback<FileProgressedEventArgs> ImageUploadProgressed { get; set; }

    /// <summary>
    /// Raised when the image upload action is confirmed.
    /// </summary>
    [Parameter] public EventCallback<FileUploadEventArgs> ImageUpload { get; set; }

    #endregion
}