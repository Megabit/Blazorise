using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazorise.Charts;

public abstract class BaseChartPlugin : BaseComponent, IAsyncDisposable
{
    protected internal abstract Task OnParentChartInitialized();
    protected internal abstract string Name { get; }
}

public abstract class BaseChartPlugin<TItem, TJSModule> : BaseChartPlugin
where TJSModule : IBaseJSModule, IAsyncDisposable
{

    protected abstract TJSModule GetNewJsModule();

    protected abstract Task AddPlugin();

    protected abstract bool ProceedWithExecution( ParameterView parameterView );

    protected internal override async Task OnParentChartInitialized()
    {
        if ( JSModule != null ) return;
        JSModule = GetNewJsModule();
        ExecuteAfterRender( async () =>
        {
            await AddPlugin();
        } );
        await InvokeAsync( StateHasChanged );
    }

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( Rendered && JSModule is not null )
        {
            bool optionsChanged = ProceedWithExecution( parameters );

            if ( optionsChanged )
            {
                ExecuteAfterRender( async () =>
                {
                    await AddPlugin();
                } );
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

            ParentChart?.NotifyBasePluginRemoved( this );
        }

        await base.DisposeAsync( disposing );
    }

    //  
    /// <inheritdoc/>
    protected override Task OnInitializedAsync()
    {
        if ( ParentChart is null )
            throw new InvalidOperationException( $"Chart Plugin {Name} can be used only inside the Blazoris.Chart" );
        ParentChart.NotifyBasePluginInitialized( this );
        return base.OnInitializedAsync();
    }
    
    
    [CascadingParameter] protected BaseChart<TItem> ParentChart { get; set; }

    protected abstract TJSModule JSModule { get; set; }
    
    protected override bool ShouldAutoGenerateId => true;

    
    /// <summary>
    /// Gets or sets the JS runtime.
    /// </summary>
    [Inject] protected IJSRuntime JSRuntime { get; set; }

    /// <summary>
    /// Gets or sets the version provider.
    /// </summary>
    [Inject] protected IVersionProvider VersionProvider { get; set; }

    /// <summary>
    /// Gets or sets the blazorise options.
    /// </summary>
    [Inject] protected BlazoriseOptions BlazoriseOptions { get; set; }
}