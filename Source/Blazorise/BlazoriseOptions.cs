#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Blazorise
{
    public class BlazoriseOptions
    {
        /// <summary>
        /// If true the text in <see cref="TextEdit"/> will be changed after each key press.
        /// </summary>
        public bool ChangeTextOnKeyPress { get; set; } = true;

        /// <summary>
        /// If true the entered into <see cref="TextEdit"/> will be slightly delayed before submiting it to the internal value.
        /// </summary>
        public bool? DelayTextOnKeyPress { get; set; } = false;

        /// <summary>
        /// Interval in milliseconds that entered text will be delayed from submiting to the <see cref="TextEdit"/> internal value.
        /// </summary>
        public int? DelayTextOnKeyPressInterval { get; set; } = 300;

        /// <summary>
        /// If true the value in <see cref="Slider{TValue}"/> will be changed while holding and moving the slider.
        /// </summary>
        public bool ChangeSliderOnHold { get; set; } = true;
    }
}
