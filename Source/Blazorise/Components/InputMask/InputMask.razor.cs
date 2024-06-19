#region Using directives
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise;

/// <summary>
/// Format input text content when you are typing.
/// </summary>
public partial class InputMask : BaseTextInput<string>, IAsyncDisposable
{
    #region Members

    /// <summary>
    /// Object reference that can be accessed through the JSInterop.
    /// </summary>
    private DotNetObjectReference<InputMask> dotNetObjectRef;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        if ( Rendered )
        {
            if ( parameters.TryGetValue<string>( nameof( Value ), out var paramValue ) && !paramValue.IsEqual( Value ) )
            {
                ExecuteAfterRender( Revalidate );
            }
        }

        await base.SetParametersAsync( parameters );

        if ( ParentValidation is not null )
        {
            if ( parameters.TryGetValue<Expression<Func<string>>>( nameof( ValueExpression ), out var expression ) )
                await ParentValidation.InitializeInputExpression( expression );

            if ( parameters.TryGetValue<string>( nameof( Pattern ), out var pattern ) )
            {
                // make sure we get the newest value
                var value = parameters.TryGetValue<string>( nameof( Value ), out var paramValue )
                    ? paramValue
                    : InternalValue;

                await ParentValidation.InitializeInputPattern( pattern, value );
            }

            await InitializeValidation();
        }
    }

    /// <inheritdoc/>
    protected override async Task OnFirstAfterRenderAsync()
    {
        dotNetObjectRef ??= CreateDotNetObjectRef( this );

        await JSModule.Initialize( dotNetObjectRef, ElementRef, ElementId, new
        {
            Mask,
            Regex,
            Alias,
            InputFormat,
            OutputFormat,
            MaskPlaceholder,
            ShowMaskOnFocus,
            ShowMaskOnHover,
            NumericInput,
            RightAlign,
            DecimalSeparator,
            GroupSeparator,
            Nullable,
            AutoUnmask,
            PositionCaretOnClick = PositionCaretOnClick.ToInputMaskCaretPosition(),
            ClearMaskOnLostFocus,
            ClearIncomplete,
            Disabled,
            ReadOnly,
        } );

        await base.OnFirstAfterRenderAsync();
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSModule.SafeDestroy( ElementRef, ElementId );

            DisposeDotNetObjectRef( dotNetObjectRef );
            dotNetObjectRef = null;
        }

        await base.DisposeAsync( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.InputMask( Plaintext ) );
        builder.Append( ClassProvider.InputMaskSize( ThemeSize ) );
        builder.Append( ClassProvider.InputMaskColor( Color ) );
        builder.Append( ClassProvider.InputMaskValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override Task OnChangeHandler( ChangeEventArgs e )
    {
        return CurrentValueHandler( e?.Value?.ToString() );
    }

    /// <inheritdoc/>
    protected override Task OnInternalValueChanged( string value )
    {
        return ValueChanged.InvokeAsync( value );
    }

    /// <inheritdoc/>
    protected override Task<ParseValue<string>> ParseValueFromStringAsync( string value )
    {
        return Task.FromResult( new ParseValue<string>( true, value, null ) );
    }

    /// <inheritdoc/>
    protected override string FormatValueAsString( string value )
        => value?.ToString() ?? string.Empty;

    /// <summary>
    /// Extends the alias options with the custom settings.
    /// </summary>
    /// <param name="aliasOptions">Options for the alias initialization.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async Task ExtendAliases( object aliasOptions )
    {
        if ( Rendered )
        {
            await JSModule.ExtendAliases( ElementRef, ElementId, aliasOptions );
        }
    }

    /// <summary>
    /// Notifies the component that the input mask is completed.
    /// </summary>
    /// <param name="value">Completed value.</param>
    /// <returns>Returns awaitable task</returns>
    [JSInvokable]
    public Task NotifyCompleted( string value )
    {
        return Completed.InvokeAsync( value );
    }

    /// <summary>
    /// Notifies the component that the input mask is incomplete.
    /// </summary>
    /// <param name="value">Incompleted value.</param>
    /// <returns>Returns awaitable task</returns>
    [JSInvokable]
    public Task NotifyIncompleted( string value )
    {
        return Incompleted.InvokeAsync( value );
    }

    /// <summary>
    /// Notifies the component that the input mask is cleared.
    /// </summary>
    /// <returns>Returns awaitable task</returns>
    [JSInvokable]
    public Task NotifyCleared()
    {
        return Cleared.InvokeAsync();
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <inheritdoc/>
    protected override string InternalValue { get => Value; set => Value = value; }

    /// <summary>
    /// Gets or sets the <see cref="IJSInputMaskModule"/> instance.
    /// </summary>
    [Inject] public IJSInputMaskModule JSModule { get; set; }

    /// <summary>
    /// Gets or sets the input time value.
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <summary>
    /// Occurs when the time has changed.
    /// </summary>
    [Parameter] public EventCallback<string> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets an expression that identifies the time field.
    /// </summary>
    [Parameter] public Expression<Func<string>> ValueExpression { get; set; }

    /// <summary>
    /// The mask to use for the input.
    /// </summary>
    [Parameter] public string Mask { get; set; }

    /// <summary>
    /// The placeholder that will be used for the mask.
    /// </summary>
    [Parameter] public string MaskPlaceholder { get; set; }

    /// <summary>
    /// Use a regular expression as a mask.
    /// </summary>
    [Parameter] public string Regex { get; set; }

    /// <summary>
    /// With an alias, you can define a complex mask definition and call it by using an alias name.
    /// So this is mainly to simplify the use of your masks. Some aliases found in the extensions are email,
    /// currency, decimal, integer, date, DateTime, dd/mm/yyyy, etc.
    /// </summary>
    [Parameter] public string Alias { get; set; }

    /// <summary>
    /// Defines the input format when the <see cref="Alias"/> is used.
    /// </summary>
    [Parameter] public string InputFormat { get; set; }

    /// <summary>
    /// Defines the output format of the <see cref="Value"/> when the <see cref="Alias"/> is used.
    /// </summary>
    [Parameter] public string OutputFormat { get; set; }

    /// <summary>
    /// Shows the mask when the input gets focus. (default = true)
    /// </summary>
    [Parameter] public bool ShowMaskOnFocus { get; set; } = true;

    /// <summary>
    /// Shows the mask when hovering the mouse. (default = true)
    /// </summary>
    [Parameter] public bool ShowMaskOnHover { get; set; } = true;

    /// <summary>
    /// Numeric input direction. Keeps the caret at the end.
    /// </summary>
    [Parameter] public bool NumericInput { get; set; }

    /// <summary>
    /// Align the input to the right.
    /// </summary>
    /// <remarks>
    /// By setting the rightAlign you can specify to right-align an inputmask. This is only applied
    /// in combination op the numericInput option or the dir-attribute. The default is true.
    /// </remarks>
    [Parameter] public bool RightAlign { get; set; }

    /// <summary>
    /// Define the decimal separator (numeric mode only).
    /// </summary>
    [Parameter] public string DecimalSeparator { get; set; }

    /// <summary>
    /// Define the group separator (numeric mode only).
    /// </summary>
    [Parameter] public string GroupSeparator { get; set; }

    /// <summary>
    /// Return nothing when the user hasn't entered anything. Default: false.
    /// </summary>
    [Parameter] public bool Nullable { get; set; }

    /// <summary>
    /// Automatically unmask the value when retrieved. Default: false.
    /// </summary>
    [Parameter] public bool AutoUnmask { get; set; }

    /// <summary>
    /// Defines the positioning of the caret on click.
    /// </summary>
    [Parameter] public InputMaskCaretPosition PositionCaretOnClick { get; set; } = InputMaskCaretPosition.LastValidPosition;

    /// <summary>
    /// Remove the empty mask on blur or when not empty remove the optional trailing part. Default: true
    /// </summary>
    [Parameter] public bool ClearMaskOnLostFocus { get; set; } = true;

    /// <summary>
    /// Clear the incomplete input on blur. Default: false
    /// </summary>
    [Parameter] public bool ClearIncomplete { get; set; }

    /// <summary>
    /// Execute a function when the mask is completed.
    /// </summary>
    [Parameter] public EventCallback<string> Completed { get; set; }

    /// <summary>
    /// Execute a function when the mask is incomplete. Executes on blur.
    /// </summary>
    [Parameter] public EventCallback<string> Incompleted { get; set; }

    /// <summary>
    /// Execute a function when the mask is cleared.
    /// </summary>
    [Parameter] public EventCallback Cleared { get; set; }

    #endregion
}