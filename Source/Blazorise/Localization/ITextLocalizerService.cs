#region Using directives
using System;
using System.Globalization;
#endregion

namespace Blazorise
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
        /// Changes the current thread culture.
        /// </summary>
        /// <param name="cultureName">Predefined culture name.</param>
        void ChangeLanguage( string cultureName );

        /// <summary>
        /// Gets the current culture info.
        /// </summary>
        CultureInfo SelectedCulture { get; }
    }
}
