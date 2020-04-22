﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class BarMenu : BaseComponent
    {
        #region Members

        private bool visible;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            // TODO: consolidate
            builder.Append( ClassProvider.BarMenu(), ParentBar.Mode == BarMode.Horizontal );
            builder.Append( "navbar-menu", ParentBar.Mode != BarMode.Horizontal );

            builder.Append( ClassProvider.BarMenuShow(), Visible );

            base.BuildClasses( builder );
        }

        protected override void OnInitialized()
        {
            if ( ParentBar != null && ParentBar.Mode == BarMode.Horizontal )
            {
                Visible = ParentBar.Visible;

                ParentBar.StateChanged += OnBarStateChanged;
            }

            base.OnInitialized();
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( ParentBar != null )
                {
                    ParentBar.StateChanged -= OnBarStateChanged;
                }
            }

            base.Dispose( disposing );
        }

        public void Toggle()
        {
            Visible = !Visible;
            Toggled?.Invoke( Visible );

            StateHasChanged();
        }

        private void OnBarStateChanged( object sender, BarStateEventArgs e )
        {
            Visible = e.Visible;
        }

        #endregion

        #region Properties

        [Parameter]
        public bool Visible
        {
            get => visible;
            set
            {
                visible = value;

                DirtyClasses();
            }
        }

        [Parameter] public Action<bool> Toggled { get; set; }

        [CascadingParameter] protected Bar ParentBar { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
