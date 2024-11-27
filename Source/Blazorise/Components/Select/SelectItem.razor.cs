#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Option item in the <see cref="Select{TValue}"/> component.
/// </summary>
/// <typeparam name="TValue">The type of the <see cref="Value"/>.</typeparam>
public partial class SelectItem<TValue> : BaseComponent, ISelectItem<TValue>, IDisposable
{
    #region Methods

    /// <inheritdoc/>
    protected override Task OnInitializedAsync()
    {
        ParentSelect?.AddSelectItem( this );

        return base.OnInitializedAsync();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            ParentSelect?.RemoveSelectItem( this );
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    public bool CompareTo( object value )
    {
        return Value?.IsEqual( value ) ?? false;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the flag that indicates if item is selected.
    /// </summary>
    protected bool Selected => ParentSelect?.ContainsValue( Value ) == true;

    /// <summary>
    /// Convert the value to string because option tags are working with string internally. Otherwise some datatypes like booleans will not work as expected.
    /// </summary>
    protected string StringValue => Value?.ToString();

    /// <summary>
    /// Gets or sets the item value.
    /// </summary>
    [Parameter] public TValue Value { get; set; }

    /// <summary>
    /// Disable the item from mouse click.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Hide the item from the list so it can be used as a placeholder.
    /// </summary>
    [Parameter] public bool Hidden { get; set; }

    /// <summary>
    /// Specifies the select component in which this select item is placed.
    /// </summary>
    [CascadingParameter] protected virtual Select<TValue> ParentSelect { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="SelectItem{TValue}"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}