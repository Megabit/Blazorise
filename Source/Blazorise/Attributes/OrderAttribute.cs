using System;

namespace Blazorise;

/// <summary>
/// Configures the field order.
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