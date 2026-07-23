#region Using directives
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Base class for components that need class and style building without the full utility parameter API.
/// </summary>
public abstract class BaseStyledComponent : BaseAfterRenderComponent
{
    #region Members

    private string customClass;

    private string customStyle;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new component instance.
    /// </summary>
    protected BaseStyledComponent()
    {
        ClassBuilder = new( BuildClasses, BuildCustomClasses );
        StyleBuilder = new( BuildStyles, BuildCustomStyles );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            ClassBuilder = null;
            StyleBuilder = null;
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
        {
            ClassBuilder = null;
            StyleBuilder = null;
        }

        return base.DisposeAsync( disposing );
    }

    /// <summary>
    /// Builds a list of class names for this component.
    /// </summary>
    /// <param name="builder">Class builder used to append the class names.</param>
    protected virtual void BuildClasses( ClassBuilder builder )
    {
        if ( Class is not null )
            builder.Append( Class );
    }

    /// <summary>
    /// Builds a list of styles for this component.
    /// </summary>
    /// <param name="builder">Style builder used to append the styles.</param>
    protected virtual void BuildStyles( StyleBuilder builder )
    {
        if ( Style is not null )
            builder.Append( Style );
    }

    /// <summary>
    /// Provides component-specific classes appended after the default classes.
    /// </summary>
    /// <param name="builder">Class builder used to append the class names.</param>
    protected virtual void BuildCustomClasses( ClassBuilder builder )
    {
    }

    /// <summary>
    /// Provides component-specific styles appended after the default styles.
    /// </summary>
    /// <param name="builder">Style builder used to append the styles.</param>
    protected virtual void BuildCustomStyles( StyleBuilder builder )
    {
    }

    /// <summary>
    /// Clears the class names and marks them to be regenerated.
    /// </summary>
    protected internal virtual void DirtyClasses()
    {
        ClassBuilder?.Dirty();
    }

    /// <summary>
    /// Clears the styles and marks them to be regenerated.
    /// </summary>
    protected internal virtual void DirtyStyles()
    {
        StyleBuilder?.Dirty();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the class builder.
    /// </summary>
    protected ClassBuilder ClassBuilder { get; private set; }

    /// <summary>
    /// Gets the built class names.
    /// </summary>
    public string ClassNames => ClassBuilder.Class;

    /// <summary>
    /// Gets the style builder.
    /// </summary>
    protected StyleBuilder StyleBuilder { get; private set; }

    /// <summary>
    /// Gets the built styles.
    /// </summary>
    public string StyleNames => StyleBuilder.Styles;

    /// <summary>
    /// Custom CSS class name to apply to the component.
    /// </summary>
    [Parameter]
    public string Class
    {
        get => customClass;
        set
        {
            if ( customClass.IsEqual( value ) )
                return;

            customClass = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Custom inline styles to apply to the component.
    /// </summary>
    [Parameter]
    public string Style
    {
        get => customStyle;
        set
        {
            if ( customStyle.IsEqual( value ) )
                return;

            customStyle = value;

            DirtyStyles();
        }
    }

    #endregion
}