#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Components;

/// <summary>
/// Provides a grouped multi-slot input intended for one-time codes and similar short values.
/// </summary>
public partial class OneTimeInput : BaseInputComponent<string, OneTimeInputClasses, OneTimeInputStyles>
{
    #region Members

    private const string Separator = "-";

    private readonly List<string> slotValues = new();

    private readonly List<SlotGroup> slotGroups = new();

    protected ComponentParameterInfo<string> paramPattern;

    #endregion

    #region Constructors

    /// <summary>
    /// A default <see cref="OneTimeInput"/> constructor.
    /// </summary>
    public OneTimeInput()
    {
        GroupClassBuilder = new( BuildGroupClasses, builder => builder.Append( Classes?.Group ) );
        SlotClassBuilder = new( BuildSlotClasses, builder => builder.Append( Classes?.Slot ) );
        SeparatorClassBuilder = new( BuildSeparatorClasses, builder => builder.Append( Classes?.Separator ) );
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void CaptureParameters( ParameterView parameters )
    {
        base.CaptureParameters( parameters );

        parameters.TryGetParameter( Pattern, out paramPattern );
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        SynchronizeSlotsFromValue();

        base.OnParametersSet();
    }

    /// <inheritdoc />
    protected override async Task OnAfterSetParametersAsync( ParameterView parameters )
    {
        if ( ParentValidation is not null && paramPattern.Defined )
        {
            string newValue = paramValue.Defined
                ? paramValue.Value
                : Value;

            await ParentValidation.InitializeInputPattern( paramPattern.Value, newValue );
        }

        await base.OnAfterSetParametersAsync( parameters );
    }

    /// <inheritdoc />
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-one-time-input" );
        builder.Append( Classes?.Container );

        base.BuildClasses( builder );
    }

    /// <inheritdoc />
    protected override void DirtyClasses()
    {
        GroupClassBuilder.Dirty();
        SlotClassBuilder.Dirty();
        SeparatorClassBuilder.Dirty();

        base.DirtyClasses();
    }

    /// <inheritdoc />
    protected override void BuildStyles( StyleBuilder builder )
    {
        builder.Append( Styles?.Container );

        base.BuildStyles( builder );
    }

    /// <inheritdoc />
    protected override Task<ParseValue<string>> ParseValueFromStringAsync( string value )
        => Task.FromResult( new ParseValue<string>( true, value ?? string.Empty, null ) );

    private void SynchronizeSlotsFromValue()
    {
        int normalizedDigits = NormalizeDigits( Digits );
        string normalizedValue = NormalizeValue( Value, normalizedDigits );

        EnsureSlotCount( normalizedDigits );
        UpdateGroups( normalizedDigits );

        for ( int slotIndex = 0; slotIndex < slotValues.Count; slotIndex++ )
        {
            slotValues[slotIndex] = slotIndex < normalizedValue.Length
                ? normalizedValue[slotIndex].ToString()
                : string.Empty;
        }

        if ( !string.Equals( Value ?? string.Empty, normalizedValue, StringComparison.Ordinal ) )
        {
            ExecuteAfterRender( () => CurrentValueHandler( normalizedValue ) );
        }
    }

    private void EnsureSlotCount( int digits )
    {
        while ( slotValues.Count < digits )
        {
            slotValues.Add( string.Empty );
        }

        while ( slotValues.Count > digits )
        {
            slotValues.RemoveAt( slotValues.Count - 1 );
        }
    }

    private void UpdateGroups( int digits )
    {
        slotGroups.Clear();

        int startIndex = 0;

        foreach ( int groupLength in ParseGroups( digits ) )
        {
            slotGroups.Add( new SlotGroup( startIndex, groupLength ) );
            startIndex += groupLength;
        }
    }

