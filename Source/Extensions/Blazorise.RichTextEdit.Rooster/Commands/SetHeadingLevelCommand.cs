namespace Blazorise.RichTextEdit.Rooster.Commands;

/// <summary>
/// Set heading level at selection
/// </summary>
public class SetHeadingLevelCommand : RichTextEditCommand<HeadingSize?>
{
    internal SetHeadingLevelCommand( RichTextEdit editor ) : base( editor, "setHeadingLevel" )
    {
    }

    /// <inheritdoc />
    protected override object Transform( HeadingSize? argument ) => argument switch
    {
        HeadingSize.Is1 => 1,
        HeadingSize.Is2 => 2,
        HeadingSize.Is3 => 3,
        HeadingSize.Is4 => 4,
        HeadingSize.Is5 => 5,
        HeadingSize.Is6 => 6,
        _ => default
    };
}