using System;
using System.Collections.Generic;

namespace Blazorise;

/// <summary>
/// Represents an attribute that can be applied to properties or fields to specify additional metadata for a select component.
/// </summary>
[AttributeUsage( AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false )]
public sealed class SelectAttribute : Attribute
{
    /// <summary>
    /// The name of the function that will be used to get the data source.
    /// By default it's set to "GetSelectData".
    /// </summary>
    public string GetDataFunction { get; set; } = "GetSelectData";

    /// <summary>
    /// Used to get the display field from the supplied data source.
    /// Defaults to "Description".
    /// </summary>
    public string TextField { get; set; } = "Description";

    /// <summary>
    /// Used to get the value field from the supplied data source.
    /// Defaults to "Id".
    /// </summary>
    public string ValueField { get; set; } = "Id";

    /// <summary>
    /// Specifies how many options should be shown at once.
    /// 0 means that all options will be shown.
    /// Defaults to 0.
    /// </summary>
    public int MaxVisibleItems { get; set; } = 0;
}
