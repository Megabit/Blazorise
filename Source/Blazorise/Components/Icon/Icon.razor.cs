#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Container for any type of icon font.
/// </summary>
public partial class Icon : BaseComponent
{
    #region Members

    private object name;

    private IconStyle? iconStyle;

    private IconSize? iconSize;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( IconProvider.Icon( Name, GetIconStyle() ) );

        var iconSize = GetIconSize();

        if ( iconSize != Blazorise.IconSize.Default )
            builder.Append( IconProvider.IconSize( GetIconSize() ) );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Handles the icon onclick event.
    /// </summary>
    /// <param name="eventArgs">Supplies information about a mouse event that is being raised.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected Task OnClickHandler( MouseEventArgs eventArgs )
    {
        return Clicked.InvokeAsync( eventArgs );
    }

    /// <summary>
    /// Handles the icon onmouseover event.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected Task OnMouseOverHandler( MouseEventArgs eventArgs )
    {
        return MouseOver.InvokeAsync( eventArgs );
    }

    /// <summary>
    /// Handles the icon onmouseout event.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected Task OnMouseOutHandler( MouseEventArgs eventArgs )
    {
        return MouseOut.InvokeAsync( eventArgs );
    }

    /// <summary>
    /// Get the icon style based on the current settings.
    /// </summary>
    /// <returns>Icon style.</returns>
    protected IconStyle GetIconStyle() => IconStyle ?? Options.IconStyle ?? Blazorise.IconStyle.Solid;

    /// <summary>
    /// Get the icon size based on the current settings.
    /// </summary>
    /// <returns>Icon size.</returns>
    protected IconSize GetIconSize() => IconSize ?? Options.IconSize ?? Blazorise.IconSize.Default;

    #endregion

    #region Properties

    /// <summary>
    /// Holds the information about the Blazorise global options.
    /// </summary>
    [Inject] protected BlazoriseOptions Options { get; set; }

    /// <summary>
    /// An icon provider that is responsible to give the icon a class-name.
    /// </summary>
    [Inject] protected IIconProvider IconProvider { get; set; }

    /// <summary>
    /// Icon name that can be either a string or <see cref="IconName"/>.
    /// </summary>
    [Parameter]
    public object Name
    {
        get => name;
        set
        {
            name = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Suggested icon style.
    /// </summary>
    [Parameter]
    public IconStyle? IconStyle
    {
        get => iconStyle;
        set
        {
            iconStyle = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Defines the icon size.
    /// </summary>
    [Parameter]
    public IconSize? IconSize
    {
        get => iconSize;
        set
        {
            iconSize = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Occurs when the icon is clicked.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> Clicked { get; set; }

    /// <summary>
    /// Occurs when the mouse has entered the icon area.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> MouseOver { get; set; }

    /// <summary>
    /// Occurs when the mouse has left the icon area.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> MouseOut { get; set; }

    #endregion
}