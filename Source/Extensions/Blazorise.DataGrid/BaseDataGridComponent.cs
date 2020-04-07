#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using Blazorise.Utils;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    /// <summary>
    /// Minimal base class for datagrid components.
    /// </summary>
    public class BaseDataGridComponent : ComponentBase
    {
        #region Members

        private string elementId;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the element id.
        /// </summary>
        public string ElementId
        {
            get
            {
                // generate ID only on first use
                if ( elementId == null )
                    elementId = IDGenerator.Instance.Generate;

                return elementId;
            }
            private set
            {
                elementId = value;
            }
        }

        #endregion
    }
}
