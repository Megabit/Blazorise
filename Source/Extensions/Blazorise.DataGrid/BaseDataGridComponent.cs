#region Using directives

using Microsoft.AspNetCore.Components;

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
            base.OnInitialized();

            ElementId ??= IdGenerator.Generate;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the classname provider.
        /// </summary>
        [Inject]
        protected IClassProvider ClassProvider { get; set; }

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