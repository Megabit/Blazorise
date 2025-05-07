#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise.RichTextEdit;

/// <summary>
/// Blazorise RichTextEdit extension options
/// </summary>
public sealed class RichTextEditOptions
{
    /// <summary>
    /// Load the QuillJs snow theme related resources.
    /// </summary>
    public bool UseShowTheme { get; set; } = true;

    /// <summary>
    /// Load the QuillJs bubble theme related resources.
    /// </summary>
    public bool UseBubbleTheme { get; set; }

    /// <summary>
    /// If true enables the QuillJs table module. Please be aware that this module is not part of the core QuillJs library, and it is still experimental.
    /// </summary>
    public bool UseTables { get; set; }

    /// <summary>
    /// If true enables the QuillJs image resize module. Please be aware that this module is not part of the core QuillJs library, and it is still experimental.
    /// </summary>
    public bool UseResize { get; set; }

    /// <summary>
    /// The QuillJs version to load
    /// </summary>
    [Obsolete( "Dynamic loading no longer used." )]
    public string QuillJsVersion { get; set; } = "2.0.0";

    /// <summary>
    /// Load the RichTextEdit scripts and stylesheets on demand.
    /// </summary>
    [Obsolete( "Dynamic loading no longer used." )]
    public bool DynamicallyLoadReferences { get; set; } = false;

    /// <summary>
    /// Dynamic references to be loaded when initializing the RichTextEdit component.
    /// </summary>
    [Obsolete( "Dynamic loading no longer used." )]
    public List<DynamicReference> DynamicReferences { get; set; } = new();
}