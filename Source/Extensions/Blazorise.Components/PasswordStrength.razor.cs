#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Localization;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Components;

/// <summary>
/// Provides a password input with configurable validation rules, progress indicator, and localized rule texts.
/// </summary>
public partial class PasswordStrength : BaseTextInput<string>
{
    #region Members

    private static readonly string[] DefaultBlockedPasswords =
    [
        "123456",
        "12345678",
        "123456789",
        "abc123",
        "admin",
        "dragon",
        "football",
        "iloveyou",
        "letmein",
        "login",
        "master",
        "monkey",
        "passw0rd",
        "password",
        "password1",
        "princess",
        "qwerty",
        "qwerty123",
        "welcome",
        "111111",
    ];

    private readonly List<PasswordRuleState> ruleStates = new();

    private readonly HashSet<string> effectiveBlockedPasswords = new( StringComparer.OrdinalIgnoreCase );

    private TextInput textInputRef;

    private bool showPassword;

    private int strengthScore;

    private int totalRulesCount;

    private int passedRulesCount;

    private bool isValid;

    private PasswordStrengthLevel strengthLevel;

    private int? rulesSignature;

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if ( LocalizerService is not null )
        {
            LocalizerService.LocalizationChanged += OnLocalizationChanged;
        }

        base.OnInitialized();
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        BuildBlockedPasswordsLookup();
        EvaluatePassword();

        int currentRulesSignature = BuildRulesSignature();

        if ( Rendered && rulesSignature.HasValue && rulesSignature.Value != currentRulesSignature )
        {
            ExecuteAfterRender( Revalidate );
        }

        rulesSignature = currentRulesSignature;

