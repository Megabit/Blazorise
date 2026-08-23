#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Wrapper for text, buttons, or button groups on either side of textual inputs.
/// </summary>
public partial class Addons : BaseComponent, IDisposable
{
    #region Members

    private Size? size;

    private List<Button> registeredButtons;

    private readonly List<ValidationFeedbackRegistration> validationFeedbackRegistrations = new();

    #endregion

    #region Constructors

    /// <summary>
    /// A default <see cref="Addons"/> constructor.
    /// </summary>
    public Addons()
    {
        FeedbackClassBuilder = new( BuildFeedbackClasses );
        FeedbackStyleBuilder = new( BuildFeedbackStyles );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        if ( ParentValidation is not null )
            ParentValidation.ValidationStatusChanged += OnValidationStatusChanged;

        await base.OnInitializedAsync();

        if ( Theme is not null )
        {
            Theme.Changed += OnThemeChanged;
        }
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender && registeredButtons?.Count > 0 )
        {
            DirtyClasses();

            await InvokeAsync( StateHasChanged );
        }

        await base.OnAfterRenderAsync( firstRender );
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            if ( ParentValidation is not null )
            {
                ParentValidation.ValidationStatusChanged -= OnValidationStatusChanged;
            }

            foreach ( var registration in validationFeedbackRegistrations )
            {
                registration.Validation.ValidationStatusChanged -= OnValidationStatusChanged;
            }

            validationFeedbackRegistrations.Clear();

