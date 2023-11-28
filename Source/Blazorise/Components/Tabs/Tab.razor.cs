#region Using directives
using System.Threading.Tasks;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// A clickable item for <see cref="Tabs"/> component.
/// </summary>
public partial class Tab : BaseComponent
{
    #region Members

    /// <summary>
    /// A reference to the parent tabs state.
    /// </summary>
    private TabsState parentTabsState;

    /// <summary>
    /// Flag to indicate that the tab is not responsive for user interaction.
    /// </summary>
    private bool disabled;

    #endregion

    #region Constructors

    /// <summary>
    /// Default <see cref="Tab"/> constructor.
    /// </summary>
    public Tab()
    {
        LinkClassBuilder = new( BuildLinkClasses );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ParentTabs?.NotifyTabInitialized( Name );

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.TabItem() );
        builder.Append( ClassProvider.TabItemActive( Active ) );
        builder.Append( ClassProvider.TabItemDisabled( Disabled ) );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Builds a list of classnames for the link element.
    /// </summary>
    /// <param name="builder">Class builder used to append the classnames.</param>
    private void BuildLinkClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.TabLink( ParentTabs?.TabPosition ?? TabPosition.Top ) );
        builder.Append( ClassProvider.TabLinkActive( Active ) );
        builder.Append( ClassProvider.TabLinkDisabled( Disabled ) );
    }

    /// <summary>
    /// Marks the classnames as dirty so they can be rebuilt.
    /// </summary>
    internal protected override void DirtyClasses()
    {
        LinkClassBuilder.Dirty();

        base.DirtyClasses();
    }

    /// <summary>
    /// Handles the item onclick event.
    /// </summary>
    /// <param name="eventArgs">Supplies information about a mouse event that is being raised.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected async Task ClickHandler( MouseEventArgs eventArgs )
    {
        if ( Disabled )
            return;

        await Clicked.InvokeAsync( eventArgs );

        if ( ParentTabs is not null )
            await ParentTabs.SelectTab( Name );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the class builder for the link element.
    /// </summary>
    protected ClassBuilder LinkClassBuilder { get; private set; }

    /// <summary>
    /// Gets the link class-names.
    /// </summary>
    protected string LinkClassNames => LinkClassBuilder.Class;

    /// <summary>
    /// True if this tab is currently set as selected.
    /// </summary>
    protected bool Active => ParentTabsState?.SelectedTab == Name;

    /// <summary>
    /// Defines the tab name. Must match the corresponding panel name.
    /// </summary>
    [Parameter] public string Name { get; set; }

    /// <summary>
    /// Flag to indicate that the tab is not responsive for user interaction.
    /// </summary>
    [Parameter]
    public bool Disabled
    {
        get => disabled;
        set
        {
            disabled = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Occurs when the item is clicked.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> Clicked { get; set; }

    /// <summary>
    /// Cascaded parent <see cref="Tabs"/> state.
    /// </summary>
    [CascadingParameter]
    protected TabsState ParentTabsState
    {
        get => parentTabsState;
        set
        {
            if ( parentTabsState == value )
                return;

            parentTabsState = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Cascaded parent <see cref="Tabs"/> component.
    /// </summary>
    [CascadingParameter] protected Tabs ParentTabs { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Tab"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}