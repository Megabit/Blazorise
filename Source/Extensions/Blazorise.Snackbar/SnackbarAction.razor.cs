#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Snackbar.Utils;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Snackbar
{
    public partial class SnackbarAction : BaseComponent, IDisposable
    {
        #region Members

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            ParentSnackbar?.NotifySnackbarActionInitialized( this );

            base.OnInitialized();
        }

        /// <inheritdoc/>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                DisposeResources();
            }

            base.Dispose( disposing );
        }

        /// <inheritdoc/>
        protected override ValueTask DisposeAsync( bool disposing )
        {
            if ( disposing )
            {
                DisposeResources();
            }

            return base.DisposeAsync( disposing );
        }

        private void DisposeResources()
        {
            ParentSnackbar?.NotifySnackbarActionRemoved( this );
        }

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( "snackbar-action-button" );
            builder.Append( $"snackbar-action-button-{ ParentSnackbar.Color.GetName()}", ParentSnackbar != null && ParentSnackbar.Color != SnackbarColor.None );

            base.BuildClasses( builder );
        }

        protected Task ClickHandler()
        {
            return Clicked.InvokeAsync();
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
