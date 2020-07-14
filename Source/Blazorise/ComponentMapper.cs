#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public class ComponentMapper : IComponentMapper
    {
        #region Members

        private readonly Dictionary<Type, Type> components;

        #endregion

        #region Constructors

        public ComponentMapper()
        {
            components = new Dictionary<Type, Type>();
        }

        #endregion

        #region Methods

        public Type GetImplementation<TComponent>()
            where TComponent : IComponent
        {
            return GetImplementation( typeof( TComponent ) );
        }

        public Type GetImplementation( IComponent component )
        {
            return GetImplementation( component.GetType() );
        }

        public Type GetImplementation( Type componentType )
        {
            components.TryGetValue( componentType, out var implementationType );

            return implementationType;
        }

        public void Register<TComponent, TImplementation>()
            where TComponent : IComponent
            where TImplementation : TComponent
        {
            Register( typeof( TComponent ), typeof( TImplementation ) );
        }

        public void Register( Type component, Type implementation )
        {
            if ( !components.ContainsKey( component ) )
            {
                components.Add( component, implementation );
            }
        }

        public void Replace( Type component, Type implementation )
        {
            if ( components.ContainsKey( component ) )
            {
                components[component] = implementation;
            }
        }

        public bool HasRegistration<TComponent>()
            where TComponent : IComponent
        {
            return HasRegistration( typeof( TComponent ) );
        }

        public bool HasRegistration( IComponent component )
        {
            return HasRegistration( component.GetType() );
        }

        private bool HasRegistration( Type type )
        {
            if ( components.ContainsKey( type ) )
                return true;

            // since user can use any value data-type with generic components we must register those component implementations on the fly
            if ( type.IsGenericType )
            {
                var genericComponentType = type.GetGenericTypeDefinition();

                if ( genericComponentType == null )
                    return false;

                // get the generic types that are defined as generics without value type eg. Button<> to BulmaButton<>
                if ( components.TryGetValue( genericComponentType, out var genericImplementationType ) )
                {
                    // get the generic type arguments
                    var typeArguments = type.GenericTypeArguments;

                    // create real implementation types based on the generic type
                    var realComponentType = genericComponentType.MakeGenericType( typeArguments );
                    var realImplementationType = genericImplementationType.MakeGenericType( typeArguments );

                    // save new implementations
                    Register( realComponentType, realImplementationType );

                    return true;
                }
            }

            return false;
        }

        //private Type RegisterNewGenericType( Type type, Type genericComponentType )
        //{
        //    // get the generic types that are defined as generics without value type eg. Button<> to BulmaButton<>
        //    if ( components.TryGetValue( genericComponentType, out var genericImplementationType ) )
        //    {
        //        // get the generic type arguments
        //        var typeArguments = type.GenericTypeArguments;

        //        // create real implementation types based on the generic type
        //        var realComponentType = genericComponentType.MakeGenericType( typeArguments );
        //        var realImplementationType = genericImplementationType.MakeGenericType( typeArguments );

        //        // save new implementations
        //        Register( realComponentType, realImplementationType );

        //        return realImplementationType;
        //    }

        //    return null;
        //}

        #endregion
    }
}
