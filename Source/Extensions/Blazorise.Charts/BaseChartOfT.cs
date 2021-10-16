#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts
{
    /// <summary>
    /// Base class for all chart types.
    /// </summary>
    /// <typeparam name="TItem">Generic dataset value type.</typeparam>
    public class BaseChart<TItem> : BaseComponent
    {
        #region Methods

        protected override Task OnInitializedAsync()
        {
            if ( JSModule == null )
            {
                JSModule = new JSChartModule( JSRuntime );
            }

            return base.OnInitializedAsync();
        }

        protected override async ValueTask DisposeAsync( bool disposing )
        {
            if ( disposing )
            {
                if ( Rendered )
                {
                    var jsModuleDestroyTask = JSModule.Destroy( ElementId );

                    try
                    {
                        await jsModuleDestroyTask;
                    }
                    catch when ( jsModuleDestroyTask.IsCanceled )
                    {
                    }
#if NET6_0_OR_GREATER
                    catch ( Microsoft.JSInterop.JSDisconnectedException )
                    {
                    }
#endif

                    var jsModuleDisposeTask = JSModule.DisposeAsync();

                    try
                    {
                        await jsModuleDisposeTask;
                    }
                    catch when ( jsModuleDisposeTask.IsCanceled )
                    {
                    }
#if NET6_0_OR_GREATER
                    catch ( Microsoft.JSInterop.JSDisconnectedException )
                    {
                    }
#endif

                    if ( DotNetObjectRef != null )
                    {
                        DotNetObjectRef.Dispose();
                        DotNetObjectRef = null;
                    }
                }
            }

            await base.DisposeAsync( disposing );
        }

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Chart() );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        protected override bool ShouldAutoGenerateId => true;

        protected DotNetObjectReference<ChartAdapter> DotNetObjectRef { get; set; }

        protected JSChartModule JSModule { get; private set; }

        [Inject] protected IJSRuntime JSRuntime { get; set; }

        /// <summary>
        /// Defines the chart data.
        /// </summary>
        [Parameter] public ChartData<TItem> Data { get; set; }

        [Parameter] public EventCallback<ChartMouseEventArgs> Clicked { get; set; }

        [Parameter] public EventCallback<ChartMouseEventArgs> Hovered { get; set; }

        [Parameter] public EventCallback<ChartMouseEventArgs> MouseOut { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
