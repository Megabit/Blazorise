namespace Blazorise.RichTextEdit.Rooster.Commands;

/// <summary>
/// Clear the format in current selection, after cleaning, the format will be changed to default format.
/// </summary>
public class ClearFormatCommand : RichTextEditCommand<ClearFormatMode?>
{
    internal ClearFormatCommand( RichTextEdit editor ) : base( editor, "clearFormat" )
    {
    }

    /// <inheritdoc />
    protected override object Transform( ClearFormatMode? argument )
        => argument.HasValue ? (int)argument : null;
}