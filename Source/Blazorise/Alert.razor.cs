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

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Alert() );
            builder.Append( ClassProvider.AlertColor( Color ), Color != Color.None );
            builder.Append( ClassProvider.AlertDismisable(), IsDismisable );
            builder.Append( ClassProvider.Fade(), IsDismisable );
            builder.Append( ClassProvider.Show(), IsDismisable && IsShow );

            base.BuildClasses( builder );
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

                Dirty();
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
                isShow = value;

                Dirty();
            }
        }

        [Parameter]
        public Color Color
        {
            get => color;
            set
            {
                color = value;

                Dirty();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
