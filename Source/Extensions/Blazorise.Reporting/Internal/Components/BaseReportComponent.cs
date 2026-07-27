#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Provides common infrastructure for Reporting internal Razor components.
/// </summary>
public abstract class BaseReportComponent : BaseStyledComponent
{
    #region Methods

    /// <summary>
    /// Closes the report modal instance that owns this component.
    /// </summary>
    protected Task CloseReportModal()
    {
        return ModalService?.Hide( ResolvedReportModalProviderName ) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Shows a report-owned modal component.
    /// </summary>
    protected Task<ModalInstance> ShowReportModal<TComponent>( Action<ModalProviderParameterBuilder<TComponent>> parameters = null, ModalInstanceOptions options = null )
        where TComponent : BaseReportComponent
    {
        options ??= CreateReportModalOptions();
        string providerName = ResolvedReportModalProviderName;
        options.ProviderName = providerName;

        return ModalService.Show<TComponent>( string.Empty, builder =>
        {
            builder.Add( component => component.ReportModalProviderName, providerName );
            parameters?.Invoke( builder );
        }, options );
    }

    /// <summary>
    /// Creates default options for report-owned modal components.
    /// </summary>
    protected ModalInstanceOptions CreateReportModalOptions( ModalSize size = ModalSize.Default )
    {
        return new()
        {
            UseModalStructure = false,
            Centered = true,
            Size = size,
        };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Resolved report-owned modal provider name.
    /// </summary>
    protected string ResolvedReportModalProviderName => ReportModalProviderName ?? CascadedReportModalProviderName;

    /// <summary>
    /// Service used to show report modal components.
    /// </summary>
    [Inject] protected IModalService ModalService { get; set; }

    /// <summary>
    /// Cascaded report-owned Modal Provider name used by internal designer components.
    /// </summary>
    [CascadingParameter( Name = "ReportModalProviderName" )] internal string CascadedReportModalProviderName { get; set; }

    /// <summary>
    /// Report-owned Modal Provider name used by report modal components.
    /// </summary>
    [Parameter] public string ReportModalProviderName { get; set; }

    #endregion
}