namespace Blazorise.RichTextEdit.Rooster.Commands;

///<summary>
/// Change the capitalization of text in the selection
/// </summary>
public class ChangeCapitalizationCommand : RichTextEditCommand<Capitalization>
{
    internal ChangeCapitalizationCommand( RichTextEdit editor ) : base( editor, "changeCapitalization" )
    {
    }

    /// <inheritdoc />
    protected override object Transform( Capitalization argument )
        => argument.Value;
}