#region Using directives
using System;
using System.Collections.Generic;
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
    /// Generates a new instance of a Modal
    /// </summary>
    public class ModalInstance : BaseComponent
    {
        /// <summary>
        /// Tracks the Modal Reference
        /// </summary>
        public Modal ModalRef { get; set; }
        
        /// <summary>
        /// Child Content to be rendered
        /// </summary>
        public RenderFragment ChildContent { get; private set; }

        /// <summary>
        /// Modal's Header Title
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Control's the Modal Visibility
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="title"></param>
        /// <param name="childContent"></param>
        /// <param name="modalProviderOptions"></param>
        public ModalInstance(string title, RenderFragment childContent, ModalProviderOptions modalProviderOptions)
        {
            Title = title;
            ChildContent = childContent;
            ModalProviderOptions = modalProviderOptions;
            Visible = true;
        }

        /// <summary>
        /// Sets the options for Modal Provider
        /// </summary>
        public ModalProviderOptions ModalProviderOptions;

        /// <summary>
        /// Upon closing the modal, clears the content that has been previously rendered.
        /// Defaults to true.
        /// </summary>
        [Parameter] public bool ResetOnClose { get; set; } = true;

        /// <summary>
        /// Gets whether upon closing the modal, clears the content that has been previously rendered.
        /// Defaults to true.
        /// </summary>
        public virtual bool GetResetOnClose() => ModalProviderOptions?.ResetOnClose ?? ResetOnClose;

        /// <summary>
        /// Uses the modal standard structure, by setting this to true you are only in charge of providing the custom content.
        /// Defaults to true.
        /// </summary>
        [Parameter] public bool UseModalStructure { get; set; } = true;

        /// <summary>
        /// Gets whether it uses the modal standard structure, by setting this to true you are only in charge of providing the custom content.
        /// Defaults to true.
        /// </summary>
        public virtual bool GetUseModalStructure() => ModalProviderOptions?.UseModalStructure ?? UseModalStructure;

        /// <summary>
        /// If true modal will scroll to top when opened.
        /// </summary>
        [Parameter] public bool ScrollToTop { get; set; } = true;

        /// <summary>
        /// Gets whether modal should scroll to top when opened.
        /// </summary>
        public virtual bool GetScrollToTop() => ModalProviderOptions?.ScrollToTop ?? ScrollToTop;

        /// <summary>
        /// Occurs before the modal is opened.
        /// </summary>
        [Parameter] public Func<ModalOpeningEventArgs, Task> Opening { get; set; }

        /// <summary>
        /// Gets event for before the modal is opened.
        /// </summary>
        public virtual Func<ModalOpeningEventArgs, Task> GetOpening() => ModalProviderOptions?.Opening ?? Opening;

        /// <summary>
        /// Occurs before the modal is closed.
        /// </summary>
        [Parameter] public Func<ModalClosingEventArgs, Task> Closing { get; set; }

        /// <summary>
        /// Gets event for before the modal is closed.
        /// </summary>
        public virtual Func<ModalClosingEventArgs, Task> GetClosing() => ModalProviderOptions?.Closing ?? Closing;

        /// <summary>
        /// Occurs after the modal has opened.
        /// </summary>
        [Parameter] public EventCallback Opened { get; set; }

        /// <summary>
        /// Gets event for after the modal has opened.
        /// </summary>
        public virtual EventCallback GetOpened() => ModalProviderOptions?.Opened ?? Opened;

        /// <summary>
        /// Occurs after the modal has closed.
        /// </summary>
        [Parameter] public EventCallback Closed { get; set; }

        /// <summary>
        /// Gets event for before the modal is closed.
        /// </summary>
        public virtual EventCallback GetClosed() => ModalProviderOptions?.Closed ?? Closed;

        /// <summary>
        /// Specifies the backdrop needs to be rendered for this <see cref="Modal"/>.
        /// </summary>
        [Parameter] public bool ShowBackdrop { get; set; } = true;

        /// <summary>
        /// Gets if the backdrop needs to be rendered for this <see cref="Modal"/>.
        /// </summary>
        public virtual bool GetShowBackdrop() => ModalProviderOptions?.ShowBackdrop ?? ShowBackdrop;

        /// <summary>
        /// Gets or sets whether the component has any animations.
        /// </summary>
        [Parameter] public bool Animated { get; set; } = true;

        /// <summary>
        /// Gets whether the component has any animations.
        /// </summary>
        public virtual bool GetAnimated() => ModalProviderOptions?.Animated ?? Animated;

        /// <summary>
        /// Gets or sets the animation duration.
        /// </summary>
        [Parameter] public int AnimationDuration { get; set; } = 150;

        /// <summary>
        /// Gets the animation duration.
        /// </summary>
        public virtual int GetAnimationDuration() => ModalProviderOptions?.AnimationDuration ?? AnimationDuration;

        /// <summary>
        /// Defines how the modal content will be rendered.
        /// </summary>
        [Parameter] public ModalRenderMode RenderMode { get; set; }

        /// <summary>
        /// Gets how the modal content will be rendered.
        /// </summary>
        public virtual ModalRenderMode GetRenderMode() => ModalProviderOptions?.RenderMode ?? RenderMode;

        /// <summary>
        /// Defines if the modal should keep the input focus at all times.
        /// </summary>
        [Parameter] public bool? FocusTrap { get; set; }

        /// <summary>
        /// Gets if the modal should keep the input focus at all times.
        /// </summary>
        public virtual bool? GetFocusTrap() => ModalProviderOptions?.FocusTrap ?? FocusTrap;
    }

    /// <summary>
    /// A modal provider to be set at the root of your app, providing a programmatic way to invoke modals with custom content by using ModalService.
    /// </summary>
    public partial class ModalProvider : BaseComponent
    {
        #region Members

        private List<ModalInstance> modalInstances;

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
            modalInstances ??= new();
            
            var newModalInstance = new ModalInstance( title, childContent, modalProviderOptions );
            modalInstances.Add( newModalInstance );
            
            return InvokeAsync( StateHasChanged );
        }

        internal Task Hide()
            => modalInstances[0].ModalRef.Hide(); //TODO: Which one to select? Hide Last()?

        /// <summary>
        /// Handles the closing of the modal.
        /// </summary>
        /// <returns></returns>
        protected async Task OnModalClosed()
        {
            //if ( GetResetOnClose() )
            //    modalInstances[0].ChildContent = null; //TODO: Which one to select? // Set By Method
            //await GetClosed().InvokeAsync();
        }

        #endregion

        #region Properties

        ///inheritdoc
        [Inject] protected IModalService ModalService { get; set; }

        #endregion
    }

}
