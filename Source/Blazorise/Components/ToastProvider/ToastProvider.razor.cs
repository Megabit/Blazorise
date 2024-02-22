#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise;

/// <summary>
/// A toast provider to be set at the root of your app, providing a programmatic way to invoke toasts with custom content by using the <see cref="ToastService"/>.
/// </summary>
public partial class ToastProvider : BaseComponent
{
    #region Members

    /// <summary>
    /// 
    /// </summary>
    private List<ToastInstance> toastInstances;

    #endregion

    #region Methods

    public Task Show( string title, string message )
    {
        var toastInstance = new ToastInstance( this, IdGenerator.Generate, title, message, new ToastInstanceOptions() );

        return Show( toastInstance );
    }

    internal async Task Show( ToastInstance toastInstance )
    {
        toastInstances ??= new();

        toastInstance.Visible = true;

        toastInstances.Add( toastInstance );

        DirtyClasses();
        DirtyStyles();

        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Explicitly removes the toast instance from the <see cref="ToastProvider"/>.
    /// </summary>
    /// <param name="toastInstance">The modal instance</param>
    /// <returns></returns>
    internal async Task Remove( ToastInstance toastInstance )
    {
        if ( toastInstances.IsNullOrEmpty() )
        {
            return;
        }

        toastInstances.Remove( toastInstance );

        DirtyClasses();
        DirtyStyles();

        await InvokeAsync( StateHasChanged );
    }

    protected async Task OnToastClosed( ToastInstance toastInstance )
    {
        await toastInstance.Closed.InvokeAsync();

        await Remove( toastInstance );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Specifies the position of the Toasts.
    /// </summary>
    [Parameter] public ToasterPlacement Placement { get; set; } = ToasterPlacement.BottomEnd;

    /// <summary>
    /// Specifies the visibility of the close button.
    /// </summary>
    [Parameter] public bool ShowCloseButton { get; set; } = true;

    /// <summary>
    /// Occurs after the toast has opened. This is a Global Option.
    /// </summary>
    [Parameter] public EventCallback Opened { get; set; }

    /// <summary>
    /// Occurs after the toast has closed. This is a Global Option.
    /// </summary>
    [Parameter] public EventCallback Closed { get; set; }

    /// <summary>
    /// Specifies whether the Toast should have an animated transition.
    /// </summary>
    [Parameter] public bool Animated { get; set; } = true;

    /// <summary>
    /// The duration of the animation in milliseconds.
    /// </summary>
    [Parameter] public int AnimationDuration { get; set; } = 300;

    /// <summary>
    /// Automatically hide the toast after the delay.
    /// </summary>
    [Parameter] public bool Autohide { get; set; } = true;

    /// <summary>
    /// Delay in milliseconds before hiding the toast.
    /// </summary>
    [Parameter] public double AutohideDelay { get; set; } = 5000;

    /// <summary>
    /// The content to be rendered inside the Toast.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}