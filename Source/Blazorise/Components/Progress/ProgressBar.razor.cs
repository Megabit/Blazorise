#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Inner component of <see cref="Progress"/> component used to indicate the progress so far.
/// </summary>
public partial class ProgressBar : BaseComponent
{
    #region Members

    private Color color = Color.Primary;

    private bool striped;

    private bool animated;

    private bool indeterminate;

    private int? value;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ParentProgress?.NotifyHasMessage();

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ProgressBar() );
        builder.Append( ClassProvider.ProgressBarColor( Color ) );
        builder.Append( ClassProvider.ProgressBarWidth( Percentage ?? 0 ) );
        builder.Append( ClassProvider.ProgressBarStriped( Striped ) );
        builder.Append( ClassProvider.ProgressBarAnimated( Animated ) );

        if ( ParentProgress?.ThemeSize != Size.Default )
            builder.Append( ClassProvider.ProgressBarSize( ParentProgress.ThemeSize ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        if ( Percentage is not null )
            builder.Append( StyleProvider.ProgressBarValue( Percentage ?? 0 ) );

        builder.Append( StyleProvider.ProgressBarSize( ParentProgress?.ThemeSize ?? Size.Default ) );

        base.BuildStyles( builder );
    }

    /// <summary>
    /// Sets the progress bar <see cref="Animated"/> flag.
    /// </summary>
    /// <param name="animated">True to animate the progress bar.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Animate( bool animated )
    {
        Animated = animated;

        return InvokeAsync( StateHasChanged );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Calculates the percentage based on the current value and max parameters.
    /// </summary>
    protected int? Percentage
        => Indeterminate ? null : Max == 0 ? 0 : (int?)( Value / (float?)Max * 100f );

    /// <summary>
    /// If true, the value will be showed within the progress bar.
    /// </summary>
    protected bool IsShowValue => ParentProgress?.ShowValue ?? true;

    /// <summary>
    /// Defines the progress bar color.
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
    /// Set to true to make the progress bar stripped.
    /// </summary>
    [Parameter]
    public bool Striped
    {
        get => striped;
        set
        {
            striped = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Set to true to make the progress bar animated.
    /// </summary>
    [Parameter]
    public bool Animated
    {
        get => animated;
        set
        {
            animated = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Set to true to show that an operation is being executed.
    /// </summary>
    [Parameter]
    public bool Indeterminate
    {
        get => indeterminate;
        set
        {
            indeterminate = value;

            DirtyClasses();
        }
    }


    /// <summary>
    /// Minimum value of the progress bar.
    /// </summary>
    [Parameter] public int Min { get; set; } = 0;

    /// <summary>
    /// Maximum value of the progress bar.
    /// </summary>
    [Parameter] public int Max { get; set; } = 100;

    /// <summary>
    /// The progress value.
    /// </summary>
    [Parameter]
    public int? Value
    {
        get => value;
        set
        {
            if ( this.value == value )
                return;

            this.value = value;

            DirtyClasses();
            DirtyStyles();
        }
    }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="ProgressBar"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the reference to the parent <see cref="Progress"/> component.
    /// </summary>
    [CascadingParameter] protected Progress ParentProgress { get; set; }

    #endregion
}