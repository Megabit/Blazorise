#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Timers;
using Blazorise.Snackbar.Utils;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Snackbar
{
    public partial class SnackbarStack : BaseComponent
    {
        #region Members

        private class SnackbarInfo
        {
            public SnackbarInfo( string key, string message, RenderFragment messageTemplate, SnackbarColor color, string actionButtonText )
            {
                Key = key ?? Guid.NewGuid().ToString();
                Message = message;
                MessageTemplate = messageTemplate;
                Color = color;
                ActionButtonText = actionButtonText;
            }

            public string Key { get; }

            public string Message { get; }

            public RenderFragment MessageTemplate { get; }

            public SnackbarColor Color { get; }

            public string ActionButtonText { get; }

            public bool Visible { get; } = true;
        }

        private SnackbarStackLocation location = SnackbarStackLocation.Center;

        private List<SnackbarInfo> snackbarInfos = new List<SnackbarInfo>();

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( "snackbar-stack" );
            builder.Append( $"snackbar-stack-{Location.GetName()}" );

            base.BuildClasses( builder );
        }

        public void Push( string message, SnackbarColor color = SnackbarColor.None, string actionButtonText = null )
        {
            Push( null, message, null, color, actionButtonText );
        }

        public void Push( RenderFragment messageTemplate, SnackbarColor color = SnackbarColor.None, string actionButtonText = null )
        {
            Push( null, null, messageTemplate, color, actionButtonText );
        }

        public void Push( string key, string message, RenderFragment messageTemplate, SnackbarColor color = SnackbarColor.None, string actionButtonText = null )
        {
            snackbarInfos.Add( new SnackbarInfo( key, message, messageTemplate, color, actionButtonText ) );

            StateHasChanged();
        }

        private Task OnSnackbarClosed( string key, SnackbarCloseReason closeReason )
        {
            var info = snackbarInfos.FirstOrDefault( x => x.Key == key );

            if ( info != null )
                snackbarInfos.Remove( info );

            StateHasChanged();

            return Closed.InvokeAsync( new SnackbarClosedEventArgs( key, closeReason ) );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Defines the snackbar stack location.
        /// </summary>
        [Parameter]
        public SnackbarStackLocation Location
        {
            get => location;
            set
            {
                location = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Allow snackbar to show multiple lines of text.
        /// </summary>
        [Parameter] public bool Multiline { get; set; }

        /// <summary>
        /// Defines the interval(in milliseconds) after which the snackbars will be automatically closed.
        /// </summary>
        [Parameter] public double Interval { get; set; } = 3000;

        /// <summary>
        /// Defines a text to show for snackbar action button. Leave as null to not show it!
        /// </summary>
        [Parameter] public string ActionButtonText { get; set; }

        /// <summary>
        /// Occurs after the snackbar has closed.
        /// </summary>
        [Parameter] public EventCallback<SnackbarClosedEventArgs> Closed { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
