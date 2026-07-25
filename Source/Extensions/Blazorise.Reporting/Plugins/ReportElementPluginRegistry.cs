#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Default immutable report element plugin registry.
/// </summary>
public sealed class ReportElementPluginRegistry : IReportElementPluginRegistry
{
    #region Members

    private readonly Dictionary<string, IReportElementPlugin> pluginsByTypeName;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a registry from the supplied plugins.
    /// </summary>
    public ReportElementPluginRegistry( IEnumerable<IReportElementPlugin> plugins )
    {
        Plugins = plugins?.Where( plugin => plugin is not null ).ToArray() ?? [];
        pluginsByTypeName = new( StringComparer.OrdinalIgnoreCase );

        foreach ( IReportElementPlugin plugin in Plugins )
        {
            ReportElementDescriptor descriptor = plugin.Descriptor
                ?? throw new InvalidOperationException( $"Report element plugin '{plugin.GetType().FullName}' must define a descriptor." );
            string typeName = descriptor.TypeName?.Trim();

            if ( string.IsNullOrWhiteSpace( typeName ) )
                throw new InvalidOperationException( $"Report element plugin '{plugin.GetType().FullName}' must define a type name." );

            if ( string.IsNullOrWhiteSpace( descriptor.DisplayName ) )
                throw new InvalidOperationException( $"Report element plugin '{typeName}' must define a display name." );

            if ( descriptor.Width <= 0 || descriptor.Height <= 0 )
                throw new InvalidOperationException( $"Report element plugin '{typeName}' must define positive default dimensions." );

            if ( descriptor.SchemaVersion < 1 )
                throw new InvalidOperationException( $"Report element plugin '{typeName}' must define a positive schema version." );

            if ( plugin.RendererComponentType is null || !typeof( BaseReportElementRenderer ).IsAssignableFrom( plugin.RendererComponentType ) )
                throw new InvalidOperationException( $"Report element plugin '{typeName}' must use a component derived from {nameof( BaseReportElementRenderer )}." );

            if ( plugin.PropertiesComponentType is not null && !typeof( BaseReportElementPropertiesEditor ).IsAssignableFrom( plugin.PropertiesComponentType ) )
                throw new InvalidOperationException( $"The properties component for report element plugin '{typeName}' must derive from {nameof( BaseReportElementPropertiesEditor )}." );

            if ( !pluginsByTypeName.TryAdd( typeName, plugin ) )
                throw new InvalidOperationException( $"A report element plugin with type name '{typeName}' is already registered." );
        }
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    public IReportElementPlugin Find( string typeName )
    {
        string normalizedTypeName = typeName?.Trim();

        return !string.IsNullOrWhiteSpace( normalizedTypeName )
            && pluginsByTypeName.TryGetValue( normalizedTypeName, out IReportElementPlugin plugin )
                ? plugin
                : null;
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    public IReadOnlyList<IReportElementPlugin> Plugins { get; }

    #endregion
}
