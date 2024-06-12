namespace Blazorise.RichTextEdit.Rooster.Commands;

/// <summary>
/// Rooster format commands. 
/// </summary>
/// <seealso href="https://microsoft.github.io/roosterjs/docs/modules/roosterjs_editor_api.html"/>
public sealed class Format
{
    internal Format( RichTextEdit editor )
    {
        ChangeCapitalization = new( editor );
        ChangeFontSize = new( editor );
        ClearFormat = new( editor );
        SetBackgroundColor = new( editor, "setBackgroundColor" );
        SetFontName = new( editor, "setFontName" );
        SetFontSize = new( editor );
        SetHeadingLevel = new( editor );
        SetIndentation = new( editor );
        SetTextColor = new( editor, "setTextColor" );
        ToggleBlockQuote = new( editor, "toggleBlockQuote" );
        ToggleBold = new( editor, "toggleBold" );
        ToggleBullet = new( editor );
        ToggleCodeBlock = new( editor, "toggleCodeBlock" );
        ToggleItalic = new( editor, "toggleItalic" );
        ToggleStrikethrough = new( editor, "toggleStrikethrough" );
        ToggleSubscript = new( editor, "toggleSubscript" );
        ToggleSuperscript = new( editor, "toggleSuperscript" );
        ToggleUnderline = new( editor, "toggleUnderline" );
        ApplyCellShading = new( editor, "applyCellShading" );
    }

    /// <summary>
    /// Set background color of cells.
    /// </summary>
    public SetColorCommand ApplyCellShading { get; }

    // /// <summary>
    // /// Split selection into regions, and perform a block-wise formatting action for each region.
    // /// </summary>
    // TODO public RichTextEditCommand BlockFormat { get; } = new( editor, "blockFormat" );

    ///<summary>
    /// Change the capitalization of text in the selection
    /// </summary>
    public ChangeCapitalizationCommand ChangeCapitalization { get; }

    ///<summary>
    /// Increase or decrease font size in selection
    /// </summary>
    public ChangeFontSizeCommand ChangeFontSize { get; }

    /// <summary>
    /// Clear the format in current selection, after cleaning, the format will be changed to default format.
    /// </summary>
    public ClearFormatCommand ClearFormat { get; }

    // /// <summary>
    // /// Commit changes of all list changes when experiment features are allowed
    // /// </summary>
    // TODO public RichTextEditCommand CommitListChains { get; } = new(editor, "commitListChains");

    // /// <summary>
    // /// Insert a hyperlink at cursor. 
    // /// </summary>
    // TODO public RichTextEditCommand CreateLink { get; } = new(editor, "createLink");

    // /// <summary>
    // /// Edit table with given operation. If there is no table at cursor then no op.
    // /// </summary>
    // TODO public RichTextEditCommand EditTable { get; } = new(editor, "editTable");

    /// <summary>
    /// Set background color at current selection
    /// </summary>
    public SetColorCommand SetBackgroundColor { get; }

    /// <summary>
    /// Set font name at selection.
    /// </summary>
    public RichTextEditCommand SetFontName { get; }

    /// <summary>
    /// Set font size at selection.
    /// </summary>
    public SetFontSizeCommand SetFontSize { get; }

    /// <summary>
    /// Set heading level at selection
    /// </summary>
    public SetHeadingLevelCommand SetHeadingLevel { get; }

    /// <summary>
    /// Set indentation at selection.
    /// </summary>
    public SetIndentationCommand SetIndentation { get; }

    /// <summary>
    /// Set text color at selection
    /// </summary>
    public SetColorCommand SetTextColor { get; }

    /// <summary>
    /// Toggle blockquote at selection.
    /// </summary>
    public RichTextEditCommand ToggleBlockQuote { get; }

    /// <summary>
    /// Toggle bold at selection.
    /// </summary>
    public RichTextEditCommand ToggleBold { get; }

    /// <summary>
    /// Toggle bullet at selection
    /// </summary>
    public ToggleBulletCommand ToggleBullet { get; }

    /// <summary>
    /// Toggle code block at selection.
    /// </summary>
    public RichTextEditCommand ToggleCodeBlock { get; }

    /// <summary>
    /// Toggle italic at selection.
    /// </summary>
    public RichTextEditCommand ToggleItalic { get; }

    /// <summary>
    /// Toggle strikethrough at selection.
    /// </summary>
    public RichTextEditCommand ToggleStrikethrough { get; }

    /// <summary>
    /// Toggle subscript at selection.
    /// </summary>
    public RichTextEditCommand ToggleSubscript { get; }

    /// <summary>
    /// Toggle superscript at selection.
    /// </summary>
    public RichTextEditCommand ToggleSuperscript { get; }

    /// <summary>
    /// Toggle underline at selection.
    /// </summary>
    public RichTextEditCommand ToggleUnderline { get; }
}