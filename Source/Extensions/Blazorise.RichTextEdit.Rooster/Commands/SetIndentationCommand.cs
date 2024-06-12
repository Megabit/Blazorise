namespace Blazorise.RichTextEdit.Rooster.Commands;

/// <summary>
/// Set indentation at selection.
/// </summary>

public class SetIndentationCommand : RichTextEditCommand<Indentation?>
{
    internal SetIndentationCommand( RichTextEdit editor ) : base( editor, "setIndentation" )
    {
    }

    /// <inheritdoc />
    protected override object Transform( Indentation? argument )
        => argument.HasValue ? (int)argument : null;
}