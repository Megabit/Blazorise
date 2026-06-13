#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Provides mutable settings for a data source provider editor component.
/// </summary>
public sealed class ReportDataSourceProviderEditorContext
{
    #region Constructors

    /// <summary>
    /// Initializes a new data source provider editor context.
    /// </summary>
    /// <param name="providerType">Provider type being configured.</param>
    /// <param name="settings">Initial provider settings.</param>
    public ReportDataSourceProviderEditorContext( string providerType, IDictionary<string, object> settings = null )
    {
        ProviderType = providerType;
        Settings = settings?.ToDictionary( setting => setting.Key, setting => setting.Value, StringComparer.OrdinalIgnoreCase ) ?? new( StringComparer.OrdinalIgnoreCase );
    }

    #endregion

    #region Methods

    /// <summary>
    /// Reads a string setting value.
    /// </summary>
    /// <param name="key">Provider setting key.</param>
    /// <returns>The setting value converted to string, or null when the setting is not defined.</returns>
    public string GetString( string key )
    {
        return Settings.TryGetValue( key, out object value )
            ? Convert.ToString( value, CultureInfo.InvariantCulture )
            : null;
    }

    /// <summary>
    /// Reads a nullable integer setting value.
    /// </summary>
    /// <param name="key">Provider setting key.</param>
    /// <returns>The setting value converted to nullable integer, or null when the setting is not defined.</returns>
    public int? GetInteger( string key )
    {
        if ( !Settings.TryGetValue( key, out object value ) )
            return null;

        return value switch
        {
            int intValue => intValue,
            string stringValue when int.TryParse( stringValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsedValue ) => parsedValue,
            _ => null,
        };
    }

    /// <summary>
    /// Reads a Boolean setting value.
    /// </summary>
    /// <param name="key">Provider setting key.</param>
    /// <param name="defaultValue">Fallback value returned when the setting is not defined.</param>
    /// <returns>The setting value converted to Boolean.</returns>
    public bool GetBoolean( string key, bool defaultValue = false )
    {
        if ( !Settings.TryGetValue( key, out object value ) )
            return defaultValue;

        return value switch
        {
            bool boolValue => boolValue,
            string stringValue when bool.TryParse( stringValue, out bool parsedValue ) => parsedValue,
            _ => defaultValue,
        };
    }

    /// <summary>
    /// Stores or removes a provider setting value.
    /// </summary>
    /// <param name="key">Provider setting key.</param>
    /// <param name="value">Provider setting value.</param>
    public void SetValue( string key, object value )
    {
        if ( string.IsNullOrWhiteSpace( key ) )
            return;

        if ( value is null )
            Settings.Remove( key );
        else
            Settings[key] = value;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Provider type being configured by the editor.
    /// </summary>
    public string ProviderType { get; }

    /// <summary>
    /// Provider-specific settings edited by the designer.
    /// </summary>
    public Dictionary<string, object> Settings { get; }

    #endregion
}