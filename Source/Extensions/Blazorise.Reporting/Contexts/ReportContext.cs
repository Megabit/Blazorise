using System;
using System.Collections.Generic;
using System.Linq;
using Blazorise.Reporting.Internal;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Reporting;

internal sealed class ReportContext
{
    private readonly List<ReportDataSourceDefinition> dataSources = [];

    private readonly List<ReportFormulaFieldDefinition> formulaFields = [];

    private readonly List<RegisteredRunningTotalDefinition> runningTotals = [];

    private readonly List<FontFamily> fonts = [];

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

    public void RegisterRunningTotal( ReportRunningTotalDefinition runningTotal, string resetGroup = null )
    {
        if ( string.IsNullOrWhiteSpace( runningTotal?.Name ) )
            return;

        var registeredRunningTotal = new RegisteredRunningTotalDefinition
        {
            Definition = runningTotal,
            ResetGroup = resetGroup,
        };

        var existingIndex = runningTotals.FindIndex( x => string.Equals( x.Definition?.Name, runningTotal.Name, StringComparison.OrdinalIgnoreCase ) );

        if ( existingIndex >= 0 )
            runningTotals[existingIndex] = registeredRunningTotal;
        else
            runningTotals.Add( registeredRunningTotal );
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
        var definition = new ReportDefinition
        {
            Page = ClonePage( page ?? Page ?? new() ),
            DataSources = dataSources.Select( CloneDataSource ).ToList(),
            FormulaFields = formulaFields.Select( CloneFormulaField ).ToList(),
            Fonts = fonts.Select( CloneFontFamily ).ToList(),
            Sections = sections.Select( CloneSection ).ToList(),
        };

        definition.RunningTotals = runningTotals.Select( runningTotal => CloneRunningTotal( runningTotal, definition.Sections ) ).ToList();

        return CloneDefinition( definition );
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
            DataSources = definition.DataSources?.Select( CloneDataSource ).ToList() ?? [],
            FormulaFields = definition.FormulaFields?.Select( CloneFormulaField ).ToList() ?? [],
            RunningTotals = definition.RunningTotals?.Select( CloneRunningTotal ).ToList() ?? [],
            Fonts = definition.Fonts?.Select( CloneFontFamily ).ToList() ?? [],
            Sections = definition.Sections?.Select( CloneSection ).ToList() ?? [],
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
            ClipboardElements = state.ClipboardElements?.Select( CloneElement ).ToList() ?? [],
            ClipboardSectionId = state.ClipboardSectionId,
            CanUndo = state.CanUndo,
            CanRedo = state.CanRedo,
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

    private static ReportRunningTotalDefinition CloneRunningTotal( RegisteredRunningTotalDefinition runningTotal, IReadOnlyList<ReportSectionDefinition> sections )
    {
        var definition = CloneRunningTotal( runningTotal?.Definition );

        if ( definition is null )
            return null;

        definition.ResetGroupId = ResolveResetGroupId( definition, runningTotal.ResetGroup, sections );

        return definition;
    }

    private static string ResolveResetGroupId( ReportRunningTotalDefinition runningTotal, string resetGroup, IReadOnlyList<ReportSectionDefinition> sections )
    {
        if ( runningTotal.ResetMode != ReportRunningTotalResetMode.Group )
            return runningTotal.ResetGroupId;

        if ( string.IsNullOrWhiteSpace( resetGroup ) || sections is null )
            return runningTotal.ResetGroupId;

        var section = sections.FirstOrDefault( section =>
            string.Equals( section.Id, resetGroup, StringComparison.Ordinal )
            || string.Equals( section.Name, resetGroup, StringComparison.OrdinalIgnoreCase )
            || string.Equals( ReportDefinitionHelper.GetSectionDisplayName( section ), resetGroup, StringComparison.OrdinalIgnoreCase ) );

        return section?.Id ?? runningTotal.ResetGroupId;
    }

    private static ReportSectionDefinition CloneSection( ReportSectionDefinition section )
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
            Elements = section.Elements.Select( CloneElement ).ToList(),
        };
    }

    internal static ReportElementDefinition CloneElement( ReportElementDefinition element )
    {
        if ( element is null )
            return null;

        ReportElementDefinition clone = ReportElementDefinitionFactory.Create( element.Type );

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
                fieldClone.Format = fieldElement.Format;
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
                break;
            case ReportTableElementDefinition tableElement when clone is ReportTableElementDefinition tableClone:
                tableClone.DataSource = tableElement.DataSource;
                tableClone.Columns = tableElement.Columns?.Select( CloneColumn ).ToList() ?? [];
                tableClone.Rows = tableElement.Rows?.Select( CloneRow ).ToList() ?? [];
                tableClone.Cells = tableElement.Cells?.Select( CloneCell ).ToList() ?? [];
                break;
        }

        return clone;
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
            Format = column.Format,
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
            Elements = cell.Elements?.Select( CloneElement ).ToList() ?? [],
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
            CellId = selection.CellId,
            ElementIds = selection.ElementIds?.ToList() ?? [],
        };
    }

    private sealed class RegisteredRunningTotalDefinition
    {
        internal ReportRunningTotalDefinition Definition { get; set; }

        internal string ResetGroup { get; set; }
    }
}

internal sealed class ReportViewerOptions
{
    public ReportPreviewFormat PreviewFormats { get; set; } = ReportPreviewFormat.Html;

    public ReportPreviewFormat DefaultFormat { get; set; } = ReportPreviewFormat.Html;

    public bool AllowPrint { get; set; } = true;

    public bool AllowDownload { get; set; } = true;
}