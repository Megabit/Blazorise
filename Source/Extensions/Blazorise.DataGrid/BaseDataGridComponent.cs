﻿#region Using directives
using System;
using Blazorise.Utilities;
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

            if ( ElementId == null )
                ElementId = IdGenerator.Generate;
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
