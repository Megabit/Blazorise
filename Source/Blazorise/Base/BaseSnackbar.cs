#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseSnackbar : BaseComponent
    {
        #region Members

        private bool isOpen;

        private bool isMultiline;

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
                    timer = null;
                }
            }

            base.Dispose( disposing );
        }

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Snackbar() )
                .If( () => ClassProvider.SnackbarShow(), () => IsOpen )
                .If( () => ClassProvider.SnackbarMultiline(), () => IsMultiline );

            base.RegisterClasses();
        }

        protected override void OnInit()
        {
            if ( timer == null )
            {
                timer = new Timer();
                timer.Interval = Interval;
                timer.Elapsed += Timer_Elapsed;
                timer.AutoReset = false;
            }

            base.OnInit();
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
        private bool IsOpen
        {
            get => isOpen;
            set
            {
                isOpen = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter]
        private bool IsMultiline
        {
            get => isMultiline;
            set
            {
                isMultiline = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter]
        private double Interval { get; set; } = 3000;

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
