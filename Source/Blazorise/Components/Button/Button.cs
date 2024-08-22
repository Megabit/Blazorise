#region Using directives
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Blazorise.Extensions;
using Blazorise.Modules;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Clickable button for actions in forms, dialogs, and more with support for multiple sizes, states, and more.
/// </summary>
public partial class Button : BaseComponent, IAsyncDisposable
{
    #region Members

    private Color color = Color.Default;

    private Size? size;

    private bool outline;

    private bool disabled;

    private bool active;

    private bool block;

    private bool loading;

    private DropdownState parentDropdownState;

    private ICommand command;

    private object commandParameter;

    private bool? canExecuteCommand;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Button( Outline ) );
        builder.Append( ClassProvider.ButtonColor( Color, Outline ) );
        builder.Append( ClassProvider.ButtonSize( ThemeSize, Outline ) );
        builder.Append( ClassProvider.ButtonBlock( Outline ), Block );
        builder.Append( ClassProvider.ButtonActive( Outline ), Active );
        builder.Append( ClassProvider.ButtonDisabled( Outline ), Disabled );
        builder.Append( ClassProvider.ButtonLoading( Outline ), Loading && LoadingTemplate is null );
        builder.Append( ClassProvider.ButtonStretchedLink( StretchedLink ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        // notify dropdown that the button is inside of it
        ParentDropdown?.NotifyButtonInitialized( this );

        // notify addons that the button is inside of it
        ParentAddons?.NotifyButtonInitialized( this );

        if ( PreventDefaultOnSubmit )
        {
            ExecuteAfterRender( async () =>
            {
                await JSModule.Initialize( ElementRef, ElementId, new
                {
                    PreventDefaultOnSubmit
                } );
            } );
        }

        LoadingTemplate ??= ProvideDefaultLoadingTemplate();

        if ( Theme is not null )
        {
            Theme.Changed += OnThemeChanged;
        }

        base.OnInitialized();
    }

    /// <summary>
    /// Provides a default LoadingTemplate RenderFragment.
    /// </summary>
    /// <returns>Returns the RenderFragment consisting of a loading content.</returns>
    protected virtual RenderFragment ProvideDefaultLoadingTemplate() => null;

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
        {
            // remove button from parents
            ParentDropdown?.NotifyButtonRemoved( this );
            ParentAddons?.NotifyButtonRemoved( this );

            if ( Rendered )
            {
                await JSModule.SafeDestroy( ElementRef, ElementId );
            }

            if ( command is not null )
            {
                command.CanExecuteChanged -= OnCanExecuteChanged;
            }

            if ( Theme is not null )
            {
                Theme.Changed -= OnThemeChanged;
            }
        }

        await base.DisposeAsync( disposing );
    }

    /// <summary>
    /// Handles the item onclick event.
    /// </summary>
    /// <param name="eventArgs">Supplies information about a mouse event that is being raised.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected async Task ClickHandler( MouseEventArgs eventArgs )
    {
        if ( !Disabled )
        {
            await Clicked.InvokeAsync( eventArgs );

            // Don't need to check CanExecute again is already part of Disabled check
            Command?.Execute( CommandParameter );
        }
    }

    /// <summary>
    /// Sets focus on the button element, if it can be focused.
    /// </summary>
    /// <param name="scrollToElement">If true the browser should scroll the document to bring the newly-focused element into view.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Focus( bool scrollToElement = true )
    {
        return JSUtilitiesModule.Focus( ElementRef, ElementId, scrollToElement ).AsTask();
    }

    /// <inheritdoc/>
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

        if ( Type == ButtonType.Link )
        {
            builder
                .Role( "button" )
                .Href( To )
                .Target( Target );

            if ( Disabled )
            {
                builder
                    .TabIndex( -1 )
                    .AriaDisabled( "true" );
            }
        }

        builder.OnClick( this, EventCallback.Factory.Create<MouseEventArgs>( this, ClickHandler ) );
        builder.OnClickPreventDefault( Type == ButtonType.Link && To is not null && To.StartsWith( "#" ) );

        builder.Attributes( Attributes );
        builder.ElementReferenceCapture( capturedRef => ElementRef = capturedRef );

        if ( Loading && LoadingTemplate is not null )
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

    private void BindCommand( ICommand value )
    {
        if ( command is not null )
        {
            command.CanExecuteChanged -= OnCanExecuteChanged;
        }

        command = value;

        if ( command is not null )
        {
            command.CanExecuteChanged += OnCanExecuteChanged;
        }

        OnCanExecuteChanged( value, EventArgs.Empty );
    }

    /// <summary>
    /// Occurs when changes occur that affect whether or not the command should execute.
    /// </summary>
    /// <param name="sender">Reference of the object that raised the event.</param>
    /// <param name="eventArgs">Event arguments.</param>
    protected virtual void OnCanExecuteChanged( object sender, EventArgs eventArgs )
    {
        var canExecute = Command?.CanExecute( CommandParameter );

        if ( canExecute != canExecuteCommand )
        {
            canExecuteCommand = canExecute;

            if ( Rendered )
            {
                // in case some provider is using Disabled flag for custom styles
                DirtyStyles();
                DirtyClasses();

                InvokeAsync( StateHasChanged );
            }
        }
    }

    /// <summary>
    /// An event raised when theme settings changes.
    /// </summary>
    /// <param name="sender">An object that raised the event.</param>
    /// <param name="eventArgs"></param>
    private void OnThemeChanged( object sender, EventArgs eventArgs )
    {
        DirtyClasses();
        DirtyStyles();

        InvokeAsync( StateHasChanged );
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
    protected bool ParentIsField => ParentField is not null;

    /// <summary>
    /// Gets the size based on the theme settings.
    /// </summary>
    protected Size ThemeSize => Size.GetValueOrDefault( ParentAddons?.Size ?? Theme?.InputOptions?.Size ?? Blazorise.Size.Default );

    /// <summary>
    /// Gets or sets the <see cref="IJSButtonModule"/> instance.
    /// </summary>
    [Inject] public IJSButtonModule JSModule { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="IJSUtilitiesModule"/> instance.
    /// </summary>
    [Inject] public IJSUtilitiesModule JSUtilitiesModule { get; set; }

    /// <summary>
    /// Occurs when the button is clicked.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> Clicked { get; set; }

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
    public Size? Size
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
    /// When set to 'true', disables the component's functionality and places it in a disabled state.
    /// </summary>
    [Parameter]
    public bool Disabled
    {
        get => disabled || !canExecuteCommand.GetValueOrDefault( true );
        set
        {
            disabled = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// When set to 'true', places the component in the active state with active styling.
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
    [Parameter]
    public ICommand Command
    {
        get => command;
        set => BindCommand( value );
    }

    /// <summary>
    /// Reflects the parameter to pass to the CommandProperty upon execution.
    /// </summary>
    [Parameter]
    public object CommandParameter
    {
        get => commandParameter;
        set
        {
            if ( commandParameter.IsEqual( value ) )
                return;

            commandParameter = value;

            OnCanExecuteChanged( this, EventArgs.Empty );
        }
    }

    /// <summary>
    /// Denotes the target route of the <see cref="ButtonType.Link"/> button.
    /// </summary>
    [Parameter] public string To { get; set; }

    /// <summary>
    /// The target attribute specifies where to open the linked document for a <see cref="ButtonType.Link"/>.
    /// </summary>
    [Parameter] public Target Target { get; set; } = Target.Default;

    /// <summary>
    /// Makes any HTML element or component clickable by “stretching” a nested link.
    /// </summary>
    [Parameter] public bool StretchedLink { get; set; }

    /// <summary>
    /// If defined, indicates that its element can be focused and can participates in sequential keyboard navigation.
    /// </summary>
    [Parameter] public int? TabIndex { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Button"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Cascaded theme settings.
    /// </summary>
    [CascadingParameter] public Theme Theme { get; set; }

    #endregion
}