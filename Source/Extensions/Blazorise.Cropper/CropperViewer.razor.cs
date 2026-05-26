#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Infrastructure;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Cropper
{
    /// <summary>
    /// Displays a synchronized preview of the cropper selection.
    /// </summary>
    public partial class CropperViewer : BaseComponent, IDisposable
    {
        #region Members

        private readonly EventCallbackSubscriber<Cropper> cropperInitialized;

        #endregion

        #region Constructors

        /// <summary>
        /// A default <see cref="CropperViewer"/> constructor.
        /// </summary>
        public CropperViewer()
        {
            cropperInitialized = new EventCallbackSubscriber<Cropper>( EventCallback.Factory.Create<Cropper>( this, OnCropperInitialized ) );
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override Task OnParametersSetAsync()
        {
            cropperInitialized.SubscribeOrReplace( CropperState?.CropperInitialized );

            return base.OnParametersSetAsync();
        }

        /// <inheritdoc/>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                cropperInitialized.Dispose();
            }

            base.Dispose( disposing );
        }

        private async Task OnCropperInitialized( Cropper cropper )
        {
            if ( cropper?.JSModule != null )
            {
                await cropper.JSModule.InitializeViewer( cropper.ElementRef, cropper.ElementId, ElementRef, ElementId, new
                {
                } );
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Provides the shared state and synchronization context between the cropper and cropper viewer.
        /// </summary>
        [Parameter] public CropperState CropperState { get; set; }

        #endregion
    }
}