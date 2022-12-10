#region Using directives
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Base class for components that are containers for other components.
/// </summary>
public abstract class BaseRowableComponent : BaseComponent
{
    #region Properties

    /// <summary>
    /// Gets the rowable context used to calculate used space by the columns.
    /// </summary>
    [Inject] protected IRowableContext RowableContext { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="BaseRowableComponent"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}