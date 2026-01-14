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

    private AccordionBody accordionBody;

    private AccordionToggle accordionToggle;

    private string generatedBodyElementId;

    private string generatedToggleElementId;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        ParentAccordion?.NotifyAccordionItemInitialized( this );

        if ( generatedBodyElementId is null && !string.IsNullOrEmpty( ElementId ) )
            generatedBodyElementId = $"{ElementId}-body";

        if ( generatedToggleElementId is null && !string.IsNullOrEmpty( ElementId ) )
            generatedToggleElementId = $"{ElementId}-toggle";
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
    /// Registers the element id for the accordion body.
    /// </summary>
    /// <param name="elementId">Element id.</param>
    internal void NotifyAccordionBodyInitialized( AccordionBody body )
    {
        if ( body is null )
            return;

        if ( ReferenceEquals( accordionBody, body ) )
            return;

        accordionBody = body;
        InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Removes the element id for the accordion body.
    /// </summary>
    /// <param name="elementId">Element id.</param>
    internal void NotifyAccordionBodyRemoved( AccordionBody body )
    {
        if ( !ReferenceEquals( accordionBody, body ) )
            return;

        accordionBody = null;
        InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Registers the element id for the accordion toggle.
    /// </summary>
    /// <param name="elementId">Element id.</param>
    internal void NotifyAccordionToggleInitialized( AccordionToggle toggle )
    {
        if ( toggle is null )
            return;

        if ( ReferenceEquals( accordionToggle, toggle ) )
            return;

        accordionToggle = toggle;
        InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Removes the element id for the accordion toggle.
    /// </summary>
    /// <param name="elementId">Element id.</param>
    internal void NotifyAccordionToggleRemoved( AccordionToggle toggle )
    {
        if ( !ReferenceEquals( accordionToggle, toggle ) )
            return;

        accordionToggle = null;
        InvokeAsync( StateHasChanged );
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

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Gets the element id of the accordion body.
    /// </summary>
    internal string BodyElementId => accordionBody?.ElementId ?? generatedBodyElementId;

    /// <summary>
    /// Gets the element id of the accordion toggle.
    /// </summary>
    internal string ToggleElementId => accordionToggle?.ElementId ?? generatedToggleElementId;

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