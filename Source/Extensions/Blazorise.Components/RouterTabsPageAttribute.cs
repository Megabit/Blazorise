#region Using directives
using System;
#endregion

namespace Blazorise.Components;

/// <summary>
/// An attribute that defines configuration settings for a Router Tabs page.
/// </summary>
public class RouterTabsPageAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RouterTabsPageAttribute"/> class.
    /// </summary>
    /// <param name="Name">
    /// The name of the router tab.  
    /// This can be used as a key for localization.  
    /// If left empty or <c>null</c>, the <see href="https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.components.routedata.pagetype">RouteData.PageType</see>  
    /// will be used as the tab name.
    /// </param>
    /// <param name="TabClass">The CSS class to apply to the router tab.</param>
    /// <param name="TabPanelClass">The CSS class to apply to the router tab panel.</param>
    /// <param name="Closeable">Indicates whether the router tab can be closed. Defaults to <c>true</c>.</param>
    public RouterTabsPageAttribute( string Name, string TabClass = "", string TabPanelClass = "", bool Closeable = true )
    {
        this.Name = Name;
        this.TabClass = TabClass;
        this.TabPanelClass = TabPanelClass;
        this.Closeable = Closeable;
    }

    /// <summary>
    /// Gets the name of the router tab.
    /// </summary>
    /// <remarks>
    /// - This name can be used as a key for localization.
    /// - If left empty or <c>null</c>, the system will use <see href="https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.components.routedata.pagetype">RouteData.PageType</see>.
    /// </remarks>
    public readonly string Name;

    /// <summary>
    /// Gets the CSS class assigned to the router tab.
    /// </summary>
    public readonly string TabClass;

    /// <summary>
    /// Gets the CSS class assigned to the router tab panel.
    /// </summary>
    public readonly string TabPanelClass;

    /// <summary>
    /// Gets a value indicating whether the router tab is closeable.
    /// </summary>
    public readonly bool Closeable;
}