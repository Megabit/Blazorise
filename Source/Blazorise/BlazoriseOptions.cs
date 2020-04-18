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
        /// If true the value in <see cref="Slider{TValue}"/> will be changed while holding and moving the slider.
        /// </summary>
        public bool ChangeSliderOnHold { get; set; } = true;
    }
}
