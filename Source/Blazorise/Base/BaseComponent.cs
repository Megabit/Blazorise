#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseComponent : ComponentBase, IDisposable
    {
        #region Members

        private bool disposed;

        //private bool rendered = false;

        private ElementRef elementRef;

        private string customClass;

        private IClassProvider classProvider;

        private IStyleProvider styleProvider;

        private Float @float = Float.None;

        private IFluentSpacing margin;

        private IFluentSpacing padding;

        private Visibility visibility = Visibility.Default;

        private ParameterCollection parameters;

        #endregion

        #region Constructors

        public BaseComponent()
        {
            ElementId = Utils.IDGenerator.Instance.Generate;
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        protected virtual void Dispose( bool disposing )
        {
            if ( !disposed )
            {
                if ( disposing )
                {
                    if ( ClassMapper != null )
                    {
                        ClassMapper.Dispose();
                        ClassMapper = null;
                    }

                    if ( StyleMapper != null )
                    {
                        StyleMapper.Dispose();
                        StyleMapper = null;
                    }

                    margin = null;
                    padding = null;
                }

                disposed = true;
            }
        }

        //protected override void OnAfterRender()
        //{
        //    if ( !rendered )
        //    {
        //        JSRunner.Init( elementRef, this );

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
                .Add( () => StyleProvider.Visibility( Visibility ) );
        }

        // use this until https://github.com/aspnet/Blazor/issues/1732 is fixed!!
        internal protected virtual void Dirty()
        {
        }

        public override Task SetParametersAsync( ParameterCollection parameters )
        {
            if ( ComponentMapper.HasRegistration( this ) )
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
        public ElementRef ElementRef { get => elementRef; protected set => elementRef = value; }

        /// <summary>
        /// Gets the unique id of the element.
        /// </summary>
        public string ElementId { get; }

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
        protected IComponentMapper ComponentMapper { get; set; }

        /// <summary>
        /// Gets or set the javascript runner.
        /// </summary>
        [Inject]
        protected IJSRunner JSRunner { get; set; }

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
        /// Defines the element custom css classname(s).
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
