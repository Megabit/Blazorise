#region Using directives
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
#endregion

namespace Blazorise.Localization;

/// <summary>
/// A default implementation of <see cref="ITextLocalizer"/> that reads all localized JSON resources for <typeparamref name="T"/> and cache it.
/// </summary>
/// <typeparam name="T">The <see cref="System.Type"/> to provide strings for.</typeparam>
public class TextLocalizer<T> : ITextLocalizer<T>
{
    #region Members

    private readonly ITextLocalizerService localizerService;

    private readonly ConcurrentDictionary<string, IReadOnlyDictionary<string, string>> translationsByCulture = new();

    #endregion

    #region Constructors

    /// <summary>
    /// A default constructor for <see cref="TextLocalizer{T}"/>.
    /// </summary>
    /// <param name="localizerService">Service that is responsible for triggering the localization change.</param>
    public TextLocalizer( ITextLocalizerService localizerService )
    {
        this.localizerService = localizerService;

        ReadResources( typeof( T ) );
    }

    #endregion

    #region Methods

    /// <summary>
    /// Reads the localization resource for the given type.
    /// </summary>
    /// <param name="resourceType">A resource type.</param>
    public void ReadResources( Type resourceType )
    {
        var assembly = resourceType.Assembly;

        var resourceName = GetResourceName( resourceType );

        var localizationResourceNames = GetLocalizationResourceNames( assembly, resourceName );

        var localizationResources = localizationResourceNames.Select( resourceName => DeserializeResourceAsJson( assembly, resourceName ) ).ToList();

        foreach ( var localizationResource in localizationResources )
        {
            AddLanguageResource( localizationResource );
        }
    }

    /// <summary>
    /// Adds a language resource to the list.
    /// </summary>
    /// <param name="localizationResource">Localization resource to add.</param>
    public void AddLanguageResource( TextLocalizationResource localizationResource )
    {
        translationsByCulture.TryAdd( localizationResource.Culture, localizationResource.Translations );

        localizerService.AddLanguageResource( localizationResource.Culture );
    }

    /// <summary>
    /// Gets the resource name by the resource type.
    /// </summary>
    /// <param name="resourceType">A resource type.</param>
    /// <returns>A resource name.</returns>
    protected virtual string GetResourceName( Type resourceType )
    {
        if ( resourceType.IsGenericType )
            return resourceType.Name.Substring( 0, resourceType.Name.IndexOf( '`' ) );

        return resourceType.Name;
    }

    /// <summary>
    /// Deserializes a resource from the given assembly as the json string.
    /// </summary>
    /// <param name="assembly">Assembly that contains the resource.</param>
    /// <param name="resourceName">A resource name.</param>
    /// <returns>A deserialized resource object.</returns>
    protected virtual TextLocalizationResource DeserializeResourceAsJson( Assembly assembly, string resourceName )
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        return JsonSerializer.Deserialize<TextLocalizationResource>( ReadResourceAsString( assembly, resourceName ), options );
    }

    /// <summary>
    /// Reads the resource from the given assembly and returns it as a string.
    /// </summary>
    /// <param name="assembly">Assembly that contains the resource.</param>
    /// <param name="resourceName">A resource name.</param>
    /// <returns>A resource content as a string.</returns>
    protected virtual string ReadResourceAsString( Assembly assembly, string resourceName )
    {
        using ( var stream = assembly.GetManifestResourceStream( resourceName ) )
        {
            using ( var reader = new StreamReader( stream ) )
            {
                return reader.ReadToEnd();
            }
        }
    }

    /// <summary>
    /// Search for all localization resource names in a given assembly.
    /// </summary>
    /// <param name="assembly">Assembly that contains the resource.</param>
    /// <param name="resourceName">Part of a resource name to search.</param>
    /// <returns>List of resource names.</returns>
    protected virtual string[] GetLocalizationResourceNames( Assembly assembly, string resourceName )
    {
        return assembly.GetManifestResourceNames()
            .Where( r => r.Contains( $"Resources.Localization.{resourceName}." ) && r.EndsWith( ".json" ) )
            .ToArray();
    }

    /// <summary>
    /// Gets all of the translations.
    /// </summary>
    /// <returns>Key/value pairs of translations.</returns>
    protected virtual IReadOnlyDictionary<string, string> GetTranslations()
    {
        // The selected culture can either be a neutral culture (2-digit:"cn") or a specific culture
        // (5-digit:"en-UK").
        // Both is possible depending on application, user or browser settings.
        // The search order is
        // 1. localizerService.SelectedCulture: exact match (e.g. "de-AT" if available)
        // 2. localizerService.SelectedCulture: match of parent neutral culture (e.g. "de-AT" will fallback to
        //    "de" if "de-AT not available)
        // 3. Current thread ui culture: exact match (e.g. "es-CR" if available)
        // 4. Current thread ui culture: match of parent neutral culture (e.g. "es-CR" will fallback to "es")
        // 5. Invariant culture (defaults to "en")
        IReadOnlyDictionary<string, string> result;

        if ( localizerService.SelectedCulture is not null
             && translationsByCulture.TryGetValue( localizerService.SelectedCulture.Name, out result ) )
            return result;

        if ( localizerService.SelectedCulture?.Parent is not null && !localizerService.SelectedCulture.IsNeutralCulture
                                                              && translationsByCulture.TryGetValue( localizerService.SelectedCulture.Parent.Name, out result ) )
            return result;

        if ( CultureInfo.CurrentUICulture is not null
             && translationsByCulture.TryGetValue( CultureInfo.CurrentUICulture.Name, out result ) )
            return result;

        if ( CultureInfo.CurrentUICulture?.Parent is not null && !CultureInfo.CurrentUICulture.IsNeutralCulture
                                                          && translationsByCulture.TryGetValue( CultureInfo.CurrentUICulture.Parent.Name, out result ) )
            return result;

        if ( translationsByCulture.TryGetValue( "en", out result ) )
            return result;

        return null;
    }

    /// <inheritdoc/>
    public virtual string GetString( string name, params object[] arguments )
    {
        var translations = GetTranslations();

        if ( translations is null || !translations.TryGetValue( name, out var value ) )
            value = name;

        if ( arguments.Length > 0 )
            value = string.Format( localizerService.SelectedCulture, value, arguments );

        return value;
    }

    /// <inheritdoc/>
    public virtual IReadOnlyDictionary<string, string> GetStrings( params object[] arguments )
    {
        var translations = GetTranslations();

        return ( from t in translations
                 select new
                 {
                     t.Key,
                     Value = arguments.Length > 0
                         ? string.Format( localizerService.SelectedCulture, t.Value, arguments )
                         : t.Value
                 } ).ToDictionary( x => x.Key, x => x.Value );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public string this[string name] => GetString( name );

    /// <inheritdoc/>
    public string this[string name, params object[] arguments] => GetString( name, arguments );

    #endregion
}