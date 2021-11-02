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

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Chart() );

            base.BuildClasses( builder );
        }

        /// <inheritdoc/>
        protected override ValueTask DisposeAsync( bool disposing )
        {
            if ( disposing )
            {
                _ = JS.Destroy( JSRuntime, ElementId );
                JS.DisposeDotNetObjectRef( DotNetObjectRef );
            }

            return base.DisposeAsync( disposing );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        protected override bool ShouldAutoGenerateId => true;

        protected DotNetObjectReference<ChartAdapter> DotNetObjectRef { get; set; }

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
