#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Blazorise.Snackbar.Utils;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Snackbar
{
    public partial class Snackbar : BaseComponent
    {
        #region Members

        private bool visible;

        private bool multiline;

        private SnackbarLocation location;

        private SnackbarColor snackbarColor = SnackbarColor.None;

        private Timer timer;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( "snackbar" );
            builder.Append( "show", Visible );
            builder.Append( "snackbar-multi-line", Multiline );
            builder.Append( $"snackbar-{ Location.GetName()}", Location != SnackbarLocation.None );
            builder.Append( $"snackbar-{Color.GetName()}", Color != SnackbarColor.None );

            base.BuildClasses( builder );
        }

        protected override void OnInitialized()
        {
            if ( timer == null )
            {
                timer = new Timer
                {
                    Interval = Interval
                };
                timer.Elapsed += Timer_Elapsed;
                timer.AutoReset = false;
            }

            base.OnInitialized();
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( timer != null )
                {
                    timer.Elapsed -= Timer_Elapsed;
                    timer.Dispose();
                    timer = null;
                }
            }

            base.Dispose( disposing );
        }

        protected Task OnClickHandler()
        {
            Hide( SnackbarCloseReason.UserClosed );

            return Task.CompletedTask;
        }

        /// <summary>
        /// Shows the snackbar.
        /// </summary>
        public void Show()
        {
            if ( Visible )
                return;

            timer.Start();

            Visible = true;

            StateHasChanged();
        }

        /// <summary>
        /// Hides the snackbar.
        /// </summary>
        public void Hide()
        {
            Hide( SnackbarCloseReason.UserClosed );
        }

        private void Hide( SnackbarCloseReason closeReason )
        {
            if ( !Visible )
                return;

            Visible = false;

            _ = Closed.InvokeAsync( new SnackbarClosedEventArgs( closeReason ) );

            StateHasChanged();
        }

        private void Timer_Elapsed( object sender, ElapsedEventArgs e )
        {
            // InvokeAsync is used to prevent from blocking threads
            InvokeAsync( () => Hide( SnackbarCloseReason.None ) );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Defines the visibility of snackbar.
        /// </summary>
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

        /// <summary>
        /// Allow snackbar to show multiple lines of text.
        /// </summary>
        [Parameter]
        public bool Multiline
        {
            get => multiline;
            set
            {
                multiline = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Defines the snackbar location.
        /// </summary>
        [Parameter]
        public SnackbarLocation Location
        {
            get => location;
            set
            {
                location = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Defines the snackbar color.
        /// </summary>
        [Parameter]
        public SnackbarColor Color
        {
            get => snackbarColor;
            set
            {
                snackbarColor = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Defines the interval(in milliseconds) after which the snackbar will be automatically closed.
        /// </summary>
        [Parameter] public double Interval { get; set; } = 3000;

        /// <summary>
        /// Occurs after the snackbar has closed.
        /// </summary>
        [Parameter] public EventCallback<SnackbarClosedEventArgs> Closed { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
