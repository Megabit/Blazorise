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
    public partial class BarDropdownItem : BaseComponent
    {
        #region Members

        private BarDropdownStore parentStore;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarDropdownItem( ParentStore.Mode ) );

            base.BuildClasses( builder );
        }

        protected Task ClickHandler()
        {
            return Clicked.InvokeAsync( null );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Occurs when the item is clicked.
        /// </summary>
        [Parameter] public EventCallback Clicked { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public string To { get; set; }

        [Parameter] public Match Match { get; set; } = Match.All;

        [Parameter] public string Title { get; set; }

        [CascadingParameter] 
        protected BarDropdownStore ParentStore
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

        #endregion
    }
}
