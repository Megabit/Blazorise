#region Using directives
using System;
using System.Threading.Tasks;
#endregion

namespace Blazorise;

/// <summary>
/// Describes the input currently controlled by the on-screen keyboard.
/// </summary>
public class OnScreenKeyboardContext
{
    /// <summary>
    /// Gets or sets the focused input element id.
    /// </summary>
    public string ElementId { get; set; }

    /// <summary>
    /// Gets or sets the focused component type.
    /// </summary>
    public Type ComponentType { get; set; }

    /// <summary>
    /// Gets or sets the keyboard layout.
    /// </summary>
    public OnScreenKeyboardLayout Layout { get; set; } = OnScreenKeyboardLayout.Text;

    /// <summary>
    /// Gets or sets the current input value accessor.
    /// </summary>
    public Func<string> GetValue { get; set; }

    /// <summary>
    /// Gets or sets the current input value mutator.
    /// </summary>
    public Func<string, Task> SetValue { get; set; }

    /// <summary>
    /// Gets or sets the current input preview value accessor.
    /// </summary>
    public Func<string> GetPreviewValue { get; set; }

    /// <summary>
    /// Gets or sets a callback that inserts text into the current input.
    /// </summary>
    public Func<string, Task> InsertText { get; set; }

    /// <summary>
    /// Gets or sets a callback that removes text from the current input.
    /// </summary>
    public Func<Task> Backspace { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the enter key is pressed.
    /// </summary>
    public Func<Task> Enter { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the current input should submit.
    /// </summary>
    public Func<Task> Submit { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the input is disabled.
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the input is read-only.
    /// </summary>
    public bool ReadOnly { get; set; }
}