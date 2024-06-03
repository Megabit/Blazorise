using System;

namespace Blazorise;

/// <summary>
/// Represents an attribute that can be applied to date properties or fields to specify additional metadata.
/// </summary>
[AttributeUsage( AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false )]
public sealed class DateAttribute : Attribute
{
    /// <summary>
    /// Hints at the type of data that might be entered by the user while editing the element or its contents.
    /// </summary>
    public DateInputMode InputMode { get; set; }
}
