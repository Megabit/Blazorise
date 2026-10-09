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

    private _ColorPickerSurface surfaceRef;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnAfterRenderAsync( bool firstRender )
        => firstRender ? Parent.OnMenuRendered() : Task.CompletedTask;

    private Task OnPointerMoveHandler( PointerEventArgs eventArgs ) => surfaceRef?.OnPointerMoveHandler( eventArgs ) ?? Task.CompletedTask;

    private Task OnPointerUpHandler( PointerEventArgs eventArgs ) => surfaceRef?.OnPointerUpHandler( eventArgs ) ?? Task.CompletedTask;

    private Task OnPointerCancelHandler( PointerEventArgs eventArgs ) => surfaceRef?.OnPointerCancelHandler( eventArgs ) ?? Task.CompletedTask;

    private Task OnKeyDownHandler( KeyboardEventArgs eventArgs ) => Parent.OnMenuKeyDownHandler( eventArgs );

    #endregion

    #region Properties

    private Func<PointerEventArgs, Task> NonRenderingPointerMoveHandler
        => EventUtil.AsNonRenderingEventHandler<PointerEventArgs>( OnPointerMoveHandler );

    private Func<PointerEventArgs, Task> NonRenderingPointerUpHandler
        => EventUtil.AsNonRenderingEventHandler<PointerEventArgs>( OnPointerUpHandler );

    private Func<PointerEventArgs, Task> NonRenderingPointerCancelHandler
        => EventUtil.AsNonRenderingEventHandler<PointerEventArgs>( OnPointerCancelHandler );

    private Func<KeyboardEventArgs, Task> NonRenderingKeyDownHandler
        => EventUtil.AsNonRenderingEventHandler<KeyboardEventArgs>( OnKeyDownHandler );

    [CascadingParameter] private ColorPicker Parent { get; set; }

    #endregion
}