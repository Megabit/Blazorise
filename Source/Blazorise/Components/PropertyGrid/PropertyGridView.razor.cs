#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Builds a <see cref="PropertyGrid"/> from a schema while allowing each rendering level to be customized.
/// </summary>
public partial class PropertyGridView : BaseComponent
{
    #region Members

    private const int AtomicComponentParameterCapacity = 20;

    private PropertyGridToolbarContext toolbarContext;

    private readonly Dictionary<string, bool> groupExpandedStates = [];

    #endregion

    #region Methods

    internal async Task ChangeValueAsync( PropertyGridProperty property, object value )
    {
        await PropertyValueChanged.InvokeAsync( new PropertyGridValueChangedEventArgs( property, value ) );
    }

    internal async Task ChangeViewModeAsync( PropertyGridViewMode viewMode )
    {
        if ( ViewMode == viewMode )
            return;

        ViewMode = viewMode;

        await ViewModeChanged.InvokeAsync( viewMode );
    }

    internal async Task ChangeSearchTextAsync( string searchText )
    {
        searchText ??= string.Empty;

        if ( string.Equals( SearchText, searchText, System.StringComparison.Ordinal ) )
            return;

        SearchText = searchText;

        await SearchTextChanged.InvokeAsync( searchText );
    }

    internal async Task ChangeGroupExpandedAsync( PropertyGridGroupDefinition group, bool expanded )
    {
        if ( group is null || IsGroupExpanded( group ) == expanded )
            return;

        groupExpandedStates[group.Key] = expanded;

        await GroupExpandedChanged.InvokeAsync( new PropertyGridGroupExpandedEventArgs( group, expanded ) );
    }

    internal async Task SelectPropertyAsync( PropertyGridProperty property )
    {
        if ( property is null || IsPropertySelected( property ) )
            return;

        SelectedProperty = property;

        await SelectedPropertyChanged.InvokeAsync( property );
    }

    internal async Task InvokeActionAsync( PropertyGridProperty property )
    {
        if ( property.Action is null || !property.Action.Visible || property.Action.Disabled )
            return;

        await ActionInvoked.InvokeAsync( new PropertyGridActionEventArgs( property, property.Action ) );
    }

    private RenderFragment GetGroupHeader( PropertyGridGroupDefinition group )
    {
        RenderFragment<PropertyGridGroupContext> template = group.HeaderTemplate ?? GroupHeaderTemplate;

        return template is null ? null : template( new PropertyGridGroupContext( this, group ) );
    }

    private RenderFragment GetLabelContent( PropertyGridProperty property )
    {
        RenderFragment<PropertyGridLabelContext> template = property.LabelTemplate ?? LabelTemplate;

        return template is null ? null : template( new PropertyGridLabelContext( property ) );
    }

    private RenderFragment GetActionContent( PropertyGridProperty property )
    {
        PropertyGridAction action = property.Action;

        if ( action?.Visible != true )
            return null;

        RenderFragment<PropertyGridActionContext> actionTemplate = action.ActionTemplate ?? ActionTemplate;

        if ( actionTemplate is not null )
            return actionTemplate( new PropertyGridActionContext( this, property ) );

        return builder =>
        {
            builder.OpenComponent<Button>( 0 );
            builder.AddAttribute( 1, nameof( Button.Color ), action.Color );
            builder.AddAttribute( 2, nameof( Button.Size ), property.Size );
            builder.AddAttribute( 3, nameof( Button.Disabled ), action.Disabled );
            builder.AddAttribute( 4, nameof( Button.Clicked ), EventCallback.Factory.Create<MouseEventArgs>( this, () => InvokeActionAsync( property ) ) );
            builder.AddAttribute( 5, "title", action.Title );
            builder.AddAttribute( 6, "aria-label", action.Title );
            builder.AddAttribute( 7, nameof( Button.ChildContent ), (RenderFragment)( contentBuilder =>
            {
                if ( action.Icon is not null )
                {
                    contentBuilder.OpenComponent<Icon>( 0 );
                    contentBuilder.AddAttribute( 1, nameof( Icon.Name ), action.Icon );
                    contentBuilder.CloseComponent();
                }

                if ( action.Icon is not null && !string.IsNullOrEmpty( action.Text ) )
                    contentBuilder.AddContent( 2, " " );

                contentBuilder.AddContent( 3, action.Text );
            } ) );
            builder.CloseComponent();
        };
    }

    private RenderFragment<PropertyGridEditorContext> GetEditorTemplate( PropertyGridProperty property )
    {
        if ( property.EditorTemplate is not null )
            return property.EditorTemplate;

        return property.EditorType switch
        {
            PropertyGridEditorType.Text => TextEditorTemplate,
            PropertyGridEditorType.Boolean => BooleanEditorTemplate,
            PropertyGridEditorType.Numeric => NumericEditorTemplate,
            PropertyGridEditorType.Select => SelectEditorTemplate,
            PropertyGridEditorType.Color => ColorEditorTemplate,
            _ => null,
        };
    }

