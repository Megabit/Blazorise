namespace Blazorise.RichTextEdit.Rooster.Commands;

/// <summary>
/// Rooster format commands. 
/// </summary>
/// <seealso href="https://microsoft.github.io/roosterjs/docs/modules/roosterjs_editor_api.html"/>
public sealed class Format
{
    private readonly RichTextEdit editor;

    internal Format( RichTextEdit editor )
    {
        this.editor = editor;
    }

    /// <summary>
    /// Set background color of cells.
    /// </summary>
    public SetColorCommand ApplyCellShading => new( editor, "applyCellShading" );

    // /// <summary>
    // /// Split selection into regions, and perform a block-wise formatting action for each region.
    // /// </summary>
    // TODO public RichTextEditCommand BlockFormat => new( editor, "blockFormat" );

    ///<summary>
    /// Change the capitalization of text in the selection
    /// </summary>
    public ChangeCapitalizationCommand ChangeCapitalization => new( editor );

    ///<summary>
    /// Increase or decrease font size in selection
    /// </summary>
    public ChangeFontSizeCommand ChangeFontSize => new( editor );

    /// <summary>
    /// Clear the format in current selection, after cleaning, the format will be changed to default format.
    /// </summary>
    public ClearFormatCommand ClearFormat => new( editor );

    // /// <summary>
    // /// Commit changes of all list changes when experiment features are allowed
    // /// </summary>
    // TODO public RichTextEditCommand CommitListChains => new(editor, "commitListChains");

    // /// <summary>
    // /// Insert a hyperlink at cursor. 
    // /// </summary>
    // TODO public RichTextEditCommand CreateLink => new(editor, "createLink");

    /// <summary>
    /// Set background color at current selection
    /// </summary>
    public SetColorCommand SetBackgroundColor => new( editor, "setBackgroundColor" );

    /// <summary>
    /// Set font name at selection.
    /// </summary>
    public RichTextEditCommand SetFontName => new( editor, "setFontName" );

    /// <summary>
    /// Set font size at selection.
    /// </summary>
    public RichTextEditCommand SetFontSize => new( editor, "setFontSize" );

    /// <summary>
    /// Set heading level at selection
    /// </summary>
    public SetHeadingLevelCommand SetHeadingLevel => new( editor );

    /// <summary>
    /// Set indentation at selection.
    /// </summary>
    public SetIndentationCommand SetIndentation => new( editor );

    /// <summary>
    /// Set text color at selection
    /// </summary>
    public SetColorCommand SetTextColor => new( editor, "setTextColor" );

    /// <summary>
    /// Toggle blockquote at selection.
    /// </summary>
    public RichTextEditCommand ToggleBlockQuote => new( editor, "toggleBlockQuote" );

    /// <summary>
    /// Toggle bold at selection.
    /// </summary>
    public RichTextEditCommand ToggleBold => new( editor, "toggleBold" );

    /// <summary>
    /// Toggle bullet at selection
    /// </summary>
    public ToggleBulletCommand ToggleBullet => new( editor );

    /// <summary>
    /// Toggle code block at selection.
    /// </summary>
    public RichTextEditCommand ToggleCodeBlock => new( editor, "toggleCodeBlock" );

    /// <summary>
    /// Toggle italic at selection.
    /// </summary>
    public RichTextEditCommand ToggleItalic => new( editor, "toggleItalic" );

    /// <summary>
    /// Toggle strikethrough at selection.
    /// </summary>
    public RichTextEditCommand ToggleStrikethrough => new( editor, "toggleStrikethrough" );

    /// <summary>
    /// Toggle subscript at selection.
    /// </summary>
    public RichTextEditCommand ToggleSubscript => new( editor, "toggleSubscript" );

    /// <summary>
    /// Toggle superscript at selection.
    /// </summary>
    public RichTextEditCommand ToggleSuperscript => new( editor, "toggleSuperscript" );

    /// <summary>
    /// Toggle underline at selection.
    /// </summary>
    public RichTextEditCommand ToggleUnderline => new( editor, "toggleUnderline" );
}