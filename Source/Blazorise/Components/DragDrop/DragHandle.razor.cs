#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Marks the content within a drop zone item from which dragging can begin.
/// </summary>
public partial class DragHandle : BaseComponent, IDisposable
{
    #region Methods

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        ParentDraggable?.RegisterHandle( this );

        base.OnInitialized();
    }

    /// <inheritdoc />
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            ParentDraggable?.UnregisterHandle( this );
        }

        base.Dispose( disposing );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets whether this handle can start dragging its item.
    /// </summary>
    protected bool CanDrag => !Disabled && ParentDraggable is { Disabled: false };

    /// <summary>
    /// Gets the native draggable attribute.
    /// </summary>
    protected string DraggableString => CanDrag ? "true" : "false";

    /// <summary>
    /// Prevents this handle from starting a drag. Other enabled handles on the item remain usable.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Specifies the content users can drag to move the owning item.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the owning draggable item.
    /// </summary>
    [CascadingParameter] internal IDragHandleOwner ParentDraggable { get; set; }

    #endregion
}