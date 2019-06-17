#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseComponent : ComponentBase
    {
        #region Members

        private string elementId;

        //private bool rendered = false;

        private string customClass;

        private string customStyle;

        private IClassProvider classProvider;

        private IStyleProvider styleProvider;

        private IComponentMapper componentMapper;

        private Float @float = Float.None;

        private IFluentSpacing margin;

        private IFluentSpacing padding;

        private Visibility visibility = Visibility.Default;

        private ParameterCollection parameters;

        #endregion

        #region Constructors

        public BaseComponent()
        {
        }

        #endregion

        #region Methods

        //protected override void OnAfterRender()
        //{
        //    if ( !rendered )
        //    {
        //        JSRunner.Init( ElementRef, this );

        //        rendered = true;
        //    }

        //    base.OnAfterRender();
        //}

        protected virtual void RegisterClasses()
        {
            ClassMapper
                .If( () => Class, () => Class != null )
                .If( () => Margin.Class( classProvider ), () => Margin != null )
                .If( () => Padding.Class( classProvider ), () => Padding != null )
                .If( () => ClassProvider.Float( Float ), () => Float != Float.None );
        }

        protected virtual void RegisterStyles()
        {
            StyleMapper
                .If( () => Style, () => Style != null )
                .Add( () => StyleProvider.Visibility( Visibility ) );
        }

        // use this until https://github.com/aspnet/Blazor/issues/1732 is fixed!!
        internal protected virtual void Dirty()
        {
        }

        public override Task SetParametersAsync( ParameterCollection parameters )
        {
            if ( HasCustomRegistration )
            {
                // the component has a custom implementation so we need to copy the parameters for manual rendering
                this.parameters = parameters;

                return base.SetParametersAsync( ParameterCollection.Empty );
            }
            else
                return base.SetParametersAsync( parameters );
        }

        /// <summary>
        /// Main method to render custom component implementation.
        /// </summary>
        /// <returns></returns>
        protected RenderFragment RenderCustomComponent() => builder =>
        {
            builder.OpenComponent( 0, ComponentMapper.GetImplementation( this ) );

            foreach ( var parameter in parameters )
            {
                builder.AddAttribute( 1, parameter.Name, parameter.Value );
            }

            builder.CloseComponent();
        };

        #endregion

        #region Properties

        /// <summary>
        /// Gets the reference to the rendered element.
        /// </summary>
        public ElementRef ElementRef { get; protected set; }

        /// <summary>
        /// Gets the unique id of the element.
        /// </summary>
        /// <remarks>
        /// Note that this ID is not defined for the component but instead for the underlined component that it represents.
        /// eg: for the TextEdit the ID will be set on the input element.
        /// </remarks>
        public string ElementId
        {
            get
            {
                // generate ID only on first use
                if ( elementId == null )
                    elementId = Utils.IDGenerator.Instance.Generate;

                return elementId;
            }
        }

        /// <summary>
        /// Gets the class mapper.
        /// </summary>
        protected ClassMapper ClassMapper { get; private set; } = new ClassMapper();

        /// <summary>
        /// Gets the style mapper.
        /// </summary>
        protected StyleMapper StyleMapper { get; private set; } = new StyleMapper();

        /// <summary>
        /// Gets or sets the custom components mapper.
        /// </summary>
        [Inject]
        protected IComponentMapper ComponentMapper
        {
            get => componentMapper;
            set
            {
                componentMapper = value;

                HasCustomRegistration = componentMapper.HasRegistration( this );
            }
        }

        /// <summary>
        /// Indicates if components has a registered custom component.
        /// </summary>
        protected bool HasCustomRegistration { get; private set; }

        /// <summary>
        /// Gets or set the javascript runner.
        /// </summary>
        [Inject] protected IJSRunner JSRunner { get; set; }

        /// <summary>
        /// Gets or sets the classname provider.
        /// </summary>
        [Inject]
        protected IClassProvider ClassProvider
        {
            get => classProvider;
            set
            {
                classProvider = value;

                RegisterClasses();
            }
        }

        /// <summary>
        /// Gets or sets the style provider.
        /// </summary>
        [Inject]
        protected IStyleProvider StyleProvider
        {
            get => styleProvider;
            set
            {
                styleProvider = value;

                RegisterStyles();
            }
        }

        /// <summary>
        /// Custom css classname.
        /// </summary>
        [Parameter]
        protected string Class
        {
            get => customClass;
            set
            {
                customClass = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Custom html style.
        /// </summary>
        [Parameter]
        protected string Style
        {
            get => customStyle;
            set
            {
                customStyle = value;

                StyleMapper.Dirty();
            }
        }

        /// <summary>
        /// Floats an element to the defined side.
        /// </summary>
        [Parameter]
        protected Float Float
        {
            get => @float;
            set
            {
                @float = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Defines the element margin spacing.
        /// </summary>
        [Parameter]
        protected IFluentSpacing Margin
        {
            get => margin;
            set
            {
                margin = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Defines the element padding spacing.
        /// </summary>
        [Parameter]
        protected IFluentSpacing Padding
        {
            get => padding;
            set
            {
                padding = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Gets or sets the element visibility.
        /// </summary>
        [Parameter]
        protected Visibility Visibility
        {
            get => visibility;
            set
            {
                visibility = value;

                StyleMapper.Dirty();
            }
        }

        #endregion
    }
}
