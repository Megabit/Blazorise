#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Components
{
    public partial class SelectList<TItem> : ComponentBase
    {
        #region Members

        #endregion

        #region Methods

        protected Task HandleSelectedValueChanged( object value )
        {
            SelectedValue = value;
            return SelectedValueChanged.InvokeAsync( value );
        }

        #endregion

        #region Properties

        [Parameter] public IEnumerable<TItem> Data { get; set; }

        [Parameter] public Func<TItem, string> TextField { get; set; }

        [Parameter] public Func<TItem, object> ValueField { get; set; }

        [Parameter] public object SelectedValue { get; set; }

        [Parameter] public EventCallback<object> SelectedValueChanged { get; set; }

        [Parameter] public string Class { get; set; }

        [Parameter] public string Style { get; set; }

        [Parameter( CaptureUnmatchedValues = true )]
        public Dictionary<string, object> Attributes { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
