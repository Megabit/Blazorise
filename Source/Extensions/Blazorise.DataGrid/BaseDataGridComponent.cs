#region Using directives
using System;
using Blazorise.Base;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    /// <summary>
    /// Minimal base class for datagrid components.
    /// </summary>
    public class BaseDataGridComponent : BaseAfterRenderComponent, IDisposable
    {
        #region Methods

        protected override void OnInitialized()
        {
            if ( ElementId == null )
                ElementId = IdGenerator.Generate;

            base.OnInitialized();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or set the javascript runner.
        /// </summary>
        [Inject] protected IIdGenerator IdGenerator { get; set; }

        /// <summary>
        /// Gets or sets the datagrid element id.
        /// </summary>
        [Parameter] public string ElementId { get; set; }

        #endregion
    }
}
