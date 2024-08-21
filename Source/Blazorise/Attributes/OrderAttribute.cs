using System;

namespace Blazorise;

/// <summary>
/// Represents an attribute that can be applied to properties or fields to specify the order in which they should be displayed or edited.
/// </summary>
[AttributeUsage( AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false )]
public sealed class OrderAttribute : Attribute
{
    /// <summary>
    /// The order in which the field should be displayed.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// The order in which the field should be edited.
    /// </summary>
    public int EditOrder { get; set; }
}