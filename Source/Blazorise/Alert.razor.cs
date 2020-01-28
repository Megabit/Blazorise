#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Alert : BaseComponent
    {
        #region Members

        private bool dismisable;

        private bool visible;

        private Color color = Color.None;

        public event EventHandler<AlertStateEventArgs> StateChanged;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Alert() );
            builder.Append( ClassProvider.AlertColor( Color ), Color != Color.None );
            builder.Append( ClassProvider.AlertDismisable(), Dismisable );
            builder.Append( ClassProvider.AlertFade(), Dismisable );
            builder.Append( ClassProvider.AlertShow(), Dismisable && Visible );

            base.BuildClasses( builder );
        }

        /// <summary>
        /// Displays the alert to the user.
        /// </summary>
        public void Show()
        {
            Visible = true;
            StateHasChanged();
        }

        /// <summary>
        /// Conceals the alert from the user.
        /// </summary>
        public void Hide()
        {
            Visible = false;
            StateHasChanged();
        }

        /// <summary>
        /// Toggles the visibility of the alert.
        /// </summary>
        public void Toggle()
        {
            Visible = !Visible;
            StateHasChanged();
        }

        private void HandleVisibilityState( bool active )
        {
            Visibility = active ? Visibility.Always : Visibility.Never;

            StateChanged?.Invoke( this, new AlertStateEventArgs( active ) );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Enables the alert to be closed by placing the padding for close button.
        /// </summary>
        [Parameter]
        public bool Dismisable
        {
            get => dismisable;
            set
            {
                dismisable = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Sets the alert visibilty.
        /// </summary>
        [Parameter]
        public bool Visible
        {
            get => visible;
            set
            {
                // prevent alert from calling the same code multiple times
                if ( value == visible )
                    return;

                visible = value;

                HandleVisibilityState( value );

                DirtyClasses();
            }
        }

        /// <summary>
        /// Gets or sets the alert color.
        /// </summary>
        [Parameter]
        public Color Color
        {
            get => color;
            set
            {
                color = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
