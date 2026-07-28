#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Builds a <see cref="PropertyGrid"/> from a schema while allowing each rendering level to be customized.
/// </summary>
public partial class PropertyGridView : BaseComponent
{
    #region Methods

    internal async Task ChangeValueAsync( PropertyGridProperty property, object value )
    {
        await ValueChanged.InvokeAsync( new PropertyGridValueChangedEventArgs( property, value ) );
    }

    internal async Task ChangeViewModeAsync( PropertyGridViewMode viewMode )
    {
        if ( ViewMode == viewMode )
            return;

        ViewMode = viewMode;

        await ViewModeChanged.InvokeAsync( viewMode );
    }

    internal async Task InvokeActionAsync( PropertyGridProperty property )
    {
        if ( property.Action is null || !property.Action.Visible || property.Action.Disabled )
            return;

        await ActionInvoked.InvokeAsync( new PropertyGridActionEventArgs( property, property.Action ) );
    }

    private RenderFragment GetGroupHeader( PropertyGridGroupDefinition group, PropertyGridGroupContext context )
    {
        RenderFragment<PropertyGridGroupContext> template = group.HeaderTemplate ?? GroupHeaderTemplate;

        return template is null ? null : template( context );
    }

    private RenderFragment GetLabelContent( PropertyGridProperty property )
    {
        RenderFragment<PropertyGridLabelContext> template = property.LabelTemplate ?? LabelTemplate;

        return template is null ? null : template( new PropertyGridLabelContext( property ) );
    }

    private RenderFragment GetActionTemplate( PropertyGridProperty property )
    {
        if ( property.Action is null )
            return null;

        RenderFragment<PropertyGridActionContext> template = property.Action.ActionTemplate ?? ActionTemplate;

        return template is null ? null : template( new PropertyGridActionContext( this, property ) );
    }

    private RenderFragment<PropertyGridEditorContext> GetEditorTemplate( PropertyGridProperty property )
    {
        if ( property.EditorTemplate is not null )
            return property.EditorTemplate;

        Type propertyType = property.GetType();

        if ( property is PropertyGridTextProperty )
            return TextEditorTemplate;

        if ( property is PropertyGridBooleanProperty )
            return BooleanEditorTemplate;

        if ( property is PropertyGridStringSelectProperty )
            return SelectEditorTemplate;

        if ( property is PropertyGridColorProperty )
            return ColorEditorTemplate;

        if ( propertyType.IsGenericType )
        {
            Type genericType = propertyType.GetGenericTypeDefinition();

            if ( genericType == typeof( PropertyGridNumericProperty<> ) )
                return NumericEditorTemplate;

            if ( genericType == typeof( PropertyGridSelectProperty<> ) )
                return SelectEditorTemplate;
        }

        return null;
    }

    private IDictionary<string, object> GetAtomicComponentParameters( PropertyGridProperty property )
    {
        Dictionary<string, object> parameters = new()
        {
            [nameof( PropertyGridItem.Label )] = property.Label,
            [nameof( PropertyGridItem.Size )] = property.Size,
            [nameof( PropertyGridItem.Class )] = property.Class,
            [nameof( PropertyGridItem.Style )] = property.Style,
            [nameof( BasePropertyGridEditorItem.Mixed )] = property.Mixed,
            [nameof( BasePropertyGridEditorItem.LabelContent )] = GetLabelContent( property ),
            [nameof( BasePropertyGridEditorItem.ActionVisible )] = property.Action?.Visible == true,
            [nameof( BasePropertyGridEditorItem.ActionDisabled )] = property.Action?.Disabled == true,
            [nameof( BasePropertyGridEditorItem.ActionColor )] = property.Action?.Color ?? Color.Light,
            [nameof( BasePropertyGridEditorItem.ActionIcon )] = property.Action?.Icon,
            [nameof( BasePropertyGridEditorItem.ActionText )] = property.Action?.Text,
            [nameof( BasePropertyGridEditorItem.ActionTitle )] = property.Action?.Title,
            [nameof( BasePropertyGridEditorItem.ActionTemplate )] = GetActionTemplate( property ),
            [nameof( BasePropertyGridEditorItem.ActionClicked )] = EventCallback.Factory.Create( this, () => InvokeActionAsync( property ) ),
            [nameof( BasePropertyGridEditorItem.Attributes )] = property.Attributes,
            ["Value"] = property.Value,
            ["ValueChanged"] = property.CreateValueChangedCallback( this ),
        };

        property.AddAtomicComponentParameters( parameters );

        return parameters;
    }

    internal IconName GetViewModeIcon( PropertyGridViewMode viewMode )
        => viewMode == PropertyGridViewMode.Alphabetical ? AlphabeticalButtonIcon : CategorizedButtonIcon;

    internal string GetViewModeTitle( PropertyGridViewMode viewMode )
        => viewMode == PropertyGridViewMode.Alphabetical ? AlphabeticalButtonTitle : CategorizedButtonTitle;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the provider class for the toolbar.
    /// </summary>
    protected string ToolbarClassName => ClassProvider.PropertyGridToolbar();

    /// <summary>
    /// Gets or sets the schema rendered by the property grid.
    /// </summary>
    [Parameter] public PropertyGridSchema Schema { get; set; }

    /// <summary>
    /// Occurs after a property value changes.
    /// </summary>
    [Parameter] public EventCallback<PropertyGridValueChangedEventArgs> ValueChanged { get; set; }

    /// <summary>
    /// Occurs after a property action is invoked.
    /// </summary>
    [Parameter] public EventCallback<PropertyGridActionEventArgs> ActionInvoked { get; set; }

    /// <summary>
    /// Gets or sets whether the categorized and alphabetical view buttons are shown.
    /// </summary>
    [Parameter] public bool ShowToolbar { get; set; } = true;

    /// <summary>
    /// Gets or sets how properties are arranged.
    /// </summary>
    [Parameter] public PropertyGridViewMode ViewMode { get; set; } = PropertyGridViewMode.Categorized;

    /// <summary>
    /// Occurs after the property arrangement changes.
    /// </summary>
    [Parameter] public EventCallback<PropertyGridViewMode> ViewModeChanged { get; set; }

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
    /// Defines content rendered after all schema groups.
    /// </summary>
    [Parameter] public RenderFragment AdditionalContent { get; set; }

    /// <summary>
    /// Defines content rendered when the schema and additional content are empty.
    /// </summary>
    [Parameter] public RenderFragment EmptyTemplate { get; set; }

    #endregion
}