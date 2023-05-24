#region Using directives
using System.Threading.Tasks;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// List groups are a flexible and powerful component for displaying a series of content.
/// </summary>
public partial class ListGroup : BaseComponent
{
    #region Members

    /// <summary>
    /// Holds the state of this list group.
    /// </summary>
    private ListGroupState state = new()
    {
        Mode = ListGroupMode.Static,
    };

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ListGroup() );
        builder.Append( ClassProvider.ListGroupFlush( Flush ) );
        builder.Append( ClassProvider.ListGroupScrollable( Scrollable ) );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Sets the active item by the name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task SelectItem( string name )
    {
        SelectedItem = name;

        return InvokeAsync( StateHasChanged );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the list group state object.
    /// </summary>
    protected ListGroupState State => state;

    /// <summary>
    /// Remove some borders and rounded corners to render list group items edge-to-edge in a parent container (e.g., cards).
    /// </summary>
    [Parameter]
    public bool Flush
    {
        get => state.Flush;
        set
        {
            state = state with { Flush = value };

            DirtyClasses();
        }
    }

    /// <summary>
    /// Makes the list group scrollable by adding a vertical scrollbar.
    /// </summary>
    [Parameter]
    public bool Scrollable
    {
        get => state.Scrollable;
        set
        {
            state = state with { Scrollable = value };

            DirtyClasses();
        }
    }

    /// <summary>
    /// Defines the list-group behavior mode.
    /// </summary>
    [Parameter]
    public ListGroupMode Mode
    {
        get => state.Mode;
        set
        {
            state = state with { Mode = value };

            DirtyClasses();
        }
    }

    /// <summary>
    /// Gets or sets currently selected item name.
    /// </summary>
    [Parameter]
    public string SelectedItem
    {
        get => state.SelectedItem;
        set
        {
            // prevent item from calling the same code multiple times
            if ( value == state.SelectedItem )
                return;

            state = state with { SelectedItem = value };

            // raise the SelectedItemChanged notification                
            SelectedItemChanged.InvokeAsync( state.SelectedItem );

            DirtyClasses();
        }
    }

    /// <summary>
    /// An event raised when <see cref="SelectedItem"/> is changed.
    /// </summary>
    [Parameter] public EventCallback<string> SelectedItemChanged { get; set; }

    /// <summary>
    /// Gets or sets the component child content.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}