#region Using directives
using System.Collections.Generic;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Steps : BaseComponent
    {
        #region Members

        private StepsState state = new();

        private readonly List<string> stepItems = new();

        private readonly List<string> stepPanels = new();

        #endregion

        #region Constructors

        public Steps()
        {
            ContentClassBuilder = new ClassBuilder( BuildContentClasses );
        }

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Steps() );

            base.BuildClasses( builder );
        }

        private void BuildContentClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.StepsContent() );
        }

        internal void NotifyStepInitialized( string name )
        {
            if ( !stepItems.Contains( name ) )
                stepItems.Add( name );
        }

        internal void NotifyStepRemoved( string name )
        {
            if ( stepItems.Contains( name ) )
                stepItems.Remove( name );
        }

        internal void NotifyStepPanelInitialized( string name )
        {
            if ( !stepPanels.Contains( name ) )
                stepPanels.Add( name );
        }

        internal void NotifyStepPanelRemoved( string name )
        {
            if ( stepPanels.Contains( name ) )
                stepPanels.Remove( name );
        }

        /// <summary>
        /// Sets the active step by the name.
        /// </summary>
        /// <param name="stepName"></param>
        public void SelectStep( string stepName )
        {
            SelectedStep = stepName;

            InvokeAsync( StateHasChanged );
        }

        /// <summary>
        /// Returns the index of the step item.
        /// </summary>
        /// <param name="name">Name of the step item.</param>
        /// <returns>The one-based index or 0 if not found.</returns>
        internal int IndexOfStep( string name )
        {
            return stepItems.IndexOf( name ) + 1;
        }

        #endregion

        #region Properties

        protected StepsState State => state;

        protected IReadOnlyList<string> StepItems => stepItems;

        protected IReadOnlyList<string> StepPanels => stepPanels;

        protected ClassBuilder ContentClassBuilder { get; private set; }

        protected string ContentClassNames => ContentClassBuilder.Class;

        /// <summary>
        /// Gets or sets currently selected step name.
        /// </summary>
        [Parameter]
        public string SelectedStep
        {
            get => state.SelectedStep;
            set
            {
                // prevent steps from calling the same code multiple times
                if ( value == state.SelectedStep )
                    return;

                state = state with { SelectedStep = value };

                // raise the changed notification
                SelectedStepChanged.InvokeAsync( state.SelectedStep );

                DirtyClasses();
            }
        }

        /// <summary>
        /// Occurs after the selected step has changed.
        /// </summary>
        [Parameter] public EventCallback<string> SelectedStepChanged { get; set; }

        [Parameter] public RenderFragment Items { get; set; }

        [Parameter] public RenderFragment Content { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
