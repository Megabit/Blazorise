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

        private BarTogglerMode mode = BarTogglerMode.Normal;

        private Bar bar;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarToggler( ParentStore.Mode, Mode ) );
            builder.Append( ClassProvider.BarTogglerCollapsed( ParentStore.Mode, Mode, ParentStore.Visible ) );

            base.BuildClasses( builder );
        }

        protected override void BuildStyles( StyleBuilder builder )
        {
            if ( Bar != null )
            {
                builder.Append( "display: inline-flex" );
            }

            base.BuildStyles( builder );
        }

        protected Task ClickHandler()
        {
            if ( Clicked != null )
            {
                Clicked.Invoke();
            }
            else if ( Bar != null )
            {
                Bar.Toggle();
            }
            else
            {
                ParentBar?.Toggle();
            } 

            return Task.CompletedTask;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Occurs when the button is clicked.
        /// </summary>
        [Parameter] public Action Clicked { get; set; }

        /// <summary>
        /// Provides options for inline or popout styles. Only supported by Vertical Bar. Uses inline by default.
        /// </summary>
        [Parameter]
        public BarTogglerMode Mode
        {
            get => mode;
            set
            {
                if ( mode == value )
                    return;

                mode = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Controls which Bar will be toggled. Uses parent Bar by default. 
        /// </summary>
        [Parameter]
        public Bar Bar
        {
            get => bar;
            set
            {
                if ( bar == value )
                    return;

                bar = value;

                DirtyClasses();
            }
        }

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
