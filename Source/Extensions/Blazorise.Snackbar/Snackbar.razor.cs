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
    public abstract class BaseSnackbar : BaseComponent, IDisposable
    {
        #region Members

        private bool isOpen;

        private bool isMultiline;

        private SnackbarLocation location;

        private Timer timer;

        #endregion

        #region Methods

        public void Dispose()
        {
            if ( timer != null )
            {
                timer.Elapsed -= Timer_Elapsed;
                timer = null;
            }
        }

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => "snackbar" )
                .If( () => "show", () => IsOpen )
                .If( () => "snackbar-multi-line", () => IsMultiline )
                .If( () => GetSnackbarLocation( Location ), () => Location != SnackbarLocation.None );

            base.RegisterClasses();
        }

        private static string GetSnackbarLocation( SnackbarLocation snackbarLocation )
        {
            switch ( snackbarLocation )
            {
                case SnackbarLocation.Left:
                    return "snackbar-left";
                case SnackbarLocation.Right:
                    return "snackbar-right";
                default:
                    return null;
            }
        }

        protected override void OnInitialized()
        {
            if ( timer == null )
            {
                timer = new Timer();
                timer.Interval = Interval;
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
            Hide();
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

                ClassMapper.Dirty();
            }
        }

        [Parameter]
        public bool IsMultiline
        {
            get => isMultiline;
            set
            {
                isMultiline = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter]
        public SnackbarLocation Location
        {
            get => location;
            set
            {
                location = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] public double Interval { get; set; } = 3000;

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
