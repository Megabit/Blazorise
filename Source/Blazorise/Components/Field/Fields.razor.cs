#region Using directives
using System;
using System.Collections.Generic;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Container for multiple <see cref="Field"/> component that needs to be placed in a flexbox grid row.
/// </summary>
public partial class Fields : BaseColumnComponent
{
    #region Members

    private bool group;

    private string label;

    private string help;

    private IFluentGutter gutter;

    private FieldsLabel fieldsLabel;

    /// <summary>
    /// Raises when the fields label reference changes.
    /// </summary>
    internal event Action LabelElementChanged;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Fields() );

        if ( ColumnSize is not null )
        {
            builder.Append( ClassProvider.FieldsColumn() );
        }

        if ( gutter is not null )
            builder.Append( Gutter.Class( ClassProvider ) );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Registers the fields label component inside this container.
    /// </summary>
    /// <param name="fieldsLabel">Fields label component.</param>
    internal void NotifyFieldsLabelInitialized( FieldsLabel fieldsLabel )
    {
        if ( fieldsLabel is null )
            return;

        if ( ReferenceEquals( this.fieldsLabel, fieldsLabel ) )
            return;

        this.fieldsLabel = fieldsLabel;
        LabelElementChanged?.Invoke();
    }

    /// <summary>
    /// Removes the fields label component inside this container.
    /// </summary>
    /// <param name="fieldsLabel">Fields label component.</param>
    internal void NotifyFieldsLabelRemoved( FieldsLabel fieldsLabel )
    {
        if ( !ReferenceEquals( this.fieldsLabel, fieldsLabel ) )
            return;

        this.fieldsLabel = null;
        LabelElementChanged?.Invoke();
    }

    #endregion

    #region Properties   

    /// <summary>
    /// Gets the tag name rendered by this component.
    /// </summary>
    protected string ContainerTagName => Group ? "fieldset" : "div";

    /// <summary>
    /// Gets the element id of the fields label.
    /// </summary>
    internal string LabelElementId => fieldsLabel?.ElementId;

    /// <summary>
    /// Determines whether the fields container should render as a semantic group container.
    /// </summary>
    [Parameter]
    public bool Group
    {
        get => group;
        set
        {
            group = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Sets the field label.
    /// </summary>
    [Parameter]
    public string Label
    {
        get => label;
        set
        {
            label = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Sets the field help-text positioned bellow the field.
    /// </summary>
    [Parameter]
    public string Help
    {
        get => help;
        set
        {
            help = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Defines the padding between your columns, used to responsively space and align content in the Blazorise grid system.
    /// </summary>
    [Parameter]
    public IFluentGutter Gutter
    {
        get => gutter;
        set
        {
            if ( gutter == value )
                return;

            gutter = value;

            DirtyClasses();
        }
    }

    #endregion
}