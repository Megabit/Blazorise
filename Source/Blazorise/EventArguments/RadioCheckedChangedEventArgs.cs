#region Using directives
using System;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Provides the information about the currently checked radio.
    /// </summary>
    public class RadioCheckedChangedEventArgs<TValue> : EventArgs
    {
        /// <summary>
        /// A default <see cref="RadioCheckedChangedEventArgs{TValue}"/> constructor.
        /// </summary>
        /// <param name="value">Radio value.</param>
        public RadioCheckedChangedEventArgs( TValue value )
        {
            Value = value;
        }

        /// <summary>
        /// Gets the checked radio value.
        /// </summary>
        public TValue Value { get; }
    }
}
