#region Using directives
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using System.Timers;
#endregion

namespace Blazorise;

/// <summary>
/// Component that allows you to display and edit multi-line text.
/// </summary>
public partial class MemoEdit : BaseInputComponent<string>, ISelectableComponent, IAsyncDisposable
{
    private Timer debounceTimer;

    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        await base.SetParametersAsync( parameters );

        if ( ParentValidation is not null )
        {
            if ( parameters.TryGetValue<Expression<Func<string>>>( nameof( TextExpression ), out var expression ) )
                await ParentValidation.InitializeInputExpression( expression );

            if ( parameters.TryGetValue<string>( nameof( Pattern ), out var pattern ) )
            {
                // make sure we get the newest value
                var value = parameters.TryGetValue<string>( nameof( Text ), out var paramText )
                    ? paramText
                    : InternalValue;

                await ParentValidation.InitializeInputPattern( pattern, value );
            }

            await InitializeValidation();
        }
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override async Task OnFirstAfterRenderAsync()
    {
        await base.OnFirstAfterRenderAsync();
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && debounceTimer != null )
        {
            debounceTimer.Dispose();
            debounceTimer = null;
        }

        await base.DisposeAsync( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.MemoEdit( Plaintext ) );
        builder.Append( ClassProvider.MemoEditSize( ThemeSize ) );
        builder.Append( ClassProvider.MemoEditValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override Task OnInternalValueChanged( string value )
    {
        return TextChanged.InvokeAsync( value );
    }

    /// <inheritdoc/>
    protected override Task<ParseValue<string>> ParseValueFromStringAsync( string value )
    {
        return Task.FromResult( new ParseValue<string>( true, value, null ) );
    }

    /// <inheritdoc/>
    protected Task OnChangeHandler( ChangeEventArgs e )
    {
        if ( !IsImmediate )
        {
            return CurrentValueHandler( e?.Value?.ToString() );
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected Task OnInputHandler( ChangeEventArgs e )
    {
        if ( IsImmediate )
        {
            // Debounce logic
            if ( debounceTimer != null )
            {
                debounceTimer.Stop();
                debounceTimer.Dispose();
            }

            debounceTimer = new Timer( 300 ); // Adjust debounce delay as needed
            debounceTimer.Elapsed += async ( _, __ ) =>
            {
                debounceTimer.Dispose();
                debounceTimer = null;
                await CurrentValueHandler( e?.Value?.ToString() );
            };
            debounceTimer.Start();
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual async Task Select( bool focus = true )
    {
        await JSUtilitiesModule.Select( ElementRef, ElementId, focus );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override string InternalValue { get => Text; set => Text = value; }

    /// <inheritdoc/>
    protected override string DefaultValue => string.Empty;

    /// <summary>
    /// Returns true if internal value should be updated with each key press.
    /// </summary>
    protected bool IsImmediate => Immediate.GetValueOrDefault( Options?.Immediate ?? true );

    /// <summary>
    /// The name of the event for the textarea element.
    /// </summary>
    protected string BindValueEventName => IsImmediate ? "oninput" : "onchange";

    /// <summary>
    /// Sets the placeholder for the empty text.
    /// </summary>
    [Parameter] public string Placeholder { get; set; }

    /// <summary>
    /// Sets the class to remove the default form field styling and preserve the correct margin and padding.
    /// </summary>
    [Parameter] public bool Plaintext { get; set; }

    /// <summary>
    /// Gets or sets the text inside the input field.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Occurs after text has changed.
    /// </summary>
    [Parameter] public EventCallback<string> TextChanged { get; set; }

    /// <summary>
    /// Gets or sets an expression that identifies the text value.
    /// </summary>
    [Parameter] public Expression<Func<string>> TextExpression { get; set; }

    /// <summary>
    /// Specifies the maximum number of characters allowed in the input element.
    /// </summary>
    [Parameter] public int? MaxLength { get; set; }

    /// <summary>
    /// Specifies the number lines in the input element.
    /// </summary>
    [Parameter] public int? Rows { get; set; }

    /// <summary>
    /// The pattern attribute specifies a regular expression that the input element's value is checked against on form validation.
    /// </summary>
    /// <remarks>
    /// Please be aware that <see cref="Pattern"/> on <see cref="MemoEdit"/> is used only for the validation process.
    /// </remarks>
    [Parameter] public string Pattern { get; set; }

    /// <summary>
    /// If true the text in will be changed after each key press.
    /// </summary>
    /// <remarks>
    /// Note that setting this will override global settings in <see cref="BlazoriseOptions.Immediate"/>.
    /// </remarks>
    [Parameter] public bool? Immediate { get; set; }

    #endregion
}
