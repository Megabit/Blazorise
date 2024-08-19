#region Using directives
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Small and adaptive tag for adding context to just about any content.
/// </summary>
public partial class Badge : BaseComponent
{
    #region Members

    private bool pill;

    private Color color = Color.Default;

    private string link;

    #endregion

    #region Constructors

    /// <summary>
    /// A default <see cref="Badge"/> constructor.
    /// </summary>
    public Badge()
    {
        CloseClassBuilder = new( BuildCloseClasses );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Badge() );
        builder.Append( ClassProvider.BadgeColor( Color ) );
        builder.Append( ClassProvider.BadgePill( Pill ) );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Builds the classnames for a close button.
    /// </summary>
    /// <param name="builder">Class builder used to append the classnames.</param>
    private void BuildCloseClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.BadgeClose() );
        builder.Append( ClassProvider.BadgeCloseColor( Color ) );
    }

    /// <summary>
    /// Handles the close button onclick event.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected Task OnCloseClickedHandler()
    {
        return CloseClicked.InvokeAsync();
    }

    /// <summary>
    /// Handles the close button onclick event.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected Task OnCloseKeyDownHandler( KeyboardEventArgs args )
    {
        if ( ( args.Code == "Enter" || args.Code == "NumpadEnter" ) && !args.IsModifierKey() )
            return CloseClicked.InvokeAsync();

        return Task.CompletedTask;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Indicated if badge can have closable icon.
    /// </summary>
    protected bool Closable => CloseClicked.HasDelegate;

    /// <summary>
    /// Close button class builder.
    /// </summary>
    protected ClassBuilder CloseClassBuilder { get; private set; }

    /// <summary>
    /// Make the badge more rounded.
    /// </summary>
    [Parameter]
    public bool Pill
    {
        get => pill;
        set
        {
            pill = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Sets the badge contextual color.
    /// </summary>
    [Parameter]
    public Color Color
    {
        get => color;
        set
        {
            color = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Create a badge link and provide actionable badges with hover and focus states.
    /// </summary>
    [Parameter]
    public string Link
    {
        get => link;
        set
        {
            link = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Occurs on close button click.
    /// </summary>
    [Parameter] public EventCallback CloseClicked { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Badge"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}