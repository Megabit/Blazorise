#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.Docs.Models.ApiDocsDtos;

public interface IComponentsApiDocsSource
{
    Dictionary<Type, ApiDocsForComponent> Components { get; }
}

public class ComponentsApiDocsSource
{
    #region Members

    private static readonly Lazy<ComponentsApiDocsSource> instance = new( () => new ComponentsApiDocsSource() );

    #endregion

    #region Constructors

    private ComponentsApiDocsSource()
    {
        Components = new Dictionary<Type, ApiDocsForComponent>();

        // Find all types implementing IComponentsApiDocsSource
        var sources = typeof( ComponentsApiDocsSource ).Assembly
            .GetTypes().Where( type => typeof( IComponentsApiDocsSource ).IsAssignableFrom( type ) && type is { IsInterface: false, IsAbstract: false } );

        foreach ( var sourceType in sources )
        {
            // Create an instance of the source type
            if ( Activator.CreateInstance( sourceType ) is not IComponentsApiDocsSource sourceInstance )
                continue;

            // Merge Components dictionary
            foreach ( var component in sourceInstance.Components )
            {
                if ( !Components.ContainsKey( component.Key ) )
                {
                    Components[component.Key] = component.Value;
                }
                else
                {
                    // Handle duplicates if needed, e.g., log or overwrite
                }
            }
        }
    }

    #endregion

    #region Properties

    public Dictionary<Type, ApiDocsForComponent> Components { get; }


    public static ComponentsApiDocsSource Instance => instance.Value;

    #endregion
}