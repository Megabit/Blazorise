﻿#region Using directives
using System.Threading.Tasks;
using Blazorise.Snackbar.Utils;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Snackbar
{
    public partial class SnackbarAction : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            if ( ParentSnackbar != null )
            {
                ParentSnackbar.NotifySnackbarActionInitialized( this );
            }

            base.OnInitialized();
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( ParentSnackbar != null )
                {
                    ParentSnackbar.NotifySnackbarActionRemoved( this );
                }
            }

            base.Dispose( disposing );
        }

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( "snackbar-action-button" );
            builder.Append( $"snackbar-action-button-{ ParentSnackbar.Color.GetName()}", ParentSnackbar != null && ParentSnackbar.Color != SnackbarColor.None );

            base.BuildClasses( builder );
        }

        protected Task ClickHandler()
        {
            return Clicked.InvokeAsync( null );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Occurs when the button is clicked.
        /// </summary>
        [Parameter] public EventCallback Clicked { get; set; }

        [CascadingParameter] protected Snackbar ParentSnackbar { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="SnackbarAction"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
