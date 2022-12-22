using System;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Splitter;

/// <summary>
/// A resizable section of a <see cref="Splitter"/> component
/// </summary>
public partial class SplitterSection : BaseComponent
{
    #region Methods

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if ( Parent == null )
            throw new ArgumentNullException( nameof( Parent ), "SplitSection must exist within a Split" );
        base.OnInitialized();
    }

    /// <inheritdoc />
    protected override void OnAfterRender( bool firstRender )
    {
        if ( firstRender )
            Parent.RegisterSection( ElementRef );

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
        Parent.UnregisterSection( ElementRef );
        base.Dispose( disposing );
    }

    #endregion

    #region Parameters

    /// <summary>
    /// Child content to render in this section
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Parent <see cref="Splitter"/> component
    /// </summary>
    [CascadingParameter] public Splitter Parent { get; set; } = null!;

    #endregion
}