#region Using directives
using System;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Base class for field and input components that can be sized in grid layout.
/// </summary>
/// <typeparam name="TClasses">Component-specific classes type.</typeparam>
/// <typeparam name="TStyles">Component-specific styles type.</typeparam>
public abstract class BaseSizableFieldComponent<TClasses, TStyles> : BaseColumnComponent<TClasses, TStyles>, IDisposable
    where TClasses : ComponentClasses
    where TStyles : ComponentStyles
{
    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        // link to the parent component
        ParentField?.Hook( this );

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            // unlink from the parent component
            ParentField?.UnHook( this );
        }

        base.Dispose( disposing );
    }

    #endregion

    #region Properties

    /// <summary>
    /// True if component is inside of a <see cref="Field"/> marked as horizontal.
    /// </summary>
    protected virtual bool IsHorizontal => ParentField?.Horizontal == true;

    /// <summary>
    /// True if component is inside of a <see cref="Field"/> component.
    /// </summary>
    protected virtual bool IsInsideField => ParentField is not null;

    /// <summary>
    /// Cascaded parent <see cref="Field"/> component.
    /// </summary>
    [CascadingParameter] protected Field ParentField { get; set; }

    #endregion
}

/// <summary>
/// Base class for field and input components that can be sized in grid layout.
/// </summary>
public abstract class BaseSizableFieldComponent : BaseSizableFieldComponent<ComponentClasses, ComponentStyles>
{
}