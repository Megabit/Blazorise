#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor.Components;
#endregion

namespace Blazorise
{
    public interface IComponentMapper
    {
        Type Get<TComponent>()
            where TComponent : IComponent;

        Type Get( IComponent baseComponent );

        void Register<TComponent, TImplementation>()
            where TComponent : IComponent
            where TImplementation : IComponent;

        bool IsRegistered<TComponent>()
            where TComponent : IComponent;

        bool IsRegistered( IComponent component );
    }
}
