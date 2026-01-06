using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;

namespace Blazorise;

/// <summary>
/// Base component for check inputs that expose typed class and style customization.
/// </summary>
/// <typeparam name="TValue">Checked value type.</typeparam>
/// <typeparam name="TClasses">Component-specific classes type.</typeparam>
/// <typeparam name="TStyles">Component-specific styles type.</typeparam>
public abstract class BaseCheckComponent<TValue, TClasses, TStyles> : BaseCheckComponent<TValue>
    where TClasses : ComponentClasses
    where TStyles : ComponentStyles
{
    private TClasses classes;
    private TStyles styles;

    /// <summary>
    /// Custom CSS class names for component elements.
    /// </summary>
    [Parameter]
    public TClasses Classes
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
    /// Custom inline styles for component elements.
    /// </summary>
    [Parameter]
    public TStyles Styles
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
}