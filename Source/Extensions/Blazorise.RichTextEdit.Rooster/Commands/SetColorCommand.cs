using Microsoft.AspNetCore.Components;

namespace Blazorise.RichTextEdit.Rooster.Commands;

/// <summary>
/// Set a color value
/// </summary>
public class SetColorCommand : RichTextEditCommand<string>
{
    internal SetColorCommand( RichTextEdit editor, string action ) : base( editor, action )
    {
    }

    /// <inheritdoc />
    protected override object Transform( string argument )
        => argument;

    /// <summary>
    /// Convert command to eventcallback for binding to events
    /// </summary>
    /// <param name="cmd">the command to bind</param>
    public static implicit operator EventCallback<string>( SetColorCommand cmd )
        => EventCallback.Factory.Create<string>( cmd, cmd.Execute );
}