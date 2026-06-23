#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// The central content region of a <see cref="DockLayout"/>.
/// </summary>
public partial class DockContent : BaseComponent
{
    #region Members

    private DockNodeState node;

    private int definitionVersion;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DockContent() );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.TryGetValue<RenderFragment>( nameof( ChildContent ), out _ ) )
            definitionVersion++;

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        ParentDockLayout?.RegisterContent( this );
        ParentCollector?.AddNode( Node );
    }

    #endregion

    #region Properties

    internal DockNodeState Node => node ??= new()
    {
        Kind = DockNodeKind.Content,
    };

    internal int DefinitionVersion => definitionVersion;

    [CascadingParameter] internal DockLayout ParentDockLayout { get; set; }

    [CascadingParameter] internal DockNodeCollector ParentCollector { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="DockContent"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}