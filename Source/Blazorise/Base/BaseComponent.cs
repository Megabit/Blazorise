#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Licensing;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise;

/// <summary>
/// Base class for all DOM based components.
/// </summary>
public abstract class BaseComponent : BaseAfterRenderComponent
{
    #region Members

    private string customClass;
    private string customStyle;

    private Float @float = Float.Default;

    private bool clearfix;

    private Visibility visibility = Visibility.Default;

    private IFluentSizing width;

    private IFluentSizing height;

    private IFluentSpacing margin;

    private IFluentSpacing padding;

    private IFluentGap gap;

    private IFluentDisplay display;

    private IFluentBorder border;

    private IFluentFlex flex;

    private IFluentPosition position;

    private IFluentOverflow overflow;

    private CharacterCasing characterCasing = CharacterCasing.Normal;

    private TextColor textColor = TextColor.Default;

    private TextAlignment textAlignment = TextAlignment.Default;

    private TextTransform textTransform = TextTransform.Default;

    private TextDecoration textDecoration = TextDecoration.Default;

    private TextWeight textWeight = TextWeight.Default;

    private TextOverflow textOverflow = TextOverflow.Default;

    private IFluentTextSize textSize;

    private IFluentObjectFit objectFit;

    private VerticalAlignment verticalAlignment = VerticalAlignment.Default;

    private Background background = Background.Default;

    private Shadow shadow = Shadow.None;

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor for <see cref="BaseComponent"/>.
    /// </summary>
    public BaseComponent()
    {
        ClassBuilder = new( BuildClasses, BuildCustomClasses );
        StyleBuilder = new( BuildStyles, BuildCustomStyles );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        object heightAttribute = null;

        // WORKAROUND for: https://github.com/dotnet/aspnetcore/issues/32252
        // HTML native width/height attributes are recognized as Width/Height parameters
        // and Blazor tries to convert them resulting in error. This workaround tries to fix it by removing
        // width/height from parameter list and moving them to Attributes(as unmatched values).
        //
        // This behavior is really an edge-case and shouldn't affect performance too much.
        // Only in some rare cases when width/height are used will the parameters be rebuilt.
        if ( parameters.TryGetValue( "width", out object widthAttribute )
             || parameters.TryGetValue( "height", out heightAttribute ) )
        {
            var parametersDictionary = (Dictionary<string, object>)parameters.ToDictionary();

            Attributes ??= new();

            if ( widthAttribute is not null && parametersDictionary.Remove( "width" ) )
            {
                Attributes.Add( "width", widthAttribute );
            }

            if ( heightAttribute is not null && parametersDictionary.Remove( "height" ) )
            {
                Attributes.Add( "height", heightAttribute );
            }

            return base.SetParametersAsync( ParameterView.FromDictionary( parametersDictionary ) );
        }

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if ( ShouldAutoGenerateId && ElementId is null )
        {
            ElementId = IdGenerator.Generate;
        }

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            if ( LicenseChecker.ShouldPrint() )
            {
                await JSUtilitiesModule.Log( LicenseChecker.ShowBanner(), $"%c{LicenseChecker.GetPrintMessage()}", "color: #3B82F6; padding: 0;" );
            }
        }

