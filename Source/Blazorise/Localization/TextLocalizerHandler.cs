namespace Blazorise.Localization
{
    /// <summary>
    /// Delegate used to handle custom localization that will override a default <see cref="ITextLocalizer"/>.
    /// </summary>
    /// <param name="name">Localization key.</param>
    /// <param name="arguments">Arguments used to format the localized text.</param>
    /// <returns>Returns the localized text.</returns>
    public delegate string TextLocalizerHandler( string name, params object[] arguments );
}
