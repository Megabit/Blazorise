#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders the report designer right-side tabbed panel.
/// </summary>
public partial class _ReportDesignerPanel
{
    private const string ExplorerTab = "Explorer";

    private const string PropertiesTab = "Properties";

    private Color GetTabColor( string tab )
    {
        return string.Equals( SelectedTab, tab, StringComparison.Ordinal ) ? Color.Primary : Color.Light;
    }

    private Task SelectTab( string tab )
    {
        return SelectedTabChanged.InvokeAsync( tab );
    }

    /// <summary>
    /// Name of the currently selected designer panel tab.
    /// </summary>
    [Parameter] public string SelectedTab { get; set; }

    /// <summary>
    /// Raised when the selected designer panel tab changes.
    /// </summary>
    [Parameter] public EventCallback<string> SelectedTabChanged { get; set; }

    /// <summary>
    /// Content shown by the properties tab.
    /// </summary>
    [Parameter] public RenderFragment Properties { get; set; }

    /// <summary>
    /// Content shown by the report explorer tab.
    /// </summary>
    [Parameter] public RenderFragment Explorer { get; set; }
}