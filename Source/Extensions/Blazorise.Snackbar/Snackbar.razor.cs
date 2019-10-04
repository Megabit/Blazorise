#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Snackbar
{
    public abstract class BaseSnackbar : BaseComponent
    {
        #region Members

        private bool isOpen;

        private bool isMultiline;

        private SnackbarLocation location;

        private Timer timer;

        #endregion

        #region Methods

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

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( "snackbar" );
            builder.Append( "show", IsOpen );
            builder.Append( "snackbar-multi-line", IsMultiline );
            builder.Append( GetSnackbarLocation( Location ), Location != SnackbarLocation.None );

            base.BuildClasses( builder );
        }

        private static string GetSnackbarLocation( SnackbarLocation snackbarLocation )
        {
            switch ( snackbarLocation )
            {
                case SnackbarLocation.Left:
                    return "snackbar-left";
                case SnackbarLocation.Right:
                    return "snackbar-right";
                case SnackbarLocation.None:
                default:
                    return null;
            }
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

        public void Show()
        {
            timer?.Start();

            IsOpen = true;
            StateHasChanged();

            timer.Start();
        }

        public void Hide()
        {
            IsOpen = false;
            StateHasChanged();
        }

        private void Timer_Elapsed( object sender, ElapsedEventArgs e )
        {
            InvokeAsync( () => Hide() );
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

        [Parameter]
        public bool IsMultiline
        {
            get => isMultiline;
            set
            {
                isMultiline = value;

                DirtyClasses();
            }
        }

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

        [Parameter] public double Interval { get; set; } = 3000;

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
