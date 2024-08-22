#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Small progress bar shown at the top of the page or a container.
/// </summary>
public partial class PageProgress : BaseComponent
{
    #region Members

    private bool visible;

    private int? value;

    private Color color = Color.Default;

    #endregion

    #region Constructors

    /// <summary>
    /// A default <see cref="PageProgress"/> constructor.
    /// </summary>
    public PageProgress()
    {
        IndicatorClassBuilder = new( BuildIndicatorClasses );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-page-progress" );
        builder.Append( "b-page-progress-active", Visible );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Builds a list of classnames for the indicator element.
    /// </summary>
    /// <param name="builder">Class builder used to append the classnames.</param>
    protected virtual void BuildIndicatorClasses( ClassBuilder builder )
    {
        builder.Append( "b-page-progress-indicator" );
        builder.Append( $"b-page-progress-indicator-{ClassProvider.ToColor( Color )}", Color != Color.Default );
        builder.Append( "b-page-progress-indicator-indeterminate", Value is null );
    }

    /// <inheritdoc/>
    protected internal override void DirtyClasses()
    {
        IndicatorClassBuilder.Dirty();

        base.DirtyClasses();
    }

    /// <summary>
    /// Sets the progress value and redraws the component.
    /// </summary>
    /// <param name="value">Progress value, in range from 0 to 100.</param>
    /// <returns>Returns awaitable task.</returns>
    public Task SetValueAsync( int? value )
    {
        Value = value;

        return InvokeAsync( StateHasChanged );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Indicator classname builder.
    /// </summary>
    protected ClassBuilder IndicatorClassBuilder { get; private set; }

    /// <summary>
    /// Gets the classnames for an indicator container.
    /// </summary>
    protected string IndicatorClassNames
        => IndicatorClassBuilder.Class;

    /// <summary>
    /// Gets the stylenames for an indicator container.
    /// </summary>
    protected string IndicatorStyleNames
        => Value is null ? null : $"width: {Value}%;";

    /// <summary>
    /// Defines the visibility of progress bar.
    /// </summary>
    [Parameter]
    public bool Visible
    {
        get => visible;
        set
        {
            if ( visible == value )
                return;

            visible = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// The progress value. Leave as null for indeterminate progress bar.
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
        }
    }

    /// <summary>
    /// Type color of the progress bar, optional.
    /// </summary>
    [Parameter]
    public Color Color
    {
        get => color;
        set
        {
            if ( color == value )
                return;

            color = value;

            DirtyClasses();
        }
    }

    #endregion
}