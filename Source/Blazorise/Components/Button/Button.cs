﻿#region Using directives
using System.Threading.Tasks;
using System.Windows.Input;
using Blazorise.Extensions;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Clickable button for actions in forms, dialogs, and more with support for multiple sizes, states, and more.
    /// </summary>
    public partial class Button : BaseComponent
    {
        #region Members

        private Color color = Color.None;

        private Size size = Size.None;

        private bool outline;

        private bool disabled;

        private bool active;

        private bool block;

        private bool loading;

        private DropdownState parentDropdownState;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Button() );
            builder.Append( ClassProvider.ButtonColor( Color ), Color != Color.None && !Outline );
            builder.Append( ClassProvider.ButtonOutline( Color ), Color != Color.None && Outline );
            builder.Append( ClassProvider.ButtonSize( Size ), Size != Size.None );
            builder.Append( ClassProvider.ButtonBlock(), Block );
            builder.Append( ClassProvider.ButtonActive(), Active );
            builder.Append( ClassProvider.ButtonLoading(), Loading && LoadingTemplate == null );

            base.BuildClasses( builder );
        }

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            // notify dropdown that the button is inside of it
            ParentDropdown?.NotifyButtonInitialized( this );

            // notify addons that the button is inside of it
            ParentAddons?.Register( this );

            ExecuteAfterRender( async () =>
            {
                await JSRunner.InitializeButton( ElementRef, ElementId, PreventDefaultOnSubmit );
            } );

            if ( LoadingTemplate == null )
            {
                LoadingTemplate = ProvideDefaultLoadingTemplate();
            }

            base.OnInitialized();
        }

        /// <summary>
        /// Provides a default LoadingTemplate RenderFragment.
        /// </summary>
        /// <returns>Returns the RenderFragment consisting of a loading content.</returns>
        protected virtual RenderFragment ProvideDefaultLoadingTemplate() => null;

        /// <inheritdoc/>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                // remove button from parents
                ParentDropdown?.NotifyButtonRemoved( this );
                ParentAddons?.UnRegister( this );

                if ( Rendered )
                {
                    JSRunner.DestroyButton( ElementId );
                }
            }

            base.Dispose( disposing );
        }

        /// <summary>
        /// Handles the item onclick event.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
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

        /// <summary>
        /// Sets focus on the button element, if it can be focused.
        /// </summary>
        /// <param name="scrollToElement">If true the browser should scroll the document to bring the newly-focused element into view.</param>
        public void Focus( bool scrollToElement = true )
        {
            _ = JSRunner.Focus( ElementRef, ElementId, scrollToElement );
        }

        protected override void BuildRenderTree( RenderTreeBuilder builder )
        {
            builder
                .OpenElement( Type.ToButtonTagName() )
                .Id( ElementId )
                .Type( Type.ToButtonTypeString() )
                .Class( ClassNames )
                .Style( StyleNames )
                .Disabled( Disabled )
                .AriaPressed( Active )
                .TabIndex( TabIndex );

            if ( Type == ButtonType.Link && To != null )
            {
                builder
                    .Role( "button" )
                    .Href( To )
                    .Target( Target );
            }
            else
            {
                builder.OnClick( this, EventCallback.Factory.Create( this, ClickHandler ) );
            }

            builder.Attributes( Attributes );
            builder.ElementReferenceCapture( capturedRef => ElementRef = capturedRef );

            if ( Loading && LoadingTemplate != null )
            {
                builder.Content( LoadingTemplate );
            }
            else
            {
                builder.Content( ChildContent );
            }

            builder.CloseElement();

            base.BuildRenderTree( builder );
        }

        #endregion

        #region Properties 

        /// <inheritdoc/>
        protected override bool ShouldAutoGenerateId => true;

        /// <summary>
        /// True if button is part of an addons or dropdown group.
        /// </summary>
        protected bool IsAddons => ParentButtons?.Role == ButtonsRole.Addons || ParentDropdown?.IsGroup == true;

        /// <summary>
        /// True if button or it's parent dropdown is disabled.
        /// </summary>
        protected bool IsDisabled => ParentDropdown?.Disabled ?? Disabled;

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
        public Size Size
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
        /// Shows the loading spinner or a <see cref="LoadingTemplate"/>.
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
        /// Gets or sets the component loading template.
        /// </summary>
        [Parameter] public RenderFragment LoadingTemplate { get; set; }

        /// <summary>
        /// Prevents a default form-post when button type is set to <see cref="ButtonType.Submit"/>.
        /// </summary>
        [Parameter] public bool PreventDefaultOnSubmit { get; set; }

        /// <summary>
        /// Gets or sets the reference to the parent dropdown.
        /// </summary>
        [CascadingParameter] protected Dropdown ParentDropdown { get; set; }

        /// <summary>
        /// Gets or sets the reference to the parent buttons.
        /// </summary>
        [CascadingParameter] protected Buttons ParentButtons { get; set; }

        /// <summary>
        /// Gets or sets the reference to the parent addons.
        /// </summary>
        [CascadingParameter] protected Addons ParentAddons { get; set; }

        /// <summary>
        /// Gets or sets the reference to the parent field.
        /// </summary>
        [CascadingParameter] protected Field ParentField { get; set; }

        /// <summary>
        /// Gets or sets the parent dropdown state object.
        /// </summary>
        [CascadingParameter]
        protected DropdownState ParentDropdownState
        {
            get => parentDropdownState;
            set
            {
                if ( parentDropdownState == value )
                    return;

                parentDropdownState = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Gets or sets the command to be executed when clicked on a button.
        /// </summary>
        [Parameter] public ICommand Command { get; set; }

        /// <summary>
        /// Reflects the parameter to pass to the CommandProperty upon execution.
        /// </summary>
        [Parameter] public object CommandParameter { get; set; }

        /// <summary>
        /// Denotes the target route of the <see cref="ButtonType.Link"/> button.
        /// </summary>
        [Parameter] public string To { get; set; }

        /// <summary>
        /// The target attribute specifies where to open the linked document for a <see cref="ButtonType.Link"/>.
        /// </summary>
        [Parameter] public Target Target { get; set; } = Target.None;

        /// <summary>
        /// If defined, indicates that its element can be focused and can participates in sequential keyboard navigation.
        /// </summary>
        [Parameter] public int? TabIndex { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="Button"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
