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

        public Type Get<TComponent>()
            where TComponent : IComponent
        {
            throw new NotImplementedException();
        }

        public Type Get( IComponent baseComponent )
        {
            throw new NotImplementedException();
        }

        public void Register<TComponent, TImplementation>()
            where TComponent : IComponent
            where TImplementation : IComponent
        {
            throw new NotImplementedException();
        }

        public bool IsRegistered<TComponent>()
            where TComponent : IComponent
        {
            throw new NotImplementedException();
        }

        public bool IsRegistered( IComponent component )
        {
            throw new NotImplementedException();
        }

        //public Type Get( IComponent baseComponent )
        //{
        //    var baseType = baseComponent.GetType();

        //    components.TryGetValue( baseType, out var implementationType );

        //    return implementationType;
        //}

        //public void Register( IComponent baseComponent, IComponent implementationComponent )
        //{
        //    var baseType = baseComponent.GetType();
        //    var implementationType = implementationComponent.GetType();

        //    components.Add( baseType, implementationType );
        //}

        //public bool IsRegistered( IComponent component )
        //{
        //    return components.ContainsKey( component.GetType() );
        //}

        #endregion

        #region Properties

        #endregion
    }
}
