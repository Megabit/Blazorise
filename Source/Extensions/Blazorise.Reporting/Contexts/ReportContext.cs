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
        return new()
        {
            Page = ClonePage( page ?? Page ?? new() ),
            DataSources = dataSources.Select( CloneDataSource ).ToList(),
            Sections = sections.Select( CloneSection ).ToList(),
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
            Name = dataSource.Name,
            Data = dataSource.Data,
        };
    }

    private static ReportSectionDefinition CloneSection( ReportSectionDefinition section )
    {
        return new()
        {
            Name = section.Name,
            Type = section.Type,
            Layout = section.Layout,
            Height = section.Height,
            DataSource = section.DataSource,
            Class = section.Class,
            Style = section.Style,
            Elements = section.Elements.Select( CloneElement ).ToList(),
        };
    }

    private static ReportElementDefinition CloneElement( ReportElementDefinition element )
    {
        return new()
        {
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
            Class = element.Class,
            Style = element.Style,
            Columns = element.Columns.Select( CloneColumn ).ToList(),
        };
    }

    private static ReportTableColumnDefinition CloneColumn( ReportTableColumnDefinition column )
    {
        return new()
        {
            Title = column.Title,
            Field = column.Field,
            Format = column.Format,
            Width = column.Width,
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