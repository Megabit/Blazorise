#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor.Components;
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
            components.TryGetValue( typeof( TComponent ), out var implementationType );

            return implementationType;
        }

        public Type GetImplementation( IComponent component )
        {
            components.TryGetValue( component.GetType(), out var implementationType );

            return implementationType;
        }

        public void Register<TComponent, TImplementation>()
            where TComponent : IComponent
            where TImplementation : IComponent
        {
            components.Add( typeof( TComponent ), typeof( TImplementation ) );
        }

        public bool HasRegistration<TComponent>()
            where TComponent : IComponent
        {
            return components.ContainsKey( typeof( TComponent ) );
        }

        public bool HasRegistration( IComponent component )
        {
            return components.ContainsKey( component.GetType() );
        }

        #endregion
    }
}
