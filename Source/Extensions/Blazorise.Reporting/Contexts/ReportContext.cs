#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using Blazorise.Reporting.Internal;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

internal sealed class ReportContext
{
    private readonly List<ReportDataSourceDefinition> dataSources = [];

    private readonly List<ReportFormulaFieldDefinition> formulaFields = [];

    private readonly List<ReportRunningTotalDefinition> runningTotals = [];

    private readonly List<FontFamily> fonts = [];

    private readonly List<ReportBandDefinition> sections = [];

    public ReportPageDefinition Page { get; private set; } = new();

    public ReportViewerOptions ViewerOptions { get; } = new();

    public RenderFragment ToolbarContent { get; private set; }

    public RenderFragment<ReportToolbarItemContext> ToolbarButtonTemplate { get; private set; }

    public IReadOnlyCollection<ReportCommand> HiddenToolbarCommands { get; private set; }

    public bool ShowToolbarPanesMenu { get; private set; } = true;

    public bool ShowToolbarPersistenceButtons { get; private set; } = true;

    public bool ShowToolbarEditButtons { get; private set; } = true;

    public bool ShowToolbarHistoryButtons { get; private set; } = true;

    public bool ShowToolbarDataSourceButtons { get; private set; } = true;

    public bool ShowToolbarExportButtons { get; private set; } = true;

    public bool ShowToolbarModeButtons { get; private set; } = true;

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

    public void RegisterFormulaField( ReportFormulaFieldDefinition formulaField )
    {
        if ( string.IsNullOrWhiteSpace( formulaField?.Name ) )
            return;

        var existingIndex = formulaFields.FindIndex( x => string.Equals( x.Name, formulaField.Name, StringComparison.OrdinalIgnoreCase ) );

        if ( existingIndex >= 0 )
            formulaFields[existingIndex] = formulaField;
        else
            formulaFields.Add( formulaField );
    }

    public void RegisterRunningTotal( ReportRunningTotalDefinition runningTotal )
    {
        if ( string.IsNullOrWhiteSpace( runningTotal?.Name ) )
            return;

        var existingIndex = runningTotals.FindIndex( x => string.Equals( x.Name, runningTotal.Name, StringComparison.OrdinalIgnoreCase ) );

        if ( existingIndex >= 0 )
            runningTotals[existingIndex] = runningTotal;
        else
            runningTotals.Add( runningTotal );
    }

    public void RegisterFont( FontFamily font )
    {
        if ( string.IsNullOrWhiteSpace( font?.Name ) )
            return;

        int existingIndex = fonts.FindIndex( x => string.Equals( x.Name, font.Name, StringComparison.OrdinalIgnoreCase ) );

        if ( existingIndex >= 0 )
            fonts[existingIndex] = font;
        else
            fonts.Add( font );
    }

    public ReportBandDefinition RegisterSection( ReportBandDefinition section )
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
        existing.Height = section.Height;
        existing.DataSource = section.DataSource;
        existing.Class = section.Class;
        existing.Style = section.Style;
        existing.Default = section.Default;
        existing.Suppress = CloneValue( section.Suppress );
        existing.ReserveSpaceWhenSuppressed = section.ReserveSpaceWhenSuppressed;
        existing.PrintOnFirstPage = section.PrintOnFirstPage;
        existing.PrintOnLastPage = section.PrintOnLastPage;
        existing.RepeatOnEveryPage = section.RepeatOnEveryPage;
        existing.KeepTogether = CloneValue( section.KeepTogether );
        existing.NewPageBefore = CloneValue( section.NewPageBefore );
        existing.NewPageAfter = CloneValue( section.NewPageAfter );
        existing.Appearance = CloneAppearance( section.Appearance );
        existing.Border = CloneBorder( section.Border );
        existing.Elements.Clear();

