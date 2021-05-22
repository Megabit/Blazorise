﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Base;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Base class for all DOM based components.
    /// </summary>
    public abstract class BaseComponent : BaseAfterRenderComponent
    {
        #region Members

        private string customClass;

        private string customStyle;

        private Float @float = Float.None;

        private Visibility visibility = Visibility.None;

        private IFluentSizing width;

        private IFluentSizing height;

        private IFluentSpacing margin;

        private IFluentSpacing padding;

        private IFluentDisplay display;

        private IFluentBorder border;

        private IFluentFlex flex;

        private CharacterCasing characterCasing = CharacterCasing.Normal;

        private TextColor textColor = TextColor.None;

        private TextAlignment textAlignment = TextAlignment.None;

        private TextTransform textTransform = TextTransform.None;

        private TextWeight textWeight = TextWeight.None;

        private VerticalAlignment verticalAlignment = VerticalAlignment.None;

        private Background background = Background.None;

        private Shadow shadow = Shadow.None;

        private Overflow overflow = Overflow.None;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor for <see cref="BaseComponent"/>.
        /// </summary>
        public BaseComponent()
        {
            ClassBuilder = new ClassBuilder( BuildClasses );
            StyleBuilder = new StyleBuilder( BuildStyles );
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override Task SetParametersAsync( ParameterView parameters )
        {
            object heightAttribute = null;

            // WORKAROUND for: https://github.com/dotnet/aspnetcore/issues/32252
            // HTML native width/height attributes are recognized as Width/Height parameters
            // and Blazor tries to convert them resulting in error. This workworund tries to fix it by removing
            // width/height from parameter list and moving them to Attributes(as unmatched values).
            //
            // This behavior is really an edge-case and shouldn't affect performance too much.
            // Only in some rare cases when width/height are used will the parameters be rebuilt.
            if ( parameters.TryGetValue( "width", out object widthAttribute )
                || parameters.TryGetValue( "height", out heightAttribute ) )
            {
                var paremetersDictionary = parameters.ToDictionary() as Dictionary<string, object>;

                if ( Attributes == null )
                    Attributes = new();

                if ( widthAttribute != null && paremetersDictionary.ContainsKey( "width" ) )
                {
                    paremetersDictionary.Remove( "width" );

                    Attributes.Add( "width", widthAttribute );
                }

                if ( heightAttribute != null && paremetersDictionary.ContainsKey( "height" ) )
                {
                    paremetersDictionary.Remove( "height" );

                    Attributes.Add( "height", heightAttribute );
                }

                return base.SetParametersAsync( ParameterView.FromDictionary( paremetersDictionary ) );
            }

            return base.SetParametersAsync( parameters );
        }

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            if ( ShouldAutoGenerateId && ElementId == null )
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
                await OnFirstAfterRenderAsync();
            }

            await base.OnAfterRenderAsync( firstRender );
        }

        /// <summary>
        /// Method is called only once when component is first rendered.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual Task OnFirstAfterRenderAsync()
            => Task.CompletedTask;

        /// <summary>
        /// Builds a list of classnames for this component.
        /// </summary>
        /// <param name="builder">Class builder used to append the classnames.</param>
        protected virtual void BuildClasses( ClassBuilder builder )
        {
            if ( Class != null )
                builder.Append( Class );

            if ( Margin != null )
                builder.Append( Margin.Class( ClassProvider ) );

            if ( Padding != null )
                builder.Append( Padding.Class( ClassProvider ) );

            if ( Display != null )
                builder.Append( Display.Class( ClassProvider ) );

            if ( Border != null )
                builder.Append( Border.Class( ClassProvider ) );

            if ( Flex != null )
                builder.Append( Flex.Class( ClassProvider ) );

            if ( Float != Float.None )
                builder.Append( ClassProvider.Float( Float ) );

            if ( Visibility != Visibility.None )
                builder.Append( ClassProvider.Visibility( Visibility ) );

            if ( VerticalAlignment != VerticalAlignment.None )
                builder.Append( ClassProvider.VerticalAlignment( VerticalAlignment ) );

            if ( Width != null )
                builder.Append( Width.Class( ClassProvider ) );

            if ( Height != null )
                builder.Append( Height.Class( ClassProvider ) );

            if ( Casing != CharacterCasing.Normal )
                builder.Append( ClassProvider.Casing( Casing ) );

            if ( TextColor != TextColor.None )
                builder.Append( ClassProvider.TextColor( TextColor ) );

            if ( TextAlignment != TextAlignment.None )
                builder.Append( ClassProvider.TextAlignment( TextAlignment ) );

            if ( TextTransform != TextTransform.None )
                builder.Append( ClassProvider.TextTransform( TextTransform ) );

            if ( TextWeight != TextWeight.None )
                builder.Append( ClassProvider.TextWeight( TextWeight ) );

            if ( Background != Background.None )
                builder.Append( ClassProvider.BackgroundColor( Background ) );

            if ( Shadow != Shadow.None )
                builder.Append( ClassProvider.Shadow( Shadow ) );

            if ( Overflow != Overflow.None )
                builder.Append( ClassProvider.Overflow( Overflow ) );
        }

        /// <summary>
        /// Builds a list of styles for this component.
        /// </summary>
        /// <param name="builder">Style builder used to append the styles.</param>
        protected virtual void BuildStyles( StyleBuilder builder )
        {
            if ( Style != null )
                builder.Append( Style );
        }

        /// <summary>
        /// Clears the class-names and mark them to be regenerated.
        /// </summary>
        internal protected virtual void DirtyClasses()
        {
            ClassBuilder.Dirty();
        }

        /// <summary>
        /// Clears the styles-names and mark them to be regenerated.
        /// </summary>
        protected virtual void DirtyStyles()
        {
            StyleBuilder.Dirty();
        }

        /// <summary>
        /// Creates a new instance of <see cref="DotNetObjectReference{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="value">The reference of the tracked object.</param>
        /// <returns>An instance of <see cref="DotNetObjectReference{T}"/>.</returns>
        protected DotNetObjectReference<T> CreateDotNetObjectRef<T>( T value ) where T : class
        {
            return DotNetObjectReference.Create( value );
        }

        /// <summary>
        /// Destroys the instance of <see cref="DotNetObjectReference{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="value">The reference of the tracked object.</param>
        protected void DisposeDotNetObjectRef<T>( DotNetObjectReference<T> value ) where T : class
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
        /// Gets or set the javascript runner.
        /// </summary>
        [Inject] protected IJSRunner JSRunner { get; set; }

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
        /// Custom css classname.
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
        /// Controls the visibility, without modifying the display, of elements with visibility utilities.
        /// </summary>
        [Parameter]
        public Visibility Visibility
        {
            get => visibility;
            set
            {
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
                padding = value;

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
                flex = value;

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
        /// The overflow property controls what happens to content that is too big to fit into an area.
        /// </summary>
        [Parameter]
        public Overflow Overflow
        {
            get => overflow;
            set
            {
                overflow = value;

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
}
