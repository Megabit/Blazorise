#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Main component for stacked progress bars.
/// </summary>
public partial class Progress : BaseComponent, IDisposable
{
    #region Members

    /// <summary>
    /// Holds the state of the <see cref="Progress"/> component.
    /// </summary>
    private ProgressState state = new()
    {
        Color = Color.Primary,
        Min = 0,
        Max = 100,
    };

    /// <summary>
    /// Flag that indicates if <see cref="Progress"/> contains the <see cref="ProgressBar"/> component.
    /// </summary>
    private bool hasProgressBar;

    #endregion

    #region Constructors

    /// <summary>
    /// A default <see cref="Progress"/> constructor.
    /// </summary>
    public Progress()
    {
        ProgressBarClassBuilder = new( BuildProgressBarClasses );
        ProgressBarStyleBuilder = new( BuildProgressBarStyles );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if ( Theme is not null )
        {
            Theme.Changed += OnThemeChanged;
        }

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            if ( Theme is not null )
            {
                Theme.Changed -= OnThemeChanged;
            }
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Progress() );
        builder.Append( ClassProvider.ProgressSize( ThemeSize ) );
        builder.Append( ClassProvider.ProgressColor( Color ) );
        builder.Append( ClassProvider.ProgressStriped( Striped ) );
        builder.Append( ClassProvider.ProgressAnimated( Animated ) );
        builder.Append( ClassProvider.ProgressIndeterminate( Indeterminate ) );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Builds the classnames for a progress bar.
    /// </summary>
    /// <param name="builder">Class builder used to append the classnames.</param>
    private void BuildProgressBarClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ProgressBar() );
        builder.Append( ClassProvider.ProgressBarColor( Color ) );
        builder.Append( ClassProvider.ProgressBarWidth( Percentage ?? 0 ) );
        builder.Append( ClassProvider.ProgressBarStriped( Striped ) );
        builder.Append( ClassProvider.ProgressBarAnimated( Animated ) );
        builder.Append( ClassProvider.ProgressBarIndeterminate( Indeterminate ) );

        if ( ThemeSize != Blazorise.Size.Default )
            builder.Append( ClassProvider.ProgressBarSize( ThemeSize ) );
    }

    /// <summary>
    /// Builds the styles for a progress bar.
    /// </summary>
    /// <param name="builder">Styles builder used to append the styles.</param>
    private void BuildProgressBarStyles( StyleBuilder builder )
    {
        if ( Percentage is not null )
            builder.Append( StyleProvider.ProgressBarValue( Percentage ?? 0 ) );

        builder.Append( StyleProvider.ProgressBarSize( ThemeSize ) );
    }

    /// <inheritdoc/>
    protected internal override void DirtyClasses()
    {
        ProgressBarClassBuilder.Dirty();

        base.DirtyClasses();
    }

    /// <inheritdoc/>
    protected override void DirtyStyles()
    {
        ProgressBarStyleBuilder.Dirty();

        base.DirtyStyles();
    }

    /// <summary>
    /// Notifies the progress that one of the child components is a progressbar.
    /// </summary>
    internal void NotifyHasMessage()
    {
        hasProgressBar = true;

        DirtyClasses();
        DirtyStyles();

        InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// An event raised when theme settings changes.
    /// </summary>
    /// <param name="sender">An object that raised the event.</param>
    /// <param name="eventArgs"></param>
    private void OnThemeChanged( object sender, EventArgs eventArgs )
    {
        DirtyClasses();
        DirtyStyles();

        InvokeAsync( StateHasChanged );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Indicates if <see cref="Progress"/> contains the <see cref="ProgressBar"/> component.
    /// </summary>
    protected bool HasProgressBar => hasProgressBar;

    /// <summary>
    /// Gets the progress state.
    /// </summary>
    protected ProgressState State => state;

    /// <summary>
    /// Calculates the percentage based on the current value and max parameters.
    /// </summary>
    protected int? Percentage
        => Indeterminate ? null : Max == 0 ? 0 : (int?)( Value / (float?)Max * 100f );

    /// <summary>
    /// Gets the classnames of the progress bar.
    /// </summary>
    protected string ProgressBarClassNames
        => ProgressBarClassBuilder.Class;

    /// <summary>
    /// Gets the stylenames of the progress bar.
    /// </summary>
    protected string ProgressBarStyleNames
        => ProgressBarStyleBuilder.Styles;

    /// <summary>
    /// Gets the size based on the theme settings.
    /// </summary>
    protected internal Size ThemeSize
        => Size ?? Theme?.ProgressOptions?.Size ?? Blazorise.Size.Default;

    /// <summary>
    /// Progress bar class builder.
    /// </summary>
    protected ClassBuilder ProgressBarClassBuilder { get; private set; }

    /// <summary>
    /// Progress bar style builder.
    /// </summary>
    protected StyleBuilder ProgressBarStyleBuilder { get; private set; }

    /// <summary>
    /// Defines the progress bar color.
    /// </summary>
    [Parameter]
    public Color Color
    {
        get => state.Color;
        set
        {
            state = state with { Color = value };

            DirtyClasses();
        }
    }

    /// <summary>
    /// Size of the progress bar.
    /// </summary>
    [Parameter]
    public Size? Size
    {
        get => state.Size;
        set
        {
            state = state with { Size = value };

            DirtyClasses();
        }
    }

    /// <summary>
    /// Set to true to make the progress bar stripped.
    /// </summary>
    [Parameter]
    public bool Striped
    {
        get => state.Striped;
        set
        {
            state = state with { Striped = value };

            DirtyClasses();
        }
    }

    /// <summary>
    /// Set to true to make the progress bar animated.
    /// </summary>
    [Parameter]
    public bool Animated
    {
        get => state.Animated;
        set
        {
            state = state with { Animated = value };

            DirtyClasses();
        }
    }

    /// <summary>
    /// Set to true to show that an operation is being executed.
    /// </summary>
    [Parameter]
    public bool Indeterminate
    {
        get => state.Indeterminate;
        set
        {
            state = state with { Indeterminate = value };

            DirtyClasses();
        }
    }

    /// <summary>
    /// Minimum value of the progress bar.
    /// </summary>
    [Parameter]
    public int Min
    {
        get => state.Min;
        set
        {
            state = state with { Min = value };

            DirtyClasses();
        }
    }

    /// <summary>
    /// Maximum value of the progress bar.
    /// </summary>
    [Parameter]
    public int Max
    {
        get => state.Max;
        set
        {
            state = state with { Max = value };

            DirtyClasses();
        }
    }

    /// <summary>
    /// The progress value.
    /// </summary>
    [Parameter]
    public int? Value
    {
        get => state.Value;
        set
        {
            if ( state.Value == value )
                return;

            state = state with { Value = value };

            DirtyClasses();
            DirtyStyles();
        }
    }

    /// <summary>
    /// If true, the value will be showed within the progress bar.
    /// </summary>
    [Parameter] public bool ShowValue { get; set; } = true;

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Progress"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Cascaded theme settings.
    /// </summary>
    [CascadingParameter] public Theme Theme { get; set; }

    #endregion
}