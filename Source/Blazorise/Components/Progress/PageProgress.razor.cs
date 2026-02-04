#region Using directives
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Small progress bar shown at the top of the page or a container.
/// </summary>
public partial class PageProgress : BaseComponent<PageProgressClasses, PageProgressStyles>
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
        IndicatorClassBuilder = new( BuildIndicatorClasses, builder => builder.Append( Classes?.Indicator ) );
        IndicatorStyleBuilder = new( BuildIndicatorStyles, builder => builder.Append( Styles?.Indicator ) );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-page-progress" );

        if ( Visible )
            builder.Append( "b-page-progress-active" );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Builds a list of classnames for the indicator element.
    /// </summary>
    /// <param name="builder">Class builder used to append the classnames.</param>
    protected virtual void BuildIndicatorClasses( ClassBuilder builder )
    {
        builder.Append( "b-page-progress-indicator" );

        if ( Color.IsNotNullOrDefault() )
            builder.Append( $"b-page-progress-indicator-{ClassProvider.ToColor( Color )}" );

        if ( Value is null )
            builder.Append( "b-page-progress-indicator-indeterminate" );
    }

    /// <summary>
    /// Builds a list of styles for the indicator element.
    /// </summary>
    /// <param name="builder">Style builder used to append the styles.</param>
    protected virtual void BuildIndicatorStyles( StyleBuilder builder )
    {
        if ( Value is not null )
            builder.Append( $"width: {Value}%" );
    }

    /// <inheritdoc/>
    protected internal override void DirtyClasses()
    {
        IndicatorClassBuilder.Dirty();

        base.DirtyClasses();
    }

    /// <inheritdoc/>
    protected internal override void DirtyStyles()
    {
        IndicatorStyleBuilder.Dirty();

        base.DirtyStyles();
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
        => IndicatorStyleBuilder.Styles;

    /// <summary>
    /// Indicator style builder.
    /// </summary>
    protected StyleBuilder IndicatorStyleBuilder { get; private set; }

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
            DirtyStyles();
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

    /// <summary>
    /// Type intent of the progress bar, optional.
    /// </summary>
    [Parameter]
    public Intent Intent
    {
        get => Color.ToIntent();
        set => Color = value.ToColor();
    }

    #endregion
}