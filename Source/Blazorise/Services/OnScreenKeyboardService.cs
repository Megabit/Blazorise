#region Using directives
using System;
using System.Threading.Tasks;
#endregion

namespace Blazorise;

/// <summary>
/// Default implementation of <see cref="IOnScreenKeyboardService"/>.
/// </summary>
public class OnScreenKeyboardService : IOnScreenKeyboardService
{
    #region Members

    private OnScreenKeyboardState state = new();

    private bool ignoreNextBlur;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public Task Show( OnScreenKeyboardContext context )
    {
        if ( context is null || context.Disabled || context.ReadOnly )
            return Hide();

        state = new()
        {
            Visible = true,
            Context = context,
        };

        NotifyStateChanged();

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task Hide()
    {
        if ( !state.Visible && state.Context is null )
            return Task.CompletedTask;

        state = new();

        NotifyStateChanged();

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task Hide( string elementId )
    {
        if ( string.IsNullOrEmpty( elementId ) || !string.Equals( state.Context?.ElementId, elementId, StringComparison.Ordinal ) )
            return Task.CompletedTask;

        return Hide();
    }

    /// <inheritdoc/>
    public void SuppressHideOnBlur()
    {
        ignoreNextBlur = true;
    }

    /// <inheritdoc/>
    public void ClearHideOnBlurSuppression()
    {
        ignoreNextBlur = false;
    }

    /// <inheritdoc/>
    public Task PressKey( OnScreenKeyboardKey key )
    {
        if ( key is null )
            return Task.CompletedTask;

        return key.KeyType switch
        {
            OnScreenKeyboardKeyType.Text => InsertText( key.Text ),
            OnScreenKeyboardKeyType.Space => InsertText( " " ),
            OnScreenKeyboardKeyType.Backspace => Backspace(),
            OnScreenKeyboardKeyType.Clear => Clear(),
            OnScreenKeyboardKeyType.Enter => Enter(),
            _ => Task.CompletedTask,
        };
    }

    /// <inheritdoc/>
    public async Task InsertText( string text )
    {
        if ( string.IsNullOrEmpty( text ) || state.Context is null )
            return;

        if ( state.Context.InsertText is not null )
        {
            await state.Context.InsertText( text );
            return;
        }

        if ( state.Context.SetValue is null )
            return;

        var value = state.Context.GetValue?.Invoke() ?? string.Empty;

        await state.Context.SetValue( value + text );
    }

    /// <inheritdoc/>
    public async Task Backspace()
    {
        if ( state.Context is null )
            return;

        if ( state.Context.Backspace is not null )
        {
            await state.Context.Backspace();
            return;
        }

        if ( state.Context.SetValue is null )
            return;

        var value = state.Context.GetValue?.Invoke() ?? string.Empty;

        if ( value.Length == 0 )
            return;

        await state.Context.SetValue( value.Substring( 0, value.Length - 1 ) );
    }

    /// <inheritdoc/>
    public Task Clear()
    {
        return state.Context?.SetValue is null
            ? Task.CompletedTask
            : state.Context.SetValue( string.Empty );
    }

    /// <inheritdoc/>
    public Task Enter()
    {
        return state.Context?.Enter is null
            ? Task.CompletedTask
            : state.Context.Enter();
    }

    private void NotifyStateChanged()
    {
        StateChanged?.Invoke( this, new( state ) );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public OnScreenKeyboardState State => state;

    /// <inheritdoc/>
    public bool ShouldIgnoreBlur => ignoreNextBlur;

    /// <inheritdoc/>
    public event EventHandler<OnScreenKeyboardStateChangedEventArgs> StateChanged;

    #endregion
}