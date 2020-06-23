#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Button : BaseComponent
    {
        #region Members

        private Color color = Color.None;

        private ButtonSize size = ButtonSize.None;

        private bool outline;

        private bool disabled;

        private bool active;

        private bool block;

        private bool loading;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Button() );
            builder.Append( ClassProvider.ButtonColor( Color ), Color != Color.None && !Outline );
            builder.Append( ClassProvider.ButtonOutline( Color ), Color != Color.None && Outline );
            builder.Append( ClassProvider.ButtonSize( Size ), Size != ButtonSize.None );
            builder.Append( ClassProvider.ButtonBlock(), Block );
            builder.Append( ClassProvider.ButtonActive(), Active );
            builder.Append( ClassProvider.ButtonLoading(), Loading );

            base.BuildClasses( builder );
        }

        protected async Task ClickHandler()
        {
            if ( !Disabled )
            {
                await Clicked.InvokeAsync( null );

                if ( Command?.CanExecute( CommandParameter ) ?? false )
                {
                    Command.Execute( CommandParameter );
                }
            }
        }

        protected override void OnInitialized()
        {
            // notify dropdown that the button is inside of it
            ParentDropdown?.Register( this );

            // notify addons that the button is inside of it
            ParentAddons?.Register( this );

            ExecuteAfterRender( async () =>
            {
                await JSRunner.InitializeButton( ElementRef, ElementId, PreventDefaultOnSubmit );
            } );

            base.OnInitialized();
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                // remove button from parents
                ParentDropdown?.UnRegister( this );
                ParentAddons?.UnRegister( this );

                if ( Rendered )
                {
                    JSRunner.DestroyButton( ElementId );
                }
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

        /// <summary>
        /// True if button is part of an addons or dropdown group.
        /// </summary>
        protected bool IsAddons => ParentButtons?.Role == ButtonsRole.Addons || ParentDropdown?.IsGroup == true;

        /// <summary>
        /// True if button is placed inside of a <see cref="Field"/>.
        /// </summary>
        protected bool ParentIsField => ParentField != null;

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
        public bool Outline
        {
            get => outline;
            set
            {
                outline = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Makes button look inactive.
        /// </summary>
        [Parameter]
        public bool Disabled
        {
            get => disabled;
            set
            {
                disabled = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Makes the button to appear as pressed.
        /// </summary>
        [Parameter]
        public bool Active
        {
            get => active;
            set
            {
                active = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Makes the button to span the full width of a parent.
        /// </summary>
        [Parameter]
        public bool Block
        {
            get => block;
            set
            {
                block = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Shows the loading spinner.
        /// </summary>
        [Parameter]
        public bool Loading
        {
            get => loading;
            set
            {
                loading = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Prevents a default form-post when button type is set to <see cref="ButtonType.Submit"/>.
        /// </summary>
        [Parameter] public bool PreventDefaultOnSubmit { get; set; }

        [CascadingParameter] protected Dropdown ParentDropdown { get; set; }

        [CascadingParameter] protected Buttons ParentButtons { get; set; }

        [CascadingParameter] protected Addons ParentAddons { get; set; }

        [CascadingParameter] protected Field ParentField { get; set; }

        /// <summary>
        /// Gets or sets the command to be executed when clicked on a button.
        /// </summary>
        [Parameter] public ICommand Command { get; set; }

        /// <summary>
        /// Reflects the parameter to pass to the CommandProperty upon execution.
        /// </summary>
        [Parameter] public object CommandParameter { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
