#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseButton : BaseSizableComponent
    {
        #region Members

        private Color color = Color.None;

        private ButtonSize size = ButtonSize.None;

        private bool isOutline;

        private bool isDisabled;

        private bool isActive;

        private bool isBlock;

        private bool isLoading;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Button() );
            builder.Append( ClassProvider.ButtonColor( Color ), Color != Color.None && !IsOutline );
            builder.Append( ClassProvider.ButtonOutline( Color ), Color != Color.None && IsOutline );
            builder.Append( ClassProvider.ButtonSize( Size ), Size != ButtonSize.None );
            builder.Append( ClassProvider.ButtonBlock(), IsBlock );
            builder.Append( ClassProvider.ButtonActive(), IsActive );
            builder.Append( ClassProvider.ButtonLoading(), IsLoading );

            base.BuildClasses( builder );
        }

        protected void ClickHandler()
        {
            if ( !IsDisabled )
                Clicked.InvokeAsync( null );
        }

        protected override void OnInitialized()
        {
            // notify dropdown that the button is inside of it
            ParentDropdown?.Register( this );

            ExecuteAfterRender( async () =>
            {
                await JSRunner.InitializeButton( ElementId, ElementRef, PreventDefaultOnSubmit );
            } );

            base.OnInitialized();
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                JSRunner.DestroyButton( ElementId );
            }

            base.Dispose( disposing );
        }

        /// <summary>
        /// Sets focus on the button element, if it can be focused.
        /// </summary>
        /// <param name="scrollToElement">If true the browser should scroll the document to bring the newly-focused element into view.</param>
        public void Focus( bool scrollToElement = true )
        {
            _ = JSRunner.Focus( ElementRef, ElementId, scrollToElement );
        }

        #endregion

        #region Properties

        protected bool IsAddons => ParentButtons?.Role == ButtonsRole.Addons || ParentDropdown?.IsGroup == true;

        /// <summary>
        /// Occurs when the button is clicked.
        /// </summary>
        [Parameter] public EventCallback Clicked { get; set; }

        /// <summary>
        /// Defines the button type.
        /// </summary>
        [Parameter] public ButtonType Type { get; set; } = ButtonType.Button;

        /// <summary>
        /// Gets or sets the button color.
        /// </summary>
        [Parameter]
        public Color Color
        {
            get => color;
            set
            {
                color = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Changes the size of a button.
        /// </summary>
        [Parameter]
        public ButtonSize Size
        {
            get => size;
            set
            {
                size = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Makes the button to have the outlines.
        /// </summary>
        [Parameter]
        public bool IsOutline
        {
            get => isOutline;
            set
            {
                isOutline = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Makes button look inactive.
        /// </summary>
        [Parameter]
        public bool IsDisabled
        {
            get => isDisabled;
            set
            {
                isDisabled = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Makes the button to appear as pressed.
        /// </summary>
        [Parameter]
        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Makes the button to span the full width of a parent.
        /// </summary>
        [Parameter]
        public bool IsBlock
        {
            get => isBlock;
            set
            {
                isBlock = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Shows the loading spinner.
        /// </summary>
        [Parameter]
        public bool IsLoading
        {
            get => isLoading;
            set
            {
                isLoading = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Prevents a default form-post when button type is set to <see cref="ButtonType.Submit"/>.
        /// </summary>
        [Parameter] public bool PreventDefaultOnSubmit { get; set; }

        [CascadingParameter] public BaseDropdown ParentDropdown { get; set; }

        [CascadingParameter] public BaseButtons ParentButtons { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
