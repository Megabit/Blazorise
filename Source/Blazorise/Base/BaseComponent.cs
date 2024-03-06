#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
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

    private TextWeight textWeight = TextWeight.Default;

    private TextOverflow textOverflow = TextOverflow.Default;

    private IFluentTextSize textSize;

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
        ClassBuilder = new( BuildClasses );
        StyleBuilder = new( BuildStyles );
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
                await JSUtilitiesModule.Log( "%cThank you for using the free version of the Blazorise component library! We're happy to offer it to you for personal use. If you'd like to remove this message, consider purchasing a commercial license from https://blazorise.com/commercial. We appreciate your support!", "color: #3B82F6; padding: 0;" );
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
    /// eg: for the TextEdit the ID will be set on the input element.
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
    [Inject]
    protected IClassProvider ClassProvider { get; set; }

    /// <summary>
    /// Gets or sets the style provider.
    /// </summary>
    [Inject]
    protected IStyleProvider StyleProvider { get; set; }

    /// <summary>
    /// Gets or sets the IJSUtilitiesModule reference.
    /// </summary>
    [Inject] private IJSUtilitiesModule JSUtilitiesModule { get; set; }

    /// <summary>
    /// Gets or sets the license checker for the user session.
    /// </summary>
    [Inject] internal BlazoriseLicenseChecker LicenseChecker { get; set; }

    /// <summary>
    /// Custom css class name.
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
    /// Custom html style.
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
    /// Floats an element to the defined side.
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
    /// Fixes an element's floating children.
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
    /// Controls the visibility, without modifying the display, of elements with visibility utilities.
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
    /// Defined the sizing for the element width attribute(s).
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
    /// Defined the sizing for the element height attribute(s).
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
    /// Defines the element margin spacing.
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
    /// Defines the element padding spacing.
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
    /// Defines the element gap spacing.
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
    /// Specifies the display behavior of an element.
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
    /// Specifies the border of an element.
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
    /// Specifies flexbox properties of an element.
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
    /// The position property specifies the type of positioning method used for an element (static, relative, fixed, absolute or sticky).
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
    /// The overflow property controls what happens to content that is too big to fit into an area.
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
    /// Changes the character casing of a element.
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
    /// Gets or sets the text color.
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
    /// Gets or sets the text alignment.
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
    /// Gets or sets the text transformation.
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
    /// Gets or sets the text weight.
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
    /// Determines how the text will behave when it is larger than a parent container.
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
    /// Determines the font size of an element.
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
    /// Changes the vertical alignment of inline, inline-block, inline-table, and table cell elements.
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
    /// Gets or sets the component background color.
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
    /// Gets or sets the component shadow box.
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
    /// Captures all the custom attribute that are not part of Blazorise component.
    /// </summary>
    [Parameter( CaptureUnmatchedValues = true )]
    public Dictionary<string, object> Attributes { get; set; }

    #endregion
}