    private IDictionary<string, object> GetAtomicComponentParameters( PropertyGridProperty property, bool selected, string ariaDescribedBy )
    {
        Dictionary<string, object> parameters = new( AtomicComponentParameterCapacity )
        {
            [nameof( PropertyGridItem.Label )] = property.Label,
            [nameof( PropertyGridItem.Size )] = property.Size,
            [nameof( PropertyGridItem.Class )] = property.Class,
            [nameof( PropertyGridItem.Style )] = property.Style,
            [nameof( BasePropertyGridEditorItem.Mixed )] = property.Mixed,
            [nameof( BasePropertyGridEditorItem.LabelContent )] = GetLabelContent( property ),
            [nameof( BasePropertyGridEditorItem.Selectable )] = true,
            [nameof( BasePropertyGridEditorItem.Selected )] = selected,
            [nameof( BasePropertyGridEditorItem.SelectedChanged )] = EventCallback.Factory.Create<bool>( this, value => value ? SelectPropertyAsync( property ) : Task.CompletedTask ),
            [nameof( BasePropertyGridEditorItem.AriaDescribedBy )] = ariaDescribedBy,
            [nameof( BasePropertyGridEditorItem.ActionContent )] = GetActionContent( property ),
            [nameof( BasePropertyGridEditorItem.Attributes )] = property.Attributes,
            ["Value"] = property.Value,
            ["ValueChanged"] = property.CreateValueChangedCallback( this ),
        };

        property.AddAtomicComponentParameters( parameters );

        return parameters;
    }

    private PropertyGridProperty GetSelectedProperty()
    {
        if ( SelectedProperty is null )
            return null;

        return Schema is null
            ? SelectedProperty
            : Schema.FindProperty( SelectedProperty.Key );
    }

    internal IconName GetViewModeIcon( PropertyGridViewMode viewMode )
        => viewMode == PropertyGridViewMode.Alphabetical ? AlphabeticalButtonIcon : CategorizedButtonIcon;

    internal string GetViewModeTitle( PropertyGridViewMode viewMode )
        => viewMode == PropertyGridViewMode.Alphabetical ? AlphabeticalButtonTitle : CategorizedButtonTitle;

    internal bool IsGroupExpanded( PropertyGridGroupDefinition group )
        => group is not null
            && ( groupExpandedStates.TryGetValue( group.Key, out bool expanded ) ? expanded : group.Expanded );

    internal bool IsPropertySelected( PropertyGridProperty property )
        => IsPropertySelected( property, GetSelectedProperty() );

    private static bool IsPropertySelected( PropertyGridProperty property, PropertyGridProperty selectedProperty )
        => property is not null
            && selectedProperty is not null
            && string.Equals( property.Key, selectedProperty.Key, System.StringComparison.Ordinal );

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Gets the provider class for empty search results.
    /// </summary>
    protected string EmptyClassNames => ClassProvider.PropertyGridEmpty();

    /// <summary>
    /// Gets the stable help element id.
    /// </summary>
    protected string HelpElementId => $"{ElementId}-help";

    private PropertyGridToolbarContext ToolbarContext => toolbarContext ??= new( this );

    /// <summary>
    /// Gets or sets the schema rendered by the property grid.
    /// </summary>
    [Parameter] public PropertyGridSchema Schema { get; set; }

    /// <summary>
    /// Occurs after a property value changes.
    /// </summary>
    [Parameter] public EventCallback<PropertyGridValueChangedEventArgs> PropertyValueChanged { get; set; }

    /// <summary>
    /// Occurs after a property action is invoked.
    /// </summary>
    [Parameter] public EventCallback<PropertyGridActionEventArgs> ActionInvoked { get; set; }

    /// <summary>
    /// Occurs after a property group expansion state changes.
    /// </summary>
    [Parameter] public EventCallback<PropertyGridGroupExpandedEventArgs> GroupExpandedChanged { get; set; }

    /// <summary>
    /// Gets or sets the selected property.
    /// </summary>
    [Parameter] public PropertyGridProperty SelectedProperty { get; set; }

    /// <summary>
    /// Occurs after the selected property changes.
    /// </summary>
    [Parameter] public EventCallback<PropertyGridProperty> SelectedPropertyChanged { get; set; }

