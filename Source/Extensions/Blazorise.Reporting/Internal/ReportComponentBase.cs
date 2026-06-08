#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Provides common infrastructure for Reporting internal Razor components.
/// </summary>
public abstract class ReportComponentBase : ComponentBase
{
    #region Constructors

    /// <summary>
    /// Initializes a new reporting component base instance.
    /// </summary>
    protected ReportComponentBase()
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

    #endregion
}