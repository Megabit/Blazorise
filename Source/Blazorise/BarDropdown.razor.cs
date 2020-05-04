#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class BarDropdown : BaseComponent
    {
        #region Members

        private bool visible;

        private BarMode mode;

        public event EventHandler<BarDropdownStateEventArgs> StateChanged;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarDropdown( Mode ) );
            builder.Append( ClassProvider.BarDropdownShow( Mode ), Visible );

            base.BuildClasses( builder );
        }

        protected override void OnInitialized()
        {
            // link to the parent component
            ParentBarItem?.Hook( this );

            base.OnInitialized();
        }

        public void Show()
        {
            var temp = Visible;

            Visible = true;

            if ( temp != Visible ) // used to prevent toggle event call if Open() is called multiple times
                Toggled?.Invoke( Visible );

            ParentBarItem?.MenuChanged();

            StateHasChanged();
        }

        public void Hide()
        {
            var temp = Visible;

            Visible = false;

            if ( temp != Visible ) // used to prevent toggle event call if Close() is called multiple times
                Toggled?.Invoke( Visible );

            ParentBarItem?.MenuChanged();

            StateHasChanged();
        }

        public void Toggle()
        {
            Visible = !Visible;
            Toggled?.Invoke( Visible );

            ParentBarItem?.MenuChanged();

            StateHasChanged();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the control and all its child controls are displayed.
        /// </summary>
        [Parameter]
        public bool Visible
        {
            get => visible;
            set
            {
                // prevent dropdown from calling the same code multiple times
                if ( value == visible )
                    return;

                visible = value;

                StateChanged?.Invoke( this, new BarDropdownStateEventArgs( visible ) );

                DirtyClasses();
            }
        }

        [Parameter] public Action<bool> Toggled { get; set; }

        [CascadingParameter] protected BarItem ParentBarItem { get; set; }

        [CascadingParameter( Name = "IconName" )] protected object IconName { get; set; }

        [CascadingParameter( Name = "Mode" )] protected BarMode Mode
        {
            get => mode;
            set
            {
                mode = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
