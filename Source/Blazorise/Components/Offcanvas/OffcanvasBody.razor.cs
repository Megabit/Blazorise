#region Using directives
using System;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Center area of the offcanvas component.
/// </summary>
public partial class OffcanvasBody : BaseComponent, IDisposable
{
    #region Members

    private int? maxHeight;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        ParentOffcanvas?.NotifyOffcanvasBodyInitialized();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            ParentOffcanvas?.NotifyOffcanvasBodyRemoved();
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.OffcanvasBody() );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the cascaded parent offcanvas-content component.
    /// </summary>
    [CascadingParameter] protected Offcanvas ParentOffcanvas { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="OffcanvasBody"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}
