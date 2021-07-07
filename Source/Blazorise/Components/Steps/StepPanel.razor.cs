﻿#region Using directives
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// <see cref="Step"/> content area that is linked with a <see cref="Step"/> with the same name and that is placed within the <see cref="Steps"/> component.
    /// </summary>
    public partial class StepPanel : BaseComponent
    {
        #region Members

        private StepsState parentStepsState;

        private StepsContentState parentStepsContentState;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            if ( ParentSteps != null )
            {
                ParentSteps.NotifyStepInitialized( Name );
            }

            if ( ParentStepsContent != null )
            {
                ParentStepsContent.NotifyStepPanelInitialized( Name );
            }

            base.OnInitialized();
        }

        /// <inheritdoc/>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( ParentSteps != null )
                {
                    ParentSteps.NotifyStepRemoved( Name );
                }

                if ( ParentStepsContent != null )
                {
                    ParentStepsContent.NotifyStepPanelRemoved( Name );
                }
            }

            base.Dispose( disposing );
        }

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.StepPanel() );
            builder.Append( ClassProvider.StepPanelActive( Active ) );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// True if the step panel is currently selected.
        /// </summary>
        protected bool Active => ParentStepsState?.SelectedStep == Name || ParentStepsContentStore?.SelectedPanel == Name;

        /// <summary>
        /// Defines the panel name. Must match the corresponding step name.
        /// </summary>
        [Parameter] public string Name { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="StepPanel"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Cascaded <see cref="Steps"/> component state object.
        /// </summary>
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

        /// <summary>
        /// Cascaded <see cref="StepsContent"/> component state object.
        /// </summary>
        [CascadingParameter]
        protected StepsContentState ParentStepsContentStore
        {
            get => parentStepsContentState;
            set
            {
                if ( parentStepsContentState == value )
                    return;

                parentStepsContentState = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Gets or sets the reference to the parent <see cref="Steps"/> component.
        /// </summary>
        [CascadingParameter] protected Steps ParentSteps { get; set; }

        /// <summary>
        /// Gets or sets the reference to the parent <see cref="StepsContent"/> component.
        /// </summary>
        [CascadingParameter] protected StepsContent ParentStepsContent { get; set; }

        #endregion
    }
}
