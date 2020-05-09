#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Stores;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class BarToggler : BaseComponent
    {
        #region Members

        private BarStore parentStore;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarToggler( ParentStore.Mode ) );
            builder.Append( ClassProvider.BarTogglerCollapsed( ParentStore.Mode, ParentStore.Visible ) );

            base.BuildClasses( builder );
        }

        protected Task ClickHandler()
        {
            // NOTE: is this right?
            if ( Clicked == null )
                ParentBar?.Toggle();
            else
                Clicked?.Invoke();

            return Task.CompletedTask;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Occurs when the button is clicked.
        /// </summary>
        [Parameter] public Action Clicked { get; set; }

        [CascadingParameter]
        protected BarStore ParentStore
        {
            get => parentStore;
            set
            {
                if ( parentStore == value )
                    return;

                parentStore = value;

                DirtyClasses();
            }
        }

        [CascadingParameter] protected Bar ParentBar { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
