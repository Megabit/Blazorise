#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Base component for text inputs that support immediate/debounced updates.
/// </summary>
/// <typeparam name="TValue">Editable value type.</typeparam>
public abstract class BaseBufferedTextInput<TValue> : BaseTextInput<TValue>
{
    #region Members

    private ValueDebouncer inputValueDebouncer;

    #endregion

    #region Methods

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
    protected override void ReleaseResources()
    {
        if ( inputValueDebouncer is not null )
        {
            inputValueDebouncer.Debounce -= OnInputValueDebounce;
            inputValueDebouncer = null;
        }

        base.ReleaseResources();
    }

    /// <summary>
    /// Handler for @onchange event.
    /// </summary>
    /// <param name="eventArgs">Information about the changed event.</param>
    /// <returns>Returns awaitable task</returns>
    protected override Task OnChangeHandler( ChangeEventArgs eventArgs )
    {
        if ( !IsImmediate )
        {
            return CurrentValueHandler( eventArgs?.Value?.ToString() );
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Handler for @oninput event.
    /// </summary>
    /// <param name="eventArgs">Information about the changed event.</param>
    /// <returns>Returns awaitable task</returns>
    protected virtual Task OnInputHandler( ChangeEventArgs eventArgs )
    {
        if ( IsImmediate )
        {
            var value = eventArgs?.Value?.ToString();
            if ( IsDebounce )
            {
                inputValueDebouncer?.Update( value );
            }
            else
            {
                return CurrentValueHandler( value );
            }
        }

        return Task.CompletedTask;
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
        InvokeAsync( StateHasChanged );
    }

    #endregion

    #region Properties

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
    /// the name of the event for the input element.
    /// </summary>
    protected string BindValueEventName
        => IsImmediate ? "oninput" : "onchange";

    /// <summary>
    /// Gets the debouncer reference.
    /// </summary>
    protected ValueDebouncer InputValueDebouncer => inputValueDebouncer;

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

    #endregion
}