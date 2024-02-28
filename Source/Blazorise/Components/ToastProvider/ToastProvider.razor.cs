#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// A toast provider to be set at the root of your app, providing a programmatic way to invoke toasts with custom content by using the <see cref="ToastService"/>.
/// </summary>
public partial class ToastProvider : BaseComponent, IDisposable
{
    #region Members

    /// <summary>
    /// 
    /// </summary>
    private List<ToastInstance> toastInstances;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        ToastService.ToastReceived += OnToastReceived;
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing && ToastService is not null )
        {
            ToastService.ToastReceived -= OnToastReceived;
        }

        base.Dispose( disposing );
    }

    private async void OnToastReceived( object sender, ToastEventArgs e )
    {
        if ( e is null )
            return;

        await Show( e.Title, e.Message );
    }

    /// <summary>
    /// Show the simple info toast message.
    /// </summary>
    /// <param name="title">Toast title.</param>
    /// <param name="message">Info toast to show.</param>
    /// <param name="intent">Intent of the toast message.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Show( string title, string message, ToastIntent intent = ToastIntent.Info )
    {
        return Show( title, (MarkupString)message, intent );
    }

    /// <summary>
    /// Show the simple info toast message.
    /// </summary>
    /// <param name="title">Toast title.</param>
    /// <param name="message">Info toast to show.</param>
    /// <param name="intent">Intent of the toast message.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Show( string title, MarkupString message, ToastIntent intent = ToastIntent.Info )
    {
        var toastInstance = new ToastInstance( this, IdGenerator.Generate, title, message, ToastInstanceOptions.Default );

        return AddToastInstance( toastInstance );
    }

    /// <summary>
    /// Explicitly adds the toast instance into the <see cref="ToastProvider"/>.
    /// </summary>
    /// <param name="toastInstance">The toast instance.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    internal async Task AddToastInstance( ToastInstance toastInstance )
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
    /// <param name="toastInstance">The toast instance.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    internal async Task RemoveToastInstance( ToastInstance toastInstance )
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

    /// <summary>
    /// An event that is invoked after the <see cref="Toast"/> was closed.
    /// </summary>
    /// <param name="toastInstance">The toast instance.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected async Task OnToastClosed( ToastInstance toastInstance )
    {
        await toastInstance.Closed.InvokeAsync();

        await RemoveToastInstance( toastInstance );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the <see cref="IToastService"/> to which this component is responding.
    /// </summary>
    [Inject] protected IToastService ToastService { get; set; }

    /// <summary>
    /// Specifies the position of the <see cref="Toaster" /> component.
    /// </summary>
    [Parameter] public ToasterPlacement Placement { get; set; } = ToasterPlacement.BottomEnd;

    /// <summary>
    /// Specifies the placement strategy of the <see cref="Toaster" /> component.
    /// </summary>
    [Parameter] public ToasterPlacementStrategy PlacementStrategy { get; set; } = ToasterPlacementStrategy.Fixed;

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