#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseAlert : BaseComponent
    {
        #region Members

        private bool isDismisable;

        private bool isShow;

        private Color color = Color.None;

        public event EventHandler<AlertStateEventArgs> StateChanged;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Alert() );
            builder.Append( ClassProvider.AlertColor( Color ), Color != Color.None );
            builder.Append( ClassProvider.AlertDismisable(), IsDismisable );
            builder.Append( ClassProvider.AlertFade(), IsDismisable );
            builder.Append( ClassProvider.AlertShow(), IsDismisable && IsShow );

            base.BuildClasses( builder );
        }

        protected override void OnInitialized()
        {
            HandleVisibilityState( IsShow );

            base.OnInitialized();
        }

        public void Show()
        {
            IsShow = true;
            StateHasChanged();
        }

        public void Hide()
        {
            IsShow = false;
            StateHasChanged();
        }

        public void Toggle()
        {
            IsShow = !IsShow;
            StateHasChanged();
        }

        private void HandleVisibilityState( bool active )
        {
            Visibility = active ? Visibility.Always : Visibility.Never;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Enables the alert to be closed by placing the padding for close button.
        /// </summary>
        [Parameter]
        public bool IsDismisable
        {
            get => isDismisable;
            set
            {
                isDismisable = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Sets the alert visibilty.
        /// </summary>
        [Parameter]
        public bool IsShow
        {
            get => isShow;
            set
            {
                // prevent alert from calling the same code multiple times
                if ( value == isShow )
                    return;

                isShow = value;

                HandleVisibilityState( value );

                StateChanged?.Invoke( this, new AlertStateEventArgs( isShow ) );

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
