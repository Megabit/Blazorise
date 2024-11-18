using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Blazorise.DataGrid;

public class DataGridSelectColumn<TItem> : DataGridColumn<TItem>
{
    public override DataGridColumnType ColumnType => DataGridColumnType.Select;

    /// <summary>
    /// Gets or sets the select data-source.
    /// </summary>
    [Parameter] public IEnumerable<object> Data { get; set; }

    /// <summary>
    /// Method used to get the display field from the supplied data source.
    /// <para>You can unbox the object by using the following example: TextField="(x) => ((Model)x).Text"</para>
    /// </summary>
    [Parameter] public Func<object, string> TextField { get; set; }

    /// <summary>
    /// Method used to get the value field from the supplied data source.
    ///<para> You can unbox the object by using the following example: ValueField="(x) => ((Model)x).Value"</para>
    /// </summary>
    [Parameter] public Func<object, object> ValueField { get; set; }

    /// <summary>
    /// Method used to determine if an item should be disabled.
    /// <para>You can unbox the object by using the following example: ItemDisabled="(x) => !((Model)x).Enabled"</para>
    /// </summary>
    [Parameter] public Func<object, bool> ItemDisabled { get; set; }

    /// <summary>
    /// Display text of the default select item.
    /// </summary>
    [Parameter] public string DefaultItemText { get; set; }

    /// <summary>
    /// Value of the default select item.
    /// </summary>
    [Parameter] public object DefaultItemValue { get; set; }

    /// <summary>
    /// If true, disables the default item.
    /// </summary>
    [Parameter] public bool DefaultItemDisabled { get; set; } = false;

    /// <summary>
    /// If true, disables the default item.
    /// </summary>
    [Parameter] public bool DefaultItemHidden { get; set; } = false;

    /// <summary>
    /// Custom css class-names.
    /// </summary>
    [Parameter] public string Class { get; set; }

    /// <summary>
    /// Custom styles.
    /// </summary>
    [Parameter] public string Style { get; set; }

    /// <summary>
    /// Size of a select field.
    /// </summary>
    [Parameter] public Size? Size { get; set; }

    /// <summary>
    /// Specifies how many options should be shown at once.
    /// </summary>
    [Parameter] public int? MaxVisibleItems { get; set; }

    /// <summary>
    /// If defined, indicates that its element can be focused and can participates in sequential keyboard navigation.
    /// </summary>
    [Parameter] public int? TabIndex { get; set; }

    /// <summary>
    /// Add the disabled boolean attribute on an select to prevent user interactions and make it appear lighter.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Specifies that multiple items can be selected.
    /// </summary>
    [Parameter]
    public bool Multiple { get; set; }

    /// <summary>
    /// Captures all the custom attribute that are not part of Blazorise component.
    /// </summary>
    [Parameter( CaptureUnmatchedValues = true )]
    public Dictionary<string, object> Attributes { get; set; }
}