        await base.OnAfterRenderAsync( firstRender );
    }

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
    /// Builds a list of classnames for this component.
    /// </summary>
    /// <param name="builder">Class builder used to append the classnames.</param>
    protected virtual void BuildClasses( ClassBuilder builder )
    {
        if ( Class is not null )
            builder.Append( Class );

        if ( Margin is not null )
            builder.Append( Margin.Class( ClassProvider ) );

        if ( Padding is not null )
            builder.Append( Padding.Class( ClassProvider ) );

        if ( Gap is not null )
            builder.Append( Gap.Class( ClassProvider ) );

        if ( Display is not null )
            builder.Append( Display.Class( ClassProvider ) );

        if ( Border is not null )
            builder.Append( Border.Class( ClassProvider ) );

        if ( Flex is not null )
            builder.Append( Flex.Class( ClassProvider ) );

        if ( Position is not null )
            builder.Append( Position.Class( ClassProvider ) );

        if ( Overflow is not null )
            builder.Append( Overflow.Class( ClassProvider ) );

        if ( ObjectFit is not null )
            builder.Append( ObjectFit.Class( ClassProvider ) );

        if ( Float != Float.Default )
            builder.Append( ClassProvider.Float( Float ) );

        if ( Clearfix )
            builder.Append( ClassProvider.Clearfix() );

        if ( Visibility != Visibility.Default )
            builder.Append( ClassProvider.Visibility( Visibility ) );

        if ( VerticalAlignment != VerticalAlignment.Default )
            builder.Append( ClassProvider.VerticalAlignment( VerticalAlignment ) );

        if ( Width is not null )
            builder.Append( Width.Class( ClassProvider ) );

        if ( Height is not null )
            builder.Append( Height.Class( ClassProvider ) );

        if ( Casing != CharacterCasing.Normal )
            builder.Append( ClassProvider.Casing( Casing ) );

        if ( TextColor != TextColor.Default )
            builder.Append( ClassProvider.TextColor( TextColor ) );

        if ( TextAlignment != TextAlignment.Default )
            builder.Append( ClassProvider.TextAlignment( TextAlignment ) );

        if ( TextTransform != TextTransform.Default )
            builder.Append( ClassProvider.TextTransform( TextTransform ) );

        if ( TextDecoration != TextDecoration.Default )
            builder.Append( ClassProvider.TextDecoration( TextDecoration ) );

        if ( TextWeight != TextWeight.Default )
            builder.Append( ClassProvider.TextWeight( TextWeight ) );

        if ( TextOverflow != TextOverflow.Default )
            builder.Append( ClassProvider.TextOverflow( TextOverflow ) );

        if ( TextSize is not null )
            builder.Append( TextSize.Class( ClassProvider ) );

        if ( Background != Background.Default )
            builder.Append( ClassProvider.BackgroundColor( Background ) );

        if ( Shadow != Shadow.None )
            builder.Append( ClassProvider.Shadow( Shadow ) );

    }

    /// <summary>
    /// Builds a list of styles for this component.
    /// </summary>
    /// <param name="builder">Style builder used to append the styles.</param>
    protected virtual void BuildStyles( StyleBuilder builder )
    {
        if ( Style is not null )
            builder.Append( Style );

        if ( Width != null )
            builder.Append( Width.Style( StyleProvider ) );

        if ( Height != null )
            builder.Append( Height.Style( StyleProvider ) );
    }

    /// <summary>
    /// Provides component-specific classes appended after the default classes.
    /// </summary>
    /// <param name="builder">Class builder used to append the classnames.</param>
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
    /// Clears the class-names and mark them to be regenerated.
    /// </summary>
    internal protected virtual void DirtyClasses()
    {
        ClassBuilder?.Dirty();
    }

    /// <summary>
    /// Clears the styles-names and mark them to be regenerated.
    /// </summary>
    protected virtual void DirtyStyles()
    {
        StyleBuilder?.Dirty();
    }

    /// <summary>
    /// Creates a new instance of <see cref="DotNetObjectReference{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of the object.</typeparam>
    /// <param name="value">The reference of the tracked object.</param>
    /// <returns>An instance of <see cref="DotNetObjectReference{T}"/>.</returns>
    protected static DotNetObjectReference<T> CreateDotNetObjectRef<T>( T value ) where T : class
    {
        return DotNetObjectReference.Create( value );
    }

    /// <summary>
    /// Destroys the instance of <see cref="DotNetObjectReference{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of the object.</typeparam>
    /// <param name="value">The reference of the tracked object.</param>
    protected static void DisposeDotNetObjectRef<T>( DotNetObjectReference<T> value ) where T : class
    {
        value?.Dispose();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the reference to the rendered element.
    /// </summary>
    public ElementReference ElementRef { get; set; }

    /// <summary>
    /// Gets or sets the unique id of the element.
    /// </summary>
    /// <remarks>
    /// Note that this ID is not defined for the component but instead for the underlined element that it represents.
    /// eg: for the TextInput the ID will be set on the input element.
    /// </remarks>
    [Parameter] public string ElementId { get; set; }

    /// <summary>
    /// If true, <see cref="ElementId"/> will be auto-generated on component initialize.
    /// </summary>
    /// <remarks>
    /// Override this in components that need to have an id defined before calling JSInterop.
    /// </remarks>
    protected virtual bool ShouldAutoGenerateId => false;

    /// <summary>
    /// Gets the class builder.
    /// </summary>
    protected ClassBuilder ClassBuilder { get; private set; }

    /// <summary>
    /// Gets the built class-names based on all the rules set by the component parameters.
    /// </summary>
    public string ClassNames => ClassBuilder.Class;

    /// <summary>
    /// Gets the style mapper.
    /// </summary>
    protected StyleBuilder StyleBuilder { get; private set; }

    /// <summary>
    /// Gets the built styles based on all the rules set by the component parameters.
    /// </summary>
    public string StyleNames => StyleBuilder.Styles;

    /// <summary>
    /// Gets or set the javascript runner.
    /// </summary>
    [Inject] protected IIdGenerator IdGenerator { get; set; }

    /// <summary>
    /// Gets or sets the classname provider.
    /// </summary>
    [Inject] protected IClassProvider ClassProvider { get; set; }

    /// <summary>
    /// Gets or sets the style provider.
    /// </summary>
    [Inject] protected IStyleProvider StyleProvider { get; set; }

    /// <summary>
    /// Gets or sets the IJSUtilitiesModule reference.
    /// </summary>
    [Inject] private IJSUtilitiesModule JSUtilitiesModule { get; set; }

    /// <summary>
    /// Gets or sets the license checker for the user session.
    /// </summary>
    [Inject] internal BlazoriseLicenseChecker LicenseChecker { get; set; }

    /// <summary>
    /// Custom CSS class name to apply to the component.
    /// </summary>
    [Parameter]
    public string Class
    {
        get => customClass;
        set
        {
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
            customStyle = value;

            DirtyStyles();
        }
    }

    /// <summary>
    /// Specifies how an element should float within its containing block.
    /// </summary>
    [Parameter]
    public Float Float
    {
        get => @float;
        set
        {
            @float = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Determines whether to apply clearfix to manage floating children.
    /// </summary>
    [Parameter]
    public bool Clearfix
    {
        get => clearfix;
        set
        {
            clearfix = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Controls the visibility of an element without altering its layout.
    /// </summary>
    [Parameter]
    public Visibility Visibility
    {
        get => visibility;
        set
        {
            if ( visibility == value )
                return;

            visibility = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Defines the width of the component using responsive sizing utilities.
    /// </summary>
    [Parameter]
    public IFluentSizing Width
    {
        get => width;
        set
        {
            if ( width == value )
                return;

            width = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Defines the height of the component using responsive sizing utilities.
    /// </summary>
    [Parameter]
    public IFluentSizing Height
    {
        get => height;
        set
        {
            if ( height == value )
                return;

            height = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Configures the margin spacing for the component.
    /// </summary>
    [Parameter]
    public IFluentSpacing Margin
    {
        get => margin;
        set
        {
            if ( margin == value )
                return;

            margin = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Configures the padding spacing for the component.
    /// </summary>
    [Parameter]
    public IFluentSpacing Padding
    {
        get => padding;
        set
        {
            if ( padding == value )
                return;

            padding = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Configures the gap spacing between child elements of the component.
    /// </summary>
    [Parameter]
    public IFluentGap Gap
    {
        get => gap;
        set
        {
            if ( gap == value )
                return;

            gap = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Specifies the display behavior (e.g., block, inline, flex) of the component.
    /// </summary>
    [Parameter]
    public IFluentDisplay Display
    {
        get => display;
        set
        {
            if ( display == value )
                return;

            display = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Configures the border properties of the component.
    /// </summary>
    [Parameter]
    public IFluentBorder Border
    {
        get => border;
        set
        {
            if ( border == value )
                return;

            border = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Configures the flexbox properties of the component.
    /// </summary>
    [Parameter]
    public IFluentFlex Flex
    {
        get => flex;
        set
        {
            if ( flex == value )
                return;

            flex = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Specifies the positioning method for the component (static, relative, absolute, etc.).
    /// </summary>
    [Parameter]
    public IFluentPosition Position
    {
        get => position;
        set
        {
            if ( position == value )
                return;

            position = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Configures the overflow behavior when content exceeds the component's bounds.
    /// </summary>
    [Parameter]
    public IFluentOverflow Overflow
    {
        get => overflow;
        set
        {
            if ( overflow == value )
                return;

            overflow = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Specifies the text casing transformation (e.g., uppercase, lowercase).
    /// </summary>
    [Parameter]
    public CharacterCasing Casing
    {
        get => characterCasing;
        set
        {
            characterCasing = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Defines the text color of the component.
    /// </summary>
    [Parameter]
    public TextColor TextColor
    {
        get => textColor;
        set
        {
            textColor = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Configures the horizontal alignment of text within the component.
    /// </summary>
    [Parameter]
    public TextAlignment TextAlignment
    {
        get => textAlignment;
        set
        {
            textAlignment = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Configures the text transformation (e.g., capitalize, none) of the component.
    /// </summary>
    [Parameter]
    public TextTransform TextTransform
    {
        get => textTransform;
        set
        {
            textTransform = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Defines the text decoration style (e.g., underline, none) for the component.
    /// </summary>
    [Parameter]
    public TextDecoration TextDecoration
    {
        get => textDecoration;
        set
        {
            if ( textDecoration == value )
                return;

            textDecoration = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Specifies the font weight of text in the component (e.g., bold, normal).
    /// </summary>
    [Parameter]
    public TextWeight TextWeight
    {
        get => textWeight;
        set
        {
            textWeight = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Configures how text behaves when it overflows its container.
    /// </summary>
    [Parameter]
    public TextOverflow TextOverflow
    {
        get => textOverflow;
        set
        {
            textOverflow = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Sets the font size of the text in the component.
    /// </summary>
    [Parameter]
    public IFluentTextSize TextSize
    {
        get => textSize;
        set
        {
            if ( textSize == value )
                return;

            textSize = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Configures the object-fit property, which determines how content is resized within its container.
    /// </summary>
    [Parameter]
    public IFluentObjectFit ObjectFit
    {
        get => objectFit;
        set
        {
            if ( objectFit == value )
                return;

            objectFit = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Specifies the vertical alignment of inline or table-cell elements.
    /// </summary>
    [Parameter]
    public VerticalAlignment VerticalAlignment
    {
        get => verticalAlignment;
        set
        {
            verticalAlignment = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Sets the background color of the component.
    /// </summary>
    [Parameter]
    public Background Background
    {
        get => background;
        set
        {
            background = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Configures the shadow effect of the component.
    /// </summary>
    [Parameter]
    public Shadow Shadow
    {
        get => shadow;
        set
        {
            shadow = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Captures unmatched HTML attributes for customization.
    /// </summary>
    /// <remarks>
    /// These attributes are applied directly to the component's root HTML element.
    /// </remarks>
    [Parameter( CaptureUnmatchedValues = true )]
    public Dictionary<string, object> Attributes { get; set; }


    #endregion
}

/// <summary>
/// Base class for components that expose typed class and style customization.
/// </summary>
/// <typeparam name="TClasses">Component-specific classes type.</typeparam>
/// <typeparam name="TStyles">Component-specific styles type.</typeparam>
public abstract class BaseComponent<TClasses, TStyles> : BaseComponent
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