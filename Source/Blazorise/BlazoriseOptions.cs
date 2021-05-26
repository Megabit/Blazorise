﻿#region Using directives
using System;
using System.Collections.Generic;
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

        /// <summary>
        /// A default constructors for <see cref="BlazoriseOptions"/>.
        /// </summary>
        /// <param name="serviceProvider">Service provider.</param>
        /// <param name="configureOptions">A handler for setting the blazorise options.</param>
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
        /// Maximum amount of <see cref="Theme">Themes</see> that are cached at the same time.
        /// When set to a value &lt; 1, the <see cref="Themes.ThemeCache">ThemeCache</see> is deactivated.
        /// </summary>
        public int ThemeCacheSize { get; set; } = 10;

        /// <summary>
        /// If true, the spin buttons on <see cref="NumericEdit{TValue}"/>. will be visible.
        /// </summary>
        public bool ShowSpinButtons { get; set; } = true;

        /// <summary>
        /// Gets the service provider.
        /// </summary>
        public IServiceProvider Services => serviceProvider;

        #endregion
    }
}
