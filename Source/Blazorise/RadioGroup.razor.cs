#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// RadioGroup is a helpful wrapper used to group Radio components.
    /// </summary>
    public partial class RadioGroup : BaseComponent
    {
        #region Members

        private bool inline;

        public event EventHandler<RadioCheckedChangedEventArgs> RadioChanged;

        #endregion

        #region Methods

        internal Task NotifyRadioChanged( Radio radio )
        {
            RadioChanged.Invoke( this, new RadioCheckedChangedEventArgs( radio.Value ) );

            StateHasChanged();

            return CheckedValueChanged.InvokeAsync( radio.Value );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Radio group name.
        /// </summary>
        [Parameter] public string Name { get; set; }

        /// <summary>
        /// Group radios on the same horizontal row.
        /// </summary>
        [Parameter]
        public bool Inline
        {
            get => inline;
            set
            {
                inline = value;

                DirtyClasses();
            }
        }

        [Parameter] public EventCallback<object> CheckedValueChanged { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
