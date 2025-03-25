#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// An abstract class for creating chart plugins that interact with JavaScript modules and manage their lifecycle.
/// </summary>
/// <typeparam name="TItem">Represents the type of data items that the chart will display.</typeparam>
/// <typeparam name="TJSModule">Represents the type of JavaScript module used for the plugin's functionality.</typeparam>
public abstract class ChartPlugin<TItem, TJSModule> : ChartPlugin
    where TJSModule : IBaseJSModule, IAsyncDisposable
{
    #region Methods

    /// <summary>
    /// Creates an instance of a JavaScript module for the plugin.
    /// </summary>
    /// <returns>Returns an instance of TJSModule.</returns>
    protected abstract TJSModule CreatePluginJsModule();

    /// <summary>
    /// Initializes the plugin asynchronously. This method must be implemented by derived classes.
    /// </summary>
    /// <returns>Returns a Task representing the asynchronous operation.</returns>
    protected abstract Task InitializePlugin();

    /// <summary>
    /// Updates the parameters of a plugin based on the provided view. It returns a boolean indicating success or failure.
    /// </summary>
    /// <param name="parameterView">Represents the current state and settings of the parameters to be updated.</param>
    /// <returns>Indicates whether the update operation was successful.</returns>
    protected abstract bool UpdatePluginParameters( ParameterView parameterView );

    /// <inheritdoc/>
    protected internal override async Task OnParentChartInitialized()
    {
        if ( JSModule != null )
            return;

        JSModule = CreatePluginJsModule();

        ExecuteAfterRender( InitializePlugin );

        await InvokeAsync( StateHasChanged );
    }

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( Rendered && JSModule is not null )
        {
            var optionsChanged = UpdatePluginParameters( parameters );

            if ( optionsChanged )
            {
                ExecuteAfterRender( InitializePlugin );
            }
        }

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            if ( JSModule is not null )
                await JSModule.SafeDisposeAsync();

            ParentChart?.NotifyPluginRemoved( this );
        }

        await base.DisposeAsync( disposing );
    }

    /// <inheritdoc/>
    protected override Task OnInitializedAsync()
    {
        if ( ParentChart is null )
            throw new InvalidOperationException( $"Chart Plugin {Name} can be used only inside the Blazorise.Chart" );

        ParentChart.NotifyPluginInitialized( this );

        return base.OnInitializedAsync();
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Represents the parent chart of type <see cref="BaseChart{TItem}"/> for the current component.
    /// </summary>
    [CascadingParameter] protected BaseChart<TItem> ParentChart { get; set; }

    /// <summary>
    /// Represents the JavaScript module for the current component.
    /// </summary>
    protected abstract TJSModule JSModule { get; set; }

    /// <summary>
    /// Injects an instance of IJSRuntime for JavaScript interop in a Blazor component.
    /// </summary>
    [Inject] protected IJSRuntime JSRuntime { get; set; }

    /// <summary>
    /// Injects an instance of IVersionProvider for version management.
    /// </summary>
    [Inject] protected IVersionProvider VersionProvider { get; set; }

    /// <summary>
    /// Injects BlazoriseOptions for configuration. It allows access to Blazorise settings within the class.
    /// </summary>
    [Inject] protected BlazoriseOptions BlazoriseOptions { get; set; }

    #endregion
}