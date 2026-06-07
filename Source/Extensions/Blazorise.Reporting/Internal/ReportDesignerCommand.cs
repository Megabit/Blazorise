using System;
using System.Threading.Tasks;
using Blazorise.Reporting;

namespace Blazorise.Reporting.Internal;

internal sealed class ReportDesignerCommand
{
    internal ReportDesignerCommand( string name, Func<Task> execute, Func<ReportDefinition> getDefinition = null, bool trackHistory = true, bool notifyDefinitionChanged = true )
    {
        Name = name;
        Execute = execute;
        GetDefinition = getDefinition;
        TrackHistory = trackHistory;
        NotifyDefinitionChanged = notifyDefinitionChanged;
    }

    internal string Name { get; }

    internal Func<Task> Execute { get; }

    internal Func<ReportDefinition> GetDefinition { get; }

    internal bool TrackHistory { get; }

    internal bool NotifyDefinitionChanged { get; }
}