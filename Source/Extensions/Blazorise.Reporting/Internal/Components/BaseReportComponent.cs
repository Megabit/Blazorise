#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Provides common infrastructure for Reporting internal Razor components.
/// </summary>
public abstract class BaseReportComponent : ComponentBase
{
    #region Constructors

    /// <summary>
    /// Initializes a new reporting component base instance.
    /// </summary>
    protected BaseReportComponent()
    {
        ClassBuilder = new( BuildClasses );
        StyleBuilder = new( BuildStyles );
    }

    #endregion

    #region Methods

    /// <summary>
    /// Builds component CSS classes.
    /// </summary>
    /// <param name="builder">Class builder used to append component classes.</param>
    protected virtual void BuildClasses( ClassBuilder builder )
    {
    }

    /// <summary>
    /// Builds component CSS styles.
    /// </summary>
    /// <param name="builder">Style builder used to append component styles.</param>
    protected virtual void BuildStyles( StyleBuilder builder )
    {
    }

    /// <summary>
    /// Marks component classes as requiring regeneration.
    /// </summary>
    protected internal virtual void DirtyClasses()
    {
        ClassBuilder?.Dirty();
    }

    /// <summary>
    /// Marks component styles as requiring regeneration.
    /// </summary>
    protected internal virtual void DirtyStyles()
    {
        StyleBuilder?.Dirty();
    }

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
    protected async Task<ModalInstance> ShowReportModal<TComponent>( Action<ModalProviderParameterBuilder<TComponent>> parameters = null, ModalInstanceOptions options = null )
        where TComponent : BaseReportComponent
    {
        options ??= CreateReportModalOptions();
        string providerName = ResolvedReportModalProviderName;
        options.ProviderName = providerName;

        return await ModalService.Show<TComponent>( string.Empty, builder =>
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
    /// Builder used to construct and cache component classes.
    /// </summary>
    protected ClassBuilder ClassBuilder { get; }

    /// <summary>
    /// Builder used to construct and cache component styles.
    /// </summary>
    protected StyleBuilder StyleBuilder { get; }

    /// <summary>
    /// Resolved component class names.
    /// </summary>
    protected string ClassNames => ClassBuilder.Class;

    /// <summary>
    /// Resolved component styles.
    /// </summary>
    protected string StyleNames => StyleBuilder.Styles;

    /// <summary>
    /// Resolved report-owned modal provider name.
    /// </summary>
    protected string ResolvedReportModalProviderName => ReportModalProviderName ?? CascadedReportModalProviderName;

    /// <summary>
    /// Report-owned Modal Provider name used by report modal components.
    /// </summary>
    [Parameter] public string ReportModalProviderName { get; set; }

    /// <summary>
    /// Cascaded report-owned Modal Provider name used by internal designer components.
    /// </summary>
    [CascadingParameter( Name = "ReportModalProviderName" )] internal string CascadedReportModalProviderName { get; set; }

    /// <summary>
    /// Service used to show report modal components.
    /// </summary>
    [Inject] protected IModalService ModalService { get; set; }

    #endregion
}