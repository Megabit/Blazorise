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
    public partial class Alert : BaseComponent
    {
        #region Members

        private AlertStore store = new AlertStore
        {
            Color = Color.None,
        };

        private bool hasMessage;

        private bool hasDescription;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Alert() );
            builder.Append( ClassProvider.AlertColor( Color ), Color != Color.None );
            builder.Append( ClassProvider.AlertDismisable(), Dismisable );
            builder.Append( ClassProvider.AlertFade(), Dismisable );
            builder.Append( ClassProvider.AlertShow(), Dismisable && Visible );
            builder.Append( ClassProvider.AlertHasMessage(), hasMessage );
            builder.Append( ClassProvider.AlertHasDescription(), hasDescription );

            base.BuildClasses( builder );
        }

        protected override void OnInitialized()
        {
            HandleVisibilityState( Visible );

            base.OnInitialized();
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
            Display = active
                ? Blazorise.Display.Always
                : Blazorise.Display.None;

            DirtyClasses();
        }

        private void RaiseEvents( bool visible )
        {
            VisibleChanged.InvokeAsync( visible );
        }

        /// <summary>
        /// Notifies the alert that one of the child componens is a message.
        /// </summary>
        internal void NotifyHasMessage()
        {
            hasMessage = true;

            DirtyClasses();
            StateHasChanged();
        }

        /// <summary>
        /// Notifies the alert that one of the child componens is a description.
        /// </summary>
        internal void NotifyHasDescription()
        {
            hasDescription = true;

            DirtyClasses();
            StateHasChanged();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Enables the alert to be closed by placing the padding for close button.
        /// </summary>
        [Parameter]
        public bool Dismisable
        {
            get => store.Dismisable;
            set
            {
                store.Dismisable = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Sets the alert visibilty.
        /// </summary>
        [Parameter]
        public bool Visible
        {
            get => store.Visible;
            set
            {
                if ( value == store.Visible )
                    return;

                store.Visible = value;

                HandleVisibilityState( value );
                RaiseEvents( value );
            }
        }

        /// <summary>
        /// Occurs when the alert visibility changes.
        /// </summary>
        [Parameter] public EventCallback<bool> VisibleChanged { get; set; }

        /// <summary>
        /// Gets or sets the alert color.
        /// </summary>
        [Parameter]
        public Color Color
        {
            get => store.Color;
            set
            {
                store.Color = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
