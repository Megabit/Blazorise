#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Header content for a <see cref="DockPane"/>.
/// </summary>
public partial class DockPaneHeader : BaseComponent
{
    #region Constructors

    /// <summary>
    /// Default <see cref="DockPaneHeader"/> constructor.
    /// </summary>
    public DockPaneHeader()
    {
        ContentClassBuilder = new( BuildContentClasses );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DockPaneHeader() );

        base.BuildClasses( builder );
    }

    private void BuildContentClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DockPaneHeaderContent() );
    }

    private Task BeginDrag( PointerEventArgs eventArgs )
    {
        return Context?.BeginPaneDrag( ParentDockPane, eventArgs, true )
            ?? ParentDockPane?.ParentDockLayout?.BeginPaneDrag( ParentDockPane, eventArgs, true )
            ?? Task.CompletedTask;
    }

    #endregion

    #region Properties

    [CascadingParameter] internal DockPane ParentDockPane { get; set; }

    [CascadingParameter] internal DockLayoutContext Context { get; set; }

    private string ContentClassNames => ContentClassBuilder.Class;

    private ClassBuilder ContentClassBuilder { get; set; }

    /// <summary>
    /// Specifies the header content to be rendered inside this <see cref="DockPaneHeader"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}