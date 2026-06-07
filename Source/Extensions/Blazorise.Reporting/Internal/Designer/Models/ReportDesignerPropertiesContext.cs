#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportDesignerPropertiesContext
{
    #region Properties

    internal object EventReceiver { get; init; }

    internal ReportDefinition Definition { get; init; }

    internal ReportSelectionManager SelectionManager { get; init; }

    internal bool SnapToGrid { get; init; }

    internal Action<ChangeEventArgs> SnapToGridChanged { get; init; }

    internal Func<Action<ReportPageDefinition>, Task> UpdateReportPage { get; init; }

    internal Func<Action<ReportSectionDefinition>, Task> UpdateSelectedSection { get; init; }

    internal Func<bool, Task> UpdateSelectedSectionSuppression { get; init; }

    internal Func<bool, Task> InsertSection { get; init; }

    internal Func<Task> DeleteSelectedSection { get; init; }

    internal Func<Action<ReportElementDefinition>, Task> UpdateSelectedElement { get; init; }

    internal Func<double, double, double, double, Task> MoveSelectedElement { get; init; }

    #endregion
}