            if ( Theme is not null )
            {
                Theme.Changed -= OnThemeChanged;
            }
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Addons() );
        builder.Append( ClassProvider.AddonsSize( ThemeSize ) );
        builder.Append( ClassProvider.AddonsHasButton( registeredButtons?.Count > 0 ) );
        builder.Append( ClassProvider.AddonsValidation( EffectiveValidationStatus ), ParentValidation is not null || validationFeedbackRegistrations.Count > 0 );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Builds the class names for the validation feedback container.
    /// </summary>
    /// <param name="builder">Class builder used to append the class names.</param>
    protected virtual void BuildFeedbackClasses( ClassBuilder builder )
    {
    }

    /// <summary>
    /// Builds the styles for the validation feedback container.
    /// </summary>
    /// <param name="builder">Style builder used to append the styles.</param>
    protected virtual void BuildFeedbackStyles( StyleBuilder builder )
    {
    }

    /// <inheritdoc/>
    protected internal override void DirtyClasses()
    {
        FeedbackClassBuilder.Dirty();

        base.DirtyClasses();
    }

    /// <inheritdoc/>
    protected internal override void DirtyStyles()
    {
        FeedbackStyleBuilder.Dirty();

        base.DirtyStyles();
    }

    /// <summary>
    /// Handles the <see cref="Validation.StatusChanged"/> event.
    /// </summary>
    /// <param name="sender">Object that raised the event.</param>
    /// <param name="eventArgs">Data about the <see cref="Validation"/> status change event.</param>
    protected void OnValidationStatusChanged( object sender, ValidationStatusChangedEventArgs eventArgs )
    {
        DirtyClasses();

        InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Registers validation feedback supplied by an input placed inside of this addons component.
    /// </summary>
    /// <param name="input">Input that owns the feedback.</param>
    /// <param name="validation">Validation associated with the input.</param>
    /// <param name="getFeedback">Function that gets the current feedback fragment to render.</param>
    internal void NotifyValidationFeedbackInitialized( IValidationInput input, Validation validation, Func<RenderFragment> getFeedback )
    {
        if ( input is null || validation is null )
            return;

        if ( validationFeedbackRegistrations.Exists( x => ReferenceEquals( x.Input, input ) ) )
            return;

        if ( !ReferenceEquals( validation, ParentValidation )
            && !validationFeedbackRegistrations.Exists( x => ReferenceEquals( x.Validation, validation ) ) )
        {
            validation.ValidationStatusChanged += OnValidationStatusChanged;
        }

        validationFeedbackRegistrations.Add( new( input, validation, getFeedback ) );

        DirtyClasses();
        InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Removes validation feedback supplied by an input placed inside of this addons component.
    /// </summary>
    /// <param name="input">Input that owns the feedback.</param>
    internal void NotifyValidationFeedbackRemoved( IValidationInput input )
    {
        if ( input is null )
            return;

        var registration = validationFeedbackRegistrations.Find( x => ReferenceEquals( x.Input, input ) );

        if ( registration is null )
            return;

        validationFeedbackRegistrations.Remove( registration );

        if ( !ReferenceEquals( registration.Validation, ParentValidation )
            && !validationFeedbackRegistrations.Exists( x => ReferenceEquals( x.Validation, registration.Validation ) ) )
        {
            registration.Validation.ValidationStatusChanged -= OnValidationStatusChanged;
        }

        DirtyClasses();
        InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Notify addons that a button is placed inside of it.
    /// </summary>
    /// <param name="button">A button reference that is placed inside of the addons.</param>
    internal void NotifyButtonInitialized( Button button )
    {
        if ( button is null )
            return;

        registeredButtons ??= new();

        if ( !registeredButtons.Contains( button ) )
        {
            registeredButtons.Add( button );
        }
    }

    /// <summary>
    /// Notify addons that a button is removed from it.
    /// </summary>
    /// <param name="button">A button reference that is placed inside of the addons.</param>
    internal void NotifyButtonRemoved( Button button )
    {
        if ( button is null )
            return;

        if ( registeredButtons is not null && registeredButtons.Contains( button ) )
        {
            registeredButtons.Remove( button );
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

    private static ValidationStatus ResolveValidationStatus( ValidationStatus currentStatus, ValidationStatus nextStatus )
    {
        if ( currentStatus == ValidationStatus.Error || nextStatus == ValidationStatus.Error )
            return ValidationStatus.Error;

        if ( currentStatus == ValidationStatus.Warning || nextStatus == ValidationStatus.Warning )
            return ValidationStatus.Warning;

        if ( currentStatus == ValidationStatus.None || nextStatus == ValidationStatus.None )
            return ValidationStatus.None;

        return ValidationStatus.Success;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the validation feedback registrations for inputs placed inside of this addons component.
    /// </summary>
    protected IReadOnlyList<ValidationFeedbackRegistration> ValidationFeedbackRegistrations => validationFeedbackRegistrations;

    /// <summary>
    /// True if <see cref="Addons"/> is placed inside of <see cref="Field"/> component.
    /// </summary>
    protected virtual bool ParentIsHorizontal => ParentField?.Horizontal == true;

    /// <summary>
    /// Gets the aggregate validation status for all inputs placed inside of this addons component.
    /// </summary>
    protected ValidationStatus EffectiveValidationStatus
    {
        get
        {
            var effectiveValidationStatus = ParentValidation?.Status;

            foreach ( var registration in validationFeedbackRegistrations )
            {
                effectiveValidationStatus = effectiveValidationStatus.HasValue
                    ? ResolveValidationStatus( effectiveValidationStatus.Value, registration.Validation.Status )
                    : registration.Validation.Status;
            }

            return effectiveValidationStatus ?? ValidationStatus.None;
        }
    }

    /// <summary>
    /// Validation feedback container class builder.
    /// </summary>
    protected ClassBuilder FeedbackClassBuilder { get; private set; }

    /// <summary>
    /// Validation feedback container style builder.
    /// </summary>
    protected StyleBuilder FeedbackStyleBuilder { get; private set; }

    /// <summary>
    /// Gets the class names for the validation feedback container.
    /// </summary>
    protected string FeedbackClassNames => FeedbackClassBuilder.Class;

    /// <summary>
    /// Gets the styles for the validation feedback container.
    /// </summary>
    protected string FeedbackStyleNames => FeedbackStyleBuilder.Styles;

    /// <summary>
    /// Gets the size based on the theme settings.
    /// </summary>
    protected Size ThemeSize => Size.GetValueOrDefault( Theme?.InputOptions?.Size ?? Blazorise.Size.Default );

    /// <summary>
    /// Changes the size of the elements placed inside of this <see cref="Addons"/>.
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
    /// Specifies the content to be rendered inside this <see cref="Addons"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Provides the reference to the parent <see cref="Field"/> component.
    /// </summary>
    [CascadingParameter] protected Field ParentField { get; set; }

    /// <summary>
    /// Cascaded theme settings.
    /// </summary>
    [CascadingParameter] public Theme Theme { get; set; }

    /// <summary>
    /// A reference to the parent <see cref="Validation"/> component in which this component is nested.
    /// </summary>
    [CascadingParameter] protected Validation ParentValidation { get; set; }

    #endregion

    #region Data Types

    /// <summary>
    /// Holds the validation feedback associated with an input placed inside of this addons component.
    /// </summary>
    protected sealed class ValidationFeedbackRegistration
    {
        /// <summary>
        /// Initializes a new validation feedback registration.
        /// </summary>
        /// <param name="input">Input that owns the feedback.</param>
        /// <param name="validation">Validation associated with the input.</param>
        /// <param name="getFeedback">Function that gets the current feedback fragment.</param>
        public ValidationFeedbackRegistration( IValidationInput input, Validation validation, Func<RenderFragment> getFeedback )
        {
            Input = input;
            Validation = validation;
            GetFeedback = getFeedback;
        }

        /// <summary>
        /// Gets the input that owns the feedback.
        /// </summary>
        public IValidationInput Input { get; }

        /// <summary>
        /// Gets the validation associated with the input.
        /// </summary>
        public Validation Validation { get; }

        /// <summary>
        /// Gets the validation status formatted for Razor markup.
        /// </summary>
        public string ValidationStatusString => Validation.Status switch
        {
            ValidationStatus.None => "none",
            ValidationStatus.Success => "success",
            ValidationStatus.Warning => "warning",
            ValidationStatus.Error => "error",
            _ => null,
        };

        /// <summary>
        /// Gets the function that provides the current feedback fragment.
        /// </summary>
        public Func<RenderFragment> GetFeedback { get; }
    }

    #endregion
}