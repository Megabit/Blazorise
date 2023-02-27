﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.DataGrid.Utils;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid;

public partial class DataGridColumn<TItem> : BaseDataGridColumn<TItem>
{
    #region Members

    private readonly Lazy<Func<Type>> valueTypeGetter;
    private readonly Lazy<Func<object>> defaultValueByType;
    private readonly Lazy<Func<TItem, object>> valueGetter;
    private readonly Lazy<Action<TItem, object>> valueSetter;
    private readonly Lazy<Func<TItem, object>> sortFieldGetter;

    private Dictionary<DataGridSortMode, SortDirection> currentSortDirection { get; set; } = new();

    #endregion

    #region Constructors

    public DataGridColumn()
    {
        // TODO: move this to cached FunctionCompiler so it doesn't get compiled every time
        valueTypeGetter = new( () => FunctionCompiler.CreateValueTypeGetter<TItem>( Field ) );
        defaultValueByType = new( () => FunctionCompiler.CreateDefaultValueByType<TItem>( Field ) );
        valueGetter = new( () => FunctionCompiler.CreateValueGetter<TItem>( Field ) );
        valueSetter = new( () => FunctionCompiler.CreateValueSetter<TItem>( Field ) );
        sortFieldGetter = new( () => FunctionCompiler.CreateValueGetter<TItem>( SortField ) );
    }

    #endregion

    #region Methods

    protected override void OnInitialized()
    {
        base.OnInitialized();

        currentSortDirection[DataGridSortMode.Single] = SortDirection;
        currentSortDirection[DataGridSortMode.Multiple] = SortDirection;

        if ( ParentDataGrid is not null )
        {
            ParentDataGrid.AddColumn( this, true );

            Filter?.Subscribe( OnSearchValueChanged );
        }
    }

    /// <inheritdoc/>
    protected override ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
        {
            DisposeSubscriptions();
        }

