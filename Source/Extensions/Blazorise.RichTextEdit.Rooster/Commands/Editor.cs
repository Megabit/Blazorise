namespace Blazorise.RichTextEdit.Rooster.Commands;

/// <summary>
/// Rooster format commands. 
/// </summary>
/// <see href="https://microsoft.github.io/roosterjs/docs/interfaces/roosterjs_editor_types.ieditor.html"/>
public class Editor
{
    private readonly RichTextEdit editor;

    internal Editor( RichTextEdit editor )
    {
        this.editor = editor;
    }

    /// <summary>
    /// Undo last edit operation.
    /// </summary>
    public RichTextEditCommand Undo => new( editor, "undo" );
    /// <summary>
    /// Redo next edit operation.
    /// </summary>
    public RichTextEditCommand Redo => new( editor, "redo" );
    /// <summary>
    /// Focus to this editor, the selection was restored to where it was before, no unexpected scroll.
    /// </summary>
    public RichTextEditCommand Focus => new( editor, "focus" );
    /// <summary>
    /// Delete selected content
    /// </summary>
    public RichTextEditCommand DeleteSelectedContent => new( editor, "deleteSelectedContent" );
    /// <summary>
    /// Set current zoom scale, default value is 1.
    /// </summary>
    public RichTextEditCommand SetZoomScale => new( editor, "setZoomScale" );
    /// <summary>
    /// Clears the editor.
    /// </summary>
    public RichTextEditCommand Clear => new( editor, "setContent" );
}