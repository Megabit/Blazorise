﻿#region Using directives
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Provide contextual feedback messages for typical user actions with the handful of available and flexible alert messages.
    /// </summary>
    public partial class Alert : BaseComponent
    {
        #region Members

        /// <summary>
        /// Holds the state of the <see cref="Alert"/> component.
        /// </summary>
        private AlertState state = new()
        {
            Color = Color.None,
        };

        /// <summary>
        /// Flag that indicates if <see cref="Alert"/> contains the <see cref="AlertMessage"/> component.
        /// </summary>
        private bool hasMessage;

        /// <summary>
        /// Flag that indicates if <see cref="Alert"/> contains the <see cref="AlertDescription"/> component.
        /// </summary>
        private bool hasDescription;

        #endregion

        #region Methods

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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
            if ( Visible )
                return;

            Visible = true;
            InvokeAsync( StateHasChanged );
        }

        /// <summary>
        /// Conceals the alert from the user.
        /// </summary>
        public void Hide()
        {
            if ( !Visible )
                return;

            Visible = false;
            InvokeAsync( StateHasChanged );
        }

        /// <summary>
        /// Toggles the visibility of the alert.
        /// </summary>
        public void Toggle()
        {
            Visible = !Visible;
            InvokeAsync( StateHasChanged );
        }

        /// <summary>
        /// Sets the visibility state of this <see cref="Alert"/> component.
        /// </summary>
        /// <param name="visible">True if <see cref="Alert"/> is visible.</param>
        private void HandleVisibilityState( bool visible )
        {
            Display = visible
                ? Blazorise.Display.Always
                : Blazorise.Display.None;

            DirtyClasses();
        }

        /// <summary>
        /// Raises all registered events for this <see cref="Alert"/> component.
        /// </summary>
        /// <param name="visible">True if <see cref="Alert"/> is visible.</param>
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
            InvokeAsync( StateHasChanged );
        }

        /// <summary>
        /// Notifies the alert that one of the child componens is a description.
        /// </summary>
        internal void NotifyHasDescription()
        {
            hasDescription = true;

            DirtyClasses();
            InvokeAsync( StateHasChanged );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Enables the alert to be closed by placing the padding for close button.
        /// </summary>
        [Parameter]
        public bool Dismisable
        {
            get => state.Dismisable;
            set
            {
                state = state with { Dismisable = value };

                DirtyClasses();
            }
        }

        /// <summary>
        /// Sets the alert visibilty.
        /// </summary>
        [Parameter]
        public bool Visible
        {
            get => state.Visible;
            set
            {
                if ( value == state.Visible )
                    return;

                state = state with { Visible = value };

                HandleVisibilityState( value );
                RaiseEvents( value );
            }
        }

        /// <summary>
        /// Occurs when the alert visibility state changes.
        /// </summary>
        [Parameter] public EventCallback<bool> VisibleChanged { get; set; }

        /// <summary>
        /// Gets or sets the alert color.
        /// </summary>
        [Parameter]
        public Color Color
        {
            get => state.Color;
            set
            {
                state = state with { Color = value };

                DirtyClasses();
            }
        }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="Alert"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
