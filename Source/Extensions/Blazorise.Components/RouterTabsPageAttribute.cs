using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorise.Components;
/// <summary>
/// Attribute that sets the Router Tabs Page attributes
/// </summary>
public class RouterTabsPageAttribute : Attribute
{
    /// <summary>
    /// Sets the title of the router tab.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Sets the css class of the router tab.
    /// </summary>
    public string TabCssClass { get; set; }

    /// <summary>
    /// Sets the css class of the router tab panel.
    /// </summary>
    public string TabPanelCssClass { get; set; }

    /// <summary>
    /// Whether the router tab is closeable.
    /// </summary>
    public bool Closeable { get; set; } = true;

}