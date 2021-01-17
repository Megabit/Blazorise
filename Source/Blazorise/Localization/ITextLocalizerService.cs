#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
#endregion

namespace Blazorise.Localization
{
    /// <summary>
    /// Provides the way to change the current culture and notify the app of the change.
    /// </summary>
    public interface ITextLocalizerService
    {
        /// <summary>
        /// Occurs after the culture has changed.
        /// </summary>
        event EventHandler LocalizationChanged;


        /// <summary>
        /// Add's a new culture name to the list of supported cultures.
        /// </summary>
        /// <param name="cultureName">Culture name.</param>
        void AddLanguageResource( string cultureName );

        /// <summary>
        /// Changes the current thread culture.
        /// </summary>
        /// <param name="cultureName">Culture name to be set by the current thread.</param>
        /// <param name="changeThreadCulture">If true, the <see cref="CultureInfo.DefaultThreadCurrentCulture"/> and <see cref="CultureInfo.CurrentCulture"/> will also be updated.</param>
        void ChangeLanguage( string cultureName, bool changeThreadCulture = true );

        /// <summary>
        /// Gets the current culture info.
        /// </summary>
        CultureInfo SelectedCulture { get; }

        /// <summary>
        /// Gets the list of all available cultures supported by the Blazorise.
        /// </summary>
        IEnumerable<CultureInfo> AvailableCultures { get; }
    }
}
