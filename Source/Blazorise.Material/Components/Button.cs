#region Using directives
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Material.Components;

public class Button : Blazorise.Button
{
    #region Methods

    /// <inheritdoc/>
    protected override RenderFragment ProvideDefaultLoadingTemplate()
    {
        return builder =>
        {
            builder
                .OpenElement( "span" )
                .Class( "mui-button-loading-content" );

            builder
                .OpenElement( "span" )
                .Class( "mui-button-loading-indicator" )
                .AriaHidden( "true" );
            builder.CloseElement();

            builder
                .OpenElement( "span" )
                .Class( "mui-button-loading-label" )
                .Content( ChildContent )
                .CloseElement();

            builder.CloseElement();
        };
    }

    #endregion
}