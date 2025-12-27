using System.Collections.Generic;

namespace Blazorise.Analyzers.Migration;

public enum TValueShape
{
    Any,
    Single,
    SingleOrMultiListOrArray,
}

public sealed record ComponentMapping
{
    public string OldFullName { get; init; }

    public string? NewFullName { get; init; }

    public IReadOnlyDictionary<string, string> ParameterRenames { get; init; }

    public IReadOnlyDictionary<string, string> ParameterRemovals { get; init; }

    public TValueShape TValueShape { get; init; }

    public string Notes { get; init; }

    public ComponentMapping(
        string oldFullName,
        string? newFullName,
        IReadOnlyDictionary<string, string> parameterRenames,
        TValueShape tValueShape,
        string notes,
        IReadOnlyDictionary<string, string>? parameterRemovals = null )
    {
        OldFullName = oldFullName;
        NewFullName = newFullName;
        ParameterRenames = parameterRenames;
        ParameterRemovals = parameterRemovals ?? new Dictionary<string, string>();
        TValueShape = tValueShape;
        Notes = notes;
    }
}

public sealed record SymbolMapping
{
    public string ContainingType { get; init; }

    public string OldName { get; init; }

    public string? NewName { get; init; }

    public string Notes { get; init; }

    public SymbolMapping( string containingType, string oldName, string? newName, string notes )
    {
        ContainingType = containingType;
        OldName = oldName;
        NewName = newName;
        Notes = notes;
    }
}

public sealed record TypeMapping
{
    public string OldFullName { get; init; }

    public string? NewFullName { get; init; }

    public string Notes { get; init; }

    public TypeMapping( string oldFullName, string? newFullName, string notes )
    {
        OldFullName = oldFullName;
        NewFullName = newFullName;
        Notes = notes;
    }
}

public static partial class BlazoriseMigrationMappings
{
    public static IReadOnlyList<ComponentMapping> Components { get; } = CreateComponentMappings();

    public static IReadOnlyList<SymbolMapping> Symbols { get; } = CreateSymbolMappings();

    public static IReadOnlyList<TypeMapping> Types { get; } = CreateTypeMappings();

