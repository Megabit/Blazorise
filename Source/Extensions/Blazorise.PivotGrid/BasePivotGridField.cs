#region Using directives
using System;
using System.Globalization;
using System.Threading.Tasks;
using Blazorise.PivotGrid.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.PivotGrid;

/// <summary>
/// Base class for PivotGrid declarative fields.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
[CascadingTypeParameter( nameof( TItem ) )]
public abstract class BasePivotGridField<TItem> : ComponentBase, IDisposable
{
    #region Members

    private Func<TItem, object> valueGetter;
    private string valueGetterField;

    #endregion

    #region Methods

    /// <inheritdoc />
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        await base.SetParametersAsync( parameters );

        PivotGrid?.RegisterField( this );
    }

    /// <summary>
    /// Gets the field value from the supplied item.
    /// </summary>
    public virtual object GetValue( TItem item )
    {
        if ( item is null || string.IsNullOrWhiteSpace( Field ) )
            return null;

        if ( valueGetter is null || !string.Equals( valueGetterField, Field, StringComparison.Ordinal ) )
        {
            valueGetterField = Field;
            valueGetter = PivotGridFunctionCompiler.CreateValueGetter<TItem>( Field );
        }

        return valueGetter.Invoke( item );
    }

    /// <summary>
    /// Formats a field value.
    /// </summary>
    public virtual string FormatValue( object value )
    {
        if ( DisplayFormat is not null )
        {
            return string.Format( DisplayFormatProvider ?? CultureInfo.CurrentCulture, DisplayFormat, value );
        }

        return value?.ToString() ?? EmptyText;
    }

    /// <summary>
    /// Gets the field caption.
    /// </summary>
    public string GetCaption()
        => string.IsNullOrWhiteSpace( Caption ) ? Field : Caption;

    /// <inheritdoc />
    public void Dispose()
    {
        PivotGrid?.RemoveField( this );
    }

    /// <summary>
    /// Gets a hash representing field parameter state that affects pivot calculation or rendering.
    /// </summary>
    internal virtual int GetFieldStateHash()
        => HashCode.Combine(
            Field,
            Caption,
            Visible,
            DisplayFormat,
            DisplayFormatProvider,
            EmptyText,
            FieldArea );

    #endregion

    #region Properties

    /// <summary>
    /// Parent PivotGrid component.
    /// </summary>
    [CascadingParameter] public PivotGrid<TItem> PivotGrid { get; set; }

    /// <summary>
    /// Field path bound to this PivotGrid field.
    /// </summary>
    [Parameter] public string Field { get; set; }

    /// <summary>
    /// Optional caption shown in headers.
    /// </summary>
    [Parameter] public string Caption { get; set; }

    /// <summary>
    /// Determines whether this field participates in the PivotGrid.
    /// </summary>
    [Parameter] public bool Visible { get; set; } = true;

    /// <summary>
    /// Specifies display format for default display text.
    /// </summary>
    [Parameter] public string DisplayFormat { get; set; }

    /// <summary>
    /// Specifies display format provider.
    /// </summary>
    [Parameter] public IFormatProvider DisplayFormatProvider { get; set; }

    /// <summary>
    /// Text shown when the field value is null.
    /// </summary>
    [Parameter] public string EmptyText { get; set; } = string.Empty;

    /// <summary>
    /// Defines custom header template.
    /// </summary>
    [Parameter] public RenderFragment<PivotGridHeaderContext<TItem>> HeaderTemplate { get; set; }

    /// <summary>
    /// Defines custom field value template.
    /// </summary>
    [Parameter] public RenderFragment<PivotGridFieldValueContext<TItem>> DisplayTemplate { get; set; }

    /// <summary>
    /// Gets the PivotGrid layout area for this field.
    /// </summary>
    public abstract PivotGridFieldArea FieldArea { get; }

    #endregion
}