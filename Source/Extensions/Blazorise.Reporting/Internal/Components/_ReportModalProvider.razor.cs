#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Supplies the modal provider used by report dialogs.
/// </summary>
public partial class _ReportModalProvider
{
    #region Properties

    /// <summary>
    /// Name assigned to the report modal provider.
    /// </summary>
    [Parameter] public string ProviderName { get; set; }

    #endregion
}