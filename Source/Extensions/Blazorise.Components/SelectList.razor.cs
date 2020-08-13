#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Components
{
    public partial class SelectList<TItem, TValue> : ComponentBase
    {
        #region Members

        #endregion

        #region Methods

        protected Task HandleSelectedValueChanged( TValue value )
        {
            SelectedValue = value;
            return SelectedValueChanged.InvokeAsync( value );
        }

        /// <summary>
        /// Sets focus on the input element, if it can be focused.
        /// </summary>
        /// <param name="scrollToElement">If true the browser should scroll the document to bring the newly-focused element into view.</param>
        public void Focus( bool scrollToElement = true )
        {
            select.Focus( scrollToElement );
        }

        #endregion

        #region Properties

        protected Select<TValue> select;

        /// <summary>
        /// Gets or sets the select data-source.
        /// </summary>
        [Parameter] public IEnumerable<TItem> Data { get; set; }

        /// <summary>
        /// Method used to get the display field from the supplied data source.
        /// </summary>
        [Parameter] public Func<TItem, string> TextField { get; set; }

        /// <summary>
        /// Method used to get the value field from the supplied data source.
        /// </summary>
        [Parameter] public Func<TItem, TValue> ValueField { get; set; }

        /// <summary>
        /// Currently selected item value.
        /// </summary>
        [Parameter] public TValue SelectedValue { get; set; }

        /// <summary>
        /// Occurs after the selected value has changed.
        /// </summary>
        [Parameter] public EventCallback<TValue> SelectedValueChanged { get; set; }

        /// <summary>
        /// Custom css class-names.
        /// </summary>
        [Parameter] public string Class { get; set; }

        /// <summary>
        /// Custom styles.
        /// </summary>
        [Parameter] public string Style { get; set; }

        /// <summary>
        /// Size of a select field.
        /// </summary>
        [Parameter] public Size Size { get; set; } = Size.None;

        [Parameter( CaptureUnmatchedValues = true )]
        public Dictionary<string, object> Attributes { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
