#region Using directives
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Component that allows you to display and edit multi-line text.
/// </summary>
public partial class MemoEdit : BaseInputComponent<string>, ISelectableComponent, IAsyncDisposable
{
    #region Members

    private ValueDebouncer inputValueDebouncer;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        var replaceTabChanged = parameters.TryGetValue( nameof( ReplaceTab ), out bool paramReplaceTab ) && ReplaceTab != paramReplaceTab;
        var tabSizeChanged = parameters.TryGetValue( nameof( TabSize ), out int paramTabSize ) && TabSize != paramTabSize;
        var softTabsChanged = parameters.TryGetValue( nameof( SoftTabs ), out bool paramSoftTabs ) && SoftTabs != paramSoftTabs;
        var autoSizeChanged = parameters.TryGetValue( nameof( AutoSize ), out bool paramAutoSize ) && AutoSize != paramAutoSize;

        if ( Rendered && ( replaceTabChanged
                           || tabSizeChanged
                           || softTabsChanged
                           || autoSizeChanged ) )
        {
            ExecuteAfterRender( async () => await JSModule.UpdateOptions( ElementRef, ElementId, new
            {
                ReplaceTab = new { Changed = replaceTabChanged, Value = paramReplaceTab },
                TabSize = new { Changed = tabSizeChanged, Value = paramTabSize },
                SoftTabs = new { Changed = softTabsChanged, Value = paramSoftTabs },
                AutoSize = new { Changed = autoSizeChanged, Value = paramAutoSize },
            } ) );
        }

        if ( Rendered )
        {
            if ( parameters.TryGetValue<string>( nameof( Value ), out var paramValue ) && !paramValue.IsEqual( Value ) )
            {
                ExecuteAfterRender( async () =>
                {
                    await Revalidate();

                    if ( AutoSize )
                    {
                        await JSModule.RecalculateAutoHeight( ElementRef, ElementId );
                    }
                } );
            }
        }

        await base.SetParametersAsync( parameters );

        if ( ParentValidation is not null )
        {
            if ( parameters.TryGetValue<Expression<Func<string>>>( nameof( ValueExpression ), out var expression ) )
                await ParentValidation.InitializeInputExpression( expression );

            if ( parameters.TryGetValue<string>( nameof( Pattern ), out var paramPattern ) )
            {
                // make sure we get the newest value
                var newValue = parameters.TryGetValue<string>( nameof( Value ), out var paramValue )
                    ? paramValue
                    : Value;

                await ParentValidation.InitializeInputPattern( paramPattern, newValue );
            }

            await InitializeValidation();
        }
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if ( IsDebounce )
        {
            inputValueDebouncer = new( DebounceIntervalValue );
            inputValueDebouncer.Debounce += OnInputValueDebounce;
        }

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected async override Task OnFirstAfterRenderAsync()
    {
        await JSModule.Initialize( ElementRef, ElementId, new
        {
            ReplaceTab,
            TabSize,
            SoftTabs,
            AutoSize,
        } );

        await base.OnFirstAfterRenderAsync();
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSModule.SafeDestroy( ElementRef, ElementId );

            if ( inputValueDebouncer is not null )
            {
                inputValueDebouncer.Debounce -= OnInputValueDebounce;
            }
        }

        await base.DisposeAsync( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.MemoEdit( Plaintext ) );
        builder.Append( ClassProvider.MemoEditSize( ThemeSize ) );
        builder.Append( ClassProvider.MemoEditValidation( ParentValidation?.Status ?? ValidationStatus.None ) );

