#region Using directives
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Base class for components that are containers for column components.
/// </summary>
public abstract class BaseRowComponent : BaseComponent
{
    #region Properties

    /// <summary>
    /// Gets the row state used to calculate used space by the columns.
    /// </summary>
    [Inject] protected IRowState RowState { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="BaseRowComponent"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}