    /// <summary>
    /// Gets or sets whether the property grid toolbar is shown.
    /// </summary>
    [Parameter] public bool ShowToolbar { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the categorized and alphabetical view buttons are shown.
    /// </summary>
    [Parameter] public bool ShowViewModeButtons { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the property search editor is shown.
    /// </summary>
    [Parameter] public bool ShowSearch { get; set; } = true;

    /// <summary>
    /// Gets or sets whether help for the selected property is shown.
    /// </summary>
    [Parameter] public bool ShowHelp { get; set; } = true;

    /// <summary>
    /// Gets or sets the accessible property grid label.
    /// </summary>
    [Parameter] public string AriaLabel { get; set; } = "Properties";

    /// <summary>
    /// Gets or sets how properties are arranged.
    /// </summary>
    [Parameter] public PropertyGridViewMode ViewMode { get; set; } = PropertyGridViewMode.Categorized;

    /// <summary>
    /// Occurs after the property arrangement changes.
    /// </summary>
    [Parameter] public EventCallback<PropertyGridViewMode> ViewModeChanged { get; set; }

    /// <summary>
    /// Gets or sets the property search text.
    /// </summary>
    [Parameter] public string SearchText { get; set; }

    /// <summary>
    /// Occurs after the property search text changes.
    /// </summary>
    [Parameter] public EventCallback<string> SearchTextChanged { get; set; }

    /// <summary>
    /// Gets or sets the property search placeholder.
    /// </summary>
    [Parameter] public string SearchPlaceholder { get; set; } = "Search";

    /// <summary>
    /// Gets or sets whether property search changes are debounced.
    /// </summary>
    [Parameter] public bool SearchDebounce { get; set; } = true;

    /// <summary>
    /// Gets or sets the property search debounce interval in milliseconds.
    /// </summary>
    [Parameter] public int SearchDebounceInterval { get; set; } = 300;

    /// <summary>
    /// Gets or sets the categorized button icon.
    /// </summary>
    [Parameter] public IconName CategorizedButtonIcon { get; set; } = IconName.List;

    /// <summary>
    /// Gets or sets the categorized button title.
    /// </summary>
    [Parameter] public string CategorizedButtonTitle { get; set; } = "Categorized";

    /// <summary>
    /// Gets or sets the alphabetical button icon.
    /// </summary>
    [Parameter] public IconName AlphabeticalButtonIcon { get; set; } = IconName.SortAlphaDown;

    /// <summary>
    /// Gets or sets the alphabetical button title.
    /// </summary>
    [Parameter] public string AlphabeticalButtonTitle { get; set; } = "Alphabetical";

    /// <summary>
    /// Defines the complete toolbar template.
    /// </summary>
    [Parameter] public RenderFragment<PropertyGridToolbarContext> ToolbarTemplate { get; set; }

    /// <summary>
    /// Defines the categorized view button template.
    /// </summary>
    [Parameter] public RenderFragment<PropertyGridViewModeContext> CategorizedButtonTemplate { get; set; }

    /// <summary>
    /// Defines the alphabetical view button template.
    /// </summary>
    [Parameter] public RenderFragment<PropertyGridViewModeContext> AlphabeticalButtonTemplate { get; set; }

    /// <summary>
    /// Defines the property search template.
    /// </summary>
    [Parameter] public RenderFragment<PropertyGridSearchContext> SearchTemplate { get; set; }

    /// <summary>
    /// Defines a complete group template.
    /// </summary>
    [Parameter] public RenderFragment<PropertyGridGroupContext> GroupTemplate { get; set; }

    /// <summary>
    /// Defines a group header template.
    /// </summary>
    [Parameter] public RenderFragment<PropertyGridGroupContext> GroupHeaderTemplate { get; set; }

    /// <summary>
    /// Defines a complete property item template.
    /// </summary>
    [Parameter] public RenderFragment<PropertyGridItemContext> ItemTemplate { get; set; }

    /// <summary>
    /// Defines a property label template.
    /// </summary>
    [Parameter] public RenderFragment<PropertyGridLabelContext> LabelTemplate { get; set; }

    /// <summary>
    /// Defines a text editor template.
    /// </summary>
    [Parameter] public RenderFragment<PropertyGridEditorContext> TextEditorTemplate { get; set; }

    /// <summary>
    /// Defines a numeric editor template.
    /// </summary>
    [Parameter] public RenderFragment<PropertyGridEditorContext> NumericEditorTemplate { get; set; }

    /// <summary>
    /// Defines a boolean editor template.
    /// </summary>
    [Parameter] public RenderFragment<PropertyGridEditorContext> BooleanEditorTemplate { get; set; }

    /// <summary>
    /// Defines a select editor template.
    /// </summary>
    [Parameter] public RenderFragment<PropertyGridEditorContext> SelectEditorTemplate { get; set; }

    /// <summary>
    /// Defines a color editor template.
    /// </summary>
    [Parameter] public RenderFragment<PropertyGridEditorContext> ColorEditorTemplate { get; set; }

    /// <summary>
    /// Defines an action template.
    /// </summary>
    [Parameter] public RenderFragment<PropertyGridActionContext> ActionTemplate { get; set; }

    /// <summary>
    /// Defines the selected property help template.
    /// </summary>
    [Parameter] public RenderFragment<PropertyGridHelpContext> HelpTemplate { get; set; }

    /// <summary>
    /// Defines content rendered after all schema groups.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Defines content rendered when the schema and child content are empty.
    /// </summary>
    [Parameter] public RenderFragment EmptyTemplate { get; set; }

    /// <summary>
    /// Defines content rendered when no properties match the current search.
    /// </summary>
    [Parameter] public RenderFragment NoResultsTemplate { get; set; }

    /// <summary>
    /// Defines the text rendered when no properties match the current search.
    /// </summary>
    [Parameter] public string NoResultsText { get; set; } = "No properties found.";

    #endregion
}