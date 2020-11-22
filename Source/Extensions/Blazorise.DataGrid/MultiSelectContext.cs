#region Using directives
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public class MultiSelectContext<TItem>
    {
        #region Constructors

        public MultiSelectContext( EventCallback<bool> selectedChanged )
        {
            SelectedChanged = selectedChanged;
        }

        public MultiSelectContext( EventCallback<bool> selectedChanged, TItem item )
            : this( selectedChanged )
        {
            Item = item;
        }

        #endregion

        #region Properties

        public EventCallback<bool> SelectedChanged { get; set; }

        public TItem Item { get; set; }

        #endregion
    }
}
