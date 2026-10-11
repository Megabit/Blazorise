#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Renders the color picker controls and forwards surface interaction.
/// </summary>
public partial class _ColorPickerMenu : ComponentBase
{
    #region Members

    /// <summary>
    /// The color surface that receives pointer interaction forwarded by the menu.
    /// </summary>
    private _ColorPickerSurface surfaceRef;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnAfterRenderAsync( bool firstRender )
        => firstRender ? Parent.OnMenuRendered() : Task.CompletedTask;

    /// <summary>
    /// Forwards pointer movement in the menu to the color surface.
    /// </summary>
    private Task OnPointerMoveHandler( PointerEventArgs eventArgs ) => surfaceRef?.OnPointerMoveHandler( eventArgs ) ?? Task.CompletedTask;

    /// <summary>
    /// Forwards pointer release in the menu to the color surface.
    /// </summary>
    private Task OnPointerUpHandler( PointerEventArgs eventArgs ) => surfaceRef?.OnPointerUpHandler( eventArgs ) ?? Task.CompletedTask;

    /// <summary>
    /// Forwards pointer cancellation in the menu to the color surface.
    /// </summary>
    private Task OnPointerCancelHandler( PointerEventArgs eventArgs ) => surfaceRef?.OnPointerCancelHandler( eventArgs ) ?? Task.CompletedTask;

    /// <summary>
    /// Forwards menu keyboard interaction to the parent picker.
    /// </summary>
    private Task OnKeyDownHandler( KeyboardEventArgs eventArgs ) => Parent.OnMenuKeyDownHandler( eventArgs );

    #endregion

    #region Properties

    /// <summary>
    /// Gets the pointer movement handler that suppresses automatic menu rendering.
    /// </summary>
    private Func<PointerEventArgs, Task> NonRenderingPointerMoveHandler
        => EventUtil.AsNonRenderingEventHandler<PointerEventArgs>( OnPointerMoveHandler );

    /// <summary>
    /// Gets the pointer release handler that suppresses automatic menu rendering.
    /// </summary>
    private Func<PointerEventArgs, Task> NonRenderingPointerUpHandler
        => EventUtil.AsNonRenderingEventHandler<PointerEventArgs>( OnPointerUpHandler );

    /// <summary>
    /// Gets the pointer cancellation handler that suppresses automatic menu rendering.
    /// </summary>
    private Func<PointerEventArgs, Task> NonRenderingPointerCancelHandler
        => EventUtil.AsNonRenderingEventHandler<PointerEventArgs>( OnPointerCancelHandler );

    /// <summary>
    /// Gets the keyboard handler that suppresses automatic menu rendering.
    /// </summary>
    private Func<KeyboardEventArgs, Task> NonRenderingKeyDownHandler
        => EventUtil.AsNonRenderingEventHandler<KeyboardEventArgs>( OnKeyDownHandler );

    /// <summary>
    /// Gets or sets the color picker that owns this menu.
    /// </summary>
    [CascadingParameter] private ColorPicker Parent { get; set; }

    #endregion
}