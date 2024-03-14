namespace Blazorise.RichTextEdit.Rooster.Commands;

/// <summary>
/// Rooster format commands. 
/// </summary>
/// <see href="https://microsoft.github.io/roosterjs/docs/interfaces/roosterjs_editor_types.ieditor.html"/>
public class Editor
{
    internal Editor( RichTextEdit editor )
    {
        Undo = new( editor, "undo", x => x.FormatState.CanUndo );
        Redo = new( editor, "redo", x => x.FormatState.CanRedo );
        Focus = new( editor, "focus" );
        DeleteSelectedContent = new( editor, "deleteSelectedContent" );
        SetZoomScale = new( editor, "setZoomScale" );
        Clear = new( editor, "setContent" );
    }

    /// <summary>
    /// Undo last edit operation.
    /// </summary>
    public RichTextEditCommand Undo { get; }
    /// <summary>
    /// Redo next edit operation.
    /// </summary>
    public RichTextEditCommand Redo { get; }
    /// <summary>
    /// Focus to this editor, the selection was restored to where it was before, no unexpected scroll.
    /// </summary>
    public RichTextEditCommand Focus { get; }
    /// <summary>
    /// Delete selected content
    /// </summary>
    public RichTextEditCommand DeleteSelectedContent { get; }
    /// <summary>
    /// Set current zoom scale, default value is 1.
    /// </summary>
    public RichTextEditCommand SetZoomScale { get; }
    /// <summary>
    /// Clears the editor.
    /// </summary>
    public RichTextEditCommand Clear { get; }
}