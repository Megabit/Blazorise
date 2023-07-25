#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Splitter;

/// <summary>
/// A resizable section of a <see cref="Splitter"/> component.
/// </summary>
public partial class SplitterSection : BaseComponent, IDisposable
{
    #region Methods

    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( Rendered )
        {
            var sizeChanged = parameters.TryGetValue<double?>( nameof( Size ), out var paramSize ) && !Size.IsEqual( paramSize );
            var minSizeChanged = parameters.TryGetValue<double>( nameof( MinSize ), out var paramMinSize ) && !MinSize.IsEqual( paramMinSize );
            var maxSizeChanged = parameters.TryGetValue<double>( nameof( MaxSize ), out var paramMaxSize ) && !MaxSize.IsEqual( paramMaxSize );
            var snapOffsetChanged = parameters.TryGetValue<double>( nameof( SnapOffset ), out var paramSnapOffset ) && !SnapOffset.IsEqual( paramSnapOffset );

            if ( sizeChanged
                || minSizeChanged
                || maxSizeChanged
                || snapOffsetChanged )
            {
                Parent.UpdateSection( this );
            }
        }

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if ( Parent is null )
            throw new ArgumentNullException( nameof( Parent ), "SplitSection must exist within a Split" );

        base.OnInitialized();
    }

    /// <inheritdoc />
    protected override void OnAfterRender( bool firstRender )
    {
        if ( firstRender )
            Parent.RegisterSection( this );

        base.OnAfterRender( firstRender );
    }

    /// <inheritdoc />
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "split-section" );

        base.BuildClasses( builder );
    }

    /// <inheritdoc />
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            Parent.UnregisterSection( this );
        }

        base.Dispose( disposing );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines the index of the section item.
    /// </summary>
    [Parameter] public int Index { get; init; } = -1;

    /// <summary>
    /// Initial size of a section element in percents or CSS values.
    /// </summary>
    [Parameter] public double? Size { get; init; }

    /// <summary>
    /// Minimum size of the section, specified as pixel value.
    /// </summary>
    [Parameter] public double MinSize { get; init; } = 100;

    /// <summary>
    /// Maximum size of the section, specified as pixel value.
    /// </summary>
    [Parameter] public double MaxSize { get; init; } = double.PositiveInfinity;

    /// <summary>
    /// Snap to minimum size offset in pixels.
    /// </summary>
    [Parameter] public double SnapOffset { get; init; }

    /// <summary>
    /// Parent <see cref="Splitter"/> component.
    /// </summary>
    [CascadingParameter] public Splitter Parent { get; set; } = null!;

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="SplitterSection"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}