#region Using directives
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Renders a text property editor.
/// </summary>
public partial class PropertyGridTextItem : BasePropertyGridEditorItem
{
    #region Methods

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.IsParameterChanged( Multiline ) )
        {
            DirtyClasses();
        }

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.PropertyGridItemMultiline( Multiline ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    private string DisplayValue => Mixed ? null : Value;

    private bool EffectiveImmediate => Immediate ?? ValueChanged.HasDelegate;

    /// <summary>
    /// Gets or sets the property value.
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <summary>
    /// Occurs when the property value changes.
    /// </summary>
    [Parameter] public EventCallback<string> ValueChanged { get; set; }

    /// <summary>
    /// Defines whether the property is read-only.
    /// </summary>
    [Parameter] public bool ReadOnly { get; set; }

    /// <summary>
    /// Defines whether the text editor supports multiple lines.
    /// </summary>
    [Parameter] public bool Multiline { get; set; }

    /// <summary>
    /// Defines whether value changes are reported immediately. Defaults to <c>ValueChanged.HasDelegate</c>.
    /// </summary>
    /// <remarks>When omitted, enables immediate updates if ValueChanged has a delegate.</remarks>
    [Parameter] public bool? Immediate { get; set; }

    #endregion
}