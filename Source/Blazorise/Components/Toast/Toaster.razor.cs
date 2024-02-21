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
/// A container for placing the <see cref="Toast"/> component on the UI.
/// </summary>
public partial class Toaster : BaseComponent
{
    #region Members

    /// <summary>
    /// 
    /// </summary>
    private List<ToastInstance> toastInstances;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Toaster() );
        builder.Append( ClassProvider.ToasterPlacement( Placement, toastInstances?.Count > 0 ) );

        base.BuildClasses( builder );
    }

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
    /// Explicitly removes the toast instance from the <see cref="Toaster"/>.
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
    /// Occurs after the toast has opened.
    /// Global Option.
    /// </summary>
    [Parameter] public EventCallback Opened { get; set; }

    /// <summary>
    /// Occurs after the toast has closed.
    /// Global Option.
    /// </summary>
    [Parameter] public EventCallback Closed { get; set; }

    /// <summary>
    /// The content to be rendered inside the Toast.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}