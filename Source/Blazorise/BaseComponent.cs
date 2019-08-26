﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseComponent : ComponentBase
    {
        #region Members

        private string elementId;

        private bool rendered = false;

        private string customClass;

        private string customStyle;

        private IClassProvider classProvider;

        private IStyleProvider styleProvider;

        private IComponentMapper componentMapper;

        private Float @float = Float.None;

        private IFluentSpacing margin;

        private IFluentSpacing padding;

        private Visibility visibility = Visibility.Default;

        private ParameterView parameters;

        /// <summary>
        /// A stack of functions to execute after the rendering.
        /// </summary>
        private Queue<Func<Task>> executeAfterRendereQueue;

        #endregion

        #region Constructors

        public BaseComponent()
        {
        }

        #endregion

        #region Methods

        protected void ExecuteAfterRender( Func<Task> action )
        {
            if ( executeAfterRendereQueue == null )
                executeAfterRendereQueue = new Queue<Func<Task>>();

            executeAfterRendereQueue.Enqueue( action );
        }

        protected override async Task OnAfterRenderAsync()
        {
            // If the component has custom implementation we need to postpone the initialisation
            // until the custom component is rendered!
            if ( !HasCustomRegistration )
            {
                if ( !rendered )
                {
                    rendered = true;

                    await OnFirstAfterRenderAsync();
                }

                if ( executeAfterRendereQueue?.Count > 0 )
                {
                    var actions = executeAfterRendereQueue.ToArray();
                    executeAfterRendereQueue.Clear();

                    foreach ( var action in actions )
                    {
                        await action();
                    }
                }
            }

            await base.OnAfterRenderAsync();
        }

        protected virtual Task OnFirstAfterRenderAsync()
            => Task.CompletedTask;

        protected virtual void RegisterClasses()
        {
            ClassMapper
                .If( () => Class, () => Class != null )
                .If( () => Margin.Class( classProvider ), () => Margin != null )
                .If( () => Padding.Class( classProvider ), () => Padding != null )
                .If( () => ClassProvider.ToFloat( Float ), () => Float != Float.None );
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

        public override Task SetParametersAsync( ParameterView parameters )
        {
            if ( HasCustomRegistration )
            {
                // the component has a custom implementation so we need to copy the parameters for manual rendering
                this.parameters = parameters;

                return base.SetParametersAsync( ParameterView.Empty );
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
        public ElementReference ElementRef { get; protected set; }

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
        public string Class
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
        public string Style
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
        public Float Float
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
        public IFluentSpacing Margin
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
        public IFluentSpacing Padding
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
        public Visibility Visibility
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