        return existing;
    }

    public void RegisterToolbar( ReportToolbar toolbar )
    {
        ToolbarContent = toolbar.ChildContent;
        ToolbarButtonTemplate = toolbar.ButtonTemplate;
        HiddenToolbarCommands = toolbar.HiddenCommands;
        ShowToolbarPanesMenu = toolbar.ShowPanesMenu;
        ShowToolbarPersistenceButtons = toolbar.ShowPersistenceButtons;
        ShowToolbarEditButtons = toolbar.ShowEditButtons;
        ShowToolbarHistoryButtons = toolbar.ShowHistoryButtons;
        ShowToolbarDataSourceButtons = toolbar.ShowDataSourceButtons;
        ShowToolbarExportButtons = toolbar.ShowExportButtons;
        ShowToolbarModeButtons = toolbar.ShowModeButtons;
    }

    public void RegisterPage( ReportPageDefinition page )
    {
        Page = page ?? new();
    }

    public ReportDefinition BuildDefinition( ReportPageDefinition page = null )
    {
        var definition = new ReportDefinition
        {
            Page = ClonePage( page ?? Page ?? new() ),
            DataSources = dataSources.Select( CloneDataSource ).ToList(),
            FormulaFields = formulaFields.Select( CloneFormulaField ).ToList(),
            Fonts = fonts.Select( CloneFontFamily ).ToList(),
            Bands = sections.Select( section => CloneSection( section ) ).ToList(),
        };

        definition.RunningTotals = runningTotals.Select( CloneRunningTotal ).ToList();

        return definition;
    }

    internal static ReportDefinition CloneDefinition( ReportDefinition definition )
        => CloneDefinition( definition, 0 );

    private static ReportDefinition CloneDefinition( ReportDefinition definition, int subreportDepth )
    {
        if ( definition is null )
            return null;

        return new()
        {
            FormatVersion = definition.FormatVersion,
            Id = definition.Id,
            Name = definition.Name,
            Designer = CloneDesigner( definition.Designer ),
            Page = ClonePage( definition.Page ?? new() ),
            DataSources = definition.DataSources?.Select( CloneDataSource ).ToList() ?? [],
            FormulaFields = definition.FormulaFields?.Select( CloneFormulaField ).ToList() ?? [],
            RunningTotals = definition.RunningTotals?.Select( CloneRunningTotal ).ToList() ?? [],
            Fonts = definition.Fonts?.Select( CloneFontFamily ).ToList() ?? [],
            Bands = definition.Bands?.Select( section => CloneSection( section, subreportDepth ) ).ToList() ?? [],
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
            Selection = CloneSelection( state.Selection ),
            ClipboardElements = state.ClipboardElements?.Select( element => CloneElement( element ) ).ToList() ?? [],
            ClipboardBandId = state.ClipboardBandId,
        };
    }

    private static ReportDesignerDefinition CloneDesigner( ReportDesignerDefinition designer )
    {
        designer ??= new();

        return new()
        {
            SnapToGrid = designer.SnapToGrid,
            GridSize = designer.GridSize,
            ShowRulers = designer.ShowRulers,
            ShowFineRulerTicks = designer.ShowFineRulerTicks,
            ShowCursorGuides = designer.ShowCursorGuides,
            ShowCollisionWarnings = designer.ShowCollisionWarnings,
            BandMode = designer.BandMode,
        };
    }

    private static ReportPageDefinition ClonePage( ReportPageDefinition page )
    {
        page ??= new();

        return new()
        {
            Size = page.Size,
            MeasurementUnit = page.MeasurementUnit,
            Orientation = page.Orientation,
            Width = page.Width,
            Height = page.Height,
            Margins = ClonePageMargins( page.Margins ),
        };
    }

    private static ReportPageMarginsDefinition ClonePageMargins( ReportPageMarginsDefinition margins )
    {
        if ( margins is null )
            return new();

        return new()
        {
            Left = margins.Left,
            Top = margins.Top,
            Right = margins.Right,
            Bottom = margins.Bottom,
        };
    }

    private static ReportDataSourceDefinition CloneDataSource( ReportDataSourceDefinition dataSource )
    {
        return new()
        {
            Id = dataSource.Id,
            Name = dataSource.Name,
            ProviderType = dataSource.ProviderType,
            Data = dataSource.Data,
            Settings = dataSource.Settings?.ToDictionary( item => item.Key, item => item.Value ) ?? [],
            Schema = CloneSchema( dataSource.Schema ),
        };
    }

    private static ReportDataSourceSchema CloneSchema( ReportDataSourceSchema schema )
    {
        if ( schema is null )
            return null;

        return new()
        {
            IsCollection = schema.IsCollection,
            Fields = schema.Fields?.Select( CloneSchemaField ).ToList() ?? [],
        };
    }

    private static ReportDataSourceSchemaField CloneSchemaField( ReportDataSourceSchemaField field )
    {
        if ( field is null )
            return null;

        return new()
        {
            Name = field.Name,
            DisplayName = field.DisplayName,
            DataType = field.DataType,
            IsCollection = field.IsCollection,
            Fields = field.Fields?.Select( CloneSchemaField ).ToList() ?? [],
        };
    }

    private static ReportFormulaFieldDefinition CloneFormulaField( ReportFormulaFieldDefinition formulaField )
    {
        if ( formulaField is null )
            return null;

        return new()
        {
            Id = formulaField.Id,
            Name = formulaField.Name,
            Formula = formulaField.Formula,
        };
    }

    private static ReportRunningTotalDefinition CloneRunningTotal( ReportRunningTotalDefinition runningTotal )
    {
        if ( runningTotal is null )
            return null;

        return new()
        {
            Id = runningTotal.Id,
            Name = runningTotal.Name,
            DataSource = runningTotal.DataSource,
            Field = runningTotal.Field,
            AggregateFunction = runningTotal.AggregateFunction,
            EvaluateMode = runningTotal.EvaluateMode,
            EvaluateFormula = runningTotal.EvaluateFormula,
            ResetMode = runningTotal.ResetMode,
            ResetGroupId = runningTotal.ResetGroupId,
        };
    }

    private static ReportBandDefinition CloneSection( ReportBandDefinition section )
        => CloneSection( section, 0 );

    private static ReportBandDefinition CloneSection( ReportBandDefinition section, int subreportDepth )
    {
        return new()
        {
            Id = section.Id,
            Name = section.Name,
            Type = section.Type,
            Height = section.Height,
            DataSource = section.DataSource,
            GroupBy = section.GroupBy,
            Class = section.Class,
            Style = section.Style,
            Default = section.Default,
            Suppress = CloneValue( section.Suppress ),
            ReserveSpaceWhenSuppressed = section.ReserveSpaceWhenSuppressed,
            PrintOnFirstPage = section.PrintOnFirstPage,
            PrintOnLastPage = section.PrintOnLastPage,
            RepeatOnEveryPage = section.RepeatOnEveryPage,
            KeepTogether = CloneValue( section.KeepTogether ),
            NewPageBefore = CloneValue( section.NewPageBefore ),
            NewPageAfter = CloneValue( section.NewPageAfter ),
            Appearance = CloneAppearance( section.Appearance ),
            Border = CloneBorder( section.Border ),
            Elements = section.Elements.Select( element => CloneElement( element, subreportDepth ) ).Where( element => element is not null ).ToList(),
        };
    }

    internal static ReportElementDefinition CloneElement( ReportElementDefinition element )
        => CloneElement( element, 0 );

    private static ReportElementDefinition CloneElement( ReportElementDefinition element, int subreportDepth )
    {
        if ( element is null )
            return null;

        if ( element is ReportSubreportElementDefinition && subreportDepth > 0 )
            return null;

        ReportElementDefinition clone = Internal.ReportElementDefinitionFactory.Create( element.Type );

        clone.Id = element.Id;
        clone.Name = element.Name;
        clone.X = element.X;
        clone.Y = element.Y;
        clone.Width = element.Width;
        clone.Height = element.Height;
        clone.CanGrow = CloneValue( element.CanGrow );
        clone.Suppress = CloneValue( element.Suppress );
        clone.SnapToGrid = CloneValue( element.SnapToGrid );
        clone.Font = CloneFont( element.Font );
        clone.Appearance = CloneAppearance( element.Appearance );
        clone.Border = CloneBorder( element.Border );
        clone.Class = element.Class;
        clone.Style = element.Style;

        switch ( element )
        {
            case ReportTextElementDefinition textElement when clone is ReportTextElementDefinition textClone:
                textClone.Text = textElement.Text;
                textClone.DataSource = textElement.DataSource;
                break;
            case ReportFieldElementDefinition fieldElement when clone is ReportFieldElementDefinition fieldClone:
                fieldClone.Field = fieldElement.Field;
                fieldClone.Format = CloneFormat( fieldElement.Format );
                fieldClone.DataSource = fieldElement.DataSource;
                fieldClone.Aggregate = CloneAggregate( fieldElement.Aggregate );
                break;
            case ReportImageElementDefinition imageElement when clone is ReportImageElementDefinition imageClone:
                imageClone.Source = imageElement.Source;
                imageClone.Fit = imageElement.Fit;
                imageClone.Text = imageElement.Text;
                break;
            case ReportLineElementDefinition lineElement when clone is ReportLineElementDefinition lineClone:
                lineClone.Thickness = lineElement.Thickness;
                lineClone.Orientation = lineElement.Orientation;
                break;
            case ReportTableElementDefinition tableElement when clone is ReportTableElementDefinition tableClone:
                tableClone.DataSource = tableElement.DataSource;
                tableClone.Columns = tableElement.Columns?.Select( CloneColumn ).ToList() ?? [];
                tableClone.Rows = tableElement.Rows?.Select( CloneRow ).ToList() ?? [];
                tableClone.Cells = tableElement.Cells?.Select( cell => CloneCell( cell, subreportDepth ) ).ToList() ?? [];
                break;
            case ReportPanelElementDefinition panelElement when clone is ReportPanelElementDefinition panelClone:
                panelClone.Elements = panelElement.Elements?.Select( element => CloneElement( element, subreportDepth ) ).Where( element => element is not null ).ToList() ?? [];
                break;
            case ReportSubreportElementDefinition subreportElement when clone is ReportSubreportElementDefinition subreportClone:
                subreportClone.Report = CloneDefinition( ResolveSubreportDefinition( subreportElement ), subreportDepth + 1 );
                subreportClone.DataSource = subreportElement.DataSource;
                break;
        }

        return clone;
    }

    private static ReportDefinition ResolveSubreportDefinition( ReportSubreportElementDefinition subreportElement )
    {
        ReportDefinition definition = subreportElement.DeclarativeContext is not null
            ? subreportElement.DeclarativeContext.BuildDefinition()
            : subreportElement.Report;

        if ( definition is not null && string.IsNullOrWhiteSpace( definition.Name ) )
            definition.Name = string.IsNullOrWhiteSpace( subreportElement.Name ) ? "Subreport" : subreportElement.Name;

        return definition;
    }

    private static ReportValue<T> CloneValue<T>( ReportValue<T> value )
    {
        if ( value is null )
            return default;

        return new()
        {
            Value = value.Value,
            Formula = value.Formula,
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
            VerticalAlignment = font.VerticalAlignment,
        };
    }

    private static ReportFormatDefinition CloneFormat( ReportFormatDefinition format )
        => ReportFormats.Clone( format );

    private static FontFamily CloneFontFamily( FontFamily font )
    {
        if ( font is null )
            return null;

        return new()
        {
            Name = font.Name,
            DisplayName = font.DisplayName,
            CssFamily = font.CssFamily,
            Regular = CloneFontSource( font.Regular ),
            Bold = CloneFontSource( font.Bold ),
            Italic = CloneFontSource( font.Italic ),
            BoldItalic = CloneFontSource( font.BoldItalic ),
            Visible = font.Visible,
        };
    }

    private static FontSource CloneFontSource( FontSource source )
    {
        if ( source is null )
            return null;

        return new()
        {
            Url = source.Url,
            Data = source.Data,
            FileName = source.FileName,
            Format = source.Format,
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
            Style = border.Style,
            Radius = border.Radius,
        };
    }

    private static ReportTableColumnDefinition CloneColumn( ReportTableColumnDefinition column )
    {
        if ( column is null )
            return null;

        return new()
        {
            Id = column.Id,
            Title = column.Title,
            Field = column.Field,
            Format = CloneFormat( column.Format ),
            Width = column.Width,
        };
    }

    private static ReportTableRowDefinition CloneRow( ReportTableRowDefinition row )
    {
        if ( row is null )
            return null;

        return new()
        {
            Id = row.Id,
            Height = row.Height,
        };
    }

    private static ReportTableCellDefinition CloneCell( ReportTableCellDefinition cell )
        => CloneCell( cell, 0 );

    private static ReportTableCellDefinition CloneCell( ReportTableCellDefinition cell, int subreportDepth )
    {
        if ( cell is null )
            return null;

        return new()
        {
            Id = cell.Id,
            RowIndex = cell.RowIndex,
            ColumnIndex = cell.ColumnIndex,
            RowSpan = cell.RowSpan,
            ColumnSpan = cell.ColumnSpan,
            Elements = cell.Elements?.Select( element => CloneElement( element, subreportDepth ) ).Where( element => element is not null ).ToList() ?? [],
        };
    }

    private static ReportSelectionState CloneSelection( ReportSelectionState selection )
    {
        if ( selection is null )
            return new();

        return new()
        {
            Type = selection.Type,
            BandId = selection.BandId,
            ElementId = selection.ElementId,
            CellId = selection.CellId,
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

    public RenderFragment<ReportPdfPreviewContext> PdfPreviewTemplate { get; set; }
}