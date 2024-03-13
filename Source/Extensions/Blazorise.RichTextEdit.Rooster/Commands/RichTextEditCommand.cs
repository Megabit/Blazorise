using System;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;

namespace Blazorise.RichTextEdit.Rooster.Commands;

/// <summary>
/// Command wrapper for RichTextEdit actions.
/// </summary>
public abstract class RichTextEditCommand<T> : RichTextEditCommand
{
    internal RichTextEditCommand( RichTextEdit editor, string action )
        : base( editor, action )
    {
    }

    /// <inheritdoc />
    protected sealed override object TransformArgument( object argument )
    {
        return base.TransformArgument( Transform( (T)argument ) );
    }

    /// <summary>
    /// Transform the argument value
    /// </summary>
    protected abstract object Transform( T argument );
}

/// <summary>
/// Command wrapper for RichTextEdit actions.
/// </summary>
public class RichTextEditCommand : ICommand
{
    private readonly RichTextEdit editor;
    private readonly string action;

    /// <inheritdoc />
    public event EventHandler CanExecuteChanged;

    internal RichTextEditCommand( RichTextEdit editor, string action )
    {
        this.editor = editor;
        this.action = action;
    }

    /// <summary>
    /// Is the command disabled
    /// </summary>
    public bool Disabled { get; private set; }

    /// <inheritdoc />
    public virtual bool CanExecute( object parameter )
    {
        // TODO
        return true;
    }

    /// <inheritdoc />
    public virtual async void Execute( object parameter )
    {
        var argument = TransformArgument( parameter );
        await editor.JSModule.Format( editor.ElementRef, editor.ElementId, action, argument );
    }

    /// <summary>
    /// Convert command to eventcallback for binding to Clicked etc events
    /// </summary>
    /// <param name="cmd">the command to bind</param>
    public static implicit operator EventCallback<object>( RichTextEditCommand cmd )
        => EventCallback.Factory.Create<object>( cmd, cmd.Execute );

    /// <summary>
    /// Transform the argument value before executing the action.
    /// </summary>
    /// <param name="argument">the value to transform</param>
    /// <returns>the transformed value</returns>
    protected virtual object TransformArgument( object argument ) => argument;
}