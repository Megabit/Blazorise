#region Using directives
using System;
using System.Globalization;
using System.Threading.Tasks;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Step : BaseComponent
    {
        #region Members

        private StepsState parentStepsState;

        private bool completed;

        private Color color = Color.None;

        #endregion

        #region Constructors

        public Step()
        {
            MarkerClassBuilder = new ClassBuilder( BuildMarkerClasses );
            DescriptionClassBuilder = new ClassBuilder( BuildDescriptionClasses );
        }

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            if ( ParentSteps != null )
            {
                ParentSteps.NotifyStepInitialized( Name );
            }

            base.OnInitialized();
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( ParentStepsState != null )
                {
                    ParentSteps.NotifyStepRemoved( Name );
                }
            }

            base.Dispose( disposing );
        }

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.StepItem() );
            builder.Append( ClassProvider.StepItemActive( Active ) );
            builder.Append( ClassProvider.StepItemCompleted( Completed ) );
            builder.Append( ClassProvider.StepItemColor( Color ), Color != Color.None );

            base.BuildClasses( builder );
        }

        protected virtual void BuildMarkerClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.StepItemMarker() );
        }

        protected virtual void BuildDescriptionClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.StepItemDescription() );
        }

        protected internal override void DirtyClasses()
        {
            MarkerClassBuilder.Dirty();
            DescriptionClassBuilder.Dirty();

            base.DirtyClasses();
        }

        protected Task ClickHandler()
        {
            Clicked?.Invoke();
            ParentSteps?.SelectStep( Name );

            return Task.CompletedTask;
        }

        #endregion

        #region Properties

        protected ClassBuilder MarkerClassBuilder { get; private set; }

        protected ClassBuilder DescriptionClassBuilder { get; private set; }

        protected bool Active => parentStepsState?.SelectedStep == Name;

        protected int? CalculatedIndex => Index ?? ParentSteps?.IndexOfStep( Name );

        /// <summary>
        /// Gets the classnames for marker part of a step item.
        /// </summary>
        protected string MarkerClassNames
            => MarkerClassBuilder.Class;

        /// <summary>
        /// Gets the classnames for description part of a step item.
        /// </summary>
        protected string DescriptionClassNames
            => DescriptionClassBuilder.Class;

        /// <summary>
        /// Overrides the index of the step item.
        /// </summary>
        [Parameter] public int? Index { get; set; }

        /// <summary>
        /// Defines the step name.
        /// </summary>
        [Parameter] public string Name { get; set; }

        /// <summary>
        /// Marks the step as completed.
        /// </summary>
        [Parameter]
        public bool Completed
        {
            get => completed;
            set
            {
                completed = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Overrides the step color.
        /// </summary>
        [Parameter]
        public Color Color
        {
            get => color;
            set
            {
                color = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Occurs when the item is clicked.
        /// </summary>
        [Parameter] public Action Clicked { get; set; }

        /// <summary>
        /// Custom render template for the marker(circle) part of the step item.
        /// </summary>
        [Parameter] public RenderFragment Marker { get; set; }

        /// <summary>
        /// Custom render template for the text part of the step item.
        /// </summary>
        [Parameter] public RenderFragment Caption { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        [CascadingParameter]
        protected StepsState ParentStepsState
        {
            get => parentStepsState;
            set
            {
                if ( parentStepsState == value )
                    return;

                parentStepsState = value;

                DirtyClasses();
            }
        }

        [CascadingParameter] protected Steps ParentSteps { get; set; }

        #endregion
    }
}
