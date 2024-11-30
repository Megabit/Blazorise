using System;
using System.Collections.Generic;
using System.Linq;
using Blazorise.Generator.Features.ApiDocsDtos;

namespace Blazorise;


public interface IComponentsApiDocsSource
{
     Dictionary<Type, ApiDocsForComponent> Components { get;  }
}

public class ComponentsApiDocsSource
{
    public Dictionary<Type, ApiDocsForComponent> Components { get; }

    private static readonly Lazy<ComponentsApiDocsSource> instance = new(() => new ComponentsApiDocsSource());
    public static ComponentsApiDocsSource Instance => instance.Value;

    private ComponentsApiDocsSource()
    {
        Components = new Dictionary<Type, ApiDocsForComponent>();

        // Find all types implementing IComponentsApiDocsSource
        var sources = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IComponentsApiDocsSource).IsAssignableFrom(type) && type is { IsInterface: false, IsAbstract: false });

        foreach (var sourceType in sources)
        {
            // Create an instance of the source type
            if ( Activator.CreateInstance( sourceType ) is not IComponentsApiDocsSource sourceInstance )
                continue;
            // Merge Components dictionary
            foreach (var component in sourceInstance.Components)
            {
                if (!Components.ContainsKey(component.Key))
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
}

