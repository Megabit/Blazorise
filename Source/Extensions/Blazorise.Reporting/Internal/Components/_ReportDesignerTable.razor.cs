#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders a report layout table element on the designer or viewer surface.
/// </summary>
public partial class _ReportDesignerTable
{
    #region Members

    private static readonly IReadOnlyList<ReportTableColumnDefinition> DefaultColumns =
    [
        new()
        {
            Width = 90,
        },
        new()
        {
            Width = 90,
        },
    ];

    private static readonly IReadOnlyList<ReportTableRowDefinition> DefaultRows =
    [
        new()
        {
            Height = 24,
        },
        new()
        {
            Height = 24,
        },
    ];

    private readonly StyleBuilder tableStyleBuilder;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the table renderer.
    /// </summary>
    public _ReportDesignerTable()
    {
        tableStyleBuilder = new( BuildTableStyles );
    }

    #endregion

    #region Methods

    private string TableStyle => tableStyleBuilder.Styles;

    private string TableElementKey => ReportDefinitionHelper.EnsureElementId( Element );

    private IReadOnlyList<ReportTableColumnDefinition> Columns => Element?.Columns?.Count > 0 ? Element.Columns : DefaultColumns;

    private IReadOnlyList<ReportTableRowDefinition> Rows => Element?.Rows?.Count > 0 ? Element.Rows : DefaultRows;

