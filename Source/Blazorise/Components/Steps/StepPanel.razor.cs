#region Using directives
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class StepPanel : BaseComponent
    {
        #region Members

        private StepsState parentStepsState;

        private StepsContentState parentStepsContentState;

        #endregion

        #region Methods

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

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.StepPanel() );
            builder.Append( ClassProvider.StepPanelActive( Active ) );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        protected bool Active => ParentStepsState?.SelectedStep == Name || ParentStepsContentStore?.SelectedPanel == Name;

        /// <summary>
        /// Defines the panel name. Must match the coresponding step name.
        /// </summary>
        [Parameter] public string Name { get; set; }

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

        [CascadingParameter] protected Steps ParentSteps { get; set; }

        [CascadingParameter] protected StepsContent ParentStepsContent { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
