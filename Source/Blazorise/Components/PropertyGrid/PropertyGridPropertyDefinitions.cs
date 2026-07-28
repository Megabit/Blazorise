using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Blazorise;

/// <summary>
/// Describes a custom property rendered by its editor or item template.
/// </summary>
public sealed class PropertyGridCustomProperty<TValue> : PropertyGridProperty<TValue>
{
    /// <summary>
    /// Initializes a custom property.
    /// </summary>
    public PropertyGridCustomProperty( string key, string label, TValue value, RenderFragment<PropertyGridEditorContext> editorTemplate )
        : base( key, label, value )
    {
        EditorTemplate = editorTemplate ?? throw new ArgumentNullException( nameof( editorTemplate ) );
    }
}

/// <summary>
/// Describes a text property.
/// </summary>
public sealed class PropertyGridTextProperty : PropertyGridProperty<string>
{
    /// <summary>
    /// Initializes a text property.
    /// </summary>
    public PropertyGridTextProperty( string key, string label, string value )
        : base( key, label, value )
    {
    }

    /// <summary>
    /// Gets or sets whether the text editor is read-only.
    /// </summary>
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Gets or sets whether changes are reported immediately.
    /// </summary>
    public bool? Immediate { get; set; }

    internal override Type AtomicComponentType => typeof( PropertyGridTextItem );

    internal override PropertyGridEditorType EditorType => PropertyGridEditorType.Text;

    internal override void AddAtomicComponentParameters( IDictionary<string, object> parameters )
    {
        parameters[nameof( PropertyGridTextItem.ReadOnly )] = ReadOnly;
        parameters[nameof( PropertyGridTextItem.Immediate )] = Immediate;
    }
}

/// <summary>
/// Describes a boolean property.
/// </summary>
public sealed class PropertyGridBooleanProperty : PropertyGridProperty<bool>
{
    /// <summary>
    /// Initializes a boolean property.
    /// </summary>
    public PropertyGridBooleanProperty( string key, string label, bool value )
        : base( key, label, value )
    {
    }

    /// <summary>
    /// Gets or sets the text displayed for a true value.
    /// </summary>
    public string TrueText { get; set; } = "True";

    /// <summary>
    /// Gets or sets the text displayed for a false value.
    /// </summary>
    public string FalseText { get; set; } = "False";

    internal override Type AtomicComponentType => typeof( PropertyGridBooleanItem );

    internal override PropertyGridEditorType EditorType => PropertyGridEditorType.Boolean;

    internal override void AddAtomicComponentParameters( IDictionary<string, object> parameters )
    {
        parameters[nameof( PropertyGridBooleanItem.TrueText )] = TrueText;
        parameters[nameof( PropertyGridBooleanItem.FalseText )] = FalseText;
    }
}

/// <summary>
/// Describes a numeric property.
/// </summary>
public sealed class PropertyGridNumericProperty<TValue> : PropertyGridProperty<TValue>
{
    /// <summary>
    /// Initializes a numeric property.
    /// </summary>
    public PropertyGridNumericProperty( string key, string label, TValue value )
        : base( key, label, value )
    {
    }

    /// <summary>
    /// Gets or sets the minimum accepted value.
    /// </summary>
    public double Min { get; set; }

    /// <summary>
    /// Gets or sets the maximum accepted value.
    /// </summary>
    public double? Max { get; set; }

    /// <summary>
    /// Gets or sets the numeric increment.
    /// </summary>
    public decimal? Step { get; set; } = 1m;

    /// <summary>
    /// Gets or sets whether changes are reported immediately.
    /// </summary>
    public bool Immediate { get; set; } = true;

    internal override Type AtomicComponentType => typeof( PropertyGridNumericItem<TValue> );

    internal override PropertyGridEditorType EditorType => PropertyGridEditorType.Numeric;

    internal override void AddAtomicComponentParameters( IDictionary<string, object> parameters )
    {
        parameters[nameof( PropertyGridNumericItem<TValue>.Min )] = Min;
        parameters[nameof( PropertyGridNumericItem<TValue>.Max )] = Max;
        parameters[nameof( PropertyGridNumericItem<TValue>.Step )] = Step;
        parameters[nameof( PropertyGridNumericItem<TValue>.Immediate )] = Immediate;
    }
}

/// <summary>
/// Describes a value-type select property.
/// </summary>
public sealed class PropertyGridSelectProperty<TValue> : PropertyGridProperty<TValue>
    where TValue : struct
{
    /// <summary>
    /// Initializes a value-type select property.
    /// </summary>
    public PropertyGridSelectProperty( string key, string label, TValue value, IReadOnlyList<PropertyGridSelectOption<TValue>> options )
        : base( key, label, value )
    {
        Options = options ?? [];
    }

    /// <summary>
    /// Gets the available options.
    /// </summary>
    public IReadOnlyList<PropertyGridSelectOption<TValue>> Options { get; }

    internal override Type AtomicComponentType => typeof( PropertyGridSelectItem<TValue> );

    internal override PropertyGridEditorType EditorType => PropertyGridEditorType.Select;

    internal override void AddAtomicComponentParameters( IDictionary<string, object> parameters )
        => parameters[nameof( PropertyGridSelectItem<TValue>.Options )] = Options;
}

/// <summary>
/// Describes a string select property.
/// </summary>
public sealed class PropertyGridStringSelectProperty : PropertyGridProperty<string>
{
    /// <summary>
    /// Initializes a string select property.
    /// </summary>
    public PropertyGridStringSelectProperty( string key, string label, string value, IReadOnlyList<PropertyGridSelectOption<string>> options )
        : base( key, label, value )
    {
        Options = options ?? [];
    }

    /// <summary>
    /// Gets the available options.
    /// </summary>
    public IReadOnlyList<PropertyGridSelectOption<string>> Options { get; }

    internal override Type AtomicComponentType => typeof( PropertyGridStringSelectItem );

    internal override PropertyGridEditorType EditorType => PropertyGridEditorType.Select;

    internal override void AddAtomicComponentParameters( IDictionary<string, object> parameters )
        => parameters[nameof( PropertyGridStringSelectItem.Options )] = Options;
}

/// <summary>
/// Describes a CSS color property.
/// </summary>
public sealed class PropertyGridColorProperty : PropertyGridProperty<string>
{
    /// <summary>
    /// Initializes a CSS color property.
    /// </summary>
    public PropertyGridColorProperty( string key, string label, string value )
        : base( key, label, value )
    {
    }

    /// <summary>
    /// Gets or sets the named color options.
    /// </summary>
    public IReadOnlyList<PropertyGridSelectOption<string>> NamedColors { get; set; }

    /// <summary>
    /// Gets or sets whether the color can be cleared.
    /// </summary>
    public bool Clearable { get; set; } = true;

    /// <summary>
    /// Gets or sets the accessible title for the clear action.
    /// </summary>
    public string ClearTitle { get; set; } = "Clear color";

    internal override Type AtomicComponentType => typeof( PropertyGridColorItem );

    internal override PropertyGridEditorType EditorType => PropertyGridEditorType.Color;

    internal override void AddAtomicComponentParameters( IDictionary<string, object> parameters )
    {
        parameters[nameof( PropertyGridColorItem.NamedColors )] = NamedColors;
        parameters[nameof( PropertyGridColorItem.Clearable )] = Clearable;
        parameters[nameof( PropertyGridColorItem.ClearTitle )] = ClearTitle;
    }
}