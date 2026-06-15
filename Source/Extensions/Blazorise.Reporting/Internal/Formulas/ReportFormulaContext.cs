#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportFormulaContext
{
    #region Properties

    internal ReportDefinition Definition { get; set; }

    internal object Data { get; set; }

    internal object Item { get; set; }

    internal ReportSectionDefinition Section { get; set; }

    internal ReportElementDefinition Element { get; set; }

    internal IReadOnlyDictionary<string, object> Parameters { get; set; }

    #endregion
}