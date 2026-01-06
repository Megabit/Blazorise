#region Using directives
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Container for input components when <see cref="Field"/> has <see cref="Field.Horizontal"/> set to true.
/// </summary>
public partial class FieldBody : BaseSizableFieldComponent
{
    #region Members

    private FieldBodyClasses classes;

    private FieldBodyStyles styles;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.FieldBody() );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildCustomClasses( ClassBuilder builder )
    {
        builder.Append( Classes?.Main );
    }

    /// <inheritdoc/>
    protected override void BuildCustomStyles( StyleBuilder builder )
    {
        builder.Append( Styles?.Main );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Custom CSS class names for field body elements.
    /// </summary>
    [Parameter]
    public FieldBodyClasses Classes
    {
        get => classes;
        set
        {
            if ( classes.IsEqual( value ) )
                return;

            classes = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Custom inline styles for field body elements.
    /// </summary>
    [Parameter]
    public FieldBodyStyles Styles
    {
        get => styles;
        set
        {
            if ( styles.IsEqual( value ) )
                return;

            styles = value;

            DirtyStyles();
        }
    }

    #endregion
}