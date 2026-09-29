#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Steps is a navigation bar that guides users through the steps of a task.
/// </summary>
public partial class Steps : BaseComponent<StepsClasses, StepsStyles>, IAsyncDisposable
{
    #region Members

    private StepsState state = new();

    private readonly List<Step> stepItems = new();

    private readonly List<StepPanel> stepPanels = new();

    #endregion

    #region Constructors

    /// <summary>
    /// A default <see cref="Step"/> constructor.
    /// </summary>
    public Steps()
    {
        ContentClassBuilder = new( BuildContentClasses, builder => builder.Append( Classes?.Content ) );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        await JSModule.Initialize( ElementRef, ElementId );

        await base.OnAfterRenderAsync( firstRender );
    }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        if ( SelectedStep != state.SelectedStep )
        {
            var success = await SelectStep( SelectedStep );

            if ( !success )
            {
                SelectedStep = state.SelectedStep;
                await SelectedStepChanged.InvokeAsync( state.SelectedStep );
            }
        }
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSModule.SafeDestroy( ElementRef, ElementId );
        }

        await base.DisposeAsync( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Steps() );

        base.BuildClasses( builder );
    }

    private void BuildContentClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.StepsContent() );
    }

    /// <inheritdoc/>
    protected internal override void DirtyClasses()
    {
        ContentClassBuilder.Dirty();

        base.DirtyClasses();
    }

    internal void NotifyStepInitialized( Step step )
    {
        if ( !stepItems.Contains( step ) )
        {
            stepItems.Add( step );
            InvokeAsync( StateHasChanged );
        }
    }

    internal void NotifyStepRemoved( Step step )
    {
        if ( stepItems.Remove( step ) )
        {
            InvokeAsync( StateHasChanged );
        }
    }

    internal void NotifyStepPanelInitialized( StepPanel panel )
    {
        if ( !stepPanels.Contains( panel ) )
        {
            stepPanels.Add( panel );
            InvokeAsync( StateHasChanged );
        }
    }

    internal void NotifyStepPanelRemoved( StepPanel panel )
    {
        if ( stepPanels.Remove( panel ) )
        {
            InvokeAsync( StateHasChanged );
        }
    }

    /// <summary>
    /// Gets the ID of the step associated with a panel.
    /// </summary>
    internal string GetStepElementId( string name ) => stepItems.Find( step => step.Name == name )?.ElementId;

    /// <summary>
    /// Gets the ID of the panel associated with a step.
    /// </summary>
    internal string GetStepPanelElementId( string name ) => stepPanels.Find( panel => panel.Name == name )?.ElementId;

    /// <summary>
    /// Refreshes step and panel associations after their names change.
    /// </summary>
    internal void NotifyStepParametersChanged() => InvokeAsync( StateHasChanged );

    /// <summary>
    /// Sets the active step by the name.
    /// </summary>
    /// <param name="stepName">The name of the step to set as active.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task<bool> SelectStep( string stepName )
    {
        // prevent steps from calling the same code multiple times
        if ( stepName == state.SelectedStep )
            return true;

        bool allowNavigation = NavigationAllowed == null;

        if ( NavigationAllowed is not null )
        {
            allowNavigation = await NavigationAllowed.Invoke( new StepNavigationContext
            {
                CurrentStepName = state.SelectedStep,
                CurrentStepIndex = IndexOfStep( state.SelectedStep ),
                NextStepName = stepName,
                NextStepIndex = IndexOfStep( stepName ),
            } );
        }

        if ( !allowNavigation )
            return false;

        state = state with
        {
            SelectedStep = stepName
        };

        // raise the changed notification
        await SelectedStepChanged.InvokeAsync( state.SelectedStep );

        DirtyClasses();

        await InvokeAsync( StateHasChanged );

        return true;
    }

    /// <summary>
    /// Goes to the next step.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task<bool> NextStep()
    {
        var selectedStepIndex = stepItems.FindIndex( step => step.Name == SelectedStep );

        if ( selectedStepIndex == stepItems.Count - 1 )
        {
            return false;
        }

        return await SelectStep( stepItems[selectedStepIndex + 1].Name );
    }

    /// <summary>
    /// Goes to the previous step.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task<bool> PreviousStep()
    {
        var selectedStepIndex = stepItems.FindIndex( step => step.Name == SelectedStep );

        if ( selectedStepIndex <= 0 )
        {
            return false;
        }

        return await SelectStep( stepItems[selectedStepIndex - 1].Name );
    }

    /// <summary>
    /// Returns the index of the step item.
    /// </summary>
    /// <param name="name">Name of the step item.</param>
    /// <returns>The one-based index or 0 if not found.</returns>
    internal int IndexOfStep( string name )
    {
        return stepItems.FindIndex( step => step.Name == name ) + 1;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the steps state object.
    /// </summary>
    protected StepsState State => state;

    /// <summary>
    /// Gets the list of all <see cref="Step"/>s  within the <see cref="Steps"/>.
    /// </summary>
    protected IReadOnlyList<string> StepItems => stepItems.Select( step => step.Name ).ToArray();

    /// <summary>
    /// Gets the list of all <see cref="StepPanel"/>s within the <see cref="Steps"/>.
    /// </summary>
    protected IReadOnlyList<string> StepPanels => stepPanels.Select( panel => panel.Name ).ToArray();

    /// <summary>
    /// Content element class builder.
    /// </summary>
    protected ClassBuilder ContentClassBuilder { get; private set; }

    /// <summary>
    /// Gets the classnames for the content element.
    /// </summary>
    protected string ContentClassNames => ContentClassBuilder.Class;

    /// <summary>
    /// Gets the shared tab-list keyboard navigation module.
    /// </summary>
    [Inject] protected IJSTabsModule JSModule { get; set; }

    /// <summary>
    /// Specifies the accessible name of the step list when no visible label is available.
    /// </summary>
    [Parameter] public string AriaLabel { get; set; }

    /// <summary>
    /// Specifies the space-separated IDs of elements that label the step list.
    /// </summary>
    [Parameter] public string AriaLabelledBy { get; set; }

    /// <summary>
    /// Specifies the currently selected step name.
    /// </summary>
    [Parameter] public string SelectedStep { get; set; }

    /// <summary>
    /// Specifies how the steps content will be rendered.
    /// </summary>
    [Parameter]
    public StepsRenderMode RenderMode
    {
        get => state.RenderMode;
        set
        {
            state = state with { RenderMode = value };

            DirtyClasses();
        }
    }

    /// <summary>
    /// Occurs after the selected step has changed.
    /// </summary>
    [Parameter] public EventCallback<string> SelectedStepChanged { get; set; }

    /// <summary>
    /// Disables navigation by clicking on step.
    /// </summary>
    [Parameter] public Func<StepNavigationContext, Task<bool>> NavigationAllowed { get; set; }

    /// <summary>
    /// Template for placing the <see cref="Step"/> items.
    /// </summary>
    [Parameter] public RenderFragment Items { get; set; }

    /// <summary>
    /// Template for placing the <see cref="StepPanel"/> items.
    /// </summary>
    [Parameter] public RenderFragment Content { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Steps"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}