#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using Blazorise.States;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Top footer area of the modal component.
/// </summary>
public partial class OffcanvasFooter : BaseComponent
{
    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        ParentOffcanvas?.NotifyHasOffcanvasFooter();
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.OffcanvasFooter() );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the cascaded parent Offcanvas component.
    /// </summary>
    [CascadingParameter] protected Offcanvas ParentOffcanvas { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="OffcanvasFooter"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}