        base.BuildClasses( builder );
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
    protected Task OnChangeHandler( ChangeEventArgs e )
    {
        if ( !IsImmediate )
        {
            return CurrentValueHandler( e?.Value?.ToString() );
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected async Task OnInputHandler( ChangeEventArgs e )
    {
        if ( IsImmediate )
        {
            if ( IsDebounce )
            {
                inputValueDebouncer?.Update( e?.Value?.ToString() );
            }
            else
            {
                var caret = await JSUtilitiesModule.GetCaret( ElementRef );

                await CurrentValueHandler( e?.Value?.ToString() );

                await JSUtilitiesModule.SetCaret( ElementRef, caret );
            }
        }
    }

    /// <inheritdoc/>
    protected override Task OnKeyPressHandler( KeyboardEventArgs eventArgs )
    {
        if ( IsImmediate
             && IsDebounce
             && ( eventArgs?.Key?.Equals( "Enter", StringComparison.OrdinalIgnoreCase ) ?? false ) )
        {
            inputValueDebouncer?.Flush();
        }

        return base.OnKeyPressHandler( eventArgs );
    }

    /// <inheritdoc/>
    protected override Task OnBlurHandler( FocusEventArgs eventArgs )
    {
        if ( IsImmediate
             && IsDebounce )
        {
            inputValueDebouncer?.Flush();
        }

        return base.OnBlurHandler( eventArgs );
    }

    /// <summary>
    /// Event raised after the delayed value time has expired.
    /// </summary>
    /// <param name="sender">Object that raised an event.</param>
    /// <param name="value">Latest received value.</param>
    private void OnInputValueDebounce( object sender, string value )
    {
        InvokeAsync( () => CurrentValueHandler( value ) );
    }

    /// <inheritdoc/>
    public virtual async Task Select( bool focus = true )
    {
        await JSUtilitiesModule.Select( ElementRef, ElementId, focus );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override string DefaultValue => string.Empty;

    /// <summary>
    /// Returns true if internal value should be updated with each key press.
    /// </summary>
    protected bool IsImmediate
        => Immediate.GetValueOrDefault( Options?.Immediate ?? true );

    /// <summary>
    /// Returns true if updating of internal value should be delayed.
    /// </summary>
    protected bool IsDebounce
        => Debounce.GetValueOrDefault( Options?.Debounce ?? false );

    /// <summary>
    /// Time in milliseconds by which internal value update should be delayed.
    /// </summary>
    protected int DebounceIntervalValue
        => DebounceInterval.GetValueOrDefault( Options?.DebounceInterval ?? 300 );

    /// <summary>
    /// The name of the event for the textarea element.
    /// </summary>
    protected string BindValueEventName
        => IsImmediate ? "oninput" : "onchange";

    /// <summary>
    /// Gets or sets the <see cref="IJSMemoEditModule"/> instance.
    /// </summary>
    [Inject] public IJSMemoEditModule JSModule { get; set; }

    /// <summary>
    /// Sets the placeholder for the empty text.
    /// </summary>
    [Parameter] public string Placeholder { get; set; }

    /// <summary>
    /// Sets the class to remove the default form field styling and preserve the correct margin and padding.
    /// </summary>
    [Parameter] public bool Plaintext { get; set; }

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

    /// <summary>
    /// If true the entered text will be slightly delayed before submitting it to the internal value.
    /// </summary>
    [Parameter] public bool? Debounce { get; set; }

    /// <summary>
    /// Interval in milliseconds that entered text will be delayed from submitting to the internal value.
    /// </summary>
    [Parameter] public int? DebounceInterval { get; set; }

    /// <summary>
    /// If set to true, <see cref="ReplaceTab"/> will insert a tab instead of cycle input focus.
    /// </summary>
    [Parameter] public bool ReplaceTab { get; set; } = false;

    /// <summary>
    /// Defines the number of characters that tab key will override.
    /// </summary>
    [Parameter] public int TabSize { get; set; } = 4;

    /// <summary>
    /// If set to true, spaces will be used instead of a tab character
    /// </summary>
    [Parameter] public bool SoftTabs { get; set; } = true;

    /// <summary>
    /// If true, the textarea will automatically grow in height according to its content.
    /// </summary>
    [Parameter] public bool AutoSize { get; set; }

    #endregion
}