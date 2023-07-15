#region Using directives
using System.Collections.Generic;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// An accordion is a vertically stacked list of headers that reveal or hide associated sections of content.
/// </summary>
public partial class Accordion : BaseComponent
{
    #region Members

    private List<Collapse> collapses;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Accordion() );

        base.BuildClasses( builder );
    }

    internal void NotifyCollapseInitialized( Collapse collapse )
    {
        collapses ??= new();

        collapses.Add( collapse );
    }

    internal void NotifyCollapseRemoved( Collapse collapse )
    {
        if ( collapses is not null && collapses.Contains( collapse ) )
            collapses.Remove( collapse );
    }

    internal bool IsFirstInAccordion( Collapse collapse )
    {
        if ( collapses is not null && collapses.IndexOf( collapse ) == 0 )
            return true;

        return false;
    }

    internal bool IsLastInAccordion( Collapse collapse )
    {
        if ( collapses is not null && collapses.IndexOf( collapse ) == collapses.Count - 1 )
            return true;

        return false;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Accordion"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}