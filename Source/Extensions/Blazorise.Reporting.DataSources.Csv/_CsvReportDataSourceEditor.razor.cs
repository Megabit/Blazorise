#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Reporting;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.DataSources.Csv;

/// <summary>
/// Designer editor for CSV report data source settings.
/// </summary>
public partial class _CsvReportDataSourceEditor
{
    #region Members

    private const string DefaultEncodingValue = "";

    private const string DefaultDelimiterValue = "";

    private const string CustomDelimiterValue = "other";

    private const string TabDelimiterValue = "\t";

    private const string SemicolonDelimiterValue = ";";

    private const string CommaDelimiterValue = ",";

    private const string SpaceDelimiterValue = " ";

    private static readonly CsvReportDataSourceEditorOption[] encodingOptions =
    [
        new( DefaultEncodingValue, "Default" ),
        new( CsvReportDataSourceSettings.SystemEncoding, "System" ),
        new( "utf-7", "UTF7" ),
        new( "utf-8", "UTF8" ),
        new( "unicode", "Unicode" ),
        new( "windows-1250", "1250" ),
        new( "windows-1251", "1251" ),
        new( "windows-1252", "1252" ),
        new( "windows-1253", "1253" ),
        new( "windows-1254", "1254" ),
        new( "windows-1255", "1255" ),
        new( "windows-1256", "1256" ),
    ];

    private static readonly CsvReportDataSourceEditorOption[] delimiterOptions =
    [
        new( DefaultDelimiterValue, "Default" ),
        new( TabDelimiterValue, "Tab" ),
        new( SemicolonDelimiterValue, "Semicolon" ),
        new( CommaDelimiterValue, "Comma" ),
        new( SpaceDelimiterValue, "Space" ),
        new( CustomDelimiterValue, "Other" ),
    ];

    private ReportDataSourceProviderEditorContext currentContext;

    private string selectedDelimiter;

    #endregion

    #region Methods

    /// <inheritdoc />
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.TryGetValue( nameof( Context ), out ReportDataSourceProviderEditorContext context )
            && !ReferenceEquals( currentContext, context ) )
        {
            currentContext = context;
            selectedDelimiter = null;
        }

        return base.SetParametersAsync( parameters );
    }

    private Task OnSourceChanged( string value )
    {
        Context?.SetValue( CsvReportDataSourceSettings.Source, value );

        return Task.CompletedTask;
    }

    private Task OnEncodingChanged( string value )
    {
        Context?.SetValue( CsvReportDataSourceSettings.Encoding, string.IsNullOrWhiteSpace( value ) ? null : value );

        return Task.CompletedTask;
    }

    private Task OnDelimiterChanged( string value )
    {
        selectedDelimiter = value;

        if ( value == CustomDelimiterValue )
        {
            string delimiter = Delimiter;

            Context?.SetValue( CsvReportDataSourceSettings.Delimiter, string.IsNullOrEmpty( delimiter ) || IsKnownDelimiter( delimiter ) ? null : delimiter );
        }
        else
        {
            Context?.SetValue( CsvReportDataSourceSettings.Delimiter, string.IsNullOrEmpty( value ) ? null : value );
        }

        return Task.CompletedTask;
    }

    private Task OnCustomDelimiterChanged( string value )
    {
        selectedDelimiter = CustomDelimiterValue;
        Context?.SetValue( CsvReportDataSourceSettings.Delimiter, string.IsNullOrEmpty( value ) ? null : value[..1] );

        return Task.CompletedTask;
    }

    private Task OnHasHeaderRowChanged( bool value )
    {
        Context?.SetValue( CsvReportDataSourceSettings.HasHeaderRow, value );

        return Task.CompletedTask;
    }

    private static bool IsKnownDelimiter( string value )
    {
        return value == TabDelimiterValue
            || value == SemicolonDelimiterValue
            || value == CommaDelimiterValue
            || value == SpaceDelimiterValue;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Provider settings context edited by the CSV data source editor.
    /// </summary>
    [Parameter] public ReportDataSourceProviderEditorContext Context { get; set; }

    private static IReadOnlyList<CsvReportDataSourceEditorOption> EncodingOptions => encodingOptions;

    private static IReadOnlyList<CsvReportDataSourceEditorOption> DelimiterOptions => delimiterOptions;

    private string Source => Context?.GetString( CsvReportDataSourceSettings.Source );

    private string SelectedEncoding
    {
        get
        {
            string encoding = Context?.GetString( CsvReportDataSourceSettings.Encoding );

            return string.IsNullOrWhiteSpace( encoding ) ? DefaultEncodingValue : encoding;
        }
    }

    private string SelectedDelimiter
    {
        get
        {
            if ( !string.IsNullOrEmpty( selectedDelimiter ) )
                return selectedDelimiter;

            string delimiter = Delimiter;

            if ( string.IsNullOrEmpty( delimiter ) )
                return DefaultDelimiterValue;

            return IsKnownDelimiter( delimiter ) ? delimiter : CustomDelimiterValue;
        }
    }

    private string Delimiter => Context?.GetString( CsvReportDataSourceSettings.Delimiter );

    private string CustomDelimiter
    {
        get
        {
            string delimiter = Delimiter;

            if ( selectedDelimiter == CustomDelimiterValue )
                return delimiter ?? string.Empty;

            return string.IsNullOrEmpty( delimiter ) || IsKnownDelimiter( delimiter ) ? string.Empty : delimiter;
        }
    }

    private bool IsCustomDelimiter => SelectedDelimiter == CustomDelimiterValue;

    private bool HasHeaderRow => Context?.GetBoolean( CsvReportDataSourceSettings.HasHeaderRow, true ) ?? true;

    #endregion
}