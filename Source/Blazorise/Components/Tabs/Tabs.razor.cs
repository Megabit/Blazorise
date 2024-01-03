#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Tabs organize content across different screens, data sets, and other interactions.
/// </summary>
public partial class Tabs : BaseComponent
{
    #region Members

    /// <summary>
    /// Holds the state of this tabs component.
    /// </summary>
    private TabsState state = new()
    {
        TabPosition = TabPosition.Top,
    };

    /// <summary>
    /// List of all tab names that are placed inside if this component.
    /// </summary>
    private List<string> tabItems = new();

    /// <summary>
    /// List of all panel names that are placed inside if this component.
    /// </summary>
    private List<string> tabPanels = new();

    #endregion

    #region Constructors

    /// <summary>
    /// Default <see cref="Tabs"/> constructor.
    /// </summary>
    public Tabs()
    {
        ContentClassBuilder = new( BuildContentClasses );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Tabs( Pills ) );
        builder.Append( ClassProvider.TabsCards(), IsCards );
        builder.Append( ClassProvider.TabsFullWidth(), FullWidth );
        builder.Append( ClassProvider.TabsJustified(), Justified );
        builder.Append( ClassProvider.TabsVertical(), TabPosition == TabPosition.Start || TabPosition == TabPosition.End );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Builds a list of classnames for the content container element.
    /// </summary>
    /// <param name="builder">Class builder used to append the classnames.</param>
    protected virtual void BuildContentClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.TabsContent() );
    }

    /// <summary>
    /// Notify this <see cref="Tabs"/> component that it's <see cref="Tab"/> child component is placed inside of it.
    /// </summary>
    /// <param name="name">The name of the tab.</param>
    internal void NotifyTabInitialized( string name )
    {
        if ( !tabItems.Contains( name ) )
            tabItems.Add( name );
    }

    /// <summary>
    /// Notify this <see cref="Tabs"/> component that it's <see cref="Tab"/> child component is removed from it.
    /// </summary>
    /// <param name="name">The name of the tab.</param>
    internal void NotifyTabRemoved( string name )
    {
        if ( tabItems.Contains( name ) )
            tabItems.Remove( name );
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
    /// Sets the active tab by the name.
    /// </summary>
    /// <param name="tabName">The name of the tab.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task SelectTab( string tabName )
    {
        SelectedTab = tabName;

        return InvokeAsync( StateHasChanged );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the reference to the tabs state object.
    /// </summary>
    protected TabsState State => state;

    /// <summary>
    /// True if <see cref="Tabs"/> is placed inside of <see cref="CardHeader"/> component.
    /// </summary>
    protected bool IsCards => CardHeader is not null;

    /// <summary>
    /// Gets or sets the class builder for the content container element.
    /// </summary>
    protected ClassBuilder ContentClassBuilder { get; private set; }

    /// <summary>
    /// Gets the content class-names.
    /// </summary>
    protected string ContentClassNames => ContentClassBuilder.Class;

    /// <summary>
    /// Get the index of the currently selected tab.
    /// </summary>
    protected int IndexOfSelectedTab => tabItems.IndexOf( state.SelectedTab );

    /// <summary>
    /// Gets the list of all tab item names that are placed inside of this container.
    /// </summary>
    protected IReadOnlyList<string> TabItems => tabItems;

    /// <summary>
    /// Gets the list of all tab panel names that are placed inside of this container.
    /// </summary>
    protected IReadOnlyList<string> TabPanels => tabPanels;

    /// <summary>
    /// Makes the tab items to appear as pills.
    /// </summary>
    [Parameter]
    public bool Pills
    {
        get => state.Pills;
        set
        {
            state = state with { Pills = value };

            DirtyClasses();
        }
    }

    /// <summary>
    /// Makes the tab items to extend the full available width.
    /// </summary>
    [Parameter]
    public bool FullWidth
    {
        get => state.FullWidth;
        set
        {
            state = state with { FullWidth = value };

            DirtyClasses();
        }
    }

    /// <summary>
    /// Makes the tab items to extend the full available width, but every item will be the same width.
    /// </summary>
    [Parameter]
    public bool Justified
    {
        get => state.Justified;
        set
        {
            state = state with { Justified = value };

            DirtyClasses();
        }
    }

    /// <summary>
    /// Position of tab items.
    /// </summary>
    [Parameter]
    public TabPosition TabPosition
    {
        get => state.TabPosition;
        set
        {
            state = state with { TabPosition = value };

            DirtyClasses();
        }
    }

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

    /// <summary>
    /// Controls the size of the items bar when in vertical mode. If left undefined it will default to the <c>ColumnSize.IsAuto</c>.
    /// </summary>
    [Parameter] public IFluentColumn VerticalItemsColumnSize { get; set; }

    /// <summary>
    /// Gets or sets currently selected tab name.
    /// </summary>
    [Parameter]
    public string SelectedTab
    {
        get => state.SelectedTab;
        set
        {
            // prevent tabs from calling the same code multiple times
            if ( value == state.SelectedTab )
                return;

            state = state with { SelectedTab = value };

            // raise the tabchanged notification                
            SelectedTabChanged.InvokeAsync( state.SelectedTab );

            DirtyClasses();
        }
    }

    /// <summary>
    /// Occurs after the selected tab has changed.
    /// </summary>
    [Parameter] public EventCallback<string> SelectedTabChanged { get; set; }

    /// <summary>
    /// Cascaded parent <see cref="CardHeader"/> when <see cref="Tabs"/> is placed inside of card.
    /// </summary>
    [CascadingParameter] protected CardHeader CardHeader { get; set; }

    /// <summary>
    /// Container for tab items.
    /// </summary>
    [Parameter] public RenderFragment Items { get; set; }

    /// <summary>
    /// Container for tab panes.
    /// </summary>
    [Parameter] public RenderFragment Content { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Tabs"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}