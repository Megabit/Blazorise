#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Steps is a navigation bar that guides users through the steps of a task.
/// </summary>
public partial class Steps : BaseComponent<StepsClasses, StepsStyles>
{
    #region Members

    private StepsState state = new();

    private readonly List<string> stepItems = new();

    private readonly List<string> stepPanels = new();

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
        var selectedStepIndex = stepItems.IndexOf( SelectedStep );

        if ( selectedStepIndex == stepItems.Count - 1 )
        {
            return false;
        }

        return await SelectStep( stepItems[selectedStepIndex + 1] );
    }

    /// <summary>
    /// Goes to the previous step.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task<bool> PreviousStep()
    {
        var selectedStepIndex = stepItems.IndexOf( SelectedStep );

        if ( selectedStepIndex <= 0 )
        {
            return false;
        }

        return await SelectStep( stepItems[selectedStepIndex - 1] );
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

    /// <summary>
    /// Gets the steps state object.
    /// </summary>
    protected StepsState State => state;

    /// <summary>
    /// Gets the list of all <see cref="Step"/>s  within the <see cref="Steps"/>.
    /// </summary>
    protected IReadOnlyList<string> StepItems => stepItems;

    /// <summary>
    /// Gets the list of all <see cref="StepPanel"/>s within the <see cref="Steps"/>.
    /// </summary>
    protected IReadOnlyList<string> StepPanels => stepPanels;

    /// <summary>
    /// Content element class builder.
    /// </summary>
    protected ClassBuilder ContentClassBuilder { get; private set; }

    /// <summary>
    /// Gets the classnames for the content element.
    /// </summary>
    protected string ContentClassNames => ContentClassBuilder.Class;

    /// <summary>
    /// Gets or sets currently selected step name.
    /// </summary>
    [Parameter] public string SelectedStep { get; set; }

    /// <summary>
    /// Defines how the steps content will be rendered.
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