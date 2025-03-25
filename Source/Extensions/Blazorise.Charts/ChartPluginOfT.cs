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

    protected abstract TJSModule GetNewJsModule();

    protected abstract Task InitializePluginByJsModule();

    protected abstract bool InitPluginInParameterSet( ParameterView parameterView );

    protected internal override async Task OnParentChartInitialized()
    {
        if ( JSModule != null )
            return;

        JSModule = GetNewJsModule();

        ExecuteAfterRender( InitializePluginByJsModule );

        await InvokeAsync( StateHasChanged );
    }

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( Rendered && JSModule is not null )
        {
            bool optionsChanged = InitPluginInParameterSet( parameters );

            if ( optionsChanged )
            {
                ExecuteAfterRender( InitializePluginByJsModule );
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