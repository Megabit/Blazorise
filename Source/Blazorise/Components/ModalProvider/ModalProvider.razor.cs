#region Using directives
using System;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Modules;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    /// <summary>
    /// A modal provider to be set at the root of your app, providing a programmatic way to invoke modals with custom content by using ModalService.
    /// </summary>
    public partial class ModalProvider : BaseComponent
    {
        #region Members

        private Modal modalRef;
        private RenderFragment childContent;
        private string title;

        #endregion

        #region Constructors



        #endregion

        #region Methods

        ///inheritdoc
        protected override Task OnInitializedAsync()
        {
            ModalService.SetModalProvider( this );
            return base.OnInitializedAsync();
        }


        internal Task Show( string title, RenderFragment childContent, ModalProviderOptions modalProviderOptions )
        {
            this.title = title;
            this.childContent = childContent;
            this.ModalProviderOptions = modalProviderOptions;

            return modalRef.Show();
        }

        internal Task Hide()
            => modalRef.Hide();

        #endregion

        #region Properties

        ///inheritdoc
        [Inject] protected IModalService ModalService { get; set; }

        /// <summary>
        /// Sets the options for Modal Provider
        /// </summary>
        protected ModalProviderOptions ModalProviderOptions;

        /// <summary>
        /// Uses the modal standard structure, by setting this to true you are only in charge of providing the custom content.
        /// Defaults to true.
        /// </summary>
        [Parameter] public bool UseModalStructure { get; set; } = true;

        /// <summary>
        /// Gets whether it uses the modal standard structure, by setting this to true you are only in charge of providing the custom content.
        /// Defaults to true.
        /// </summary>
        protected virtual bool GetUseModalStructure() => ModalProviderOptions?.UseModalStructure ?? UseModalStructure;

        /// <summary>
        /// If true modal will scroll to top when opened.
        /// </summary>
        [Parameter] public bool ScrollToTop { get; set; } = true;

        /// <summary>
        /// Gets whether modal should scroll to top when opened.
        /// </summary>
        protected virtual bool GetScrollToTop() => ModalProviderOptions?.ScrollToTop ?? ScrollToTop;

        /// <summary>
        /// Occurs before the modal is opened.
        /// </summary>
        [Parameter] public Func<ModalOpeningEventArgs, Task> Opening { get; set; }

        /// <summary>
        /// Gets event for before the modal is opened.
        /// </summary>
        protected virtual Func<ModalOpeningEventArgs, Task> GetOpening() => ModalProviderOptions?.Opening ?? Opening;

        /// <summary>
        /// Occurs before the modal is closed.
        /// </summary>
        [Parameter] public Func<ModalClosingEventArgs, Task> Closing { get; set; }

        /// <summary>
        /// Gets event for before the modal is closed.
        /// </summary>
        protected virtual Func<ModalClosingEventArgs, Task> GetClosing() => ModalProviderOptions?.Closing ?? Closing;

        /// <summary>
        /// Occurs after the modal has opened.
        /// </summary>
        [Parameter] public EventCallback Opened { get; set; }

        /// <summary>
        /// Gets event for after the modal has opened.
        /// </summary>
        protected virtual EventCallback GetOpened() => ModalProviderOptions?.Opened ?? Opened;

        /// <summary>
        /// Occurs after the modal has closed.
        /// </summary>
        [Parameter] public EventCallback Closed { get; set; }

        /// <summary>
        /// Gets event for before the modal is closed.
        /// </summary>
        protected virtual EventCallback GetClosed() => ModalProviderOptions?.Closed ?? Closed;

        /// <summary>
        /// Specifies the backdrop needs to be rendered for this <see cref="Modal"/>.
        /// </summary>
        [Parameter] public bool ShowBackdrop { get; set; } = true;

        /// <summary>
        /// Gets if the backdrop needs to be rendered for this <see cref="Modal"/>.
        /// </summary>
        protected virtual bool GetShowBackdrop() => ModalProviderOptions?.ShowBackdrop ?? ShowBackdrop;

        /// <summary>
        /// Gets or sets whether the component has any animations.
        /// </summary>
        [Parameter] public bool Animated { get; set; } = true;

        /// <summary>
        /// Gets whether the component has any animations.
        /// </summary>
        protected virtual bool GetAnimated() => ModalProviderOptions?.Animated ?? Animated;

        /// <summary>
        /// Gets or sets the animation duration.
        /// </summary>
        [Parameter] public int AnimationDuration { get; set; } = 150;

        /// <summary>
        /// Gets the animation duration.
        /// </summary>
        protected virtual int GetAnimationDuration() => ModalProviderOptions?.AnimationDuration ?? AnimationDuration;

        /// <summary>
        /// Defines how the modal content will be rendered.
        /// </summary>
        [Parameter] public ModalRenderMode RenderMode { get; set; }

        /// <summary>
        /// Gets how the modal content will be rendered.
        /// </summary>
        protected virtual ModalRenderMode GetRenderMode() => ModalProviderOptions?.RenderMode ?? RenderMode;

        /// <summary>
        /// Defines if the modal should keep the input focus at all times.
        /// </summary>
        [Parameter] public bool? FocusTrap { get; set; }

        /// <summary>
        /// Gets if the modal should keep the input focus at all times.
        /// </summary>
        protected virtual bool? GetFocusTrap() => ModalProviderOptions?.FocusTrap ?? FocusTrap;

        #endregion
    }

}