    private static IReadOnlyList<ComponentMapping> CreateComponentMappings()
    {
        var list = new List<ComponentMapping>();

        list.Add( new ComponentMapping(
            "Blazorise.ColorEdit`1",
            "Blazorise.ColorInput`1",
            new Dictionary<string, string>
            {
                ["Color"] = "Value",
                ["ColorChanged"] = "ValueChanged",
                ["ColorExpression"] = "ValueExpression",
            },
            TValueShape.Single,
            "ColorEdit was renamed to ColorInput and unified on Value / ValueChanged / ValueExpression binding." ) );

        list.Add( new ComponentMapping(
            "Blazorise.DateEdit`1",
            "Blazorise.DateInput`1",
            new Dictionary<string, string>
            {
                ["Date"] = "Value",
                ["DateChanged"] = "ValueChanged",
                ["DateExpression"] = "ValueExpression",
            },
            TValueShape.Single,
            "DateEdit was renamed to DateInput and unified on Value / ValueChanged / ValueExpression binding." ) );

        list.Add( new ComponentMapping(
            "Blazorise.FileEdit",
            "Blazorise.FileInput",
            new Dictionary<string, string>(),
            TValueShape.Any,
            "FileEdit was renamed to FileInput." ) );

        list.Add( new ComponentMapping(
            "Blazorise.MemoEdit",
            "Blazorise.MemoInput",
            new Dictionary<string, string>
            {
                ["Text"] = "Value",
                ["TextChanged"] = "ValueChanged",
                ["TextExpression"] = "ValueExpression",
            },
            TValueShape.Single,
            "MemoEdit was renamed to MemoInput and unified on Value / ValueChanged / ValueExpression binding." ) );

        list.Add( new ComponentMapping(
            "Blazorise.NumericEdit`1",
            "Blazorise.NumericInput`1",
            new Dictionary<string, string>(),
            TValueShape.Single,
            "NumericEdit was renamed to NumericInput; Value binding is preserved." ) );

        list.Add( new ComponentMapping(
            "Blazorise.TextEdit",
            "Blazorise.TextInput",
            new Dictionary<string, string>
            {
                ["Text"] = "Value",
                ["TextChanged"] = "ValueChanged",
                ["TextExpression"] = "ValueExpression",
            },
            TValueShape.Single,
            "TextEdit was renamed to TextInput and unified on Value / ValueChanged / ValueExpression binding." ) );

        list.Add( new ComponentMapping(
            "Blazorise.TimeEdit",
            "Blazorise.TimeInput",
            new Dictionary<string, string>
            {
                ["Time"] = "Value",
                ["TimeChanged"] = "ValueChanged",
                ["TimeExpression"] = "ValueExpression",
            },
            TValueShape.Single,
            "TimeEdit was renamed to TimeInput and unified on Value / ValueChanged / ValueExpression binding." ) );

        list.Add( new ComponentMapping(
            "Blazorise.Check`1",
            "Blazorise.Check`1",
            new Dictionary<string, string>
            {
                ["Checked"] = "Value",
                ["CheckedChanged"] = "ValueChanged",
                ["CheckedExpression"] = "ValueExpression",
            },
            TValueShape.Single,
            "Check now uses Value / ValueChanged / ValueExpression instead of Checked bindings." ) );

        list.Add( new ComponentMapping(
            "Blazorise.ColorPicker`1",
            "Blazorise.ColorPicker`1",
            new Dictionary<string, string>
            {
                ["Color"] = "Value",
                ["ColorChanged"] = "ValueChanged",
                ["ColorExpression"] = "ValueExpression",
            },
            TValueShape.Single,
            "ColorPicker unified its binding to Value / ValueChanged / ValueExpression." ) );

        list.Add( new ComponentMapping(
            "Blazorise.TimePicker",
            "Blazorise.TimePicker",
            new Dictionary<string, string>
            {
                ["Time"] = "Value",
                ["TimeChanged"] = "ValueChanged",
                ["TimeExpression"] = "ValueExpression",
            },
            TValueShape.Single,
            "TimePicker unified its binding to Value / ValueChanged / ValueExpression." ) );

        list.Add( new ComponentMapping(
            "Blazorise.RadioGroup`1",
            "Blazorise.RadioGroup`1",
            new Dictionary<string, string>
            {
                ["CheckedValue"] = "Value",
                ["CheckedValueChanged"] = "ValueChanged",
                ["CheckedValueExpression"] = "ValueExpression",
            },
            TValueShape.Single,
            "RadioGroup now uses Value / ValueChanged / ValueExpression; individual Radio components are not standalone binding roots." ) );

        list.Add( new ComponentMapping(
            "Blazorise.Radio",
            "Blazorise.Radio",
            new Dictionary<string, string>
            {
                ["Checked"] = "Value",
                ["CheckedChanged"] = "ValueChanged",
                ["CheckedExpression"] = "ValueExpression",
            },
            TValueShape.Any,
            "Radio removed Checked bindings and uses Value / ValueChanged / ValueExpression; should be used inside RadioGroup." ) );

        list.Add( new ComponentMapping(
            "Blazorise.Switch`1",
            "Blazorise.Switch`1",
            new Dictionary<string, string>
            {
                ["Checked"] = "Value",
                ["CheckedChanged"] = "ValueChanged",
                ["CheckedExpression"] = "ValueExpression",
            },
            TValueShape.Single,
            "Switch now uses Value / ValueChanged / ValueExpression instead of Checked bindings." ) );

        list.Add( new ComponentMapping(
            "Blazorise.DatePicker`1",
            "Blazorise.DatePicker`1",
            new Dictionary<string, string>
            {
                ["Date"] = "Value",
                ["Dates"] = "Value",
                ["DateChanged"] = "ValueChanged",
                ["DatesChanged"] = "ValueChanged",
                ["DateExpression"] = "ValueExpression",
                ["DatesExpression"] = "ValueExpression",
            },
            TValueShape.SingleOrMultiListOrArray,
            "DatePicker now uses Value / ValueChanged / ValueExpression for both single and multiple selection. For multiple selection, TValue must be IReadOnlyList<T> or T[] (e.g. DateTime[])." ) );

        list.Add( new ComponentMapping(
            "Blazorise.Select`1",
            "Blazorise.Select`1",
            new Dictionary<string, string>
            {
                ["SelectedValue"] = "Value",
                ["SelectedValues"] = "Value",
                ["SelectedValueChanged"] = "ValueChanged",
                ["SelectedValuesChanged"] = "ValueChanged",
                ["SelectedValueExpression"] = "ValueExpression",
                ["SelectedValuesExpression"] = "ValueExpression",
            },
            TValueShape.SingleOrMultiListOrArray,
            "Select now uses Value / ValueChanged / ValueExpression. For multiple selection, TValue must be IReadOnlyList<T> or T[] (e.g. string[] or int[])." ) );

        list.Add( new ComponentMapping(
            "Blazorise.SelectList`1",
            "Blazorise.SelectList`1",
            new Dictionary<string, string>
            {
                ["SelectedValue"] = "Value",
                ["SelectedValues"] = "Value",
                ["SelectedValueChanged"] = "ValueChanged",
                ["SelectedValuesChanged"] = "ValueChanged",
                ["SelectedValueExpression"] = "ValueExpression",
                ["SelectedValuesExpression"] = "ValueExpression",
            },
            TValueShape.SingleOrMultiListOrArray,
            "SelectList now uses Value / ValueChanged / ValueExpression. For multiple selection, TValue must be IReadOnlyList<T> or T[]." ) );

        list.Add( new ComponentMapping(
            "Blazorise.DropdownList`1",
            "Blazorise.DropdownList`1",
            new Dictionary<string, string>
            {
                ["SelectedValue"] = "Value",
                ["SelectedValues"] = "Value",
                ["SelectedValueChanged"] = "ValueChanged",
                ["SelectedValuesChanged"] = "ValueChanged",
                ["SelectedValueExpression"] = "ValueExpression",
                ["SelectedValuesExpression"] = "ValueExpression",
                ["RightAligned"] = "EndAligned",
            },
            TValueShape.SingleOrMultiListOrArray,
            "DropdownList now uses Value / ValueChanged / ValueExpression. For multiple selection, TValue must be IReadOnlyList<T> or T[]." ) );

        list.Add( new ComponentMapping(
            "Blazorise.Dropdown",
            "Blazorise.Dropdown",
            new Dictionary<string, string>
            {
                ["RightAligned"] = "EndAligned",
            },
            TValueShape.Any,
            "Dropdown uses EndAligned instead of RightAligned." ) );

        list.Add( new ComponentMapping(
            "Blazorise.Components.Autocomplete`2",
            "Blazorise.Components.Autocomplete`2",
            new Dictionary<string, string>
            {
                ["CurrentSearch"] = "Search",
                ["CurrentSearchChanged"] = "SearchChanged",
                ["MinLength"] = "MinSearchLength",
                ["Multiple"] = "SelectionMode",
            },
            TValueShape.Any,
            "Autocomplete now uses Search / SearchChanged and SelectionMode instead of CurrentSearch / CurrentSearchChanged and Multiple.",
            new Dictionary<string, string>
            {
                ["Validator"] = "Wrap Autocomplete in Validation instead of using the Validator parameter.",
                ["AsyncValidator"] = "Wrap Autocomplete in Validation instead of using the AsyncValidator parameter.",
            } ) );

        list.Add( new ComponentMapping(
            "Blazorise.CardLink",
            "Blazorise.CardLink",
            new Dictionary<string, string>
            {
                ["Source"] = "To",
                ["Alt"] = "Title",
            },
            TValueShape.Any,
            "CardLink now uses To instead of Source and Title instead of Alt." ) );

        list.Add( new ComponentMapping(
            "Blazorise.DataGrid.DataGrid`1",
            "Blazorise.DataGrid.DataGrid`1",
            new Dictionary<string, string>
            {
                ["CurrentPage"] = "Page",
                ["GroupRowStyling"] = "AggregateRowStyling",
                ["Navigable"] = "NavigationMode",
            },
            TValueShape.Any,
            "DataGrid uses AggregateRowStyling instead of GroupRowStyling and NavigationMode (e.g., NavigationMode.Row) instead of Navigable." ) );

        list.Add( new ComponentMapping(
            "Blazorise.DataGrid.DataGridColumn`1",
            "Blazorise.DataGrid.DataGridColumn`1",
            new Dictionary<string, string>
            {
                ["GroupCellClass"] = "AggregateCellClass",
                ["GroupCellStyle"] = "AggregateCellStyle",
                ["PopupFieldColumnSize"] = "EditFieldColumnSize",
            },
            TValueShape.Any,
            "DataGridColumn renamed grouping-related properties to Aggregate* and PopupFieldColumnSize to EditFieldColumnSize; Width also changed to a fluent sizing type." ) );

        // Legacy namespace fallbacks for migration from pre-2.0 projects.
        list.Add( new ComponentMapping(
            "Blazorise.DataGrid`1",
            "Blazorise.DataGrid.DataGrid`1",
            new Dictionary<string, string>
            {
                ["CurrentPage"] = "Page",
                ["GroupRowStyling"] = "AggregateRowStyling",
                ["Navigable"] = "NavigationMode",
            },
            TValueShape.Any,
            "Legacy Blazorise.DataGrid`1 maps to Blazorise.DataGrid.DataGrid`1; parameters follow the 2.0 migration guide." ) );

        list.Add( new ComponentMapping(
            "Blazorise.DataGridColumn`1",
            "Blazorise.DataGrid.DataGridColumn`1",
            new Dictionary<string, string>
            {
                ["GroupCellClass"] = "AggregateCellClass",
                ["GroupCellStyle"] = "AggregateCellStyle",
                ["PopupFieldColumnSize"] = "EditFieldColumnSize",
            },
            TValueShape.Any,
            "Legacy Blazorise.DataGridColumn`1 maps to Blazorise.DataGrid.DataGridColumn`1; parameters follow the 2.0 migration guide." ) );

        list.Add( new ComponentMapping(
            "Blazorise.MessageAlert",
            "Blazorise.MessageProvider",
            new Dictionary<string, string>(),
            TValueShape.Any,
            "MessageAlert has been replaced by MessageProvider." ) );

        list.Add( new ComponentMapping(
            "Blazorise.PageProgressAlert",
            "Blazorise.PageProgressProvider",
            new Dictionary<string, string>(),
            TValueShape.Any,
            "PageProgressAlert has been replaced by PageProgressProvider." ) );

        list.Add( new ComponentMapping(
            "Blazorise.NotificationAlert",
            "Blazorise.NotificationProvider",
            new Dictionary<string, string>(),
            TValueShape.Any,
            "NotificationAlert has been replaced by NotificationProvider." ) );

        list.Add( new ComponentMapping(
            "Blazorise.Row",
            "Blazorise.Row",
            new Dictionary<string, string>
            {
                ["HorizontalGutter"] = "Gutter",
                ["VerticalGutter"] = "Gutter",
                ["NoGutters"] = "Gutter",
            },
            TValueShape.Any,
            "Row now uses a single IFluentGutter on Gutter; HorizontalGutter, VerticalGutter and NoGutters have been replaced by the fluent gutter API." ) );

        list.Add( new ComponentMapping(
            "Blazorise.Fields",
            "Blazorise.Fields",
            new Dictionary<string, string>
            {
                ["HorizontalGutter"] = "Gutter",
                ["VerticalGutter"] = "Gutter",
                ["NoGutters"] = "Gutter",
            },
            TValueShape.Any,
            "Fields now uses a single IFluentGutter on Gutter; HorizontalGutter, VerticalGutter and NoGutters have been replaced by the fluent gutter API." ) );

        return list;
    }

