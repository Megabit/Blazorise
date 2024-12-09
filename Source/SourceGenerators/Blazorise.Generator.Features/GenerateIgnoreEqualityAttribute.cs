using System;

namespace Blazorise.Generator.Features;

[AttributeUsage( AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event )]
public class GenerateIgnoreEqualityAttribute : Attribute;