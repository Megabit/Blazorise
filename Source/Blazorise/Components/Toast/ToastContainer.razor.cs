#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
public partial class ToastContainer : BaseComponent
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ToastContainer() );
        builder.Append( ClassProvider.ToastContainerPlacement( Placement, true ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Specifies the position of the Toasts.
    /// </summary>
    [Parameter] public ToastPlacement Placement { get; set; } = ToastPlacement.BottomEnd;

    /// <summary>
    /// The content to be rendered inside the Toast.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}