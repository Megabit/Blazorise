using System;
using Microsoft.AspNetCore.Components;

namespace Blazorise.CodeEditor;

/// <summary>
/// Provides information for the code editor ready event.
/// </summary>
public class CodeEditorReadyEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of <see cref="CodeEditorReadyEventArgs"/>.
    /// </summary>
    /// <param name="elementId">Editor element id.</param>
    /// <param name="elementRef">Editor element reference.</param>
    public CodeEditorReadyEventArgs( string elementId, ElementReference elementRef )
    {
        ElementId = elementId;
        ElementRef = elementRef;
    }

    /// <summary>
    /// Gets the editor element id.
    /// </summary>
    public string ElementId { get; }

    /// <summary>
    /// Gets the editor element reference.
    /// </summary>
    public ElementReference ElementRef { get; }
}