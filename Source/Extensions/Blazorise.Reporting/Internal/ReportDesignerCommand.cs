using System;
using System.Threading.Tasks;
using Blazorise.Reporting;

namespace Blazorise.Reporting.Internal;

internal sealed class ReportDesignerCommand
{
    public ReportDesignerCommand( string name, Func<Task> execute, Func<ReportDefinition> getDefinition = null, bool trackHistory = true, bool notifyDefinitionChanged = true )
    {
        Name = name;
        Execute = execute;
        GetDefinition = getDefinition;
        TrackHistory = trackHistory;
        NotifyDefinitionChanged = notifyDefinitionChanged;
    }

    public string Name { get; }

    public Func<Task> Execute { get; }

    public Func<ReportDefinition> GetDefinition { get; }

    public bool TrackHistory { get; }

    public bool NotifyDefinitionChanged { get; }
}