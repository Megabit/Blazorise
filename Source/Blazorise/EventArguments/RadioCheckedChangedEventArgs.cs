#region Using directives
using System;
using System.ComponentModel;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Provides the information about the currently checked radio.
    /// </summary>
    public class RadioCheckedChangedEventArgs<TValue> : EventArgs
    {
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
