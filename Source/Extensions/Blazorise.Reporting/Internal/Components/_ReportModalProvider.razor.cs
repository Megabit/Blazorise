#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

public partial class _ReportModalProvider
{
    #region Properties

    [Parameter] public string ProviderName { get; set; }

    #endregion
}