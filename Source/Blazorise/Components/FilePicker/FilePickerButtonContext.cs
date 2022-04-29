using Microsoft.AspNetCore.Components;

namespace Blazorise
{

    /// <summary>
    /// Context for File Picker's Buttons
    /// </summary>
    public class FilePickerButtonContext
    {
        /// <summary>
        /// Default context constructor.
        /// </summary>
        public FilePickerButtonContext(  )
        {
        }

        /// <summary>
        /// Default context constructor.
        /// </summary>
        /// <param name="clear">The Clear EventCallback</param>
        /// <param name="upload">The Upload EventCallback</param>
        public FilePickerButtonContext( EventCallback clear, EventCallback upload )
        {
            Clear = clear;
            Upload = upload;
        }

        /// <summary>
        /// Activates the upload event.
        /// </summary>
        public EventCallback Upload { get; }

        /// <summary>
        /// Activates the clear event.
        /// </summary>
        public EventCallback Clear { get; }
    }
}