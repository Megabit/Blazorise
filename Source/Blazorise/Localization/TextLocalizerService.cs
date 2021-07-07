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
    /// <summary>
    /// A default implementation of <see cref="ITextLocalizerService"/>.
    /// </summary>
    public class TextLocalizerService : ITextLocalizerService
    {
        #region Members

        /// <summary>
        /// An event that is raised after localization has changed.
        /// </summary>
        public event EventHandler LocalizationChanged;

        private readonly ConcurrentDictionary<string, CultureInfo> availableCultures = new();

        #endregion

        #region Constructors

        /// <summary>
        /// A default constructor for <see cref="TextLocalizerService"/>.
        /// </summary>
        public TextLocalizerService()
        {
            ReadResource();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reads all resources in the current assembly.
        /// </summary>
        public void ReadResource()
        {
            var assembly = typeof( ITextLocalizerService ).Assembly;

            var cultureNames =
                ( from localizationResourceName in GetLocalizationResourceNames( assembly )
                  let path = Path.GetFileNameWithoutExtension( localizationResourceName )
                  let file = path[( path.LastIndexOf( '.' ) + 1 )..]
                  select file ).Distinct().ToList();

            foreach ( var cultureName in cultureNames )
            {
                AddLanguageResource( cultureName );
            }
        }

        /// <inheritdoc/>
        public void AddLanguageResource( string cultureName )
        {
            if ( !availableCultures.ContainsKey( cultureName ) )
            {
                availableCultures.TryAdd( cultureName, new( cultureName ) );
            }
        }

        /// <summary>
        /// Gets the list of all resources names in the given assembly.
        /// </summary>
        /// <param name="assembly">Assembly that contains the resources.</param>
        /// <returns>List of resource names.</returns>
        protected virtual string[] GetLocalizationResourceNames( Assembly assembly )
        {
            return assembly.GetManifestResourceNames()
                .Where( r => r.Contains( "Resources.Localization" ) && r.EndsWith( ".json" ) )
                .ToArray();
        }

        /// <inheritdoc/>
        public void ChangeLanguage( string cultureName, bool changeThreadCulture = true )
        {
            if ( string.IsNullOrEmpty( cultureName ) )
                throw new ArgumentNullException( nameof( cultureName ) );

            if ( cultureName == SelectedCulture?.Name )
                return;

            SelectedCulture = new( cultureName );

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

        /// <inheritdoc/>
        public CultureInfo SelectedCulture { get; private set; } = CultureInfo.DefaultThreadCurrentUICulture ?? CultureInfo.CurrentUICulture;

        /// <inheritdoc/>
        public IEnumerable<CultureInfo> AvailableCultures => availableCultures.Values;

        #endregion
    }
}
