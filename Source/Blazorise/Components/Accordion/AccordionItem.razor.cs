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
public partial class AccordionItem : BaseComponent, IDisposable
{
    #region Members

    private bool visible;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ParentAccordion?.NotifyAccordionItemInitialized( this );

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            ParentAccordion?.NotifyAccordionItemRemoved( this );
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.AccordionItem() );
        builder.Append( ClassProvider.AccordionItemActive( Visible ) );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Toggles the accordion item visibility state.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Toggle()
    {
        Visible = !Visible;

        return InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Sets the visibility state of this <see cref="AccordionItem"/> component.
    /// </summary>
    /// <param name="visible">True if <see cref="AccordionItem"/> is visible.</param>
    private void HandleVisibilityState( bool visible )
    {
        DirtyClasses();
    }

    /// <summary>
    /// Raises all registered events for this <see cref="AccordionItem"/> component.
    /// </summary>
    /// <param name="visible">True if <see cref="AccordionItem"/> is visible.</param>
    private void RaiseEvents( bool visible )
    {
        InvokeAsync( () => VisibleChanged.InvokeAsync( visible ) );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Determines if the accordion item is placed inside of accordion component as the first item.
    /// </summary>
    public bool FirstInAccordion => ParentAccordion?.IsFirstInAccordion( this ) == true;

    /// <summary>
    /// Determines if the accordion item is placed inside of accordion component as the last item.
    /// </summary>
    public bool LastInAccordion => ParentAccordion?.IsLastInAccordion( this ) == true;

    /// <summary>
    /// Gets or sets the accordion item visibility state.
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
    /// Occurs when the accordion item visibility state changes.
    /// </summary>
    [Parameter] public EventCallback<bool> VisibleChanged { get; set; }

    /// <summary>
    /// Gets or sets the cascaded parent accordion component.
    /// </summary>
    [CascadingParameter] protected Accordion ParentAccordion { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="AccordionItem"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}