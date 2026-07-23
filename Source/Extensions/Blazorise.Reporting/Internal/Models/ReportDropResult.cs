#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportDropResult
{
    #region Properties

    public string CommandName { get; set; }

    public string PrimaryElementKey { get; set; }

    public IReadOnlyList<string> SelectedElementKeys { get; set; } = [];

    public string SelectedCellKey { get; set; }

    #endregion
}