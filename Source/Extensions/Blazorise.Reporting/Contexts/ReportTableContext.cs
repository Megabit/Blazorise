#region Using directives
using System;
#endregion

namespace Blazorise.Reporting;

internal sealed class ReportTableContext
{
    internal void NotifyDefinitionChanged()
    {
        DefinitionChanged?.Invoke();
    }

    internal ReportTableElementDefinition Definition { get; set; }

    internal Action DefinitionChanged { get; set; }
}