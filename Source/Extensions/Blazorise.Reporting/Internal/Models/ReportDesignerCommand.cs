#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Reporting;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportDesignerCommand
{
    internal ReportDesignerCommand( string name, Func<Task> execute, Func<ReportDefinition> getDefinition = null, bool trackHistory = true, bool notifyDefinitionChanged = true, bool refreshSurface = true )
    {
        Name = name;
        Execute = execute;
        GetDefinition = getDefinition;
        TrackHistory = trackHistory;
        NotifyDefinitionChanged = notifyDefinitionChanged;
        RefreshSurface = refreshSurface;
    }

    internal string Name { get; }

    internal Func<Task> Execute { get; }

    internal Func<ReportDefinition> GetDefinition { get; }

    internal bool TrackHistory { get; }

    internal bool NotifyDefinitionChanged { get; }

    internal bool RefreshSurface { get; }
}