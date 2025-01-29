#region Using directives
using Blazorise.Localization;
#endregion

namespace Blazorise.Components;

/// <summary>
/// Provides configuration options for the <see cref="RouterTabs"/> component.
/// </summary>
public class RouterTabsOptions
{
    /// <summary>
    /// A function used to localize router tab names.
    /// </summary>
    /// <remarks>
    /// This function allows localization of router tab names based on a key.
    /// <para>Behavior:</para>
    /// <list type="bullet">
    ///     <item>
    ///         <description>Accepts a string parameter representing the name key.</description>
    ///     </item>
    ///     <item>
    ///         <description>Returns the localized name as a string.</description>
    ///     </item>
    ///     <item>
    ///         <description>Returns <c>null</c> if no localization is available.</description>
    ///     </item>
    /// </list>
    /// </remarks>
    public TextLocalizerHandler NameLocalizer { get; set; }
}