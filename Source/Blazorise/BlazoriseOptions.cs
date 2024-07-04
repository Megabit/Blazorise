#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise;

/// <summary>
/// Defines the global Blazorise options.
/// </summary>
public class BlazoriseOptions
{
    #region Members

    private readonly IServiceProvider serviceProvider;

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
        configureOptions?.Invoke( this );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines the product token issued by the Blazorise licensing system.
    /// </summary>
    [Obsolete( "LicenseKey property is deprecated, please use the ProductToken property instead." )]
    public string LicenseKey
    {
        get => ProductToken;
        set => ProductToken = value;
    }

    /// <summary>
    /// Defines the product token issued by the Blazorise licensing system.
    /// </summary>
    public string ProductToken { get; set; }

    /// <summary>
    /// If true the text in <see cref="TextEdit"/> will be changed after each key press.
    /// </summary>
    public bool Immediate { get; set; } = true;

    /// <summary>
    /// If true the entered into <see cref="TextEdit"/> will be slightly delayed before submitting it to the internal value.
    /// </summary>
    public bool? Debounce { get; set; } = false;

    /// <summary>
    /// Interval in milliseconds that entered text will be delayed from submitting to the <see cref="TextEdit"/> internal value.
    /// </summary>
    public int? DebounceInterval { get; set; } = 300;

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
    /// If true, the spin buttons on <see cref="NumericPicker{TValue}"/> will be visible.
    /// </summary>
    public bool ShowNumericStepButtons { get; set; } = true;

    /// <summary>
    /// If true, enables change of <see cref="NumericPicker{TValue}"/> by pressing on step buttons or by keyboard up/down keys.
    /// </summary>
    public bool? EnableNumericStep { get; set; }

    /// <summary>
    /// If true, modal will keep input focus.
    /// </summary>
    public bool? ModalFocusTrap { get; set; } = true;

    /// <summary>
    /// Defines the default icon style. Can be overriden on an each individual icon.
    /// </summary>
    public IconStyle? IconStyle { get; set; }

    /// <summary>
    /// Defines the default icon size. Can be overriden on an each individual icon.
    /// </summary>
    public IconSize? IconSize { get; set; }

    /// <summary>
    /// Gets the service provider.
    /// </summary>
    public IServiceProvider Services => serviceProvider;

    /// <summary>
    /// Whether to safely invoke internal javascript. Will ignore any exceptions that might be thrown as part of the javascript invoke process.
    /// </summary>
    public bool SafeJsInvoke { get; set; } = true;

    #endregion
}