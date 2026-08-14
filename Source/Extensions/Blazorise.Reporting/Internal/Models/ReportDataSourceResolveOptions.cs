#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportDataSourceResolveOptions
{
    public object DefaultData { get; set; }

    public IDictionary<string, object> DataSources { get; set; }

    public IDictionary<string, object> Parameters { get; set; }

    public bool LoadData { get; set; }

    public bool RequireData { get; set; }
}