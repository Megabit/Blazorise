#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Renders the on-screen keyboard controlled by <see cref="IOnScreenKeyboardService"/>.
/// </summary>
public partial class OnScreenKeyboardProvider : BaseComponent, IDisposable
{
    #region Members

    private bool shift;

    #endregion

    #region Constructors

    /// <summary>
    /// A default <see cref="OnScreenKeyboardProvider"/> constructor.
    /// </summary>
    public OnScreenKeyboardProvider()
    {
        RowClassBuilder = new( BuildRowClasses );
        KeyClassBuilder = new( BuildKeyClasses );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        OnScreenKeyboardService.StateChanged += OnKeyboardStateChanged;

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing && OnScreenKeyboardService is not null )
        {
            OnScreenKeyboardService.StateChanged -= OnKeyboardStateChanged;
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.OnScreenKeyboard() );
        builder.Append( ClassProvider.OnScreenKeyboardPlacement( EffectivePlacement ) );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Builds the class names for a keyboard row.
    /// </summary>
    /// <param name="builder">Class builder used to append class names.</param>
    private void BuildRowClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.OnScreenKeyboardRow() );
    }

    /// <summary>
    /// Builds the class names for a keyboard key.
    /// </summary>
    /// <param name="builder">Class builder used to append class names.</param>
    private void BuildKeyClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.OnScreenKeyboardKey() );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        if ( EffectivePlacement != OnScreenKeyboardPlacement.Inline )
        {
            builder.Append( "position:fixed" );
            builder.Append( "left:0" );
            builder.Append( "right:0" );
            builder.Append( "z-index:1050" );
            builder.Append( "background:var(--b-theme-body-bg, var(--bs-body-bg, #fff))" );
            builder.Append( "box-shadow:0 -0.25rem 1rem rgba(0, 0, 0, .12)", EffectivePlacement == OnScreenKeyboardPlacement.Bottom );
            builder.Append( "box-shadow:0 .25rem 1rem rgba(0, 0, 0, .12)", EffectivePlacement == OnScreenKeyboardPlacement.Top );
            builder.Append( "bottom:0", EffectivePlacement == OnScreenKeyboardPlacement.Bottom );
            builder.Append( "top:0", EffectivePlacement == OnScreenKeyboardPlacement.Top );
        }

        builder.Append( "padding:.5rem" );

        base.BuildStyles( builder );
    }

    private async void OnKeyboardStateChanged( object sender, OnScreenKeyboardStateChangedEventArgs eventArgs )
    {
        shift = false;

        DirtyClasses();
        DirtyStyles();

        await InvokeAsync( StateHasChanged );
    }

    private async Task OnKeyClicked( OnScreenKeyboardKey key )
    {
        if ( key.KeyType == OnScreenKeyboardKeyType.Shift )
        {
            shift = !shift;
            await InvokeAsync( StateHasChanged );
            return;
        }

        if ( key.KeyType == OnScreenKeyboardKeyType.Text && shift )
        {
            await OnScreenKeyboardService.PressKey( new( key.Text?.ToUpperInvariant() ) );
            return;
        }

        await OnScreenKeyboardService.PressKey( key );
    }

    private string GetKeyDisplayText( OnScreenKeyboardKey key )
    {
        if ( !string.IsNullOrEmpty( key.DisplayText ) )
            return key.DisplayText;

        if ( key.KeyType == OnScreenKeyboardKeyType.Text && shift )
            return key.Text?.ToUpperInvariant();

        return key.Text;
    }

    private string GetKeyStyle( OnScreenKeyboardKey key )
    {
        return $"flex:{Math.Max( 1, key.Width )} 1 0; min-height:2.5rem";
    }

    private IReadOnlyList<IReadOnlyList<OnScreenKeyboardKey>> CreateRows()
    {
        return CurrentLayout switch
        {
            OnScreenKeyboardLayout.Numeric => NumericRows,
            OnScreenKeyboardLayout.Decimal => DecimalRows,
            OnScreenKeyboardLayout.Telephone => TelephoneRows,
            OnScreenKeyboardLayout.Email => EmailRows,
            OnScreenKeyboardLayout.Url => UrlRows,
            _ => TextRows,
        };
    }

    private static IReadOnlyList<OnScreenKeyboardKey> CreateTextRow( string keys )
    {
        return keys.Select( key => new OnScreenKeyboardKey( key.ToString() ) ).ToArray();
    }

    private static OnScreenKeyboardKey CommandKey( OnScreenKeyboardKeyType keyType, string displayText, int width = 1 )
    {
        return new( keyType, displayText )
        {
            Width = width,
        };
    }

    /// <inheritdoc/>
    protected internal override void DirtyClasses()
    {
        RowClassBuilder.Dirty();
        KeyClassBuilder.Dirty();

        base.DirtyClasses();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the class builder for keyboard rows.
    /// </summary>
    protected ClassBuilder RowClassBuilder { get; }

    /// <summary>
    /// Gets the class builder for keyboard keys.
    /// </summary>
    protected ClassBuilder KeyClassBuilder { get; }

    private IReadOnlyList<IReadOnlyList<OnScreenKeyboardKey>> Rows => CreateRows();

    private OnScreenKeyboardLayout CurrentLayout => OnScreenKeyboardService.State.Context?.Layout
        ?? Options?.AccessibilityOptions?.OnScreenKeyboard?.DefaultLayout
        ?? OnScreenKeyboardLayout.Text;

    private OnScreenKeyboardPlacement EffectivePlacement => Placement
        ?? Options?.AccessibilityOptions?.OnScreenKeyboard?.Placement
        ?? OnScreenKeyboardPlacement.Bottom;

    private bool Visible => OnScreenKeyboardService.State.Visible;

    private IReadOnlyList<IReadOnlyList<OnScreenKeyboardKey>> TextRows => new[]
    {
        CreateTextRow( "qwertyuiop" ),
        CreateTextRow( "asdfghjkl" ),
        new[] { CommandKey( OnScreenKeyboardKeyType.Shift, shift ? "SHIFT" : "Shift", 2 ) }.Concat( CreateTextRow( "zxcvbnm" ) ).Concat( new[] { CommandKey( OnScreenKeyboardKeyType.Backspace, "Backspace", 2 ) } ).ToArray(),
        new[] { CommandKey( OnScreenKeyboardKeyType.Clear, "Clear", 2 ), CommandKey( OnScreenKeyboardKeyType.Space, "Space", 6 ), CommandKey( OnScreenKeyboardKeyType.Enter, "Enter", 2 ) },
    };

    private IReadOnlyList<IReadOnlyList<OnScreenKeyboardKey>> EmailRows => new[]
    {
        CreateTextRow( "qwertyuiop" ),
        CreateTextRow( "asdfghjkl" ),
        new[] { CommandKey( OnScreenKeyboardKeyType.Shift, shift ? "SHIFT" : "Shift", 2 ) }.Concat( CreateTextRow( "zxcvbnm" ) ).Concat( new[] { CommandKey( OnScreenKeyboardKeyType.Backspace, "Backspace", 2 ) } ).ToArray(),
        new[] { new OnScreenKeyboardKey( "@" ), new OnScreenKeyboardKey( "." ), CommandKey( OnScreenKeyboardKeyType.Space, "Space", 5 ), CommandKey( OnScreenKeyboardKeyType.Enter, "Enter", 2 ) },
    };

    private IReadOnlyList<IReadOnlyList<OnScreenKeyboardKey>> UrlRows => new[]
    {
        CreateTextRow( "qwertyuiop" ),
        CreateTextRow( "asdfghjkl" ),
        new[] { CommandKey( OnScreenKeyboardKeyType.Shift, shift ? "SHIFT" : "Shift", 2 ) }.Concat( CreateTextRow( "zxcvbnm" ) ).Concat( new[] { CommandKey( OnScreenKeyboardKeyType.Backspace, "Backspace", 2 ) } ).ToArray(),
        new[] { new OnScreenKeyboardKey( "/" ), new OnScreenKeyboardKey( "." ), new OnScreenKeyboardKey( "-" ), CommandKey( OnScreenKeyboardKeyType.Space, "Space", 4 ), CommandKey( OnScreenKeyboardKeyType.Enter, "Enter", 2 ) },
    };

    private static IReadOnlyList<IReadOnlyList<OnScreenKeyboardKey>> NumericRows => new[]
    {
        CreateTextRow( "123" ),
        CreateTextRow( "456" ),
        CreateTextRow( "789" ),
        new[] { CommandKey( OnScreenKeyboardKeyType.Clear, "Clear" ), new OnScreenKeyboardKey( "0" ), CommandKey( OnScreenKeyboardKeyType.Backspace, "Backspace" ), CommandKey( OnScreenKeyboardKeyType.Enter, "Enter" ) },
    };

    private static IReadOnlyList<IReadOnlyList<OnScreenKeyboardKey>> DecimalRows => new[]
    {
        CreateTextRow( "123" ),
        CreateTextRow( "456" ),
        CreateTextRow( "789" ),
        new[] { CommandKey( OnScreenKeyboardKeyType.Clear, "Clear" ), new OnScreenKeyboardKey( "0" ), new OnScreenKeyboardKey( "." ), CommandKey( OnScreenKeyboardKeyType.Backspace, "Backspace" ), CommandKey( OnScreenKeyboardKeyType.Enter, "Enter" ) },
    };

    private static IReadOnlyList<IReadOnlyList<OnScreenKeyboardKey>> TelephoneRows => new[]
    {
        CreateTextRow( "123" ),
        CreateTextRow( "456" ),
        CreateTextRow( "789" ),
        new[] { new OnScreenKeyboardKey( "+" ), new OnScreenKeyboardKey( "0" ), CommandKey( OnScreenKeyboardKeyType.Backspace, "Backspace" ), CommandKey( OnScreenKeyboardKeyType.Enter, "Enter" ) },
    };

    /// <summary>
    /// Gets the service that controls the on-screen keyboard.
    /// </summary>
    [Inject] protected IOnScreenKeyboardService OnScreenKeyboardService { get; set; }

    /// <summary>
    /// Gets the global Blazorise options.
    /// </summary>
    [Inject] protected BlazoriseOptions Options { get; set; }

    /// <summary>
    /// Gets or sets the keyboard placement.
    /// </summary>
    [Parameter] public OnScreenKeyboardPlacement? Placement { get; set; }

    /// <summary>
    /// Gets or sets the button color.
    /// </summary>
    [Parameter] public Color KeyColor { get; set; } = Color.Light;

    /// <summary>
    /// Gets or sets the button size.
    /// </summary>
    [Parameter] public Size KeySize { get; set; } = Size.Default;

    /// <summary>
    /// Gets or sets the keyboard aria-label.
    /// </summary>
    [Parameter] public string AriaLabel { get; set; } = "On-screen keyboard";

    /// <summary>
    /// Gets or sets custom keyboard content.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}