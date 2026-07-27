#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Contains the state and update callback available to a custom element properties editor.
/// </summary>
public sealed class ReportElementPropertiesContext
{
    #region Members

    private readonly Func<Action<ReportElementDefinition>, Task> update;

    #endregion

    #region Constructors

    internal ReportElementPropertiesContext( ReportDefinition definition, IReadOnlyList<ReportCustomElementDefinition> elements, Func<Action<ReportElementDefinition>, Task> update )
    {
        Definition = definition;
        Elements = elements ?? [];
        this.update = update;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Applies an undo-aware update to all selected elements of the same custom type.
    /// </summary>
    public Task Update( Action<ReportCustomElementDefinition> action )
    {
        if ( action is null || update is null )
            return Task.CompletedTask;

        return update( element =>
        {
            if ( element is ReportCustomElementDefinition customElement
                 && string.Equals( customElement.TypeName, Element?.TypeName, StringComparison.OrdinalIgnoreCase ) )
            {
                customElement.Properties ??= new();
                action( customElement );
            }
        } );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the report definition being edited.
    /// </summary>
    public ReportDefinition Definition { get; }

    /// <summary>
    /// Gets the selected custom element.
    /// </summary>
    public ReportCustomElementDefinition Element => Elements.Count > 0 ? Elements[0] : null;

    /// <summary>
    /// Gets all selected custom elements of the same plugin type.
    /// </summary>
    public IReadOnlyList<ReportCustomElementDefinition> Elements { get; }

    #endregion
}