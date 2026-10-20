#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Docs.Components;

/// <summary>
/// Registers documentation headings and section boundaries with the page TOC.
/// </summary>
public abstract class DocsTocComponent : ComponentBase, IDisposable
{
    #region Methods

    public override Task SetParametersAsync( ParameterView parameters )
    {
        var changed = TocNode is null
            || parameters.TryGetValue<string>( nameof( Title ), out var paramTitle ) && paramTitle != Title
            || parameters.TryGetValue<string>( nameof( ElementId ), out var paramElementId ) && paramElementId != ElementId
            || parameters.TryGetValue<bool>( nameof( IncludeInToc ), out var paramIncludeInToc ) && paramIncludeInToc != IncludeInToc
            || parameters.TryGetValue<RenderFragment>( nameof( ChildContent ), out var paramChildContent ) && paramChildContent != ChildContent;

        var previousLevel = TocLevel;

        parameters.SetParameterProperties( this );
        TocNode ??= ParentToc?.Add();

        if ( TocNode is not null && ( changed || previousLevel != TocLevel || TocNode.IsNew != IsNewTocEntry ) )
        {
            TocNode.Update( Title, TocContent, IncludeInToc ? TocLevel : 0, ElementId, IsNewTocEntry );
        }

        return base.SetParametersAsync( ParameterView.Empty );
    }

    public void Dispose() => TocNode?.Remove();

    #endregion

    #region Properties

    [CascadingParameter] private DocsTocNode ParentToc { get; set; }

    [Parameter] public string Title { get; set; }

    [Parameter] public string ElementId { get; set; }

    [Parameter] public bool IncludeInToc { get; set; } = true;

    [Parameter] public RenderFragment ChildContent { get; set; }

    protected DocsTocNode TocNode { get; private set; }

    protected string HeadingElementId => ElementId ?? TocNode?.ElementId;

    protected virtual int TocLevel => 0;

    protected virtual bool IsNewTocEntry => false;

    protected virtual RenderFragment TocContent => Title is null ? ChildContent : builder => builder.AddContent( 0, Title );

    #endregion
}