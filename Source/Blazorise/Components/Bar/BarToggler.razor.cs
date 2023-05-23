#region Using directives
using System.Threading.Tasks;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Controls the visibility state of the <see cref="Blazorise.Bar"/> component.
/// </summary>
public partial class BarToggler : BaseComponent
{
    #region Members

    private BarState parentBarState;

    /// <summary>
    /// Holds the state for this bar component.
    /// </summary>
    private BarTogglerState state = new();

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnInitializedAsync()
    {
        if ( parentBarState is not null )
        {
            if ( parentBarState.BarTogglerState is null )
                parentBarState.BarTogglerState = this.state;
        }

        return base.OnInitializedAsync();
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.BarToggler( ParentBarState?.Mode ?? BarMode.Horizontal, Mode ) );
        builder.Append( ClassProvider.BarTogglerCollapsed( ParentBarState?.Mode ?? BarMode.Horizontal, Mode, Bar != null ? Bar.Visible : ParentBarState.Visible ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        if ( Bar != null )
        {
            builder.Append( "display: inline-flex" );
        }

        base.BuildStyles( builder );
    }

    /// <summary>
    /// Handles the toggler onclick event.
    /// </summary>
    /// <param name="eventArgs">Supplies information about a mouse event that is being raised.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected async Task ClickHandler( MouseEventArgs eventArgs )
    {
        if ( Clicked.HasDelegate )
        {
            await Clicked.InvokeAsync( eventArgs );
        }

        if ( Bar != null )
        {
            await Bar.Toggle();

            DirtyClasses();
        }
        else if ( ParentBar != null )
        {
            await ParentBar.Toggle();
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the reference to the state object for this <see cref="BarToggler"/> component.
    /// </summary>
    protected BarTogglerState State => state;

    /// <summary>
    /// Occurs when the button is clicked.
    /// </summary>
    [Parameter]
    public EventCallback<MouseEventArgs> Clicked
    {
        get => state.Clicked;
        set
        {
            if ( state.Clicked.Equals( value ) )
                return;

            state.Clicked = value;
        }
    }

    /// <summary>
    /// Provides options for inline or popout styles. Only supported by Vertical Bar. Uses inline by default.
    /// </summary>
    [Parameter]
    public BarTogglerMode Mode
    {
        get => state.Mode;
        set
        {
            if ( state.Mode == value )
                return;

            state.Mode = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Controls which <see cref="Bar"/> will be toggled. Uses parent <see cref="Bar"/> by default. 
    /// </summary>
    [Parameter]
    public Bar Bar
    {
        get => state.Bar;
        set
        {
            if ( state.Bar == value )
                return;

            state.Bar = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="BarToggler"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Cascaded <see cref="Bar"/> component state object.
    /// </summary>
    [CascadingParameter]
    protected BarState ParentBarState
    {
        get => parentBarState;
        set
        {
            if ( parentBarState == value )
                return;

            parentBarState = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Cascaded <see cref="Bar"/> component.
    /// </summary>
    [CascadingParameter] protected Bar ParentBar { get; set; }

    #endregion
}