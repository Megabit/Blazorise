#region Using directives
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// RadioGroup is a helpful wrapper used to group Radio components.
/// </summary>
public partial class RadioGroup<TValue> : BaseInputComponent<TValue>
{
    #region Members

    /// <summary>
    /// Flag which indicates that radios will appear as button.
    /// </summary>
    private bool buttons;

    /// <summary>
    /// Defines the orientation of the radio elements.
    /// </summary>
    private Orientation orientation = Orientation.Horizontal;

    /// <summary>
    /// Defines the color or radio buttons(only when <see cref="Buttons"/> is true).
    /// </summary>
    private Color color = Color.Secondary;

    /// <summary>
    /// An event raised after the internal radio group value has changed.
    /// </summary>
    public event EventHandler<RadioCheckedChangedEventArgs<TValue>> RadioCheckedChanged;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        if ( ( parameters.TryGetValue<TValue>( nameof( CheckedValue ), out var result ) && !CheckedValue.IsEqual( result ) )
            || ( parameters.TryGetValue<TValue>( nameof( Value ), out var paramValue ) && !paramValue.IsEqual( Value ) ) )
        {
            await CurrentValueHandler( result?.ToString() );
        }

        await base.SetParametersAsync( parameters );

        if ( ParentValidation is not null )
        {
            if ( parameters.TryGetValue<Expression<Func<TValue>>>( nameof( CheckedValueExpression ), out var expression ) )
                await ParentValidation.InitializeInputExpression( expression );
            else if ( parameters.TryGetValue<Expression<Func<TValue>>>( nameof( ValueExpression ), out expression ) )
                await ParentValidation.InitializeInputExpression( expression );

            await InitializeValidation();
        }
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.RadioGroup( Buttons, Orientation ) );
        builder.Append( ClassProvider.RadioGroupSize( Buttons, Orientation, ThemeSize ) );
        builder.Append( ClassProvider.RadioGroupValidation( ParentValidation?.Status ?? ValidationStatus.None ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override Task<ParseValue<TValue>> ParseValueFromStringAsync( string value )
    {
        if ( string.IsNullOrEmpty( value ) )
            return Task.FromResult( ParseValue<TValue>.Empty );

        if ( Converters.TryChangeType<TValue>( value, out var result ) )
        {
            return Task.FromResult( new ParseValue<TValue>( true, result, null ) );
        }
        else
        {
            return Task.FromResult( ParseValue<TValue>.Empty );
        }
    }

    /// <inheritdoc/>
    protected override Task OnInternalValueChanged( TValue value )
    {
        // notify child radios they need to update their states
        RadioCheckedChanged?.Invoke( this, new( value ) );

        return CheckedValueChanged.InvokeAsync( value );
    }

    /// <summary>
    /// Notifies radio group that one of it's radios have changed.
    /// </summary>
    /// <param name="radio">Radio from which change was received.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    internal async Task NotifyRadioChanged( Radio<TValue> radio )
    {
        await CurrentValueHandler( radio.Value?.ToString() );

        await InvokeAsync( StateHasChanged );
    }

    /// <inheritdoc/>
    protected override void OnValidationStatusChanged( object sender, ValidationStatusChangedEventArgs eventArgs )
    {
        base.OnValidationStatusChanged( sender, eventArgs );

        // Since radios validation works little different when placed in radio group we need
        // to notify them to re-render when validation changes.
        RadioCheckedChanged?.Invoke( this, new( CheckedValue ) );
    }

    /// <inheritdoc/>
    protected override string GetFormatedValueExpression()
    {
        if ( CheckedValueExpression is null )
            return null;

        return HtmlFieldPrefix is not null
            ? HtmlFieldPrefix.GetFieldName( CheckedValueExpression )
            : ExpressionFormatter.FormatLambda( CheckedValueExpression );
    }

    #endregion

    #region Properties

    /// <summary>
    /// True of radio elements should be inlined.
    /// </summary>
    public bool Inline => Orientation == Orientation.Horizontal;

    /// <summary>
    /// Radio group name.
    /// </summary>
    [Parameter] public string Name { get; set; }

    /// <summary>
    /// Flag which indicates that radios will appear as button.
    /// </summary>
    [Parameter]
    public bool Buttons
    {
        get => buttons;
        set
        {
            buttons = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Defines the orientation of the radio elements.
    /// </summary>
    [Parameter]
    public Orientation Orientation
    {
        get => orientation;
        set
        {
            orientation = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Defines the color or radio buttons(only when <see cref="Buttons"/> is true).
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
    /// Gets or sets the checked value.
    /// </summary>
    [Obsolete( "The 'CheckedValue' property is obsolete and will be removed in future versions. Use 'Value' instead." )]
    [Parameter] public TValue CheckedValue { get => Value; set => Value = value; }

    /// <summary>
    /// Occurs when the checked value is changed.
    /// </summary>
    [Obsolete( "The 'CheckedValueChanged' property is obsolete and will be removed in future versions. Use 'ValueChanged' instead." )]
    [Parameter] public EventCallback<TValue> CheckedValueChanged { get => ValueChanged; set => ValueChanged = value; }

    /// <summary>
    /// Gets or sets an expression that identifies the checked value.
    /// </summary>
    [Obsolete( "The 'CheckedValueExpression' property is obsolete and will be removed in future versions. Use 'ValueExpression' instead." )]
    [Parameter] public Expression<Func<TValue>> CheckedValueExpression { get => ValueExpression; set => ValueExpression = value; }

    #endregion
}