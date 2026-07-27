#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Provides state and command execution for a custom report toolbar item.
/// </summary>
public sealed class ReportToolbarItemContext
{
    #region Members

    private readonly EventCallback execute;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a report toolbar item template context.
    /// </summary>
    /// <param name="command">Report command represented by the item.</param>
    /// <param name="text">Resolved item text.</param>
    /// <param name="icon">Resolved item icon.</param>
    /// <param name="canExecute">Whether the command can currently execute.</param>
    /// <param name="active">Whether the command represents the active report state.</param>
    /// <param name="execute">Callback that executes the command.</param>
    public ReportToolbarItemContext( ReportCommand command, string text, IconName? icon, bool canExecute, bool active, EventCallback execute )
    {
        Command = command;
        Text = text;
        Icon = icon;
        CanExecute = canExecute;
        Active = active;
        this.execute = execute;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes the report command when it is currently available.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Execute()
        => CanExecute ? execute.InvokeAsync() : Task.CompletedTask;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the report command represented by the item.
    /// </summary>
    public ReportCommand Command { get; }

    /// <summary>
    /// Gets the resolved item text.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the resolved item icon.
    /// </summary>
    public IconName? Icon { get; }

    /// <summary>
    /// Gets whether the command can currently execute.
    /// </summary>
    public bool CanExecute { get; }

    /// <summary>
    /// Gets whether the command represents the active report state.
    /// </summary>
    public bool Active { get; }

    #endregion
}