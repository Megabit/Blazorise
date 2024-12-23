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
public partial class Steps : BaseComponent
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
        ContentClassBuilder = new(BuildContentClasses);
    }

    #endregion

    #region Methods

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

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        if ( !string.IsNullOrEmpty( InitialStep ) )
            _ = await TrySelectStep( InitialStep );
    }

    /// <summary>
    /// Sets the active step by the name.
    /// </summary>
    /// <param name="stepName"></param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task<bool> TrySelectStep( string stepName )
    {
        // prevent steps from calling the same code multiple times
        if ( stepName == state.SelectedStep )
            return true;

        bool allowNavigation = NavigationAllowed == null;
        if ( NavigationAllowed != null )
            allowNavigation = await NavigationAllowed.Invoke( new StepNavigationContext
            {
                CurrentStepName = state.SelectedStep, CurrentStepIndex = IndexOfStep( state.SelectedStep ), NextStepName = stepName, NextStepIndex = IndexOfStep( stepName ),
            } );


        if ( allowNavigation == false )
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
    public async Task<bool> TryGoToNextStep()
    {
        var selectedStepIndex = stepItems.IndexOf( SelectedStep );

        if ( selectedStepIndex == stepItems.Count - 1 )
        {
            return false;
        }

        return await TrySelectStep( stepItems[selectedStepIndex + 1] );
    }

    /// <summary>
    /// Goes to the previous step.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task<bool> TryGoToPreviousStep()
    {
        var selectedStepIndex = stepItems.IndexOf( SelectedStep );

        if ( selectedStepIndex <= 0 )
        {
            return false;
        }

        return await TrySelectStep( stepItems[selectedStepIndex - 1] );
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
    /// Gets currently selected step name.
    /// </summary>
    public string SelectedStep => state.SelectedStep;

    /// <summary>
    /// Initial step to go on the component initialization
    /// </summary>
    [Parameter] public string InitialStep { get; set; } = string.Empty;

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