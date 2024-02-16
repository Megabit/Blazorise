#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// A wrapper for accordion header.
/// </summary>
public partial class AccordionHeader : BaseComponent
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.AccordionHeader() );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Handles the header onclick event.
    /// </summary>
    /// <param name="eventArgs">Supplies information about a mouse event that is being raised.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected async Task ClickHandler( MouseEventArgs eventArgs )
    {
        if ( ParentAccordionItem is not null )
            await ParentAccordionItem.Toggle();

        await Clicked.InvokeAsync( eventArgs );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the reference to the parent <see cref="AccordionItem"/> component.
    /// </summary>
    [CascadingParameter] public AccordionItem ParentAccordionItem { get; set; }

    /// <summary>
    /// Occurs when the accordion header is clicked.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> Clicked { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="AccordionHeader"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}