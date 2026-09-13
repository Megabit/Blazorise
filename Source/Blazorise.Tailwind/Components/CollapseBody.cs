#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Tailwind.Components;

/// <summary>
/// Tailwind collapse panel with a measured height transition.
/// </summary>
public class CollapseBody : Blazorise.CollapseBody, IAsyncDisposable
{
    #region Methods

    /// <inheritdoc/>
    protected override async Task OnFirstAfterRenderAsync()
    {
        await JSModule.Initialize( ElementRef, ElementId, new()
        {
            HiddenClassName = "hidden",
            AnimatingClassName = "overflow-hidden",
        } );

        await base.OnFirstAfterRenderAsync();
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
            await JSModule.SafeDestroy( ElementRef, ElementId );

        await base.DisposeAsync( disposing );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Gets or sets the collapse transition module.
    /// </summary>
    [Inject] public IJSCollapseModule JSModule { get; set; }

    #endregion
}