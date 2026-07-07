#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Internal dialog used to edit user-friendly report value formats.
/// </summary>
public partial class _ReportDesignerFormatDialog
{
    #region Members

    private ReportFormatDefinition format = ReportFormats.Text();

    private static readonly (ReportFormatCategory Value, string Text)[] CategoryOptions =
    [
        ( ReportFormatCategory.Text, "Text" ),
        ( ReportFormatCategory.Number, "Number" ),
        ( ReportFormatCategory.Currency, "Currency" ),
        ( ReportFormatCategory.Percent, "Percent" ),
        ( ReportFormatCategory.Date, "Date" ),
        ( ReportFormatCategory.Time, "Time" ),
        ( ReportFormatCategory.DateTime, "Date and time" ),
        ( ReportFormatCategory.Boolean, "Boolean" ),
        ( ReportFormatCategory.Custom, "Custom" ),
    ];

    private static readonly (ReportNegativeNumberFormat Value, string Text)[] NegativeNumberFormatOptions =
    [
        ( ReportNegativeNumberFormat.Default, "Default" ),
        ( ReportNegativeNumberFormat.MinusSign, "Minus sign" ),
        ( ReportNegativeNumberFormat.Parentheses, "Parentheses" ),
    ];

    private static readonly (ReportDateFormat Value, string Text)[] DateFormatOptions =
    [
        ( ReportDateFormat.ShortDate, "Short date" ),
        ( ReportDateFormat.LongDate, "Long date" ),
        ( ReportDateFormat.ShortTime, "Short time" ),
        ( ReportDateFormat.LongTime, "Long time" ),
        ( ReportDateFormat.ShortDateTime, "Short date and time" ),
        ( ReportDateFormat.LongDateTime, "Long date and time" ),
    ];

    #endregion

    #region Methods

    internal async Task Show( ReportFormatDefinition currentFormat )
    {
        await ShowReportModal<_ReportDesignerFormatDialog>( parameters =>
        {
            parameters.Add( nameof( InitialFormat ), currentFormat );
            parameters.Add( nameof( Confirmed ), Confirmed );
        } );
    }

    protected override void OnParametersSet()
    {
        format = ReportFormats.Clone( InitialFormat ) ?? ReportFormats.Text();
    }

    private Task Close()
    {
        return CloseReportModal();
    }

    private async Task Confirm()
    {
        await Confirmed.InvokeAsync( ReportFormats.Clone( format ) ?? ReportFormats.Text() );
        await CloseReportModal();
    }

    private Task OnCategoryChanged( ReportFormatCategory value )
    {
        format = CreateFormat( value, format );

        return Task.CompletedTask;
    }

    private Task OnDecimalPlacesChanged( int? value )
    {
        if ( NumericFormat is not null )
            NumericFormat.DecimalPlaces = value;

        return Task.CompletedTask;
    }

    private Task OnUseThousandsSeparatorChanged( bool value )
    {
        if ( NumberFormat is not null )
            NumberFormat.UseThousandsSeparator = value;

        return Task.CompletedTask;
    }

    private Task OnCurrencySymbolChanged( string value )
    {
        if ( CurrencyFormat is not null )
            CurrencyFormat.CurrencySymbol = value;

        return Task.CompletedTask;
    }

    private Task OnNegativeNumberFormatChanged( ReportNegativeNumberFormat value )
    {
        if ( NumericFormat is not null )
            NumericFormat.NegativeNumberFormat = value;

        return Task.CompletedTask;
    }

    private Task OnDateFormatChanged( ReportDateFormat value )
    {
        if ( TemporalFormat is not null )
            TemporalFormat.DateFormat = value;

        return Task.CompletedTask;
    }

    private Task OnTrueTextChanged( string value )
    {
        if ( BooleanFormat is not null )
            BooleanFormat.TrueText = value;

        return Task.CompletedTask;
    }

    private Task OnFalseTextChanged( string value )
    {
        if ( BooleanFormat is not null )
            BooleanFormat.FalseText = value;

        return Task.CompletedTask;
    }

    private Task OnCustomFormatChanged( string value )
    {
        if ( CustomFormat is not null )
            CustomFormat.Format = value;

        return Task.CompletedTask;
    }

    private Task OnCultureNameChanged( string value )
    {
        format.CultureName = value;

        return Task.CompletedTask;
    }

    private static ReportFormatDefinition CreateFormat( ReportFormatCategory category, ReportFormatDefinition currentFormat )
    {
        ReportFormatDefinition nextFormat = category switch
        {
            ReportFormatCategory.Number => ReportFormats.Number(),
            ReportFormatCategory.Currency => ReportFormats.Currency(),
            ReportFormatCategory.Percent => ReportFormats.Percent(),
            ReportFormatCategory.Date => ReportFormats.Date(),
            ReportFormatCategory.Time => ReportFormats.Time(),
            ReportFormatCategory.DateTime => ReportFormats.DateTime(),
            ReportFormatCategory.Boolean => ReportFormats.Boolean(),
            ReportFormatCategory.Custom => ReportFormats.Custom( null ),
            _ => ReportFormats.Text(),
        };

        nextFormat.CultureName = currentFormat?.CultureName;

        if ( currentFormat is ReportNumericFormatDefinition currentNumericFormat && nextFormat is ReportNumericFormatDefinition nextNumericFormat )
        {
            nextNumericFormat.DecimalPlaces = currentNumericFormat.DecimalPlaces;
            nextNumericFormat.NegativeNumberFormat = currentNumericFormat.NegativeNumberFormat;
        }

        return nextFormat;
    }

    #endregion

    #region Properties

    private bool IsNumericCategory => NumericFormat is not null;

    private ReportFormatCategory CurrentCategory => format?.Category ?? ReportFormatCategory.Text;

    private ReportNumericFormatDefinition NumericFormat => format as ReportNumericFormatDefinition;

    private ReportNumberFormatDefinition NumberFormat => format as ReportNumberFormatDefinition;

    private ReportCurrencyFormatDefinition CurrencyFormat => format as ReportCurrencyFormatDefinition;

    private ReportTemporalFormatDefinition TemporalFormat => format as ReportTemporalFormatDefinition;

    private ReportBooleanFormatDefinition BooleanFormat => format as ReportBooleanFormatDefinition;

    private ReportCustomFormatDefinition CustomFormat => format as ReportCustomFormatDefinition;

    private string PreviewText => ReportFormatResolver.GetPreviewText( format );

    [Parameter] public ReportFormatDefinition InitialFormat { get; set; }

    [Parameter] public EventCallback<ReportFormatDefinition> Confirmed { get; set; }

    #endregion
}