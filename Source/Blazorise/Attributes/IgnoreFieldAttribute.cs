using System;

namespace Blazorise;

/// <summary>
/// Represents an attribute that can be applied to properties or fields to specify they should not be automatically generated. 
/// </summary>
[AttributeUsage( AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false )]
public sealed class IgnoreFieldAttribute : Attribute
{
}
