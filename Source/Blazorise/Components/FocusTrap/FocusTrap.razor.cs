#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// The focus trap component allows to restrict TAB key navigation inside the component.
/// </summary>
public partial class FocusTrap : BaseFocusableContainerComponent
{
    #region Members

    private bool jsInitialized;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        if ( Rendered && parameters.TryGetValue<bool>( nameof( Active ), out var activeParam ) && Active != activeParam )
        {
            if ( activeParam )
            {
                ExecuteAfterRender( InitializeAndSetFocus );
            }
            else
            {
                ExecuteAfterRender( DestroyFocusTrap );
            }
        }

        await base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender && Active )
        {
            ExecuteAfterRender( InitializeAndSetFocus );
        }

        return base.OnAfterRenderAsync( firstRender );
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await DestroyFocusTrap();
        }

        await base.DisposeAsync( disposing );
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
            {
                await HandleFocusableComponent();
            }
            else
            {
                await JSFocusTrapModule.Focus( ElementRef, ElementId );
            }
        }
    }

    /// <summary>
    /// Initializes the focus trap and sets focus to the first focusable child.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task InitializeAndSetFocus()
    {
        await InitializeFocusTrap();
        await SetFocus();
    }

    /// <summary>
    /// Initializes the JavaScript focus trap handler.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task InitializeFocusTrap()
    {
        if ( !jsInitialized )
        {
            jsInitialized = true;

            await JSFocusTrapModule.Initialize( ElementRef, ElementId );
        }
    }

    /// <summary>
    /// Destroys the JavaScript focus trap handler.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task DestroyFocusTrap()
    {
        if ( jsInitialized )
        {
            jsInitialized = false;

            await JSFocusTrapModule.Destroy( ElementRef, ElementId );
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Specifies the <see cref="IJSFocusTrapModule"/> instance.
    /// </summary>
    [Inject] protected IJSFocusTrapModule JSFocusTrapModule { get; set; }

    /// <summary>
    /// If true the TAB focus will be activated.
    /// </summary>
    [Parameter] public bool Active { get; set; }

    /// <summary>
    /// Defines the content rendered inside the focus trap.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}