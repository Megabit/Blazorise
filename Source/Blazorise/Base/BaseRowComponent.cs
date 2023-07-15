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
    /// Specifies the content to be rendered inside this <see cref="BaseRowComponent"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}