        base.OnParametersSet();
    }

    /// <summary>
    /// Handles localization changes by re-rendering the component when default localization is used.
    /// </summary>
    /// <param name="sender">Object that raised the event.</param>
    /// <param name="eventArgs">Data about the localization event.</param>
    private async void OnLocalizationChanged( object sender, EventArgs eventArgs )
    {
        // no need to refresh if we're using custom localization
        if ( PasswordStrengthLocalizer is not null )
            return;

        await InvokeAsync( StateHasChanged );
    }

    private void BuildBlockedPasswordsLookup()
    {
        effectiveBlockedPasswords.Clear();

        if ( UseDefaultBlockedPasswordList )
        {
            foreach ( string blockedPassword in DefaultBlockedPasswords )
            {
                effectiveBlockedPasswords.Add( blockedPassword );
            }
        }

        if ( BlockedPasswords is null )
            return;

        foreach ( string blockedPassword in BlockedPasswords )
        {
            if ( string.IsNullOrWhiteSpace( blockedPassword ) )
                continue;

            effectiveBlockedPasswords.Add( blockedPassword.Trim() );
        }
    }

    private void EvaluatePassword( string passwordValue = null )
    {
        ruleStates.Clear();

        string password = passwordValue ?? Value ?? string.Empty;

        if ( MinimumLength > 0 )
        {
            bool hasMinimumLength = password.Length >= MinimumLength;
            ruleStates.Add( new( "At least {0} characters", hasMinimumLength, 3, MinimumLength ) );
        }

        if ( RequireUppercase )
        {
            bool hasUppercase = password.Any( char.IsUpper );
            ruleStates.Add( new( "One uppercase letter", hasUppercase, 1 ) );
        }

        if ( RequireLowercase )
        {
            bool hasLowercase = password.Any( char.IsLower );
            ruleStates.Add( new( "One lowercase letter", hasLowercase, 1 ) );
        }

        if ( RequireNumber )
        {
            bool hasNumber = password.Any( char.IsDigit );
            ruleStates.Add( new( "One number", hasNumber, 1 ) );
        }

        if ( RequireSpecialCharacter )
        {
            bool hasSpecialCharacter = password.Any( c => char.IsPunctuation( c ) || char.IsSymbol( c ) );
            ruleStates.Add( new( "One special character", hasSpecialCharacter, 1 ) );
        }

        if ( RequireNotCompromisedPassword && effectiveBlockedPasswords.Count > 0 )
        {
            bool isNotCompromised = IsNotCompromisedPassword( password );
            ruleStates.Add( new( "Not a common or breached password", isNotCompromised, 2 ) );
        }

        totalRulesCount = ruleStates.Count;
        passedRulesCount = ruleStates.Count( x => x.IsSatisfied );

        if ( string.IsNullOrWhiteSpace( password ) )
        {
            strengthScore = 0;
            strengthLevel = PasswordStrengthLevel.None;
            isValid = false;
            return;
        }

        int totalWeight = ruleStates.Sum( x => x.Weight );
        int passedWeight = ruleStates.Where( x => x.IsSatisfied ).Sum( x => x.Weight );

        if ( totalWeight > 0 )
        {
            strengthScore = (int)Math.Round( (double)passedWeight / totalWeight * 100d, MidpointRounding.AwayFromZero );
        }
        else
        {
            strengthScore = CalculateLengthOnlyScore( password );
        }

        strengthLevel = strengthScore switch
        {
            >= 100 => PasswordStrengthLevel.Strong,
            >= 75 => PasswordStrengthLevel.Good,
            >= 50 => PasswordStrengthLevel.Fair,
            _ => PasswordStrengthLevel.Weak,
        };

        isValid = totalRulesCount > 0
            ? ruleStates.All( x => x.IsSatisfied )
            : true;
    }

    private bool IsNotCompromisedPassword( string password )
    {
        if ( string.IsNullOrWhiteSpace( password ) )
            return false;

        return !effectiveBlockedPasswords.Contains( password.Trim() );
    }

    private int BuildRulesSignature()
    {
        HashCode hashCode = new();

        hashCode.Add( MinimumLength );
        hashCode.Add( RequireUppercase );
        hashCode.Add( RequireLowercase );
        hashCode.Add( RequireNumber );
        hashCode.Add( RequireSpecialCharacter );
        hashCode.Add( RequireNotCompromisedPassword );
        hashCode.Add( UseDefaultBlockedPasswordList );

        foreach ( string blockedPassword in effectiveBlockedPasswords.OrderBy( x => x, StringComparer.OrdinalIgnoreCase ) )
        {
            hashCode.Add( blockedPassword, StringComparer.OrdinalIgnoreCase );
        }

        return hashCode.ToHashCode();
    }

    private static int CalculateLengthOnlyScore( string password )
    {
        if ( string.IsNullOrEmpty( password ) )
            return 0;

        if ( password.Length >= 15 )
            return 100;

        if ( password.Length >= 12 )
            return 75;

        if ( password.Length >= 8 )
            return 50;

        return 25;
    }

    private PasswordStrengthChangedEventArgs CreateStrengthChangedEventArgs()
    {
        return new PasswordStrengthChangedEventArgs(
            Value,
            strengthLevel,
            strengthScore,
            passedRulesCount,
            totalRulesCount,
            isValid );
    }

    private Task TogglePasswordVisibility()
    {
        showPassword = !showPassword;
        return Task.CompletedTask;
    }

    private string GetRuleText( PasswordRuleState ruleState )
    {
        return GetLocalizedString( ruleState.ResourceName, ruleState.Arguments );
    }

    /// <inheritdoc />
    protected override Task<ParseValue<string>> ParseValueFromStringAsync( string value )
    {
        return Task.FromResult( new ParseValue<string>( true, value ?? string.Empty, null ) );
    }

    /// <inheritdoc />
    protected override async Task OnInternalValueChanged( string value )
    {
        EvaluatePassword( value );

        await base.OnInternalValueChanged( value );
        await StrengthChanged.InvokeAsync( CreateStrengthChangedEventArgs() );
    }

    /// <inheritdoc />
    protected override void ReleaseResources()
    {
        if ( LocalizerService is not null )
        {
            LocalizerService.LocalizationChanged -= OnLocalizationChanged;
        }

        base.ReleaseResources();
    }

    private string GetLocalizedString( string resourceName, params object[] arguments )
    {
        if ( PasswordStrengthLocalizer is not null )
            return PasswordStrengthLocalizer.Invoke( resourceName, arguments );

        if ( Localizer is null )
            return arguments?.Length > 0
                ? string.Format( CultureInfo.CurrentCulture, resourceName, arguments )
                : resourceName;

        return Localizer[resourceName, arguments];
    }

    private IconName GetRuleIconName( PasswordRuleState ruleState )
        => ruleState.IsSatisfied ? IconName.Check : IconName.Times;

    private TextColor GetRuleIconColor( PasswordRuleState ruleState )
        => ruleState.IsSatisfied ? TextColor.Success : TextColor.Secondary;

    private TextColor GetRuleTextColor( PasswordRuleState ruleState )
        => ruleState.IsSatisfied ? TextColor.Success : TextColor.Secondary;

    /// <inheritdoc />
    public override Task Focus( bool scrollToElement = true )
    {
        if ( textInputRef is null )
            return Task.CompletedTask;

        return textInputRef.Focus( scrollToElement );
    }

    #endregion

    #region Properties

    protected IReadOnlyList<PasswordRuleState> RuleStates => ruleStates;

    protected int StrengthScore => strengthScore;

    protected Color StrengthProgressColor => strengthLevel switch
    {
        PasswordStrengthLevel.Strong => Color.Success,
        PasswordStrengthLevel.Good => Color.Primary,
        PasswordStrengthLevel.Fair => Color.Warning,
        PasswordStrengthLevel.Weak => Color.Danger,
        _ => Color.Secondary,
    };

    protected TextColor StrengthTextColor => strengthLevel switch
    {
        PasswordStrengthLevel.Strong => TextColor.Success,
        PasswordStrengthLevel.Good => TextColor.Primary,
        PasswordStrengthLevel.Fair => TextColor.Warning,
        PasswordStrengthLevel.Weak => TextColor.Danger,
        _ => TextColor.Secondary,
    };

    protected string StrengthLabelCaption => GetLocalizedString( "Strength" );

    protected string StrengthLabelText => strengthLevel switch
    {
        PasswordStrengthLevel.Strong => GetLocalizedString( "Strong" ),
        PasswordStrengthLevel.Good => GetLocalizedString( "Good" ),
        PasswordStrengthLevel.Fair => GetLocalizedString( "Fair" ),
        PasswordStrengthLevel.Weak => GetLocalizedString( "Weak" ),
        _ => string.Empty,
    };

    protected string TitleText => string.IsNullOrWhiteSpace( Title )
        ? GetLocalizedString( "Password Strength" )
        : Title;

    protected string InputPlaceholder => string.IsNullOrWhiteSpace( Placeholder )
        ? GetLocalizedString( "Enter password" )
        : Placeholder;

    protected TextRole InputRole => showPassword ? TextRole.Text : TextRole.Password;

    protected IconName VisibilityIconName => showPassword ? IconName.EyeSlash : IconName.Eye;

    protected string TogglePasswordButtonLabel => showPassword
        ? GetLocalizedString( "Hide password" )
        : GetLocalizedString( "Show password" );

    protected string InputClassNames => ClassProvider.TextInputValidation( ParentValidation?.Status ?? ValidationStatus.None );

    protected string CardClassNames => string.IsNullOrWhiteSpace( Class )
        ? "b-password-strength"
        : $"b-password-strength {Class}";

    /// <summary>
    /// Gets or sets the DI registered <see cref="ITextLocalizerService"/>.
    /// </summary>
    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    /// <summary>
    /// Gets or sets the DI registered <see cref="ITextLocalizer{PasswordStrength}"/>.
    /// </summary>
    [Inject] protected ITextLocalizer<PasswordStrength> Localizer { get; set; }

    /// <summary>
    /// Gets or sets the component title.
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// Gets or sets whether to display password strength details.
    /// </summary>
    [Parameter] public bool ShowStrength { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to display the checklist of active rules.
    /// </summary>
    [Parameter] public bool ShowRules { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show a toggle button for password visibility.
    /// </summary>
    [Parameter] public bool ShowPasswordToggle { get; set; } = true;

    /// <summary>
    /// If true, the entered value will be updated after each key press.
    /// </summary>
    [Parameter] public bool? Immediate { get; set; } = true;

    /// <summary>
    /// If true, the entered value will be updated after a delay.
    /// </summary>
    [Parameter] public bool? Debounce { get; set; }

    /// <summary>
    /// Interval in milliseconds used to debounce the text update.
    /// </summary>
    [Parameter] public int? DebounceInterval { get; set; } = 300;

    /// <summary>
    /// The minimum required password length. Defaults to 15 to align with current security recommendations for single-factor passwords.
    /// </summary>
    [Parameter] public int MinimumLength { get; set; } = 15;

    /// <summary>
    /// If true, requires at least one uppercase character.
    /// <para>
    /// Disabled by default to follow modern guidance that favors longer passphrases over strict composition rules.
    /// </para>
    /// </summary>
    [Parameter] public bool RequireUppercase { get; set; }

    /// <summary>
    /// If true, requires at least one lowercase character.
    /// <para>
    /// Disabled by default to follow modern guidance that favors longer passphrases over strict composition rules.
    /// </para>
    /// </summary>
    [Parameter] public bool RequireLowercase { get; set; }

    /// <summary>
    /// If true, requires at least one digit.
    /// <para>
    /// Disabled by default to follow modern guidance that favors longer passphrases over strict composition rules.
    /// </para>
    /// </summary>
    [Parameter] public bool RequireNumber { get; set; }

    /// <summary>
    /// If true, requires at least one symbol or punctuation character.
    /// <para>
    /// Disabled by default to follow modern guidance that favors longer passphrases over strict composition rules.
    /// </para>
    /// </summary>
    [Parameter] public bool RequireSpecialCharacter { get; set; }

    /// <summary>
    /// If true, validates the value against blocked passwords.
    /// </summary>
    [Parameter] public bool RequireNotCompromisedPassword { get; set; } = true;

    /// <summary>
    /// If true, includes a small default list of commonly compromised passwords in blocked checks.
    /// </summary>
    [Parameter] public bool UseDefaultBlockedPasswordList { get; set; } = true;

    /// <summary>
    /// Gets or sets additional blocked passwords used during validation.
    /// </summary>
    [Parameter] public IEnumerable<string> BlockedPasswords { get; set; }

    /// <summary>
    /// Occurs after strength evaluation has changed.
    /// </summary>
    [Parameter] public EventCallback<PasswordStrengthChangedEventArgs> StrengthChanged { get; set; }

    /// <summary>
    /// Function used to handle custom localization that will override a default <see cref="ITextLocalizer"/>.
    /// </summary>
    [Parameter] public TextLocalizerHandler PasswordStrengthLocalizer { get; set; }

    /// <summary>
    /// Captures all the custom attributes that should be forwarded to the internal password input element.
    /// </summary>
    [Parameter] public Dictionary<string, object> InputAttributes { get; set; }

    /// <inheritdoc />
    protected override string DefaultValue => string.Empty;

    #endregion

    #region Data Types

    protected sealed class PasswordRuleState
    {
        public PasswordRuleState( string resourceName, bool isSatisfied, int weight, params object[] arguments )
        {
            ResourceName = resourceName;
            IsSatisfied = isSatisfied;
            Weight = weight;
            Arguments = arguments ?? [];
        }

        public string ResourceName { get; }

        public bool IsSatisfied { get; }

        public int Weight { get; }

        public object[] Arguments { get; }
    }

    #endregion
}