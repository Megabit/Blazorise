#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Declares a panel that groups child report elements.
/// </summary>
public partial class ReportPanel
{
    #region Members

    private readonly ReportPanelContext panelContext = new();

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override ReportElementDefinition BuildDefinition()
    {
        ReportPanelElementDefinition definition = (ReportPanelElementDefinition)base.BuildDefinition();
        panelContext.Definition = definition;

        return definition;
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    protected override ReportElementType ElementType => ReportElementType.Panel;

    /// <summary>
    /// Elements declared inside the panel.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}