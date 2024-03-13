using System;

namespace Blazorise.RichTextEdit;

/// <summary>
/// Dynamic reference definition. <see cref="RichTextEditOptions.DynamicReferences"/>
/// </summary>
[Obsolete( "Dynamic loading no longer used." )]
public record DynamicReference( string Uri, DynamicReferenceType Type );