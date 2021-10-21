#region Using directives
using System;
#endregion

namespace Blazorise.Markdown
{
    /// <summary>
    /// Supplies the information about the markdown button click event.
    /// </summary>
    public class MarkdownButtonEventArgs : EventArgs
    {
        /// <summary>
        /// A default <see cref="MarkdownButtonEventArgs"/> constructor.
        /// </summary>
        /// <param name="name">Button action name.</param>
        /// <param name="value">Button value.</param>
        public MarkdownButtonEventArgs( string name, object value )
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Gets the button action name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the button action value.
        /// </summary>
        public object Value { get; }
    }
}
