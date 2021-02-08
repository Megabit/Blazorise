#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Defines the global Blazorise options.
    /// </summary>
    public class BlazoriseOptions
    {
        #region Members

        private readonly IServiceProvider serviceProvider;

        private readonly Action<BlazoriseOptions> configureOptions;

        #endregion

        #region Constructors

        public BlazoriseOptions( IServiceProvider serviceProvider, Action<BlazoriseOptions> configureOptions )
        {
            this.serviceProvider = serviceProvider;
            this.configureOptions = configureOptions;

            this.configureOptions?.Invoke( this );
        }

        #endregion

        #region Properties

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

        /// <summary>
        /// If true, the component that can control it's parent will automatically close it.
        /// </summary>
        /// <remarks>
        /// This behavior can be seen on <see cref="CloseButton"/> that can auto-close it's <see cref="Alert"/>
        /// or it's <see cref="Modal"/> parent component.
        /// </remarks>
        public bool AutoCloseParent { get; set; } = true;

        /// <summary>
        /// Global handler that can be used to override and localize validation messages before they
        /// are shown on the <see cref="ValidationError"/> or <see cref="ValidationSuccess"/>.
        /// </summary>
        public Func<string, IEnumerable<string>, string> ValidationMessageLocalizer { get; set; }

        /// <summary>
        /// Gets the service provider.
        /// </summary>
        public IServiceProvider Services => serviceProvider;

        #endregion
    }
}
