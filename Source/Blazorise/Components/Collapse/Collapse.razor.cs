#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Toggle visibility of almost any content on your pages in a vertically collapsing container.
/// </summary>
public partial class Collapse : BaseComponent, IDisposable
{
    #region Members

    private bool visible;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Collapse() );
        builder.Append( ClassProvider.CollapseActive( Visible ) );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Toggles the collapse visibility state.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Toggle()
    {
        Visible = !Visible;

        return InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Sets the visibility state of this <see cref="Collapse"/> component.
    /// </summary>
    /// <param name="visible">True if <see cref="Collapse"/> is visible.</param>
    private void HandleVisibilityState( bool visible )
    {
        DirtyClasses();
    }

    /// <summary>
    /// Raises all registered events for this <see cref="Collapse"/> component.
    /// </summary>
    /// <param name="visible">True if <see cref="Collapse"/> is visible.</param>
    private void RaiseEvents( bool visible )
    {
        InvokeAsync( () => VisibleChanged.InvokeAsync( visible ) );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the collapse visibility state.
    /// </summary>
    [Parameter]
    public bool Visible
    {
        get => visible;
        set
        {
            if ( visible == value )
                return;

            visible = value;

            HandleVisibilityState( value );
            RaiseEvents( value );
        }
    }

    /// <summary>
    /// Occurs when the collapse visibility state changes.
    /// </summary>
    [Parameter] public EventCallback<bool> VisibleChanged { get; set; }

    /// <summary>
    /// Gets or sets the cascaded parent accordion component.
    /// </summary>
    [CascadingParameter] protected Accordion ParentAccordion { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Collapse"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}