#region Using directives
using System.Collections.Generic;
using System.Globalization;

#endregion

namespace Blazorise.Localization;

/// <summary>
/// Represents a service that provides localized strings.
/// </summary>
public interface ITextLocalizer
{
    /// <summary>
    /// Gets the string resource with the given name.
    /// </summary>
    /// <param name="name">The name of the string resource.</param>
    /// <returns>The localized string or <paramref name="name"/> if not found.</returns>
    string this[string name] { get; }

    /// <summary>
    /// Gets the string resource with the given name and formatted with the supplied arguments.
    /// </summary>
    /// <param name="name">The name of the string resource.</param>
    /// <param name="arguments">The values to format the string with.</param>
    /// <returns>The formatted string resource <paramref name="name"/> if not found.</returns>
    string this[string name, params object[] arguments] { get; }

    /// <summary>
    /// Gets the string resource with the given name and formatted with the supplied arguments using specified culture.
    /// </summary>
    /// <param name="culture">The culture to use for localization.</param>
    /// <param name="name">The name of the string resource.</param>
    /// <param name="arguments">The values to format the string with.</param>
    /// <returns>The formatted string resource <paramref name="name"/> if not found.</returns>
    string this[CultureInfo culture, string name, params object[] arguments] { get; }
    
    /// <summary>
    /// Adds a custom language resource to the list of supported cultures.
    /// </summary>
    /// <param name="localizationResource">Custom resource model.</param>
    void AddLanguageResource( TextLocalizationResource localizationResource );

    /// <summary>
    /// Gets the localized string by the name with the optional list object for formatting.
    /// </summary>
    /// <param name="name">A name to localize.</param>
    /// <param name="arguments">An object array that contains zero or more objects to format.</param>
    /// <returns>Localized string.</returns>
    string GetString( string name, params object[] arguments );
    
    /// <summary>
    /// Gets the localized string by the name with the optional list object for formatting.
    /// </summary>
    /// <param name="culture">The culture to use for localization.</param>
    /// <param name="name">A name to localize.</param>
    /// <param name="arguments">An object array that contains zero or more objects to format.</param>
    /// <returns>Localized string.</returns>
    string GetString( CultureInfo culture, string name, params object[] arguments );
    
    /// <summary>
    /// Gets the localized string for each key in the localization object.
    /// </summary>
    /// <param name="arguments">An object array that contains zero or more objects to format.</param>
    /// <returns>Localized key/value pairs.</returns>
    IReadOnlyDictionary<string, string> GetStrings( params object[] arguments );
    
    /// <summary>
    /// Gets the localized string for each key in the localization object.
    /// </summary>
    /// <param name="culture">The culture to use for localization.</param>
    /// <param name="arguments">An object array that contains zero or more objects to format.</param>
    /// <returns>Localized key/value pairs.</returns>
    IReadOnlyDictionary<string, string> GetStrings( CultureInfo culture, params object[] arguments );
}

/// <summary>
/// Represents an <see cref="ITextLocalizer"/> that provides strings for <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The <see cref="System.Type"/> to provide strings for.</typeparam>
public interface ITextLocalizer<out T> : ITextLocalizer
{
}