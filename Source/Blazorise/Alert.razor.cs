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

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Alert() )
                .If( () => ClassProvider.AlertColor( Color ), () => Color != Color.None )
                .If( () => ClassProvider.AlertDismisable(), () => IsDismisable )
                .If( () => ClassProvider.Fade(), () => IsDismisable )
                .If( () => ClassProvider.Show(), () => IsDismisable && IsShow );

            base.RegisterClasses();
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
        protected bool IsDismisable
        {
            get => isDismisable;
            set
            {
                isDismisable = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Sets the alert visibilty.
        /// </summary>
        [Parameter]
        protected bool IsShow
        {
            get => isShow;
            set
            {
                isShow = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter]
        protected Color Color
        {
            get => color;
            set
            {
                color = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
