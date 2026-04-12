#region Using directives
using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Benchmark.Blazorise;

[MemoryDiagnoser]
public class ComponentActivatorBenchmark
{
    private static readonly Type MappedValueTypeGenericComponentType = typeof( RequestedComponent<int> );
    private static readonly Type MappedReferenceTypeGenericComponentType = typeof( RequestedComponent<string> );

    private global::Blazorise.ComponentActivator componentActivator;
    private LegacyComponentActivator legacyComponentActivator;
    private GeneratedMappedComponentActivator generatedMappedComponentActivator;

    [GlobalSetup]
    public void Setup()
    {
        IServiceProvider serviceProvider = new ComponentMappingServiceProvider();

        componentActivator = new( serviceProvider );
        legacyComponentActivator = new( serviceProvider );
        generatedMappedComponentActivator = new( serviceProvider );

        global::Blazorise.GeneratedComponentMappingRegistry.Register( MappedValueTypeGenericComponentType, typeof( MappedComponent<int> ) );
        global::Blazorise.GeneratedComponentMappingRegistry.Register( MappedReferenceTypeGenericComponentType, typeof( MappedComponent<string> ) );
    }

    [Benchmark( Baseline = true )]
    public IComponent LegacyActivator_MappedValueTypeGeneric()
        => legacyComponentActivator.CreateInstance( MappedValueTypeGenericComponentType );

    [Benchmark]
    public IComponent ComponentActivator_WithGeneratedChecks_MappedValueTypeGeneric()
        => componentActivator.CreateInstance( MappedValueTypeGenericComponentType );

    [Benchmark]
    public IComponent GeneratedMappingPath_MappedValueTypeGeneric()
        => generatedMappedComponentActivator.CreateInstance( MappedValueTypeGenericComponentType );

    [Benchmark]
    public IComponent LegacyActivator_MappedReferenceTypeGeneric()
        => legacyComponentActivator.CreateInstance( MappedReferenceTypeGenericComponentType );

    [Benchmark]
    public IComponent ComponentActivator_WithGeneratedChecks_MappedReferenceTypeGeneric()
        => componentActivator.CreateInstance( MappedReferenceTypeGenericComponentType );

    private sealed class ComponentMappingServiceProvider : IServiceProvider
    {
        public object GetService( Type serviceType )
        {
            if ( serviceType.IsConstructedGenericType
                 && serviceType.GetGenericTypeDefinition() == typeof( RequestedComponent<> ) )
            {
                Type implementationType = typeof( MappedComponent<> ).MakeGenericType( serviceType.GetGenericArguments() );

                return Activator.CreateInstance( implementationType );
            }

            return null;
        }
    }

    private sealed class LegacyComponentActivator
    {
        private readonly IServiceProvider serviceProvider;

        public LegacyComponentActivator( IServiceProvider serviceProvider )
        {
            this.serviceProvider = serviceProvider;
        }

        public IComponent CreateInstance( Type componentType )
        {
            object instance = serviceProvider.GetService( componentType );

            if ( instance is null )
            {
                instance = ActivatorUtilities.CreateInstance( serviceProvider, componentType );
            }

            if ( instance is not IComponent component )
            {
                throw new ArgumentException( $"The type {componentType.FullName} does not implement {nameof( IComponent )}.", nameof( componentType ) );
            }

            return component;
        }
    }

    private sealed class GeneratedMappedComponentActivator
    {
        private readonly IServiceProvider serviceProvider;

        public GeneratedMappedComponentActivator( IServiceProvider serviceProvider )
        {
            this.serviceProvider = serviceProvider;
        }

        public IComponent CreateInstance( Type componentType )
        {
            if ( !global::Blazorise.GeneratedComponentMappingRegistry.TryResolve( componentType, out Type mappedType ) )
            {
                throw new InvalidOperationException( $"No generated mapping was registered for {componentType.FullName}." );
            }

            object instance = ActivatorUtilities.CreateInstance( serviceProvider, mappedType );

            if ( instance is not IComponent component )
            {
                throw new ArgumentException( $"The type {mappedType.FullName} does not implement {nameof( IComponent )}.", nameof( componentType ) );
            }

            return component;
        }
    }

    public class RequestedComponent<TValue> : IComponent
    {
        public void Attach( RenderHandle renderHandle )
        {
        }

        public Task SetParametersAsync( ParameterView parameters )
            => Task.CompletedTask;
    }

    public sealed class MappedComponent<TValue> : RequestedComponent<TValue>
    {
    }
}
