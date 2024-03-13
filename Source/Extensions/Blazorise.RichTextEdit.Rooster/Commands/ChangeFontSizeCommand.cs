namespace Blazorise.RichTextEdit.Rooster.Commands;

///<summary>
/// Increase or decrease font size in selection
/// </summary>
public class ChangeFontSizeCommand : RichTextEditCommand<FontSizeChange?>
{
    internal ChangeFontSizeCommand( RichTextEdit editor ) : base( editor, "changeFontSize" )
    {
    }

    /// <inheritdoc />
    protected override object Transform( FontSizeChange? argument )
        => argument.HasValue ? (int)argument : null;
}