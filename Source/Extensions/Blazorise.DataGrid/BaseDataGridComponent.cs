#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.DataGrid
{
    /// <summary>
    /// Minimal base class for datagrid components.
    /// </summary>
    public class BaseDataGridComponent : BaseAfterRenderComponent
    {
        #region Methods

        protected override void OnInitialized()
        {
            if ( JSModule == null )
            {
                JSModule = new JSDataGridModule( JSRuntime );
            }

            base.OnInitialized();

            ElementId ??= IdGenerator.Generate;
        }

        protected override async ValueTask DisposeAsync( bool disposing )
        {
            if ( disposing && Rendered )
            {
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
            }

            await base.DisposeAsync( disposing );
        }

        #endregion

        #region Properties

        protected JSDataGridModule JSModule { get; private set; }

        /// <summary>
        /// Gets or sets the JS runtime.
        /// </summary>
        [Inject] private IJSRuntime JSRuntime { get; set; }

        /// <summary>
        /// Gets or sets the classname provider.
        /// </summary>
        [Inject] protected IClassProvider ClassProvider { get; set; }

        /// <summary>
        /// Gets or set the IIdGenerator.
        /// </summary>
        [Inject] protected IIdGenerator IdGenerator { get; set; }

        /// <summary>
        /// Gets or sets the datagrid element id.
        /// </summary>
        [Parameter] public string ElementId { get; set; }

        #endregion
    }
}