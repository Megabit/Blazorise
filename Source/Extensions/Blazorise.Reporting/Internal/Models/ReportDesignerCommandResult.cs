namespace Blazorise.Reporting.Internal;

internal sealed class ReportDesignerCommandResult
{
    #region Members

    internal static readonly ReportDesignerCommandResult Empty = new();

    #endregion

    #region Properties

    internal ReportDefinition Definition { get; set; }

    internal bool NotifyDefinitionChanged { get; set; }

    internal bool RefreshSurface { get; set; }

    #endregion
}