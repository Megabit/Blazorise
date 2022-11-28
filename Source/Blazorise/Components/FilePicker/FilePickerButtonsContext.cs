#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Context for File Picker's Buttons
/// </summary>
public class FilePickerButtonsContext
{
    /// <summary>
    /// Default context constructor.
    /// </summary>
    public FilePickerButtonsContext()
    {
    }

    /// <summary>
    /// Default context constructor.
    /// </summary>
    /// <param name="clear">The Clear EventCallback</param>
    /// <param name="upload">The Upload EventCallback</param>
    /// <param name="cancel">The Cancel EventCallback.</param>
    public FilePickerButtonsContext( EventCallback clear, EventCallback upload, EventCallback cancel )
    {
        Clear = clear;
        Upload = upload;
        Cancel = cancel;
    }

    /// <summary>
    /// Activates the upload event.
    /// </summary>
    public EventCallback Upload { get; }

    /// <summary>
    /// Activates the clear event.
    /// </summary>
    public EventCallback Clear { get; }

    /// <summary>
    /// Cancels ongoing upload.
    /// </summary>
    public EventCallback Cancel { get; }
}