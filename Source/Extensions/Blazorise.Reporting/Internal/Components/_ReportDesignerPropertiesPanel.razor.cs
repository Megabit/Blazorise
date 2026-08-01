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

    private static readonly decimal TableCountStep = 1m;

    private _ReportDesignerDataSourceDialog dataSourceDialogRef;

    private _ReportDesignerFormulaDialog formulaDialogRef;

    private _ReportDesignerFormatDialog formatDialogRef;

    private _ReportDesignerImageUploadDialog imageUploadDialogRef;

    private Func<string, Task> formulaConfirmed;

    private Func<string, Task> dataSourceConfirmed;

    private PropertyGridSchema propertyGridSchema;

    private static readonly PropertyGridSelectOption<ReportPageSize>[] PageSizeOptions =
    [
        new( ReportPageSize.Custom, "Custom" ),
        new( ReportPageSize.A3, "A3" ),
        new( ReportPageSize.A4, "A4" ),
        new( ReportPageSize.A5, "A5" ),
        new( ReportPageSize.Letter, "Letter" ),
        new( ReportPageSize.Legal, "Legal" ),
    ];

    private static readonly PropertyGridSelectOption<ReportOrientation>[] PageOrientationOptions =
    [
        new( ReportOrientation.Portrait, "Portrait" ),
        new( ReportOrientation.Landscape, "Landscape" ),
    ];

    private static readonly PropertyGridSelectOption<Orientation>[] LineOrientationOptions =
    [
        new( Orientation.Horizontal, "Horizontal" ),
        new( Orientation.Vertical, "Vertical" ),
    ];

    private static readonly PropertyGridSelectOption<ReportMeasurementUnit>[] PageMeasurementUnitOptions =
    [
        new( ReportMeasurementUnit.Centimeter, "Centimeters" ),
        new( ReportMeasurementUnit.Millimeter, "Millimeters" ),
        new( ReportMeasurementUnit.Inch, "Inches" ),
        new( ReportMeasurementUnit.Point, "Points" ),
    ];

    private static readonly PropertyGridSelectOption<string>[] ElementSnapToGridOptions =
    [
        new( string.Empty, "Default" ),
        new( "true", "True" ),
        new( "false", "False" ),
    ];

    private static readonly PropertyGridSelectOption<ReportBandMode>[] BandModeOptions =
    [
        new( ReportBandMode.Rail, "Rail" ),
        new( ReportBandMode.Separator, "Separator" ),
        new( ReportBandMode.Compact, "Compact" ),
        new( ReportBandMode.Classic, "Classic" ),
    ];

    private static readonly PropertyGridSelectOption<VerticalAlignment>[] TextVerticalAlignmentOptions =
    [
        new( VerticalAlignment.Default, "Default" ),
        new( VerticalAlignment.Top, "Top" ),
        new( VerticalAlignment.Middle, "Middle" ),
        new( VerticalAlignment.Bottom, "Bottom" ),
    ];

    private static readonly PropertyGridSelectOption<TextAlignment>[] TextAlignmentOptions =
    [
        new( TextAlignment.Default, "Default" ),
        new( TextAlignment.Start, "Start" ),
        new( TextAlignment.End, "End" ),
        new( TextAlignment.Center, "Center" ),
        new( TextAlignment.Justified, "Justified" ),
    ];

    private static readonly PropertyGridSelectOption<ReportBorderStyle>[] BorderStyleOptions =
    [
        new( ReportBorderStyle.Default, "Default" ),
        new( ReportBorderStyle.Solid, "Solid" ),
        new( ReportBorderStyle.Dashed, "Dashed" ),
        new( ReportBorderStyle.Dotted, "Dotted" ),
    ];

    private static readonly PropertyGridSelectOption<ReportImageFit>[] ImageFitOptions =
    [
        new( ReportImageFit.Default, "Default" ),
        new( ReportImageFit.Contain, "Contain" ),
        new( ReportImageFit.Cover, "Cover" ),
        new( ReportImageFit.Fill, "Fill" ),
        new( ReportImageFit.None, "None" ),
        new( ReportImageFit.ScaleDown, "Scale down" ),
    ];

    #endregion

    #region Methods

    internal void InvalidatePropertyGridSchema()
        => propertyGridSchema = null;

    private bool HasSelection => ReportSelected || SelectedSection is not null || SelectedElement is not null || SelectedCell is not null;

    private bool MultipleElementsSelected => SelectedElements?.Count > 1;

    private bool AllSelectedElementsSupportCanGrow => AllSelectedElementsMatch( element =>
        element is not ReportPanelElementDefinition
        && ( element is not ReportCustomElementDefinition
            || SupportsCustomCapability( element, ReportElementCapabilities.CanGrow ) ) );

    private bool AllSelectedElementsSupportTextFormatting => AllSelectedElementsMatch( element =>
        ReportElementDefinitionHelper.SupportsTextFormatting( element.Type )
        || SupportsCustomCapability( element, ReportElementCapabilities.TextFormatting ) );

    private bool AllSelectedElementsSuppressed => AllSelectedElementsMatch( static element => element.Suppress?.Value == true );

    private bool AnySelectedElementIsLine => SelectedElements?.Any( static element => element is ReportLineElementDefinition ) == true;

    private IReportElementPlugin SelectedCustomElementPlugin
    {
        get
        {
            if ( SelectedElement is not ReportCustomElementDefinition customElement
                 || !AllSelectedElementsMatch( element => element is ReportCustomElementDefinition selectedCustomElement
                     && string.Equals( selectedCustomElement.TypeName, customElement.TypeName, StringComparison.OrdinalIgnoreCase ) ) )
            {
                return null;
            }

            return ElementPluginRegistry?.Find( customElement.TypeName );
        }
    }

    private string GetSelectedElementTypeDisplayName()
    {
        if ( SelectedCell is not null )
            return "Table Cell";

        if ( SelectedCustomElementPlugin is not null )
            return SelectedCustomElementPlugin.Descriptor.DisplayName;

        if ( SelectedElement is ReportCustomElementDefinition customElement )
        {
            return AllSelectedElementsMatch( element => element is ReportCustomElementDefinition selectedCustomElement
                && string.Equals( selectedCustomElement.TypeName, customElement.TypeName, StringComparison.OrdinalIgnoreCase ) )
                    ? customElement.TypeName ?? "Custom"
                    : "Multiple types";
        }

        ReportElementType type = SelectedElement.Type;

        return AllSelectedElementsMatch( element => element.Type == type )
            ? ReportDefinitionHelper.GetElementTypeDisplayName( type )
            : "Multiple types";
    }

    private bool AllSelectedElementsAre<TElement>()
        where TElement : ReportElementDefinition
        => AllSelectedElementsMatch( static element => element is TElement );

    private bool SupportsCustomCapability( ReportElementDefinition element, ReportElementCapabilities capability )
    {
        if ( element is not ReportCustomElementDefinition customElement )
            return false;

        return ElementPluginRegistry?.Find( customElement.TypeName )?.Descriptor.Capabilities.HasFlag( capability ) == true;
    }

    private IDictionary<string, object> GetCustomPropertiesParameters()
    {
        if ( SelectedElement is not ReportCustomElementDefinition )
            return null;

        return new Dictionary<string, object>
        {
            [nameof( BaseReportElementPropertiesEditor.Context )] = new ReportElementPropertiesContext(
                Definition,
                SelectedElements.OfType<ReportCustomElementDefinition>().ToList(),
                UpdateSelectedElement ),
        };
    }

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

    private IReadOnlyList<PropertyGridSelectOption<string>> GetFontFamilyOptions()
    {
        List<PropertyGridSelectOption<string>> options =
        [
            new( string.Empty, "Default" ),
        ];

        foreach ( FontFamily font in GetVisibleFontFamilies() )
            options.Add( new( font.Name, font.DisplayName ?? font.Name ) );

        return options;
    }

    private static void AddVisibleFontFamily( List<FontFamily> fonts, FontFamily font )
    {
        if ( font?.Visible != true || string.IsNullOrWhiteSpace( font.Name ) )
            return;

        if ( fonts.Any( existing => string.Equals( existing.Name, font.Name, StringComparison.OrdinalIgnoreCase ) ) )
            return;

        fonts.Add( font );
    }

    private static Color GetFormulaActionColor( string formula )
        => string.IsNullOrWhiteSpace( formula ) ? Color.Light : Color.Primary;

    private static string GetDataSourceDisplayValue( string value, string displayText )
        => string.IsNullOrWhiteSpace( displayText )
            ? string.IsNullOrWhiteSpace( value ) ? "None" : value
            : displayText;

    private static string GetPropertyGridColorValue( ReportColor color )
        => color.Kind switch
        {
            ReportColorKind.Default => string.Empty,
            ReportColorKind.Named or ReportColorKind.Transparent => color.Name,
            ReportColorKind.Rgb => FormattableString.Invariant( $"#{color.Red:X2}{color.Green:X2}{color.Blue:X2}" ),
            _ => color.ToCssString(),
        };

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

    private Task OpenFormatDialog()
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

    private PropertyGridSchema BuildPropertyGridSchema()
    {
        List<PropertyGridGroupDefinition> groups = [];

        AddReportPropertyGroups( groups );
        AddSectionPropertyGroups( groups );
        AddElementPropertyGroups( groups );
        AddCellPropertyGroups( groups );

        return new PropertyGridSchema( groups );
    }

    private void AddReportPropertyGroups( List<PropertyGridGroupDefinition> groups )
    {
        if ( !ReportSelected )
            return;

        groups.Add( new PropertyGridGroupDefinition(
            "report.designer",
            "Designer",
            [
                new PropertyGridTextProperty( "report.type", "Type", "Report" )
                {
                    ReadOnly = true,
                },
                new PropertyGridBooleanProperty( "report.snapToGrid", "Snap to grid", SnapToGrid ),
                new PropertyGridNumericProperty<double>( "report.gridSize", "Grid size", FromPoints( GridSize ) )
                {
                    Min = FromPoints( 1 ),
                    Max = FromPoints( Math.Min( Definition.Page.Width, Definition.Page.Height ) ),
                    Step = GetMeasurementStep(),
                },
                new PropertyGridBooleanProperty( "report.showRulers", "Show rulers", ShowRulers ),
                new PropertyGridBooleanProperty( "report.showFineRulerTicks", "Fine ruler ticks", ShowFineRulerTicks ),
                new PropertyGridBooleanProperty( "report.showCursorGuides", "Cursor guides", ShowCursorGuides ),
                new PropertyGridBooleanProperty( "report.showCollisionWarnings", "Collision warnings", ShowCollisionWarnings ),
                new PropertyGridSelectProperty<ReportBandMode>( "report.bandMode", "Band mode", BandMode, BandModeOptions ),
            ] ) );

        groups.Add( new PropertyGridGroupDefinition(
            "report.page",
            "Page",
            [
                new PropertyGridTextProperty( "page.name", "Name", Definition.Page.Name ),
                new PropertyGridSelectProperty<ReportMeasurementUnit>( "page.unit", "Unit", GetPageMeasurementUnit(), PageMeasurementUnitOptions ),
                new PropertyGridSelectProperty<ReportPageSize>( "page.size", "Paper size", Definition.Page.Size, PageSizeOptions ),
                new PropertyGridSelectProperty<ReportOrientation>( "page.orientation", "Orientation", Definition.Page.Orientation, PageOrientationOptions ),
                new PropertyGridNumericProperty<double>( "page.width", "Page width", FromPoints( Definition.Page.Width ) )
                {
                    Min = FromPoints( 1 ),
                    Step = GetMeasurementStep(),
                },
                new PropertyGridNumericProperty<double>( "page.height", "Page height", FromPoints( Definition.Page.Height ) )
                {
                    Min = FromPoints( 1 ),
                    Step = GetMeasurementStep(),
                },
                new PropertyGridNumericProperty<double>( "page.marginLeft", "Margin left", FromPoints( EnsurePageMargins( Definition.Page ).Left ) )
                {
                    Max = FromPoints( Definition.Page.Width ),
                    Step = GetMeasurementStep(),
                },
                new PropertyGridNumericProperty<double>( "page.marginTop", "Margin top", FromPoints( EnsurePageMargins( Definition.Page ).Top ) )
                {
                    Max = FromPoints( Definition.Page.Height ),
                    Step = GetMeasurementStep(),
                },
                new PropertyGridNumericProperty<double>( "page.marginRight", "Margin right", FromPoints( EnsurePageMargins( Definition.Page ).Right ) )
                {
                    Max = FromPoints( Definition.Page.Width ),
                    Step = GetMeasurementStep(),
                },
                new PropertyGridNumericProperty<double>( "page.marginBottom", "Margin bottom", FromPoints( EnsurePageMargins( Definition.Page ).Bottom ) )
                {
                    Max = FromPoints( Definition.Page.Height ),
                    Step = GetMeasurementStep(),
                },
            ] ) );
    }

    private void AddSectionPropertyGroups( List<PropertyGridGroupDefinition> groups )
    {
        if ( SelectedSection is null || SelectedElement is not null )
            return;

        List<PropertyGridProperty> statusProperties =
        [
            new PropertyGridBooleanProperty( "section.suppress", "Suppress", ReportValueResolver.ResolveStaticSuppress( SelectedSection ) )
            {
                Action = CreateFormulaAction( SelectedSection.Suppress?.Formula ),
            },
        ];

        if ( SelectedSection.Type == ReportBandType.PageFooter )
        {
            statusProperties.Add( new PropertyGridBooleanProperty(
                "section.reserveSpaceWhenSuppressed",
                "Reserve space when suppressed",
                SelectedSection.ReserveSpaceWhenSuppressed ) );
        }

        groups.Add( new PropertyGridGroupDefinition( "section.status", "Status", statusProperties ) );

        List<PropertyGridProperty> generalProperties =
        [
            new PropertyGridTextProperty( "section.type", "Type", ReportDefinitionHelper.GetSectionTypeDisplayName( SelectedSection.Type ) )
            {
                ReadOnly = true,
            },
        ];

        bool suppressed = ReportValueResolver.ResolveStaticSuppress( SelectedSection );

        if ( !suppressed )
        {
            generalProperties.Add( new PropertyGridTextProperty( "section.name", "Name", SelectedSection.Name ) );

            if ( SelectedSection.Type == ReportBandType.Detail )
            {
                generalProperties.Add( new PropertyGridTextProperty(
                    "section.dataSource",
                    "Data source",
                    GetDataSourceDisplayValue( SelectedSection.DataSource, GetSelectedSectionDataSourceDisplayName() ) )
                {
                    ReadOnly = true,
                    Action = CreateAction( "select-data-source", IconName.Search, "Select data source" ),
                } );
            }

            if ( SelectedSection.Type == ReportBandType.GroupHeader )
                generalProperties.Add( new PropertyGridTextProperty( "section.groupBy", "Group by", SelectedSection.GroupBy ) );
        }

        groups.Add( new PropertyGridGroupDefinition( "section.general", "General", generalProperties ) );

        if ( suppressed )
            return;

        groups.Add( new PropertyGridGroupDefinition(
            "section.layout",
            "Layout",
            [
                new PropertyGridNumericProperty<double>( "section.height", "Height", FromPoints( SelectedSection.Height ) )
                {
                    Min = FromPoints( ReportLayoutGeometry.DefaultMinimumElementSize ),
                    Max = FromPoints( ReportPageDefinitionHelper.GetContentHeight( Definition.Page ) ),
                    Step = GetMeasurementStep(),
                },
            ] ) );

        groups.Add( new PropertyGridGroupDefinition(
            "section.pagination",
            "Pagination",
            [
                new PropertyGridBooleanProperty( "section.keepTogether", "Keep together", SelectedSection.KeepTogether?.Value == true )
                {
                    Action = CreateFormulaAction( SelectedSection.KeepTogether?.Formula ),
                },
                new PropertyGridBooleanProperty( "section.newPageBefore", "New page before", SelectedSection.NewPageBefore?.Value == true )
                {
                    Action = CreateFormulaAction( SelectedSection.NewPageBefore?.Formula ),
                },
                new PropertyGridBooleanProperty( "section.newPageAfter", "New page after", SelectedSection.NewPageAfter?.Value == true )
                {
                    Action = CreateFormulaAction( SelectedSection.NewPageAfter?.Formula ),
                },
            ] ) );

        if ( SelectedSection.Type != ReportBandType.PageFooter )
            return;

        ReportAppearanceDefinition sectionAppearance = EnsureSelectedSectionAppearance();
        ReportBorderDefinition sectionBorder = EnsureSelectedSectionBorder();

        groups.Add( new PropertyGridGroupDefinition(
            "section.pageFooter",
            "Page footer",
            [
                new PropertyGridBooleanProperty( "section.printOnFirstPage", "Print on first page", SelectedSection.PrintOnFirstPage ),
                new PropertyGridBooleanProperty( "section.printOnLastPage", "Print on last page", SelectedSection.PrintOnLastPage ),
                new PropertyGridBooleanProperty( "section.repeatOnEveryPage", "Repeat on every page", SelectedSection.RepeatOnEveryPage ),
            ] ) );

        groups.Add( new PropertyGridGroupDefinition(
            "section.appearance",
            "Appearance",
            [
                new PropertyGridColorProperty( "section.fillColor", "Fill color", GetPropertyGridColorValue( sectionAppearance.BackgroundColor ) ),
                new PropertyGridColorProperty( "section.borderColor", "Border color", GetPropertyGridColorValue( sectionBorder.Color ) ),
                new PropertyGridNumericProperty<double?>( "section.borderWidth", "Border width", sectionBorder.Width )
                {
                    Max = Definition.Page.Width,
                },
                new PropertyGridNumericProperty<double?>( "section.cornerRadius", "Corner radius", sectionBorder.Radius )
                {
                    Max = Definition.Page.Width,
                },
                new PropertyGridNumericProperty<double?>( "section.opacity", "Opacity", sectionAppearance.Opacity )
                {
                    Max = 1,
                },
            ] ) );
    }

    private void AddElementPropertyGroups( List<PropertyGridGroupDefinition> groups )
    {
        if ( SelectedElement is null || SelectedElementSuppressed )
            return;

        List<PropertyGridProperty> statusProperties = [];

        if ( AllSelectedElementsSupportCanGrow )
        {
            statusProperties.Add( new PropertyGridBooleanProperty( "element.canGrow", "Can grow", SelectedElement.CanGrow?.Value == true )
            {
                Mixed = IsSelectedElementValueMixed( element => element.CanGrow?.Value == true ),
                Action = CreateFormulaAction( GetSharedSelectedElementValue( element => element.CanGrow?.Formula ) ),
            } );
        }

        statusProperties.Add( new PropertyGridBooleanProperty( "element.suppress", "Suppress", SelectedElement.Suppress?.Value == true )
        {
            Mixed = IsSelectedElementValueMixed( element => element.Suppress?.Value == true ),
            Action = CreateFormulaAction( GetSharedSelectedElementValue( element => element.Suppress?.Formula ) ),
        } );
        statusProperties.Add( new PropertyGridBooleanProperty( "element.showCollisionWarnings", "Collision warnings", SelectedElement.ShowCollisionWarnings )
        {
            Mixed = IsSelectedElementValueMixed( element => element.ShowCollisionWarnings ),
        } );
        statusProperties.Add( new PropertyGridStringSelectProperty(
            "element.snapToGrid",
            "Snap to grid",
            GetSelectedElementSnapToGridValue(),
            ElementSnapToGridOptions )
        {
            Mixed = IsSelectedElementValueMixed( element => element.SnapToGrid?.Value ),
            Action = CreateFormulaAction( GetSharedSelectedElementValue( element => element.SnapToGrid?.Formula ) ),
        } );

        groups.Add( new PropertyGridGroupDefinition( "element.status", "Status", statusProperties ) );

        List<PropertyGridProperty> generalProperties =
        [
            new PropertyGridTextProperty( "element.type", "Type", GetSelectedElementTypeDisplayName() )
            {
                ReadOnly = true,
            },
        ];

        if ( !MultipleElementsSelected && !AllSelectedElementsSuppressed )
            generalProperties.Add( new PropertyGridTextProperty( "element.name", "Name", SelectedElement.Name ) );

        groups.Add( new PropertyGridGroupDefinition( "element.general", "General", generalProperties ) );

        if ( AllSelectedElementsSuppressed )
            return;

        AddElementContentGroup( groups );
        AddElementTableGroup( groups );
        AddElementSubreportGroup( groups );
        AddElementPositionGroup( groups );
        AddElementTextGroup( groups );
        AddElementAppearanceGroup( groups );
    }

    private void AddElementContentGroup( List<PropertyGridGroupDefinition> groups )
    {
        if ( !AllSelectedElementsAre<ReportTextElementDefinition>()
            && !AllSelectedElementsAre<ReportFieldElementDefinition>()
            && !AllSelectedElementsAre<ReportImageElementDefinition>() )
        {
            return;
        }

        List<PropertyGridProperty> properties = [];
        string title = SelectedElement is ReportFieldElementDefinition ? "Data" : "Content";

        switch ( SelectedElement )
        {
            case ReportTextElementDefinition textElement:
                properties.Add( new PropertyGridTextProperty( "element.text", "Text", textElement.Text )
                {
                    Mixed = IsSelectedElementValueMixed( element => ( (ReportTextElementDefinition)element ).Text ),
                } );
                break;
            case ReportFieldElementDefinition fieldElement:
                if ( !MultipleElementsSelected )
                {
                    properties.Add( new PropertyGridTextProperty(
                        "element.expression",
                        "Expression",
                        ReportExpressionFormatter.FormatFieldExpression( Definition, SelectedElement ) )
                    {
                        ReadOnly = true,
                    } );

                    if ( fieldElement.Aggregate is not null )
                    {
                        properties.Add( new PropertyGridTextProperty(
                            "element.aggregate",
                            "Aggregate",
                            ReportAggregateResolver.GetFunctionDisplayName( fieldElement.Aggregate.Function ) )
                        {
                            ReadOnly = true,
                        } );
                    }
                }

                properties.Add( new PropertyGridTextProperty(
                    "element.format",
                    "Format",
                    GetSharedSelectedElementValue( element => ReportFormatResolver.GetDisplayText( ( (ReportFieldElementDefinition)element ).Format ) ) )
                {
                    ReadOnly = true,
                    Action = new PropertyGridAction( "edit-format" )
                    {
                        Text = "Edit",
                        Title = "Edit format",
                    },
                } );
                break;
            case ReportImageElementDefinition imageElement:
                properties.Add( new PropertyGridTextProperty( "element.imageSource", "Source", imageElement.Source )
                {
                    Mixed = IsSelectedElementValueMixed( element => ( (ReportImageElementDefinition)element ).Source ),
                    Immediate = true,
                    Action = new PropertyGridAction( "upload-image" )
                    {
                        Visible = UploadImage,
                        Icon = IconName.Image,
                        Title = "Upload image",
                    },
                } );
                properties.Add( new PropertyGridSelectProperty<ReportImageFit>( "element.imageFit", "Fit", imageElement.Fit, ImageFitOptions )
                {
                    Mixed = IsSelectedElementValueMixed( element => ( (ReportImageElementDefinition)element ).Fit ),
                } );
                properties.Add( new PropertyGridTextProperty( "element.imageAltText", "Alt text", imageElement.Text )
                {
                    Mixed = IsSelectedElementValueMixed( element => ( (ReportImageElementDefinition)element ).Text ),
                } );
                break;
        }

        groups.Add( new PropertyGridGroupDefinition( "element.content", title, properties ) );
    }

    private void AddElementTableGroup( List<PropertyGridGroupDefinition> groups )
    {
        if ( !AllSelectedElementsAre<ReportTableElementDefinition>() )
            return;

        groups.Add( new PropertyGridGroupDefinition(
            "element.table",
            "Table",
            [
                new PropertyGridNumericProperty<double?>( "element.tableRows", "Rows", GetSharedSelectedElementValue( element => (double?)GetTableRowCount( element ) ) )
                {
                    Min = 1,
                    Step = TableCountStep,
                },
                new PropertyGridNumericProperty<double?>( "element.tableColumns", "Columns", GetSharedSelectedElementValue( element => (double?)GetTableColumnCount( element ) ) )
                {
                    Min = 1,
                    Step = TableCountStep,
                },
            ] ) );
    }

    private void AddElementSubreportGroup( List<PropertyGridGroupDefinition> groups )
    {
        if ( !AllSelectedElementsAre<ReportSubreportElementDefinition>()
            || SelectedElement is not ReportSubreportElementDefinition subreportElement )
        {
            return;
        }

        groups.Add( new PropertyGridGroupDefinition(
            "element.subreport",
            "Subreport",
            [
                new PropertyGridTextProperty(
                    "element.subreportDataSource",
                    "Data source",
                    GetDataSourceDisplayValue( subreportElement.DataSource, GetSelectedSubreportDataSourceDisplayName( subreportElement ) ) )
                {
                    Mixed = IsSelectedElementValueMixed( element => ( (ReportSubreportElementDefinition)element ).DataSource ),
                    ReadOnly = true,
                    Action = CreateAction( "select-data-source", IconName.Search, "Select data source" ),
                },
            ] ) );
    }

    private void AddElementPositionGroup( List<PropertyGridGroupDefinition> groups )
    {
        groups.Add( new PropertyGridGroupDefinition(
            "element.position",
            "Position and size",
            [
                new PropertyGridNumericProperty<double?>( "element.x", "X", GetSharedMeasurementValue( element => element.X ) )
                {
                    Max = FromPoints( Definition.Page.Width ),
                    Step = GetMeasurementStep(),
                },
                new PropertyGridNumericProperty<double?>( "element.y", "Y", GetSharedMeasurementValue( element => element.Y ) )
                {
                    Max = FromPoints( FormulaSection?.Height ?? Definition.Page.Height ),
                    Step = GetMeasurementStep(),
                },
                new PropertyGridNumericProperty<double?>( "element.width", "Width", GetSharedMeasurementValue( element => element.Width ) )
                {
                    Min = FromPoints( ReportLayoutGeometry.DefaultMinimumElementSize ),
                    Max = FromPoints( Definition.Page.Width ),
                    Step = GetMeasurementStep(),
                },
                new PropertyGridNumericProperty<double?>( "element.height", "Height", GetSharedMeasurementValue( element => element.Height ) )
                {
                    Min = FromPoints( ReportLayoutGeometry.GetMinimumElementHeight( SelectedElement ) ),
                    Max = FromPoints( FormulaSection?.Height ?? Definition.Page.Height ),
                    Step = GetMeasurementStep(),
                },
            ] ) );
    }

    private void AddElementTextGroup( List<PropertyGridGroupDefinition> groups )
    {
        if ( !AllSelectedElementsSupportTextFormatting )
            return;

        ReportFontDefinition font = ReportElementDefinitionHelper.EnsureFont( SelectedElement );

        groups.Add( new PropertyGridGroupDefinition(
            "element.textFormatting",
            "Text",
            [
                new PropertyGridStringSelectProperty( "element.fontFamily", "Font family", GetFontFamilyValue( font.Family ), GetFontFamilyOptions() )
                {
                    Mixed = IsSelectedElementValueMixed( element => GetFontFamilyValue( element.Font?.Family ) ),
                },
                new PropertyGridNumericProperty<double?>( "element.fontSize", "Font size", GetSharedSelectedElementValue( element => element.Font?.Size ) )
                {
                    Min = 1,
                    Max = Definition.Page.Height,
                },
                new PropertyGridColorProperty( "element.fontColor", "Color", GetPropertyGridColorValue( font.Color ) )
                {
                    Mixed = IsSelectedElementValueMixed( element => element.Font?.Color ?? ReportColor.Default ),
                },
                new PropertyGridSelectProperty<TextAlignment>( "element.horizontalAlignment", "Hor. align", font.Alignment, TextAlignmentOptions )
                {
                    Mixed = IsSelectedElementValueMixed( element => element.Font?.Alignment ?? TextAlignment.Default ),
                },
                new PropertyGridSelectProperty<VerticalAlignment>( "element.verticalAlignment", "Vert. align", font.VerticalAlignment, TextVerticalAlignmentOptions )
                {
                    Mixed = IsSelectedElementValueMixed( element => element.Font?.VerticalAlignment ?? VerticalAlignment.Default ),
                },
                new PropertyGridBooleanProperty( "element.bold", "Bold", font.Bold )
                {
                    Mixed = IsSelectedElementValueMixed( element => element.Font?.Bold == true ),
                },
                new PropertyGridBooleanProperty( "element.italic", "Italic", font.Italic )
                {
                    Mixed = IsSelectedElementValueMixed( element => element.Font?.Italic == true ),
                },
                new PropertyGridBooleanProperty( "element.underline", "Underline", font.Underline )
                {
                    Mixed = IsSelectedElementValueMixed( element => element.Font?.Underline == true ),
                },
            ] ) );
    }

    private void AddElementAppearanceGroup( List<PropertyGridGroupDefinition> groups )
    {
        ReportAppearanceDefinition appearance = ReportElementDefinitionHelper.EnsureAppearance( SelectedElement );
        ReportBorderDefinition border = ReportElementDefinitionHelper.EnsureBorder( SelectedElement );
        List<PropertyGridProperty> properties = [];

        if ( AllSelectedElementsAre<ReportLineElementDefinition>() && SelectedElement is ReportLineElementDefinition lineElement )
        {
            properties.Add( new PropertyGridSelectProperty<Orientation>( "element.lineOrientation", "Orientation", lineElement.Orientation, LineOrientationOptions )
            {
                Mixed = IsSelectedElementValueMixed( element => ( (ReportLineElementDefinition)element ).Orientation ),
            } );
            properties.Add( new PropertyGridColorProperty( "element.lineColor", "Color", GetPropertyGridColorValue( border.Color ) )
            {
                Mixed = IsSelectedElementValueMixed( element => element.Border?.Color ?? ReportColor.Default ),
            } );
            properties.Add( new PropertyGridNumericProperty<double?>( "element.lineThickness", "Thickness", GetSharedSelectedElementValue( GetLineThickness ) )
            {
                Min = ReportLayoutGeometry.DefaultLineThickness,
                Max = Math.Min( Definition.Page.Width, Definition.Page.Height ),
            } );
        }
        else if ( !AnySelectedElementIsLine )
        {
            properties.Add( new PropertyGridColorProperty( "element.fillColor", "Fill", GetPropertyGridColorValue( appearance.BackgroundColor ) )
            {
                Mixed = IsSelectedElementValueMixed( element => element.Appearance?.BackgroundColor ?? ReportColor.Default ),
            } );
            properties.Add( new PropertyGridColorProperty( "element.borderColor", "Border color", GetPropertyGridColorValue( border.Color ) )
            {
                Mixed = IsSelectedElementValueMixed( element => element.Border?.Color ?? ReportColor.Default ),
            } );
            properties.Add( new PropertyGridNumericProperty<double?>( "element.borderWidth", "Border width", GetSharedSelectedElementValue( element => element.Border?.Width ) )
            {
                Max = Definition.Page.Width,
            } );
            properties.Add( new PropertyGridSelectProperty<ReportBorderStyle>( "element.borderStyle", "Border style", border.Style, BorderStyleOptions )
            {
                Mixed = IsSelectedElementValueMixed( element => element.Border?.Style ?? ReportBorderStyle.Default ),
            } );
            properties.Add( new PropertyGridNumericProperty<double?>( "element.cornerRadius", "Corner radius", GetSharedSelectedElementValue( element => element.Border?.Radius ) )
            {
                Max = Definition.Page.Width,
            } );
        }
        else
        {
            properties.Add( new PropertyGridColorProperty( "element.borderColor", "Border color", GetPropertyGridColorValue( border.Color ) )
            {
                Mixed = IsSelectedElementValueMixed( element => element.Border?.Color ?? ReportColor.Default ),
            } );
        }

        properties.Add( new PropertyGridNumericProperty<double?>( "element.opacity", "Opacity", GetSharedSelectedElementValue( element => element.Appearance?.Opacity ) )
        {
            Max = 1,
        } );

        groups.Add( new PropertyGridGroupDefinition( "element.appearance", "Appearance", properties ) );
    }

    private void AddCellPropertyGroups( List<PropertyGridGroupDefinition> groups )
    {
        if ( SelectedCell is null )
            return;

        groups.Add( new PropertyGridGroupDefinition(
            "cell.position",
            "Position",
            [
                new PropertyGridTextProperty( "cell.row", "Row", ( SelectedCell.RowIndex + 1 ).ToString() )
                {
                    ReadOnly = true,
                },
                new PropertyGridTextProperty( "cell.column", "Column", ( SelectedCell.ColumnIndex + 1 ).ToString() )
                {
                    ReadOnly = true,
                },
            ] ) );

        groups.Add( new PropertyGridGroupDefinition(
            "cell.span",
            "Span",
            [
                new PropertyGridTextProperty( "cell.rowSpan", "Row span", SelectedCell.RowSpan.ToString() )
                {
                    ReadOnly = true,
                },
                new PropertyGridTextProperty( "cell.columnSpan", "Column span", SelectedCell.ColumnSpan.ToString() )
                {
                    ReadOnly = true,
                },
            ] ) );
    }

    private static PropertyGridAction CreateFormulaAction( string formula )
        => new( "edit-formula" )
        {
            Icon = IconName.Code,
            Title = "Edit formula",
            Color = GetFormulaActionColor( formula ),
        };

    private static PropertyGridAction CreateAction( string name, object icon, string title )
        => new( name )
        {
            Icon = icon,
            Title = title,
        };

    private Task OnPropertyGridPropertyValueChanged( PropertyGridValueChangedEventArgs eventArgs )
    {
        return eventArgs.PropertyKey switch
        {
            "report.snapToGrid" => OnSnapToGridChanged( eventArgs.GetValue<bool>() ),
            "report.gridSize" => OnGridSizeChanged( eventArgs.GetValue<double>() ),
            "report.showRulers" => ShowRulersChanged.InvokeAsync( eventArgs.GetValue<bool>() ),
            "report.showFineRulerTicks" => ShowFineRulerTicksChanged.InvokeAsync( eventArgs.GetValue<bool>() ),
            "report.showCursorGuides" => ShowCursorGuidesChanged.InvokeAsync( eventArgs.GetValue<bool>() ),
            "report.showCollisionWarnings" => ShowCollisionWarningsChanged.InvokeAsync( eventArgs.GetValue<bool>() ),
            "report.bandMode" => BandModeChanged.InvokeAsync( eventArgs.GetValue<ReportBandMode>() ),
            "page.name" => UpdateReportPage( page => page.Name = eventArgs.GetValue<string>() ),
            "page.unit" => OnPageMeasurementUnitChanged( eventArgs.GetValue<ReportMeasurementUnit>() ),
            "page.size" => OnPageSizeChanged( eventArgs.GetValue<ReportPageSize>() ),
            "page.orientation" => OnPageOrientationChanged( eventArgs.GetValue<ReportOrientation>() ),
            "page.width" => OnPageWidthChanged( eventArgs.GetValue<double>() ),
            "page.height" => OnPageHeightChanged( eventArgs.GetValue<double>() ),
            "page.marginLeft" => OnPageMarginLeftChanged( eventArgs.GetValue<double>() ),
            "page.marginTop" => OnPageMarginTopChanged( eventArgs.GetValue<double>() ),
            "page.marginRight" => OnPageMarginRightChanged( eventArgs.GetValue<double>() ),
            "page.marginBottom" => OnPageMarginBottomChanged( eventArgs.GetValue<double>() ),
            "section.suppress" => UpdateSelectedSectionSuppression( eventArgs.GetValue<bool>() ),
            "section.reserveSpaceWhenSuppressed" => UpdateSelectedSection( section => section.ReserveSpaceWhenSuppressed = eventArgs.GetValue<bool>() ),
            "section.name" => UpdateSelectedSection( section => section.Name = eventArgs.GetValue<string>() ),
            "section.groupBy" => UpdateSelectedSection( section => section.GroupBy = eventArgs.GetValue<string>() ),
            "section.height" => OnSelectedSectionHeightChanged( eventArgs.GetValue<double>() ),
            "section.keepTogether" => UpdateSelectedSectionKeepTogether( eventArgs.GetValue<bool>() ),
            "section.newPageBefore" => UpdateSelectedSectionNewPageBefore( eventArgs.GetValue<bool>() ),
            "section.newPageAfter" => UpdateSelectedSectionNewPageAfter( eventArgs.GetValue<bool>() ),
            "section.printOnFirstPage" => UpdateSelectedSection( section => section.PrintOnFirstPage = eventArgs.GetValue<bool>() ),
            "section.printOnLastPage" => UpdateSelectedSection( section => section.PrintOnLastPage = eventArgs.GetValue<bool>() ),
            "section.repeatOnEveryPage" => UpdateSelectedSection( section => section.RepeatOnEveryPage = eventArgs.GetValue<bool>() ),
            "section.fillColor" => UpdateSelectedSection( section => EnsureSectionAppearance( section ).BackgroundColor = ReportColor.FromString( eventArgs.GetValue<string>() ) ),
            "section.borderColor" => UpdateSelectedSection( section => EnsureSectionBorder( section ).Color = ReportColor.FromString( eventArgs.GetValue<string>() ) ),
            "section.borderWidth" => UpdateSelectedSection( section => EnsureSectionBorder( section ).Width = ReportElementDefinitionHelper.NormalizeNullablePositiveNumber( eventArgs.GetValue<double?>() ) ),
            "section.cornerRadius" => UpdateSelectedSection( section => EnsureSectionBorder( section ).Radius = ReportElementDefinitionHelper.NormalizeNullablePositiveNumber( eventArgs.GetValue<double?>() ) ),
            "section.opacity" => UpdateSelectedSection( section => EnsureSectionAppearance( section ).Opacity = ReportElementDefinitionHelper.NormalizeOpacity( eventArgs.GetValue<double?>() ) ),
            "element.canGrow" => UpdateSelectedElementCanGrow( eventArgs.GetValue<bool>() ),
            "element.suppress" => UpdateSelectedElementSuppress( eventArgs.GetValue<bool>() ),
            "element.showCollisionWarnings" => UpdateSelectedElement( element => element.ShowCollisionWarnings = eventArgs.GetValue<bool>() ),
            "element.snapToGrid" => OnSelectedElementSnapToGridChanged( eventArgs.GetValue<string>() ),
            "element.name" => OnSelectedElementNameChanged( eventArgs.GetValue<string>() ),
            "element.text" => UpdateSelectedElementText( eventArgs.GetValue<string>() ),
            "element.imageSource" => UpdateSelectedElementImageSource( eventArgs.GetValue<string>() ),
            "element.imageFit" => UpdateSelectedElementImageFit( eventArgs.GetValue<ReportImageFit>() ),
            "element.imageAltText" => UpdateSelectedElementImageAltText( eventArgs.GetValue<string>() ),
            "element.tableRows" => OnSelectedTableRowCountChanged( eventArgs.GetValue<double?>() ),
            "element.tableColumns" => OnSelectedTableColumnCountChanged( eventArgs.GetValue<double?>() ),
            "element.x" => OnSelectedElementXChanged( eventArgs.GetValue<double?>() ),
            "element.y" => OnSelectedElementYChanged( eventArgs.GetValue<double?>() ),
            "element.width" => OnSelectedElementWidthChanged( eventArgs.GetValue<double?>() ),
            "element.height" => OnSelectedElementHeightChanged( eventArgs.GetValue<double?>() ),
            "element.fontFamily" => OnSelectedElementFontFamilyChanged( eventArgs.GetValue<string>() ),
            "element.fontSize" => UpdateSelectedElement( element => ReportElementDefinitionHelper.EnsureFont( element ).Size = ReportElementDefinitionHelper.NormalizeNullablePositiveNumber( eventArgs.GetValue<double?>() ) ),
            "element.fontColor" => UpdateSelectedElement( element => ReportElementDefinitionHelper.EnsureFont( element ).Color = ReportColor.FromString( eventArgs.GetValue<string>() ) ),
            "element.horizontalAlignment" => UpdateSelectedElement( element => ReportElementDefinitionHelper.EnsureFont( element ).Alignment = eventArgs.GetValue<TextAlignment>() ),
            "element.verticalAlignment" => UpdateSelectedElement( element => ReportElementDefinitionHelper.EnsureFont( element ).VerticalAlignment = eventArgs.GetValue<VerticalAlignment>() ),
            "element.bold" => UpdateSelectedElement( element => ReportElementDefinitionHelper.EnsureFont( element ).Bold = eventArgs.GetValue<bool>() ),
            "element.italic" => UpdateSelectedElement( element => ReportElementDefinitionHelper.EnsureFont( element ).Italic = eventArgs.GetValue<bool>() ),
            "element.underline" => UpdateSelectedElement( element => ReportElementDefinitionHelper.EnsureFont( element ).Underline = eventArgs.GetValue<bool>() ),
            "element.lineOrientation" => OnSelectedLineOrientationChanged( eventArgs.GetValue<Orientation>() ),
            "element.lineColor" => UpdateSelectedElement( element => ReportElementDefinitionHelper.EnsureBorder( element ).Color = ReportColor.FromString( eventArgs.GetValue<string>() ) ),
            "element.lineThickness" => OnSelectedLineThicknessChanged( eventArgs.GetValue<double?>() ),
            "element.fillColor" => UpdateSelectedElement( element => ReportElementDefinitionHelper.EnsureAppearance( element ).BackgroundColor = ReportColor.FromString( eventArgs.GetValue<string>() ) ),
            "element.borderColor" => UpdateSelectedElement( element => ReportElementDefinitionHelper.EnsureBorder( element ).Color = ReportColor.FromString( eventArgs.GetValue<string>() ) ),
            "element.borderWidth" => UpdateSelectedElement( element => ReportElementDefinitionHelper.EnsureBorder( element ).Width = ReportElementDefinitionHelper.NormalizeNullablePositiveNumber( eventArgs.GetValue<double?>() ) ),
            "element.borderStyle" => UpdateSelectedElement( element => ReportElementDefinitionHelper.EnsureBorder( element ).Style = eventArgs.GetValue<ReportBorderStyle>() ),
            "element.cornerRadius" => UpdateSelectedElement( element => ReportElementDefinitionHelper.EnsureBorder( element ).Radius = ReportElementDefinitionHelper.NormalizeNullablePositiveNumber( eventArgs.GetValue<double?>() ) ),
            "element.opacity" => UpdateSelectedElement( element => ReportElementDefinitionHelper.EnsureAppearance( element ).Opacity = ReportElementDefinitionHelper.NormalizeOpacity( eventArgs.GetValue<double?>() ) ),
            _ => Task.CompletedTask,
        };
    }

    private Task OnPropertyGridActionInvoked( PropertyGridActionEventArgs eventArgs )
    {
        return eventArgs.PropertyKey switch
        {
            "section.suppress" => OpenSelectedSectionSuppressFormula(),
            "section.dataSource" => OpenDataSourceDialog(),
            "section.keepTogether" => OpenSelectedSectionKeepTogetherFormula(),
            "section.newPageBefore" => OpenSelectedSectionNewPageBeforeFormula(),
            "section.newPageAfter" => OpenSelectedSectionNewPageAfterFormula(),
            "element.canGrow" => OpenSelectedElementCanGrowFormula(),
            "element.suppress" => OpenSelectedElementSuppressFormula(),
            "element.snapToGrid" => OpenSelectedElementSnapToGridFormula(),
            "element.format" => OpenFormatDialog(),
            "element.imageSource" => OpenImageUploadDialog(),
            "element.subreportDataSource" => OpenSelectedSubreportDataSourceDialog(),
            _ => Task.CompletedTask,
        };
    }

    private Task UpdateSelectedElementText( string value )
        => UpdateSelectedElement( element =>
        {
            if ( element is ReportTextElementDefinition textElement )
                textElement.Text = value;
        } );

    private Task UpdateSelectedElementImageSource( string value )
        => UpdateSelectedElement( element =>
        {
            if ( element is ReportImageElementDefinition imageElement )
                imageElement.Source = value;
        } );

    private Task UpdateSelectedElementImageFit( ReportImageFit value )
        => UpdateSelectedElement( element =>
        {
            if ( element is ReportImageElementDefinition imageElement )
                imageElement.Fit = value;
        } );

    private Task UpdateSelectedElementImageAltText( string value )
        => UpdateSelectedElement( element =>
        {
            if ( element is ReportImageElementDefinition imageElement )
                imageElement.Text = value;
        } );

    #endregion

    #region Properties

    private PropertyGridSchema PropertyGridSchema => propertyGridSchema ??= BuildPropertyGridSchema();

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
    /// Registered custom report elements.
    /// </summary>
    [Parameter] public IReportElementPluginRegistry ElementPluginRegistry { get; set; }

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