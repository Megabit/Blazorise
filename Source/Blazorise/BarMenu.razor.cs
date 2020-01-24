#region Using directives
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

        private bool isOpen;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarMenu() );
            builder.Append( ClassProvider.BarMenuShow(), IsOpen );

            base.BuildClasses( builder );
        }

        protected override void OnInitialized()
        {
            if ( ParentBar != null )
            {
                IsOpen = ParentBar.IsOpen;

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
            IsOpen = !IsOpen;
            Toggled?.Invoke( IsOpen );

            StateHasChanged();
        }

        private void OnBarStateChanged( object sender, BarStateEventArgs e )
        {
            IsOpen = e.Opened;
        }

        #endregion

        #region Properties

        [Parameter]
        public bool IsOpen
        {
            get => isOpen;
            set
            {
                isOpen = value;

                DirtyClasses();
            }
        }

        [Parameter] public Action<bool> Toggled { get; set; }

        [CascadingParameter] protected Bar ParentBar { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