    private static IReadOnlyList<TypeMapping> CreateTypeMappings()
    {
        var list = new List<TypeMapping>();

        list.Add( new TypeMapping(
            "Blazorise.BLMouseEventArgs",
            "Microsoft.AspNetCore.Components.Web.MouseEventArgs",
            "BLMouseEventArgs was removed; use MouseEventArgs from Microsoft.AspNetCore.Components.Web." ) );

        list.Add( new TypeMapping(
            "Blazorise.DataGridPageChangedEventArgs",
            null,
            "DataGridPageChangedEventArgs was removed in 2.0." ) );

        list.Add( new TypeMapping(
            "Blazorise.DataGrid.DataGridPageChangedEventArgs",
            null,
            "DataGridPageChangedEventArgs was removed in 2.0." ) );

        list.Add( new TypeMapping(
            "Blazorise.RichTextEdit.DynamicReference",
            null,
            "DynamicReference record was removed from RichTextEdit." ) );

        return list;
    }

    private static IReadOnlyList<SymbolMapping> CreateSymbolMappings()
    {
        var list = new List<SymbolMapping>();

        list.Add( new SymbolMapping(
            "Blazorise.SnackbarLocation",
            "Start",
            "BottomStart",
            "SnackbarLocation.Start was renamed to BottomStart." ) );

        list.Add( new SymbolMapping(
            "Blazorise.SnackbarLocation",
            "End",
            "BottomEnd",
            "SnackbarLocation.End was renamed to BottomEnd." ) );

        list.Add( new SymbolMapping(
            "Blazorise.SnackbarStackLocation",
            "Center",
            "Default",
            "SnackbarStackLocation.Center was renamed to Default." ) );

        list.Add( new SymbolMapping(
            "Blazorise.SnackbarStackLocation",
            "Start",
            "BottomStart",
            "SnackbarStackLocation.Start was renamed to BottomStart." ) );

        list.Add( new SymbolMapping(
            "Blazorise.SnackbarStackLocation",
            "End",
            "BottomEnd",
            "SnackbarStackLocation.End was renamed to BottomEnd." ) );

        list.Add( new SymbolMapping(
            "Blazorise.BlazoriseOptions",
            "LicenseKey",
            "ProductToken",
            "BlazoriseOptions.LicenseKey was renamed to ProductToken." ) );

        list.Add( new SymbolMapping(
            "Blazorise.Match",
            "Custom",
            null,
            "Match.Custom was removed." ) );

        list.Add( new SymbolMapping(
            "Blazorise.DataGrid.DataGrid`1",
            "CurrentPage",
            "Page",
            "DataGrid.CurrentPage was renamed to Page." ) );

        list.Add( new SymbolMapping(
            "Blazorise.DataGrid`1",
            "CurrentPage",
            "Page",
            "Legacy Blazorise.DataGrid`1.CurrentPage was renamed to Page." ) );

        list.Add( new SymbolMapping(
            "Blazorise.VideoMedia",
            "Size",
            "Height",
            "VideoMedia.Size was renamed to Height." ) );

        list.Add( new SymbolMapping(
            "Blazorise.DataGrid.CancellableRowChange`1",
            "Item",
            "OldItem",
            "DataGrid.CancellableRowChange.Item was renamed to OldItem." ) );

        list.Add( new SymbolMapping(
            "Blazorise.DataGrid.SavedRowItem`1",
            "Item",
            "OldItem",
            "DataGrid.SavedRowItem.Item was renamed to OldItem." ) );

        list.Add( new SymbolMapping(
            "Blazorise.RichTextEdit.RichTextEditOptions",
            "QuillJsVersion",
            null,
            "RichTextEditOptions.QuillJsVersion was removed; references are loaded dynamically." ) );

        list.Add( new SymbolMapping(
            "Blazorise.RichTextEdit.RichTextEditOptions",
            "DynamicallyLoadReferences",
            null,
            "RichTextEditOptions.DynamicallyLoadReferences was removed; references are loaded dynamically." ) );

        list.Add( new SymbolMapping(
            "Blazorise.RichTextEdit.RichTextEditOptions",
            "DynamicReferences",
            null,
            "RichTextEditOptions.DynamicReferences was removed." ) );

        list.Add( new SymbolMapping(
            "Blazorise.Charts.ChartAxisGridLine",
            "DrawBorder",
            "Border.Display",
            "Grid.DrawBorder moved to Border.Display." ) );

        list.Add( new SymbolMapping(
            "Blazorise.Charts.ChartAxisGridLine",
            "BorderWidth",
            "Border.Width",
            "Grid.BorderWidth moved to Border.Width." ) );

        list.Add( new SymbolMapping(
            "Blazorise.Charts.ChartAxisGridLine",
            "BorderColor",
            "Border.Color",
            "Grid.BorderColor moved to Border.Color." ) );

        list.Add( new SymbolMapping(
            "Blazorise.Charts.ChartAxisGridLine",
            "BorderDash",
            "Border.Dash",
            "Grid.BorderDash moved to Border.Dash." ) );

        list.Add( new SymbolMapping(
            "Blazorise.Charts.ChartAxisGridLine",
            "BorderDashOffset",
            "Border.DashOffset",
            "Grid.BorderDashOffset moved to Border.DashOffset." ) );

        return list;
    }
}
