#region Using directives
using System;
using System.Globalization;
using Blazorise.Gantt.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Gantt;

/// <summary>
/// Base class for all declarative Gantt tree columns.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
[CascadingTypeParameter( nameof( TItem ) )]
public abstract class BaseGanttColumn<TItem> : ComponentBase, IDisposable
{
    #region Members

    private readonly Lazy<Func<TItem, object>> valueGetter;
    private readonly Lazy<Func<TItem, object>> sortValueGetter;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new <see cref="BaseGanttColumn{TItem}"/>.
    /// </summary>
    protected BaseGanttColumn()
    {
        valueGetter = new( () => string.IsNullOrWhiteSpace( Field )
            ? null
            : GanttFunctionCompiler.CreateValueGetter<TItem>( Field ) );
        sortValueGetter = new( () =>
        {
            var fieldToSort = GetSortField();

            return string.IsNullOrWhiteSpace( fieldToSort )
                ? null
                : GanttFunctionCompiler.CreateValueGetter<TItem>( fieldToSort );
        } );
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Gantt?.AddColumn( this );

        base.OnInitialized();
    }

    /// <summary>
    /// Gets raw value for this column.
    /// </summary>
    public virtual object GetValue( TItem item )
    {
        if ( item is null || string.IsNullOrWhiteSpace( Field ) )
            return null;

        return valueGetter.Value?.Invoke( item );
    }

    /// <summary>
    /// Gets raw value used for sorting.
    /// </summary>
    public virtual object GetSortValue( TItem item )
    {
        if ( item is null )
            return null;

        return sortValueGetter.Value?.Invoke( item );
    }

    /// <summary>
    /// Gets formatted display text for this column.
    /// </summary>
    public virtual string FormatDisplayValue( object value )
    {
        if ( DisplayFormat is not null )
        {
            return string.Format( DisplayFormatProvider ?? CultureInfo.CurrentCulture, DisplayFormat, value );
        }

        return value?.ToString();
    }

    /// <summary>
    /// Gets field name to sort by.
    /// </summary>
    public string GetSortField()
        => string.IsNullOrWhiteSpace( SortField )
            ? Field
            : SortField;

    /// <summary>
    /// Returns true when column can be sorted.
    /// </summary>
    public bool CanSort()
        => Sortable && !string.IsNullOrWhiteSpace( GetSortField() );

    /// <inheritdoc />
    public void Dispose()
    {
        Gantt?.RemoveColumn( this );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Parent gantt component.
    /// </summary>
    [CascadingParameter] public Gantt<TItem> Gantt { get; set; }

    /// <summary>
    /// Field name bound to this column.
    /// </summary>
    [Parameter] public string Field { get; set; }

    /// <summary>
    /// Optional title shown in header.
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// Determines whether column is currently visible.
    /// </summary>
    [Parameter] public bool Visible { get; set; } = true;

    /// <summary>
    /// Determines whether column can be shown in column picker.
    /// </summary>
    [Parameter] public bool Displayable { get; set; } = true;

    /// <summary>
    /// Determines whether this column can be sorted.
    /// </summary>
    [Parameter] public bool Sortable { get; set; } = true;

    /// <summary>
    /// Specifies field used by sort operation. Defaults to <see cref="Field"/>.
    /// </summary>
    [Parameter] public string SortField { get; set; }

    /// <summary>
    /// Specifies width.
    /// </summary>
    [Parameter] public IFluentSizing Width { get; set; }

    /// <summary>
    /// Specifies text alignment for header and cell.
    /// </summary>
    [Parameter] public TextAlignment TextAlignment { get; set; } = TextAlignment.Default;

    /// <summary>
    /// Specifies display format for default display text.
    /// </summary>
    [Parameter] public string DisplayFormat { get; set; }

    /// <summary>
    /// Specifies display format provider.
    /// </summary>
    [Parameter] public IFormatProvider DisplayFormatProvider { get; set; }

    /// <summary>
    /// Determines whether this column should render tree expander.
    /// </summary>
    [Parameter] public bool Expandable { get; set; }

    /// <summary>
    /// Defines custom header template.
    /// </summary>
    [Parameter] public RenderFragment<GanttColumnHeaderContext<TItem>> HeaderTemplate { get; set; }

    /// <summary>
    /// Gets or sets custom display template.
    /// </summary>
    public RenderFragment<GanttColumnDisplayContext<TItem>> DisplayTemplate { get; set; }

    /// <summary>
    /// Defines custom edit template.
    /// </summary>
    [Parameter] public RenderFragment<GanttColumnEditContext<TItem>> EditTemplate { get; set; }

    #endregion
}