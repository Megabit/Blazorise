#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Button component used to toggle accordion collapse state.
/// </summary>
public partial class AccordionToggle : BaseComponent
{
    #region Members

    bool collapseVisible;

    bool accordionItemVisible;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.AccordionToggle() );
        builder.Append( ClassProvider.AccordionToggleCollapsed( ParentAccordionItem is not null ? AccordionItemVisible : CollapseVisible ) );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Handles the item onclick event.
    /// </summary>
    /// <param name="eventArgs">Supplies information about a mouse event that is being raised.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected async Task ClickHandler( MouseEventArgs eventArgs )
    {
        if ( ParentCollapse is not null )
        {
            await ParentCollapse.Toggle();
        }

        if ( ParentAccordionItem is not null )
        {
            await ParentAccordionItem.Toggle();
        }

        await Clicked.InvokeAsync( eventArgs );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the cascaded parent accordion component.
    /// </summary>
    [CascadingParameter] protected Accordion ParentAccordion { get; set; }

    /// <summary>
    /// Gets or sets the cascaded parent collapse component.
    /// </summary>
    [CascadingParameter] protected Collapse ParentCollapse { get; set; }

    /// <summary>
    /// Gets or sets the cascaded parent accordion item component.
    /// </summary>
    [CascadingParameter] public AccordionItem ParentAccordionItem { get; set; }

    /// <summary>
    /// Gets or sets the content visibility.
    /// </summary>
    [CascadingParameter( Name = "CollapseVisible" )]
    public bool CollapseVisible
    {
        get => collapseVisible;
        set
        {
            if ( collapseVisible == value )
                return;

            collapseVisible = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Gets or sets the content visibility.
    /// </summary>
    [CascadingParameter( Name = "AccordionItemVisible" )]
    public bool AccordionItemVisible
    {
        get => accordionItemVisible;
        set
        {
            if ( accordionItemVisible == value )
                return;

            accordionItemVisible = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Occurs when the toggle button is clicked.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> Clicked { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="AccordionToggle"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}