    /// <inheritdoc />
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.TryGetValue<ReportTableElementDefinition>( nameof( Element ), out _ ) )
            tableStyleBuilder.Dirty();

        return base.SetParametersAsync( parameters );
    }

    private void BuildTableStyles( StyleBuilder builder )
    {
        builder.Append( $"grid-template-columns:{BuildGridTemplateColumns()}" );
        builder.Append( $"grid-template-rows:{BuildGridTemplateRows()}" );
    }

    private string BuildGridTemplateColumns()
    {
        return string.Join( " ", Columns.Select( column => ReportMeasurementConverter.ToCssPixelString( Math.Max( 1, column.Width ) ) ) );
    }

    private string BuildGridTemplateRows()
    {
        return string.Join( " ", Rows.Select( row => ReportMeasurementConverter.ToCssPixelString( Math.Max( 1, row.Height ) ) ) );
    }

    private IEnumerable<ReportTableCellDefinition> GetCells()
    {
        IReadOnlyList<ReportTableCellDefinition> cells = Element?.Cells?.Count > 0
            ? Element.Cells
            : CreateDefaultCells();

        return cells
            .Where( cell => IsValidCell( cell ) )
            .OrderBy( cell => cell.RowIndex )
            .ThenBy( cell => cell.ColumnIndex );
    }

    private IReadOnlyList<ReportTableCellDefinition> CreateDefaultCells()
    {
        List<ReportTableCellDefinition> cells = [];

        for ( int rowIndex = 0; rowIndex < Rows.Count; rowIndex++ )
        {
            for ( int columnIndex = 0; columnIndex < Columns.Count; columnIndex++ )
            {
                cells.Add( new()
                {
                    RowIndex = rowIndex,
                    ColumnIndex = columnIndex,
                } );
            }
        }

        return cells;
    }

    private bool IsValidCell( ReportTableCellDefinition cell )
    {
        return cell is not null
            && cell.RowIndex >= 0
            && cell.ColumnIndex >= 0
            && cell.RowIndex < Rows.Count
            && cell.ColumnIndex < Columns.Count;
    }

    private string GetCellStyle( ReportTableCellDefinition cell )
    {
        StyleBuilder builder = new( styleBuilder =>
        {
            int rowSpan = Math.Clamp( cell.RowSpan, 1, Rows.Count - cell.RowIndex );
            int columnSpan = Math.Clamp( cell.ColumnSpan, 1, Columns.Count - cell.ColumnIndex );

            styleBuilder.Append( $"grid-row:{( cell.RowIndex + 1 ).ToString( CultureInfo.InvariantCulture )} / span {rowSpan.ToString( CultureInfo.InvariantCulture )}" );
            styleBuilder.Append( $"grid-column:{( cell.ColumnIndex + 1 ).ToString( CultureInfo.InvariantCulture )} / span {columnSpan.ToString( CultureInfo.InvariantCulture )}" );
        } );

        return builder.Styles;
    }

    private string GetCellClass( ReportTableCellDefinition cell )
    {
        ClassBuilder builder = new( classBuilder =>
        {
            classBuilder.Append( "b-report-table-cell" );
            classBuilder.Append( "active", DesignMode && string.Equals( SelectedCellKey, cell.Id, StringComparison.Ordinal ) );
            classBuilder.Append( "table-active", DesignMode && TableSelected );
        } );

        return builder.Class;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Report data used when resolving nested field values.
    /// </summary>
    [Parameter] public object Data { get; set; }

    /// <summary>
    /// Report definition that owns the table.
    /// </summary>
    [Parameter] public ReportDefinition Definition { get; set; }

    /// <summary>
    /// Report band that owns the table.
    /// </summary>
    [Parameter] public ReportBandDefinition Section { get; set; }

    /// <summary>
    /// Report section index rendered on the designer surface.
    /// </summary>
    [Parameter] public int SectionIndex { get; set; }

    /// <summary>
    /// Current band item used for repeated detail rendering.
    /// </summary>
    [Parameter] public object Item { get; set; }

    /// <summary>
    /// Running total values available at the current render position.
    /// </summary>
    [Parameter] public IReadOnlyDictionary<string, object> RunningTotals { get; set; }

    /// <summary>
    /// Table element definition rendered in the designer or viewer.
    /// </summary>
    [Parameter] public ReportTableElementDefinition Element { get; set; }

    /// <summary>
    /// Indicates that the table is rendered on the designer surface.
    /// </summary>
    [Parameter] public bool DesignMode { get; set; }

    /// <summary>
    /// Allows the table to receive designer interactions.
    /// </summary>
    [Parameter] public bool Editable { get; set; }

    /// <summary>
    /// Indicates that the table element is part of the current selection.
    /// </summary>
    [Parameter] public bool TableSelected { get; set; }

    /// <summary>
    /// Identifier of the selected table cell.
    /// </summary>
    [Parameter] public string SelectedCellKey { get; set; }

    /// <summary>
    /// Raised when a table cell is clicked.
    /// </summary>
    [Parameter] public EventCallback<ReportDesignerSelectionMouseEventArgs> CellClicked { get; set; }

    /// <summary>
    /// Raised when a table cell context menu is requested.
    /// </summary>
    [Parameter] public Func<int, string, MouseEventArgs, Task> CellContextMenu { get; set; }

    /// <summary>
    /// Determines whether a nested table cell element is selected.
    /// </summary>
    [Parameter] public Func<string, bool> IsElementSelected { get; set; }

    /// <summary>
    /// Determines whether a nested table cell element overlaps a sibling element.
    /// </summary>
    [Parameter] public Func<string, bool> IsElementColliding { get; set; }

    /// <summary>
    /// Raised when a nested table cell element is clicked.
    /// </summary>
    [Parameter] public EventCallback<ReportDesignerSelectionMouseEventArgs> ElementClicked { get; set; }

    /// <summary>
    /// Raised when a nested table cell element is double-clicked.
    /// </summary>
    [Parameter] public Func<string, MouseEventArgs, Task> ElementDoubleClicked { get; set; }

    /// <summary>
    /// Indicates that a text element is currently edited directly on the designer surface.
    /// </summary>
    [Parameter] public bool TextEditingActive { get; set; }

    /// <summary>
    /// Identifier of the nested table cell element currently edited.
    /// </summary>
    [Parameter] public string EditingElementKey { get; set; }

    /// <summary>
    /// Raised when inline text editing commits a nested table cell element value.
    /// </summary>
    [Parameter] public Func<string, string, Task> ElementTextEditCommitted { get; set; }

    /// <summary>
    /// Raised when inline text editing is cancelled for a nested table cell element.
    /// </summary>
    [Parameter] public Func<string, Task> ElementTextEditCancelled { get; set; }

    /// <summary>
    /// Raised when a table row or column resize starts.
    /// </summary>
    [Parameter] public Func<string, string, ReportTableResizeKind, int, PointerEventArgs, Task> ResizeStarted { get; set; }

    #endregion
}