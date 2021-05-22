﻿#region Using directives
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise
{
    /// <summary>
    /// An editor that displays a date value and allows a user to edit the value.
    /// </summary>
    /// <typeparam name="TValue">Data-type to be binded by the <see cref="DatePicker{TValue}"/> property.</typeparam>
    public partial class DatePicker<TValue> : BaseTextInput<TValue>
    {
        #region Methods

        /// <inheritdoc/>
        public override async Task SetParametersAsync( ParameterView parameters )
        {
            var dateChanged = parameters.TryGetValue<TValue>( nameof( Date ), out var date ) && !Date.Equals( date );
            var minChanged = parameters.TryGetValue( nameof( Min ), out DateTimeOffset? min ) && !Min.IsEqual( min );
            var maxChanged = parameters.TryGetValue( nameof( Max ), out DateTimeOffset? max ) && !Max.IsEqual( max );
            var firstDayOfWeekChanged = parameters.TryGetValue( nameof( FirstDayOfWeek ), out DayOfWeek firstDayOfWeek ) && !FirstDayOfWeek.IsEqual( firstDayOfWeek );
            var displayFormatChanged = parameters.TryGetValue( nameof( DisplayFormat ), out string displayFormat ) && DisplayFormat != displayFormat;
            var timeAs24hrChanged = parameters.TryGetValue( nameof( TimeAs24hr ), out bool timeAs24hr ) && TimeAs24hr != timeAs24hr;

            if ( dateChanged )
            {
                var dateString = FormatValueAsString( date );

                await CurrentValueHandler( dateString );

                if ( Rendered )
                {
                    ExecuteAfterRender( async () => await JSRunner.UpdateDatePickerValue( ElementRef, ElementId, dateString ) );
                }
            }

            if ( Rendered && ( minChanged || maxChanged || firstDayOfWeekChanged || displayFormatChanged || timeAs24hrChanged ) )
            {
                ExecuteAfterRender( async () => await JSRunner.UpdateDatePickerOptions( ElementRef, ElementId, new
                {
                    FirstDayOfWeek = new { Changed = firstDayOfWeekChanged, Value = firstDayOfWeek },
                    DisplayFormat = new { Changed = displayFormatChanged, Value = DateTimeFormatConverter.Convert( displayFormat ) },
                    TimeAs24hr = new { Changed = timeAs24hrChanged, Value = timeAs24hr },
                    Min = new { Changed = minChanged, Value = min?.ToString( DateFormat ) },
                    Max = new { Changed = maxChanged, Value = max?.ToString( DateFormat ) },
                } ) );
            }

            // Let blazor do its thing!
            await base.SetParametersAsync( parameters );

            if ( ParentValidation != null )
            {
                if ( parameters.TryGetValue<Expression<Func<TValue>>>( nameof( DateExpression ), out var expression ) )
                    await ParentValidation.InitializeInputExpression( expression );

                if ( parameters.TryGetValue<string>( nameof( Pattern ), out var pattern ) )
                {
                    // make sure we get the newest value
                    var value = parameters.TryGetValue<TValue>( nameof( Date ), out var inDate )
                        ? inDate
                        : InternalValue;

                    await ParentValidation.InitializeInputPattern( pattern, value );
                }

                await InitializeValidation();
            }
        }

        /// <inheritdoc/>
        protected override async Task OnFirstAfterRenderAsync()
        {
            await JSRunner.InitializeDatePicker( ElementRef, ElementId, new
            {
                InputMode,
                FirstDayOfWeek,
                DisplayFormat = DateTimeFormatConverter.Convert( DisplayFormat ),
                TimeAs24hr,
                Default = FormatValueAsString( Date ),
                Min = Min?.ToString( DateFormat ),
                Max = Max?.ToString( DateFormat ),
            } );

            await base.OnFirstAfterRenderAsync();
        }

        /// <inheritdoc/>
        protected override async ValueTask DisposeAsync( bool disposing )
        {
            if ( disposing )
            {
                if ( Rendered )
                {
                    var task = JSRunner.DestroyDatePicker( ElementRef, ElementId );

                    try
                    {
                        await task;
                    }
                    catch
                    {
                        if ( !task.IsCanceled )
                        {
                            throw;
                        }
                    }
                }
            }

            await base.DisposeAsync( disposing );
        }

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.DateEdit( Plaintext ) );
            builder.Append( ClassProvider.DateEditSize( Size ), Size != Size.None );
            builder.Append( ClassProvider.DateEditColor( Color ), Color != Color.None );
            builder.Append( ClassProvider.DateEditValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

            base.BuildClasses( builder );
        }

        /// <inheritdoc/>
        protected override Task OnChangeHandler( ChangeEventArgs e )
        {
            return CurrentValueHandler( e?.Value?.ToString() );
        }

        /// <inheritdoc/>
        protected async Task OnClickHandler( MouseEventArgs e )
        {
            if ( Disabled || ReadOnly )
                return;

            await JSRunner.ActivateDatePicker( ElementRef, ElementId, DateFormat );
        }

        /// <inheritdoc/>
        protected override Task OnInternalValueChanged( TValue value )
        {
            return DateChanged.InvokeAsync( value );
        }

        /// <inheritdoc/>
        protected override string FormatValueAsString( TValue value )
        {
            return value switch
            {
                null => null,
                DateTime datetime => datetime.ToString( DateFormat ),
                DateTimeOffset datetimeOffset => datetimeOffset.ToString( DateFormat ),
                _ => throw new InvalidOperationException( $"Unsupported type {value.GetType()}" ),
            };
        }

        /// <inheritdoc/>
        protected override Task<ParseValue<TValue>> ParseValueFromStringAsync( string value )
        {
            if ( Parsers.TryParseDate<TValue>( value, InputMode, out var result ) )
            {
                return Task.FromResult( new ParseValue<TValue>( true, result, null ) );
            }
            else
            {
                return Task.FromResult( new ParseValue<TValue>( false, default, null ) );
            }
        }

        /// <inheritdoc/>
        protected override Task OnKeyPressHandler( KeyboardEventArgs eventArgs )
        {
            // just call eventcallback without using debouncer in BaseTextInput
            return KeyPress.InvokeAsync( eventArgs );
        }

        /// <inheritdoc/>
        protected override Task OnBlurHandler( FocusEventArgs eventArgs )
        {
            // just call eventcallback without using debouncer in BaseTextInput
            return Blur.InvokeAsync( eventArgs );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        protected override bool ShouldAutoGenerateId => true;

        /// <inheritdoc/>
        protected override TValue InternalValue { get => Date; set => Date = value; }

        /// <summary>
        /// Gets the string representation of the input mode.
        /// </summary>
        protected string Mode => InputMode.ToDateInputMode();

        /// <summary>
        /// Gets the date format based on the current <see cref="InputMode"/> settings.
        /// </summary>
        protected string DateFormat => Parsers.GetInternalDateFormat( InputMode );

        /// <summary>
        /// Converts the supplied date format into the internal date format.
        /// </summary>
        [Inject] protected IDateTimeFormatConverter DateTimeFormatConverter { get; set; }

        /// <summary>
        /// Hints at the type of data that might be entered by the user while editing the element or its contents.
        /// </summary>
        [Parameter] public DateInputMode InputMode { get; set; } = DateInputMode.Date;

        /// <summary>
        /// Gets or sets the input date value.
        /// </summary>
        [Parameter] public TValue Date { get; set; }

        /// <summary>
        /// Occurs when the date has changed.
        /// </summary>
        [Parameter] public EventCallback<TValue> DateChanged { get; set; }

        /// <summary>
        /// Gets or sets an expression that identifies the date value.
        /// </summary>
        [Parameter] public Expression<Func<TValue>> DateExpression { get; set; }

        /// <summary>
        /// The earliest date to accept.
        /// </summary>
        [Parameter] public DateTimeOffset? Min { get; set; }

        /// <summary>
        /// The latest date to accept.
        /// </summary>
        [Parameter] public DateTimeOffset? Max { get; set; }

        /// <summary>
        /// Defines the first day of the week.
        /// </summary>
        [Parameter] public DayOfWeek FirstDayOfWeek { get; set; } = DayOfWeek.Sunday;

        /// <summary>
        /// Defines the display format of the date input.
        /// </summary>
        [Parameter] public string DisplayFormat { get; set; }

        /// <summary>
        /// Displays time picker in 24 hour mode without AM/PM selection when enabled.
        /// </summary>
        [Parameter] public bool TimeAs24hr { get; set; }

        #endregion
    }
}
