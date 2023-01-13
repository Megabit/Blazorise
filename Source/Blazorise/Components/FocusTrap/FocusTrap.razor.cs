#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// The focus trap component allows to restrict TAB key navigation inside the component.
/// </summary>
public partial class FocusTrap : BaseFocusableContainerComponent
{
    #region Members

    private ElementReference startFirstRef;

    private ElementReference startSecondRef;

    private ElementReference endFirstRef;

    private ElementReference endSecondRef;

    private bool shiftTabPressed;

    private bool shouldRender = true;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        if ( Rendered && parameters.TryGetValue<bool>( nameof( Active ), out var activeParam ) && Active != activeParam && activeParam )
        {
            ExecuteAfterRender( SetFocus );
        }

        await base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender && Active )
        {
            ExecuteAfterRender( SetFocus );
        }

        return base.OnAfterRenderAsync( firstRender );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.FocusTrap() );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Sets the focus trap.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SetFocus()
    {
        if ( Rendered )
        {
            if ( HasFocusableComponents )
                await HandleFocusableComponent();
            else
                await startFirstRef.FocusAsync();
        }
    }

    /// <summary>
    /// Handles the focus start event.
    /// </summary>
    /// <param name="args">Supplies information about a focus event that is being raised.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual async Task OnFocusStartHandler( FocusEventArgs args )
    {
        if ( !shiftTabPressed )
        {
            await startFirstRef.FocusAsync();
        }
    }

    /// <summary>
    /// Handles the focus end event.
    /// </summary>
    /// <param name="args">Supplies information about a focus event that is being raised.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual async Task OnFocusEndHandler( FocusEventArgs args )
    {
        if ( shiftTabPressed )
        {
            await endFirstRef.FocusAsync();
        }
    }

    /// <summary>
    /// Handles the keyboard events.
    /// </summary>
    /// <param name="args">Supplies information about a keyboard event that is being raised.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual void OnKeyPressedHandler( KeyboardEventArgs args )
    {
        shouldRender = false;

        if ( args.Key == "Tab" )
        {
            shiftTabPressed = args.ShiftKey;
        }
    }

    /// <inheritdoc/>
    protected override bool ShouldRender()
    {
        if ( shouldRender )
            return true;

        shouldRender = true;

        return false;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the focusable element tab index.
    /// </summary>
    protected int FocusableTabIndex => Active ? 0 : -1;

    /// <summary>
    /// If true the TAB focus will be activated.
    /// </summary>
    [Parameter] public bool Active { get; set; }

    /// <summary>
    /// Gets or sets the reference to the parent <see cref="FocusTrap"/> component.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}