using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Reporting;

internal sealed class ReportContext
{
    private readonly List<ReportDataSourceDefinition> dataSources = [];

    private readonly List<ReportSectionDefinition> sections = [];

    public ReportPageDefinition Page { get; private set; } = new();

    public ReportViewerOptions ViewerOptions { get; } = new();

    public RenderFragment ToolbarContent { get; private set; }

    public void RegisterDataSource( ReportDataSourceDefinition dataSource )
    {
        if ( string.IsNullOrWhiteSpace( dataSource.Name ) )
            dataSource.Name = "Default";

        var existingIndex = dataSources.FindIndex( x => string.Equals( x.Name, dataSource.Name, StringComparison.OrdinalIgnoreCase ) );

        if ( existingIndex >= 0 )
            dataSources[existingIndex] = dataSource;
        else
            dataSources.Add( dataSource );
    }

    public ReportSectionDefinition RegisterSection( ReportSectionDefinition section )
    {
        if ( string.IsNullOrWhiteSpace( section.Name ) )
            section.Name = section.Type.ToString();

        var existing = sections.FirstOrDefault( x => string.Equals( x.Name, section.Name, StringComparison.OrdinalIgnoreCase ) );

        if ( existing is null )
        {
            sections.Add( section );
            return section;
        }

        existing.Type = section.Type;
        existing.Layout = section.Layout;
        existing.Height = section.Height;
        existing.DataSource = section.DataSource;
        existing.Class = section.Class;
        existing.Style = section.Style;
        existing.Default = section.Default;
        existing.Suppressed = section.Suppressed;
        existing.Elements.Clear();

        return existing;
    }

    public void RegisterToolbar( RenderFragment toolbarContent )
    {
        ToolbarContent = toolbarContent;
    }

    public void RegisterPage( ReportPageDefinition page )
    {
        Page = page ?? new();
    }

    public ReportDefinition BuildDefinition( ReportPageDefinition page = null )
    {
        return CloneDefinition( new()
        {
            Page = ClonePage( page ?? Page ?? new() ),
            DataSources = dataSources.Select( CloneDataSource ).ToList(),
            Sections = sections.Select( CloneSection ).ToList(),
        } );
    }

    internal static ReportDefinition CloneDefinition( ReportDefinition definition )
    {
        if ( definition is null )
            return null;

        return new()
        {
            Id = definition.Id,
            Name = definition.Name,
            Page = ClonePage( definition.Page ?? new() ),
            DataSources = definition.DataSources.Select( CloneDataSource ).ToList(),
            Sections = definition.Sections.Select( CloneSection ).ToList(),
        };
    }

    internal static ReportState CloneState( ReportState state )
    {
        if ( state is null )
            return new();

        return new()
        {
            Definition = CloneDefinition( state.Definition ),
            Mode = state.Mode,
            PreviewFormat = state.PreviewFormat,
            SnapToGrid = state.SnapToGrid,
            Selection = CloneSelection( state.Selection ),
            ClipboardElement = CloneElement( state.ClipboardElement ),
            CanUndo = state.CanUndo,
            CanRedo = state.CanRedo,
        };
    }

    private static ReportPageDefinition ClonePage( ReportPageDefinition page )
    {
        return new()
        {
            Size = page.Size,
            Orientation = page.Orientation,
            Width = page.Width,
            Height = page.Height,
        };
    }

    private static ReportDataSourceDefinition CloneDataSource( ReportDataSourceDefinition dataSource )
    {
        return new()
        {
            Id = dataSource.Id,
            Name = dataSource.Name,
            Data = dataSource.Data,
        };
    }

    private static ReportSectionDefinition CloneSection( ReportSectionDefinition section )
    {
        return new()
        {
            Id = section.Id,
            Name = section.Name,
            Type = section.Type,
            Layout = section.Layout,
            Height = section.Height,
            DataSource = section.DataSource,
            GroupBy = section.GroupBy,
            Class = section.Class,
            Style = section.Style,
            Default = section.Default,
            Suppressed = section.Suppressed,
            Elements = section.Elements.Select( CloneElement ).ToList(),
        };
    }

    internal static ReportElementDefinition CloneElement( ReportElementDefinition element )
    {
        if ( element is null )
            return null;

        return new()
        {
            Id = element.Id,
            Name = element.Name,
            Type = element.Type,
            X = element.X,
            Y = element.Y,
            Width = element.Width,
            Height = element.Height,
            Text = element.Text,
            Field = element.Field,
            Format = element.Format,
            Source = element.Source,
            DataSource = element.DataSource,
            Font = CloneFont( element.Font ),
            Appearance = CloneAppearance( element.Appearance ),
            Border = CloneBorder( element.Border ),
            Class = element.Class,
            Style = element.Style,
            Aggregate = CloneAggregate( element.Aggregate ),
            Columns = element.Columns.Select( CloneColumn ).ToList(),
        };
    }

    private static ReportAggregateDefinition CloneAggregate( ReportAggregateDefinition aggregate )
    {
        if ( aggregate is null )
            return null;

        return new()
        {
            Function = aggregate.Function,
        };
    }

    private static ReportFontDefinition CloneFont( ReportFontDefinition font )
    {
        if ( font is null )
            return new();

        return new()
        {
            Family = font.Family,
            Size = font.Size,
            Color = font.Color,
            Bold = font.Bold,
            Italic = font.Italic,
            Underline = font.Underline,
            Alignment = font.Alignment,
        };
    }

    private static ReportAppearanceDefinition CloneAppearance( ReportAppearanceDefinition appearance )
    {
        if ( appearance is null )
            return new();

        return new()
        {
            BackgroundColor = appearance.BackgroundColor,
            Opacity = appearance.Opacity,
        };
    }

    private static ReportBorderDefinition CloneBorder( ReportBorderDefinition border )
    {
        if ( border is null )
            return new();

        return new()
        {
            Color = border.Color,
            Width = border.Width,
            Radius = border.Radius,
        };
    }

    private static ReportTableColumnDefinition CloneColumn( ReportTableColumnDefinition column )
    {
        return new()
        {
            Id = column.Id,
            Title = column.Title,
            Field = column.Field,
            Format = column.Format,
            Width = column.Width,
        };
    }

    private static ReportSelectionState CloneSelection( ReportSelectionState selection )
    {
        if ( selection is null )
            return new();

        return new()
        {
            Type = selection.Type,
            SectionId = selection.SectionId,
            ElementId = selection.ElementId,
            ElementIds = selection.ElementIds?.ToList() ?? [],
        };
    }
}

internal sealed class ReportViewerOptions
{
    public ReportPreviewFormat PreviewFormats { get; set; } = ReportPreviewFormat.Html;

    public ReportPreviewFormat DefaultFormat { get; set; } = ReportPreviewFormat.Html;

    public bool AllowPrint { get; set; } = true;

    public bool AllowDownload { get; set; } = true;
}