        return base.DisposeAsync( disposing );
    }

    private void DisposeSubscriptions()
    {
        ParentDataGrid.RemoveColumn( this );

        if ( Filter is not null )
        {
            Filter.Unsubscribe( OnSearchValueChanged );

            Filter = null;
        }
    }

    public async void OnSearchValueChanged( object filterValue )
    {
        await ParentDataGrid.OnFilterChanged( this, filterValue );
    }

    /// <summary>
    /// Gets the typeof() of the value associated with this column field.
    /// </summary>
    /// <returns></returns>
    internal Type GetValueType()
        => !string.IsNullOrEmpty( Field )
            ? valueTypeGetter.Value()
            : default;

    /// <summary>
    /// Gets default value based on the typeof() of the value associated with this column field.
    /// </summary>
    /// <returns></returns>
    internal object GetDefaultValueByType()
        => defaultValueByType.Value();

    /// <summary>
    /// Gets the current value for the field in the supplied model.
    /// </summary>
    /// <param name="item">Item for which to get the value.</param>
    /// <returns></returns>
    internal object GetValue( TItem item )
        => !string.IsNullOrEmpty( Field )
            ? valueGetter.Value( item )
            : default;

    /// <summary>
    /// Sets the value for the field in the supplied model.
    /// </summary>
    /// <param name="item">Item for which to set the value.</param>
    /// <param name="value">Value to set.</param>
    internal void SetValue( TItem item, object value )
    {
        if ( !string.IsNullOrEmpty( Field ) )
            valueSetter.Value( item, value );
    }

    /// <summary>
    /// Gets the current value for the sort field in the supplied model.
    /// </summary>
    /// <param name="item">Item for which to get the value.</param>
    /// <returns></returns>
    internal object GetSortValue( TItem item )
        => sortFieldGetter.Value( item );

    /// <summary>
    /// Gets the current value to be used for sorting.
    /// </summary>
    /// <param name="item">Item for which to get the value.</param>
    /// <returns></returns>
    internal object GetValueForSort( TItem item )
        => string.IsNullOrWhiteSpace( SortField )
            ? GetValue( item )
            : GetSortValue( item );

    /// <summary>
    /// Gets wether the column is able to sort.
    /// </summary>
    /// <returns></returns>
    internal bool CanSort()
        => Sortable && ( !string.IsNullOrEmpty( GetFieldToSort() ) );

    /// <summary>
    /// Gets the field to be used for Sorting.
    /// </summary>
    /// <returns></returns>
    internal string GetFieldToSort()
        => string.IsNullOrEmpty( SortField ) ? Field : SortField;

    /// <summary>
    /// Gets the GroupBy Func to be applied.
    /// </summary>
    /// <returns></returns>
    internal Func<TItem, object> GetGroupByFunc()
        => GroupBy is not null ? GroupBy : valueGetter.Value;

    public string FormatDisplayValue( TItem item )
    {
        return FormatDisplayValue( GetValue( item ) );
    }

    public bool CellValuesAreEditable()
    {
        return Editable &&
               ( ( CellsEditableOnNewCommand && ParentDataGrid.EditState == DataGridEditState.New )
                 || ( CellsEditableOnEditCommand && ParentDataGrid.EditState == DataGridEditState.Edit ) );
    }

    internal string BuildHeaderCellStyle()
    {
        var sb = new StringBuilder();

        if ( !string.IsNullOrEmpty( HeaderCellStyle ) )
            sb.Append( HeaderCellStyle );

        if ( Width != null )
            sb.Append( $"; width: {Width};" );

        return sb.ToString().TrimStart( ' ', ';' );
    }

    internal string BuildFilterCellStyle()
    {
        var sb = new StringBuilder();

        if ( !string.IsNullOrEmpty( FilterCellStyle ) )
            sb.Append( FilterCellStyle );

        if ( Width != null )
            sb.Append( $"; width: {Width};" );

        return sb.ToString().TrimStart( ' ', ';' );
    }

    internal string BuildAggregateCellStyle()
    {
        var sb = new StringBuilder();

        if ( !string.IsNullOrEmpty( AggregateCellStyle ) )
            sb.Append( AggregateCellStyle );

        if ( Width != null )
            sb.Append( $"; width: {Width};" );

        return sb.ToString().TrimStart( ' ', ';' );
    }

    internal string BuildCellStyle( TItem item )
    {
        var sb = new StringBuilder();

        var result = CellStyle?.Invoke( item );

        if ( !string.IsNullOrEmpty( result ) )
            sb.Append( result );

        if ( Width != null )
            sb.Append( $"; width: {Width}" );

        return sb.ToString().TrimStart( ' ', ';' );
    }

    internal Task ResetSortOrder()
        => SetSortOrder( default );

    internal Task SetSortOrder( int sortOrder )
    {
        SortOrder = sortOrder;
        return SortOrderChanged.InvokeAsync( sortOrder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Determines the text alignment for the filter cell.
    /// </summary>
    /// <returns>Text alignment value.</returns>
    internal TextAlignment FilterCellTextAlignment
       => FilterTextAlignment ?? TextAlignment;

    /// <summary>
    /// Determines the vertical alignment for the filter cell.
    /// </summary>
    /// <returns>Vertical alignment value.</returns>
    internal VerticalAlignment FilterCellVerticalAlignment
      => FilterVerticalAlignment ?? VerticalAlignment;

    /// <summary>
    /// Determines the display for the filter cell.
    /// </summary>
    /// <returns>Display value.</returns>
    internal IFluentDisplay FilterCellDisplay
        => FilterDisplay ?? Display;

    /// <summary>
    /// Determines the flex for the filter cell.
    /// </summary>
    /// <returns>Flex value.</returns>
    internal IFluentFlex FilterCellFlex
        => FilterFlex ?? Flex;

    /// <summary>
    /// Determines the gap for the filter cell.
    /// </summary>
    /// <returns>Gap value.</returns>
    internal IFluentGap FilterCellGap
        => FilterGap ?? Gap;

    /// <summary>
    /// Builds the Filter cell background.
    /// IsFixedHeader feature needs to apply background color to columns. This makes sure to syncronize with the DataGrid header styling helpers.
    /// </summary>
    /// <returns></returns>
    internal Background FilterCellBackground
        => ParentDataGrid.IsFixedHeader ? ( ParentDataGrid.FilterRowStyling?.Background ?? Background.Default ) : Background.Default;

    /// <summary>
    /// Determines the text alignment for the header cell.
    /// </summary>
    /// <returns>Text alignment value.</returns>
    internal TextAlignment HeaderCellTextAlignment
       => HeaderTextAlignment ?? TextAlignment;

    /// <summary>
    /// Determines the vertical alignment for the header cell.
    /// </summary>
    /// <returns>Vertical alignment value.</returns>
    internal VerticalAlignment HeaderCellVerticalAlignment
      => HeaderVerticalAlignment ?? VerticalAlignment;

    /// <summary>
    /// Determines the display for the header cell.
    /// </summary>
    /// <returns>Display value.</returns>
    internal IFluentDisplay HeaderCellDisplay
        => HeaderDisplay ?? Display;

    /// <summary>
    /// Determines the flex for the header cell.
    /// </summary>
    /// <returns>Flex value.</returns>
    internal IFluentFlex HeaderCellFlex
        => HeaderFlex ?? Flex;

    /// <summary>
    /// Determines the gap for the header cell.
    /// </summary>
    /// <returns>Gap value.</returns>
    internal IFluentGap HeaderCellGap
        => HeaderGap ?? Gap;

    /// <summary>
    /// Builds the Header cell background.
    /// </summary>
    /// <remarks>
    /// IsFixedHeader feature needs to apply background color to columns. This makes sure to syncronize with the DataGrid header styling helpers.
    /// </remarks>
    /// <returns>Background color.</returns>
    internal Background HeaderCellBackground
        => ParentDataGrid.IsFixedHeader ? ( ParentDataGrid.HeaderRowStyling?.Background ?? Background.Default ) : Background.Default;

    /// <summary>
    /// Determines the text alignment for the aggregate cell.
    /// </summary>
    /// <returns>Text alignment value.</returns>
    internal TextAlignment AggregateCellTextAlignment
       => AggregateTextAlignment ?? TextAlignment;

    /// <summary>
    /// Determines the vertical alignment for the aggregate cell.
    /// </summary>
    /// <returns>Vertical alignment value.</returns>
    internal VerticalAlignment AggregateCellVerticalAlignment
      => AggregateVerticalAlignment ?? VerticalAlignment;

    /// <summary>
    /// Determines the display for the aggregate cell.
    /// </summary>
    /// <returns>Display value.</returns>
    internal IFluentDisplay AggregateCellDisplay
        => AggregateDisplay ?? Display;

    /// <summary>
    /// Determines the flex for the aggregate cell.
    /// </summary>
    /// <returns>Flex value.</returns>
    internal IFluentFlex AggregateCellFlex
        => AggregateFlex ?? Flex;

    /// <summary>
    /// Determines the gap for the aggregate cell.
    /// </summary>
    /// <returns>Gap value.</returns>
    internal IFluentGap AggregateCellGap
        => AggregateGap ?? Gap;

    internal bool IsDisplayable => ( ColumnType == DataGridColumnType.Command && ParentDataGrid.EditMode == DataGridEditMode.Inline );

    internal bool ExcludeFromFilter => ColumnType == DataGridColumnType.Command || ColumnType == DataGridColumnType.MultiSelect;

    internal bool ExcludeFromEdit => ColumnType == DataGridColumnType.Command || ColumnType == DataGridColumnType.MultiSelect;

    internal bool ExcludeFromInit => ColumnType == DataGridColumnType.Command || ColumnType == DataGridColumnType.MultiSelect;

    /// <summary>
    /// Returns true if the cell value is editable.
    /// </summary>
    public bool CellValueIsEditable
        => Editable &&
           ( ( CellsEditableOnNewCommand && ParentDataGrid.EditState == DataGridEditState.New )
             || ( CellsEditableOnEditCommand && ParentDataGrid.EditState == DataGridEditState.Edit ) );

    /// <summary>
    /// Gets or sets the current sort direction.
    /// </summary>
    /// <remarks>
    /// The reason for this field is that <see cref="SortDirection"/> is reset every
    /// time when the grid is refreshed by the user.
    /// </remarks>
    public SortDirection CurrentSortDirection
    {
        get => currentSortDirection[ParentDataGrid.SortMode];
        internal set => currentSortDirection[ParentDataGrid.SortMode] = value;
    }

    /// <summary>
    /// Gets the type of column editor.
    /// </summary>
    public virtual DataGridColumnType ColumnType { get; } = DataGridColumnType.Text;

    public bool IsMultiSelectColumn => ColumnType == DataGridColumnType.MultiSelect;

    public bool IsCommandColumn => ColumnType == DataGridColumnType.Command;

    /// <summary>
    /// Gets or sets the column's display caption.
    /// </summary>
    [Parameter] public string Caption { get; set; }

    /// <summary>
    /// Gets or sets the column's display caption template.
    /// </summary>
    [Parameter] public RenderFragment<DataGridColumn<TItem>> CaptionTemplate { get; set; }

    /// <summary>
    /// Filter value for this column.
    /// </summary>
    [Parameter] public FilterContext<TItem> Filter { get; set; } = new();

    /// <summary>
    /// Custom filter function used to override internal filtering.
    /// </summary>
    [Parameter] public DataGridColumnCustomFilter CustomFilter { get; set; }

    /// <summary>
    /// Defines the alignment for column filter cell.
    /// </summary>
    [Parameter] public TextAlignment? FilterTextAlignment { get; set; }

    /// <summary>
    /// Defines the vertical alignment for column filter cell.
    /// </summary>
    [Parameter] public VerticalAlignment? FilterVerticalAlignment { get; set; }

    /// <summary>
    /// Specifies the display behavior of a filter cell.
    /// </summary>
    [Parameter] public IFluentDisplay FilterDisplay { get; set; }

    /// <summary>
    /// Specifies the flex utility of a filter cell.
    /// </summary>
    [Parameter] public IFluentFlex FilterFlex { get; set; }

    /// <summary>
    /// Specifies the gap utility of a filter cell.
    /// </summary>
    [Parameter] public IFluentGap FilterGap { get; set; }

    /// <summary>
    /// Gets or sets the column initial sort direction.
    /// </summary>
    [Parameter] public SortDirection SortDirection { get; set; }

    /// <summary>
    /// Gets or sets whether the sort direction will be reversed.
    /// </summary>
    [Parameter] public bool ReverseSorting { get; set; }

    /// <summary>
    /// Gets or sets the column's display sort direction template.
    /// </summary>
    [Parameter] public RenderFragment<SortDirection> SortDirectionTemplate { get; set; }

    /// <summary>
    /// Defines the alignment for display cell.
    /// </summary>
    [Parameter] public TextAlignment TextAlignment { get; set; }

    /// <summary>
    /// Defines the vertical alignment for display cell.
    /// </summary>
    [Parameter] public VerticalAlignment VerticalAlignment { get; set; }

    /// <summary>
    /// Specifies the display utility of a cell.
    /// </summary>
    [Parameter] public IFluentDisplay Display { get; set; }

    /// <summary>
    /// Specifies the flex utility of a cell.
    /// </summary>
    [Parameter] public IFluentFlex Flex { get; set; }

    /// <summary>
    /// Specifies the gap utility of a cell.
    /// </summary>
    [Parameter] public IFluentGap Gap { get; set; }

    /// <summary>
    /// Defines the alignment for column header cell.
    /// </summary>
    [Parameter] public TextAlignment? HeaderTextAlignment { get; set; }

    /// <summary>
    /// Defines the vertical alignment for column header cell.
    /// </summary>
    [Parameter] public VerticalAlignment? HeaderVerticalAlignment { get; set; }

    /// <summary>
    /// Specifies the display behavior of a header cell.
    /// </summary>
    [Parameter] public IFluentDisplay HeaderDisplay { get; set; }

    /// <summary>
    /// Specifies the flex utility of a header cell.
    /// </summary>
    [Parameter] public IFluentFlex HeaderFlex { get; set; }

    /// <summary>
    /// Specifies the gap utility of a header cell.
    /// </summary>
    [Parameter] public IFluentGap HeaderGap { get; set; }

    /// <summary>
    /// Gets or sets whether users can edit cell values under this column.
    /// </summary>
    [Parameter] public bool Editable { get; set; }

    /// <summary>
    /// Gets or sets whether column can be displayed on a grid.
    /// </summary>
    [Parameter] public bool Displayable { get; set; } = true;

    /// <summary>
    /// Gets or sets where column will be displayed on a grid.
    /// </summary>
    [Parameter] public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets where column will be displayed on edit row/popup.
    /// </summary>
    [Parameter] public int? EditOrder { get; set; }

    /// <summary>
    /// Allows the cell values to be entered while the grid is in the new-item state.
    /// </summary>
    [Parameter] public bool CellsEditableOnNewCommand { get; set; } = true;

    /// <summary>
    /// Allows the cell values to be entered while the grid is in the edit-item state.
    /// </summary>
    [Parameter] public bool CellsEditableOnEditCommand { get; set; } = true;

    /// <summary>
    /// Gets or sets whether end-users can sort data by the column's values.
    /// </summary>
    [Parameter] public bool Sortable { get; set; } = true;

    /// <summary>
    /// Gets or sets whether end-users are prevented from editing the column's cell values.
    /// </summary>
    [Parameter] public bool Readonly { get; set; }

    /// <summary>
    /// Gets or sets whether the column's caption is displayed within the column header.
    /// </summary>
    [Parameter] public bool ShowCaption { get; set; } = true;

    /// <summary>
    /// Gets or sets whether users can filter rows by its cell values.
    /// </summary>
    [Parameter] public bool Filterable { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the column is eligible to be used as a Group Field. A custom <see cref="GroupBy"/> function can however be provided.
    /// </summary>
    [Parameter] public bool Groupable { get; set; }

    /// <summary>
    /// Gets or sets whether the column should start grouped.
    /// </summary>
    [Parameter] public bool Grouping { get; set; }

    /// <summary>
    /// Gets or sets a custom GroupBy function. <see cref="Groupable"/> needs to be active.
    /// </summary>
    [Parameter] public Func<TItem, object> GroupBy { get; set; }

    /// <summary>
    /// The width of the column.
    /// </summary>
    [Parameter] public string Width { get; set; }

    /// <summary>
    /// Custom classname handler for cell based on the current row item.
    /// </summary>
    [Parameter] public Func<TItem, string> CellClass { get; set; }

    /// <summary>
    /// Custom style handler for cell based on the current row item.
    /// </summary>
    [Parameter] public Func<TItem, string> CellStyle { get; set; }

    /// <summary>
    /// Custom classname for header cell.
    /// </summary>
    [Parameter] public string HeaderCellClass { get; set; }

    /// <summary>
    /// Custom style for header cell.
    /// </summary>
    [Parameter] public string HeaderCellStyle { get; set; }

    /// <summary>
    /// Custom classname for filter cell.
    /// </summary>
    [Parameter] public string FilterCellClass { get; set; }

    /// <summary>
    /// Custom style for filter cell.
    /// </summary>
    [Parameter] public string FilterCellStyle { get; set; }

    /// <summary>
    /// Custom classname for the aggregate cell.
    /// </summary>
    [Obsolete( "DataGridColumn: The GroupCellClass parameter is deprecated, please use the AggregateCellClass parameter instead." )]
    [Parameter] public string GroupCellClass { get => AggregateCellClass; set => AggregateCellClass = value; }

    /// <summary>
    /// Custom style for the aggregate cell.
    /// </summary>
    [Obsolete( "DataGridColumn: The GroupCellStyle parameter is deprecated, please use the AggregateCellStyle parameter instead." )]
    [Parameter] public string GroupCellStyle { get => AggregateCellStyle; set => AggregateCellStyle = value; }

    /// <summary>
    /// Custom classname for the aggregate cell.
    /// </summary>
    [Parameter] public string AggregateCellClass { get; set; }

    /// <summary>
    /// Custom style for the aggregate cell.
    /// </summary>
    [Parameter] public string AggregateCellStyle { get; set; }

    /// <summary>
    /// Defines the alignment for column the aggregate cell.
    /// </summary>
    [Parameter] public TextAlignment? AggregateTextAlignment { get; set; }

    /// <summary>
    /// Defines the vertical alignment for column the aggregate cell.
    /// </summary>
    [Parameter] public VerticalAlignment? AggregateVerticalAlignment { get; set; }

    /// <summary>
    /// Specifies the display behavior of a the aggregate cell.
    /// </summary>
    [Parameter] public IFluentDisplay AggregateDisplay { get; set; }

    /// <summary>
    /// Specifies the flex utility of a the aggregate cell.
    /// </summary>
    [Parameter] public IFluentFlex AggregateFlex { get; set; }

    /// <summary>
    /// Specifies the gap utility of a the aggregate cell.
    /// </summary>
    [Parameter] public IFluentGap AggregateGap { get; set; }

    /// <summary>
    /// Template for custom cell display formatting.
    /// </summary>
    [Parameter] public RenderFragment<TItem> DisplayTemplate { get; set; }

    /// <summary>
    /// Template for custom column filter rendering.
    /// </summary>
    [Parameter] public RenderFragment<FilterContext<TItem>> FilterTemplate { get; set; }

    /// <summary>
    /// Defines the size of field for popup modal.
    /// </summary>
    [Obsolete( "DataGridColumn: PopupFieldColumnSize is deprecated and will be removed in the future version. Please use the EditFieldColumnSize instead." )]
    [Parameter] public IFluentColumn PopupFieldColumnSize { get; set; }

    /// <summary>
    /// Defines the size of an edit field for popup modal and edit form.
    /// </summary>
    [Parameter] public IFluentColumn EditFieldColumnSize { get; set; }

    /// <summary>
    /// Template for custom cell editing.
    /// </summary>
    [Parameter] public RenderFragment<CellEditContext<TItem>> EditTemplate { get; set; }

    /// <summary>
    /// Validates the input value after trying to save.
    /// </summary>
    [Parameter] public Action<ValidatorEventArgs> Validator { get; set; }

    /// <summary>
    /// Asynchronously validates the input value after trying to save.
    /// </summary>
    [Parameter] public Func<ValidatorEventArgs, CancellationToken, Task> AsyncValidator { get; set; }

    /// <summary>
    /// Forces validation to use regex pattern matching instead of default validator handler.
    /// </summary>
    [Parameter] public string ValidationPattern { get; set; }

    /// <summary>
    /// Provides a Sort Field to be used instead by the Sorting mechanism
    /// </summary>
    [Parameter] public string SortField { get; set; }

    /// <summary>
    /// Will set @onclick:StopProgration to true, stopping the RowClick and consequent events from triggering.
    /// </summary>
    [Parameter] public bool PreventRowClick { get; set; }

    /// <summary>
    /// Gets or sets the order for sorting when Sorting is set to multiple. 
    /// </summary>
    [Parameter] public int SortOrder { get; set; }

    /// <summary>
    /// Raises an event every time that <see cref="SortOrder"/> is changed.
    /// </summary>
    [Parameter] public EventCallback<int> SortOrderChanged { get; set; }

    /// <summary>
    /// Template for custom group.
    /// </summary>
    [Parameter] public RenderFragment<GroupContext<TItem>> GroupTemplate { get; set; }

    #endregion
}