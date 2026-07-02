#region Using directives
using System;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Represents one editable provider setting row in the data source connection dialog.
/// </summary>
internal sealed class ReportDataSourceSettingItem
{
    #region Properties

    /// <summary>
    /// Stable identifier used by the setting row UI.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString( "N" );

    /// <summary>
    /// Provider setting key.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Provider setting value stored as text.
    /// </summary>
    public string Value { get; set; }

    #endregion
}