    private IEnumerable<int> ParseGroups( int digits )
    {
        if ( digits < 6 && string.IsNullOrWhiteSpace( Group ) )
        {
            yield return digits;
            yield break;
        }

        List<int> parsedGroups = new();

        if ( !string.IsNullOrWhiteSpace( Group ) )
        {
            foreach ( string part in Group.Split( ',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries ) )
            {
                if ( int.TryParse( part, NumberStyles.Integer, CultureInfo.InvariantCulture, out int groupLength ) && groupLength > 0 )
                {
                    parsedGroups.Add( groupLength );
                }
            }
        }

        if ( parsedGroups.Count == 0 )
        {
            parsedGroups.AddRange( Enumerable.Repeat( 3, digits / 3 ) );

            if ( digits % 3 != 0 )
            {
                parsedGroups.Add( digits % 3 );
            }
        }

        int remainingDigits = digits;

        foreach ( int parsedGroup in parsedGroups )
        {
            if ( remainingDigits <= 0 )
            {
                yield break;
            }

            int currentGroupLength = Math.Min( parsedGroup, remainingDigits );

            if ( currentGroupLength > 0 )
            {
                yield return currentGroupLength;
                remainingDigits -= currentGroupLength;
            }
        }

        if ( remainingDigits > 0 )
        {
            yield return remainingDigits;
        }
    }

    private async Task OnSlotValueChangedAsync( int slotIndex, string value )
    {
        if ( slotIndex < 0 || slotIndex >= slotValues.Count )
        {
            return;
        }

        char[] characters = ExtractCharacters( value );
        bool multiCharacterInput = characters.Length > 1;

        if ( characters.Length == 0 )
        {
            slotValues[slotIndex] = string.Empty;

            await CurrentValueHandler( CurrentCombinedValue );
            return;
        }

        int nextSlotIndex = slotIndex;

        foreach ( char character in characters )
        {
            if ( nextSlotIndex >= slotValues.Count )
            {
                break;
            }

            slotValues[nextSlotIndex] = character.ToString();
            nextSlotIndex++;
        }

        await CurrentValueHandler( CurrentCombinedValue );

        if ( multiCharacterInput )
        {
            int lastFilledSlotIndex = Math.Min( nextSlotIndex - 1, slotValues.Count - 1 );
            int focusSlotIndex = nextSlotIndex < slotValues.Count
                ? nextSlotIndex
                : lastFilledSlotIndex;

            ExecuteAfterRender( () => FocusSlotAsync( focusSlotIndex ) );
        }
        else if ( nextSlotIndex < slotValues.Count )
        {
            ExecuteAfterRender( () => FocusSlotAsync( nextSlotIndex ) );
        }
    }

    private async Task OnSlotKeyDownAsync( int slotIndex, KeyboardEventArgs eventArgs )
    {
        await OnKeyDownHandler( eventArgs );

        if ( IsKey( eventArgs, "ArrowLeft" ) && slotIndex > 0 )
        {
            ExecuteAfterRender( () => FocusSlotAsync( slotIndex - 1 ) );
        }
        else if ( IsKey( eventArgs, "ArrowRight" ) && slotIndex < slotValues.Count - 1 )
        {
            ExecuteAfterRender( () => FocusSlotAsync( slotIndex + 1 ) );
        }
        else if ( IsKey( eventArgs, "Backspace" ) )
        {
            if ( !string.IsNullOrEmpty( slotValues[slotIndex] ) )
            {
                slotValues[slotIndex] = string.Empty;

                await CurrentValueHandler( CurrentCombinedValue );
            }

            if ( slotIndex > 0 )
            {
                ExecuteAfterRender( () => FocusSlotAsync( slotIndex - 1 ) );
            }
        }
    }

    private async Task OnSlotFocusAsync( int slotIndex, FocusEventArgs eventArgs )
    {
        await OnFocusHandler( eventArgs );

        ExecuteAfterRender( () => SelectSlotAsync( slotIndex ) );
    }

    /// <inheritdoc />
    public override Task Focus( bool scrollToElement = true )
    {
        if ( slotValues.Count == 0 )
        {
            return Task.CompletedTask;
        }

        int firstEmptySlot = slotValues.FindIndex( string.IsNullOrEmpty );

        return FocusSlotAsync( firstEmptySlot >= 0 ? firstEmptySlot : 0, scrollToElement );
    }

    private Task FocusSlotAsync( int slotIndex, bool scrollToElement = true )
    {
        if ( slotIndex < 0 || slotIndex >= slotValues.Count )
        {
            return Task.CompletedTask;
        }

        return JSUtilitiesModule.Focus( default, GetSlotElementId( slotIndex ), scrollToElement ).AsTask();
    }

    private Task SelectSlotAsync( int slotIndex )
    {
        if ( slotIndex < 0 || slotIndex >= slotValues.Count )
        {
            return Task.CompletedTask;
        }

        return JSUtilitiesModule.Select( default, GetSlotElementId( slotIndex ), false ).AsTask();
    }

    private string GetSlotElementId( int slotIndex )
        => $"{ElementId}-slot-{slotIndex}";

    private int? GetSlotTabIndex( int slotIndex )
        => slotIndex == 0 ? TabIndex : -1;

    private Dictionary<string, object> BuildContainerAttributes()
    {
        Dictionary<string, object> attributes = Attributes is not null
            ? new Dictionary<string, object>( Attributes )
            : new Dictionary<string, object>();

        attributes.TryAdd( "role", "group" );

        if ( !string.IsNullOrEmpty( AriaInvalid ) )
        {
            attributes["aria-invalid"] = AriaInvalid;
        }

        if ( !string.IsNullOrEmpty( AriaDescribedBy ) )
        {
            attributes["aria-describedby"] = AriaDescribedBy;
        }

        if ( Disabled )
        {
            attributes.TryAdd( "aria-disabled", "true" );
        }

        if ( ReadOnly )
        {
            attributes.TryAdd( "aria-readonly", "true" );
        }

        return attributes;
    }

    private Dictionary<string, object> BuildSlotAttributes( int slotIndex )
    {
        return new Dictionary<string, object>
        {
            ["aria-label"] = string.Format( CultureInfo.InvariantCulture, "Character {0} of {1}", slotIndex + 1, slotValues.Count ),
        };
    }

    private static char[] ExtractCharacters( string value )
        => string.IsNullOrEmpty( value ) ? Array.Empty<char>() : value.ToCharArray();

    private static bool IsKey( KeyboardEventArgs eventArgs, string key )
        => string.Equals( eventArgs?.Key, key, StringComparison.OrdinalIgnoreCase )
           || string.Equals( eventArgs?.Code, key, StringComparison.OrdinalIgnoreCase );

    private static int NormalizeDigits( int digits )
        => digits > 0 ? digits : 1;

    private static string NormalizeValue( string value, int digits )
    {
        if ( string.IsNullOrEmpty( value ) )
        {
            return string.Empty;
        }

        return value.Length <= digits
            ? value
            : value[..digits];
    }

    /// <summary>
    /// Builds a list of classnames for a slot group wrapper.
    /// </summary>
    /// <param name="builder">Class builder used to append the classnames.</param>
    protected virtual void BuildGroupClasses( ClassBuilder builder )
    {
        builder.Append( "b-one-time-input-group" );
    }

    /// <summary>
    /// Builds a list of classnames for an individual slot input.
    /// </summary>
    /// <param name="builder">Class builder used to append the classnames.</param>
    protected virtual void BuildSlotClasses( ClassBuilder builder )
    {
        builder.Append( "b-one-time-input-slot" );
        builder.Append( ClassProvider.TextInputValidation( ParentValidation?.Status ?? ValidationStatus.None ) );
    }

    /// <summary>
    /// Builds a list of classnames for the group separator.
    /// </summary>
    /// <param name="builder">Class builder used to append the classnames.</param>
    protected virtual void BuildSeparatorClasses( ClassBuilder builder )
    {
        builder.Append( "b-one-time-input-separator" );
    }

    #endregion

    #region Properties

    private string CurrentCombinedValue => string.Concat( slotValues );

    /// <summary>
    /// Group wrapper class builder.
    /// </summary>
    protected ClassBuilder GroupClassBuilder { get; private set; }

    /// <summary>
    /// Slot input class builder.
    /// </summary>
    protected ClassBuilder SlotClassBuilder { get; private set; }

    /// <summary>
    /// Separator class builder.
    /// </summary>
    protected ClassBuilder SeparatorClassBuilder { get; private set; }

    protected IReadOnlyList<SlotGroup> SlotGroups => slotGroups;

    protected string GroupClassNames => GroupClassBuilder.Class;

    protected string GroupStyleNames => Styles?.Group;

    protected string SlotClassNames => SlotClassBuilder.Class;

    protected string SlotStyleNames => Styles?.Slot;

    protected string SeparatorClassNames => SeparatorClassBuilder.Class;

    protected string SeparatorStyleNames => Styles?.Separator;

    protected Dictionary<string, object> ContainerAttributes => BuildContainerAttributes();

    /// <inheritdoc />
    protected override string DefaultValue => string.Empty;

    /// <summary>
    /// Gets or sets the number of input slots to render.
    /// </summary>
    [Parameter] public int Digits { get; set; } = 6;

    /// <summary>
    /// Gets or sets the slot grouping definition, such as <c>2,3</c>.
    /// </summary>
    [Parameter] public string Group { get; set; }

    /// <summary>
    /// Gets or sets the role of each slot input.
    /// </summary>
    [Parameter] public TextRole Role { get; set; } = TextRole.Text;

    /// <summary>
    /// Gets or sets the input mode used by each slot input.
    /// </summary>
    [Parameter] public TextInputMode InputMode { get; set; } = TextInputMode.Numeric;

    /// <summary>
    /// Gets or sets the regex pattern used by pattern-based validation.
    /// </summary>
    [Parameter] public string Pattern { get; set; }

    #endregion

    #region Data Types

    protected readonly record struct SlotGroup( int StartIndex, int Length );

    #endregion
}