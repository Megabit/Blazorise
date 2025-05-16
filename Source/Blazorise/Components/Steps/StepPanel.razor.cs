#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// <see cref="Step"/> content area that is linked with a <see cref="Step"/> with the same name and that is placed within the <see cref="Steps"/> component.
/// </summary>
public partial class StepPanel : BaseComponent, IDisposable
{
    #region Members    
    
    /// <summary>
    /// Tracks whether the component fulfills the requirements to be lazy loaded and then kept rendered to the DOM.
    /// </summary>
    private bool lazyLoaded;

    private StepsState parentStepsState;

    private StepsContentState parentStepsContentState;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ParentSteps?.NotifyStepInitialized( Name );

        ParentStepsContent?.NotifyStepPanelInitialized( Name );

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override Task OnParametersSetAsync()
    {
        if ( Active )
            lazyLoaded = ( RenderMode == StepsRenderMode.LazyLoad );
        return base.OnParametersSetAsync();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            ParentSteps?.NotifyStepRemoved( Name );

            ParentStepsContent?.NotifyStepPanelRemoved( Name );
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
    /// Gets the current render mode.
    /// </summary>
    protected StepsRenderMode RenderMode => ParentStepsState?.RenderMode ?? ParentStepsState?.RenderMode ?? StepsRenderMode.Default;

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