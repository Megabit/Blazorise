#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Docs.Components;

/// <summary>
/// A heading or a section containing headings, in document order.
/// </summary>
public sealed partial class DocsTocNode
{
    #region Events

    public event Action Changed;

    #endregion

    #region Members

    private readonly List<DocsTocNode> children = [];

    private int nextChildIndex;

    #endregion

    #region Methods

    public DocsTocNode Add()
    {
        var child = new DocsTocNode
        {
            Parent = this,
            Path = $"{Path}-{++nextChildIndex}",
        };

        children.Add( child );

        return child;
    }

    public void Update( string title, RenderFragment content, int level, string elementId, bool isNew )
    {
        Content = content;
        Level = level;
        IsNew = isNew;

        var baseId = string.IsNullOrWhiteSpace( title )
            ? $"toc{Path}"
            : "toc_" + NonAlphanumericRegex().Replace( title.Trim(), "-" );

        ElementId = elementId ?? ( Root.Headings.Any( node => node != this && node.ElementId == baseId ) ? $"{baseId}{Path}" : baseId );
        Root.Changed?.Invoke();
    }

    public void Remove()
    {
        if ( Parent is not null && Parent.children.Remove( this ) )
        {
            Root.Changed?.Invoke();
        }
    }

    [GeneratedRegex( "[^A-Za-z0-9]" )]
    private static partial Regex NonAlphanumericRegex();

    #endregion

    #region Properties

    public string ElementId { get; private set; }

    public RenderFragment Content { get; private set; }

    public int Level { get; private set; }

    public bool IsNew { get; private set; }

    public IEnumerable<DocsTocNode> Headings
    {
        get
        {
            foreach ( var child in children )
            {
                if ( child.Level > 0 && child.Content is not null )
                {
                    yield return child;
                }

                foreach ( var heading in child.Headings )
                {
                    yield return heading;
                }
            }
        }
    }

    private DocsTocNode Parent { get; init; }

    private DocsTocNode Root => Parent?.Root ?? this;

    private string Path { get; init; } = string.Empty;

    #endregion
}