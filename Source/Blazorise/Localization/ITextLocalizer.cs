#region Using directives
using Blazorise.Localization;
#endregion

namespace Blazorise.Localization
{
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
        /// Adds a custom language resource to the list of supported cultures.
        /// </summary>
        /// <param name="localizationResource">Custom resource model.</param>
        void AddLanguageResource( TextLocalizationResource localizationResource );
    }

    /// <summary>
    /// Represents an <see cref="ITextLocalizer"/> that provides strings for <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="System.Type"/> to provide strings for.</typeparam>
    public interface ITextLocalizer<out T> : ITextLocalizer
    {
    }
}
