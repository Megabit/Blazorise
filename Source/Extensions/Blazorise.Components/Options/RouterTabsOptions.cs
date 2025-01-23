using System;

namespace Blazorise.Components;

/// <summary>
/// Options for RouterTabs component
/// </summary>
public class RouterTabsOptions
{
    /// <summary>
    /// Func used for localizing router tabs names.
    /// In: Name key, Out: Localized name.
    /// Return null if localization isn't provided.
    /// </summary>
    public Func<string, string> NamesLocalizer { get; set; }
}