namespace Blazorise.RichTextEdit.Rooster.Commands;

/// <summary>
/// Set font size at selection.
/// </summary>
public class SetFontSizeCommand : RichTextEditCommand<object>
{
    internal SetFontSizeCommand( RichTextEdit editor ) : base( editor, "setFontSize" )
    {
    }

    /// <inheritdoc />
    protected override object Transform( object argument ) => argument switch
    {
        int pixels => $"{pixels}px",
        string size => size,
        _ => argument
    };
}