#region Using directives
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Blazorise.Localization;
#endregion

namespace Blazorise.Localization
{
    /// <summary>
    /// A default implementation of <see cref="ITextLocalizer"/> that reads all localized JSON resources for <typeparamref name="T"/> and cache it.
    /// </summary>
    /// <typeparam name="T">The <see cref="System.Type"/> to provide strings for.</typeparam>
    public class TextLocalizer<T> : ITextLocalizer<T>
    {
        #region Members

        private readonly ITextLocalizerService localizerService;

        private ConcurrentDictionary<string, IReadOnlyDictionary<string, string>> translationsByCulture
            = new ConcurrentDictionary<string, IReadOnlyDictionary<string, string>>();

        #endregion

        #region Constructors

        public TextLocalizer( ITextLocalizerService localizerService )
        {
            this.localizerService = localizerService;

            ReadResources( typeof( T ) );
        }

        #endregion

        #region Methods

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

        public void AddLanguageResource( TextLocalizationResource localizationResource )
        {
            translationsByCulture.TryAdd( localizationResource.Culture, localizationResource.Translations );
        }

        protected virtual string GetResourceName( Type resourceType )
        {
            if ( resourceType.IsGenericType )
                return resourceType.Name.Substring( 0, resourceType.Name.IndexOf( '`' ) );

            return resourceType.Name;
        }

        protected virtual TextLocalizationResource DeserializeResourceAsJson( Assembly assembly, string resourceName )
        {
            return JsonSerializer.Deserialize<TextLocalizationResource>( ReadResourceAsString( assembly, resourceName ), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            } );
        }

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

        protected virtual string[] GetLocalizationResourceNames( Assembly assembly, string resourceName )
        {
            return assembly.GetManifestResourceNames()
                .Where( r => r.Contains( $"Resources.Localization.{resourceName}" ) && r.EndsWith( ".json" ) )
                .ToArray();
        }

        protected virtual IReadOnlyDictionary<string, string> GetTranslations()
        {
            if ( translationsByCulture.TryGetValue( localizerService.SelectedCulture.Name, out var st ) )
                return st;
            else if ( translationsByCulture.TryGetValue( CultureInfo.CurrentUICulture.Name, out var it ) )
                return it;
            else if ( translationsByCulture.TryGetValue( "en-US", out var dt ) )
                return dt;

            return null;
        }

        protected virtual string GetString( string name, params object[] arguments )
        {
            var translations = GetTranslations();

            if ( translations == null || !translations.TryGetValue( name, out var value ) )
                value = name;

            if ( arguments.Length > 0 )
                value = string.Format( localizerService.SelectedCulture, value, arguments );

            return value;
        }

        #endregion

        #region Properties

        public string this[string name] => GetString( name );

        public string this[string name, params object[] arguments] => GetString( name, arguments );

        #endregion
    }
}
