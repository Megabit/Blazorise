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
    /// <param name="editor">Initialized editor instance.</param>
    public CodeEditorReadyEventArgs( CodeEditor editor )
    {
        ArgumentNullException.ThrowIfNull( editor );

        Editor = editor;
    }

    /// <summary>
    /// Gets the initialized editor instance.
    /// </summary>
    public CodeEditor Editor { get; }

    /// <summary>
    /// Gets the editor element id.
    /// </summary>
    public string ElementId => Editor.ElementId;

    /// <summary>
    /// Gets the editor element reference.
    /// </summary>
    public ElementReference ElementRef => Editor.ElementRef;
}