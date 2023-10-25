#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// A container for tab panels.
/// </summary>
public partial class TabsContent : BaseComponent
{
    #region Members

    /// <summary>
    /// Holds the state of this tabs content component.
    /// </summary>
    private TabsContentState state = new();

    /// <summary>
    /// List of all panel names that are placed inside if this component.
    /// </summary>
    private readonly List<string> tabPanels = new();

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.TabsContent() );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Notify this <see cref="Tabs"/> component that it's <see cref="TabPanel"/> child component is placed inside of it.
    /// </summary>
    /// <param name="name">The name of the panel.</param>
    internal void NotifyTabPanelInitialized( string name )
    {
        if ( !tabPanels.Contains( name ) )
            tabPanels.Add( name );
    }

    /// <summary>
    /// Notify this <see cref="Tabs"/> component that it's <see cref="TabPanel"/> child component is removed from it.
    /// </summary>
    /// <param name="name">The name of the panel.</param>
    internal void NotifyTabPanelRemoved( string name )
    {
        if ( tabPanels.Contains( name ) )
            tabPanels.Remove( name );
    }

    /// <summary>
    /// Sets the active panel by the name.
    /// </summary>
    /// <param name="name">The name of the panel.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task SelectPanel( string name )
    {
        SelectedPanel = name;

        return InvokeAsync( StateHasChanged );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the reference to the tabs content state object.
    /// </summary>
    protected TabsContentState State => state;

    /// <summary>
    /// Get the index of the currently selected panel.
    /// </summary>
    protected int IndexOfSelectedPanel => tabPanels.IndexOf( state.SelectedPanel );

    /// <summary>
    /// Gets or sets currently selected panel name.
    /// </summary>
    [Parameter]
    public string SelectedPanel
    {
        get => state.SelectedPanel;
        set
        {
            // prevent panels from calling the same code multiple times
            if ( value == state.SelectedPanel )
                return;

            state = state with { SelectedPanel = value };

            // raise the tabchanged notification
            SelectedPanelChanged.InvokeAsync( state.SelectedPanel );

            DirtyClasses();
        }
    }

    /// <summary>
    /// Occurs after the selected panel has changed.
    /// </summary>
    [Parameter] public EventCallback<string> SelectedPanelChanged { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="TabsContent"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Defines how the tabs content will be rendered.
    /// </summary>
    [Parameter]
    public TabsRenderMode RenderMode
    {
        get => state.RenderMode;
        set
        {
            state = state with { RenderMode = value };

            DirtyClasses();
        }
    }

    #endregion
}