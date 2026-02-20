using System;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Blazorise.LoadingIndicator;

/// <summary>
/// A fullscreen loading indicator intended for application-wide busy state.
/// </summary>
public class ApplicationLoadingIndicator : LoadingIndicator
{
    #region Methods

    /// <summary>
    /// Initializes the application loading indicator with default fullscreen settings.
    /// </summary>
    public ApplicationLoadingIndicator()
    {
        SpinnerHeight = "128px";
        FullScreen = true;
        FadeIn = true;
        IndicatorTemplate = BlazoriseSpinner;
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        if ( Service == null )
        {
            Service = serviceProvider.GetService<ILoadingIndicatorService>();
        }

        base.OnParametersSet();
    }

    #endregion

    #region Properties

    private RenderFragment<LoadingIndicatorContext> BlazoriseSpinner => ( _ ) => ( builder ) =>
    {
        builder.OpenRegion( 0 );
        builder.AddMarkupContent( 1, @$"
                <svg class='b-loading-indicator-blazorise' viewBox='0 0 52.917 52.917'
                    {( !string.IsNullOrEmpty( SpinnerWidth ) ? $"width = '{SpinnerWidth}'" : "" )}
                    {( !string.IsNullOrEmpty( SpinnerHeight ) ? $"height='{SpinnerHeight}'" : "" )}>
                    <path style='fill:#9317e1;fill-opacity:1;fill-rule:nonzero;stroke:none' d='m0 0-35.896-20.173v-85.736L0-83.956Z' transform='matrix(.25315 0 0 -.25315 17.296 2.539)'/>
                    <path style='fill:#b24bf2;fill-opacity:1;fill-rule:nonzero;stroke:none' d='m0 0-36.045 20.767-35.896-21.954 35.97-20.47z' transform='matrix(.25315 0 0 -.25315 26.42 29.05)'/>
                    <path style='fill:#b24bf2;fill-opacity:1;fill-rule:nonzero;stroke:none' d='M0 0v-41.533l36.341-21.063 35.897 21.36z' transform='matrix(.25315 0 0 -.25315 26.42 7.946)'/>
                    <path style='fill:#9317e1;fill-opacity:1;fill-rule:nonzero;stroke:none' d='m0 0 72.09 42.423 36.119-21.211-72.238-41.385Z' transform='matrix(.25315 0 0 -.25315 17.315 45.271)'/>
                    <path style='fill:#bb61f6;fill-opacity:1;fill-rule:nonzero;stroke:none' d='M0 0v-84.995l-36.119 21.212.223 42.423z' transform='matrix(.25315 0 0 -.25315 44.708 18.385)'/>
                </svg>" );
        builder.CloseRegion();
    };

    [Inject]
    IServiceProvider serviceProvider { get; set; }

    #endregion
}