namespace Blazorise.RichTextEdit.Rooster.Commands;

/// <summary>
/// Toggle bullet at selection
/// </summary>
public class ToggleBulletCommand : RichTextEditCommand<BulletListType?>
{
    internal ToggleBulletCommand( RichTextEdit editor ) : base( editor, "toggleBullet" )
    {
    }

    /// <inheritdoc />
    protected override object Transform( BulletListType? argument )
    => argument.HasValue ? (int)argument : null;
}