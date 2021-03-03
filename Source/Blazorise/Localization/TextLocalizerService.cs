#region Using directives
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
#endregion

namespace Blazorise.Localization
{
    public class TextLocalizerService : ITextLocalizerService
    {
        #region Members

        public event EventHandler LocalizationChanged;

        private readonly ConcurrentDictionary<string, CultureInfo> availableCultures = new ConcurrentDictionary<string, CultureInfo>();

        #endregion

        #region Constructors

        public TextLocalizerService()
        {
            ReadResource();
        }

        #endregion

        #region Methods

        public void ReadResource()
        {
            var assembly = typeof( ITextLocalizerService ).Assembly;

            var cultureNames =
                ( from localizationResourceName in GetLocalizationResourceNames( assembly )
                  let path = Path.GetFileNameWithoutExtension( localizationResourceName )
                  let file = path.Substring( path.LastIndexOf( '.' ) + 1 )
                  select file ).Distinct().ToList();

            foreach ( var cultureName in cultureNames )
            {
                AddLanguageResource( cultureName );
            }
        }

        public void AddLanguageResource( string cultureName )
        {
            if ( !availableCultures.ContainsKey( cultureName ) )
            {
                availableCultures.TryAdd( cultureName, new CultureInfo( cultureName ) );
            }
        }

        protected virtual string[] GetLocalizationResourceNames( Assembly assembly )
        {
            return assembly.GetManifestResourceNames()
                .Where( r => r.Contains( $"Resources.Localization" ) && r.EndsWith( ".json" ) )
                .ToArray();
        }

        public void ChangeLanguage( string cultureName, bool changeThreadCulture = true )
        {
            if ( string.IsNullOrEmpty( cultureName ) )
                throw new ArgumentNullException( nameof( cultureName ) );

            if ( cultureName == SelectedCulture?.Name )
                return;

            SelectedCulture = new CultureInfo( cultureName );

            if ( changeThreadCulture )
            {
                CultureInfo.DefaultThreadCurrentCulture = SelectedCulture;
                CultureInfo.DefaultThreadCurrentUICulture = SelectedCulture;

                CultureInfo.CurrentCulture = SelectedCulture;
                CultureInfo.CurrentUICulture = SelectedCulture;
            }

            LocalizationChanged?.Invoke( this, EventArgs.Empty );
        }

        #endregion

        #region Properties

        public CultureInfo SelectedCulture { get; private set; } = CultureInfo.DefaultThreadCurrentUICulture ?? CultureInfo.CurrentUICulture;

        public IEnumerable<CultureInfo> AvailableCultures => availableCultures.Values;

        #endregion
    }
}
