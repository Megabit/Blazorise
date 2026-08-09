#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Reporting;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed record ReportDesignerCommand(
    string Name,
    Func<Task> Execute,
    Func<ReportDefinition> GetDefinition = null,
    bool TrackHistory = true,
    bool NotifyDefinitionChanged = true,
    bool RefreshSurface = true,
    ReportDesignerRefreshTarget RefreshTargets = ReportDesignerRefreshTarget.Designer );