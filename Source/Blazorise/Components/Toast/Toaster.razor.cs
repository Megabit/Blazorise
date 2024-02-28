#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// A component that acts as a container for the <see cref="Toast"/> components.
/// </summary>
public partial class Toaster : BaseComponent
{
    #region Members

    private ToasterPlacement placement = ToasterPlacement.BottomEnd;

    private ToasterPlacementStrategy placementStrategy = ToasterPlacementStrategy.Fixed;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Toaster() );
        builder.Append( ClassProvider.ToasterPlacement( Placement ) );
        builder.Append( ClassProvider.ToasterPlacementStrategy( PlacementStrategy ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Specifies the position of the <see cref="Toaster" /> component.
    /// </summary>
    [Parameter]
    public ToasterPlacement Placement
    {
        get => placement;
        set
        {
            if ( placement == value )
                return;

            placement = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Specifies the placement strategy of the <see cref="Toaster" /> component.
    /// </summary>
    [Parameter]
    public ToasterPlacementStrategy PlacementStrategy
    {
        get => placementStrategy;
        set
        {
            if ( placementStrategy == value )
                return;

            placementStrategy = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// The content to be rendered inside the <see cref="Toaster" /> component.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}