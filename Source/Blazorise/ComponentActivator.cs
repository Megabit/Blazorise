#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public class ComponentActivator : IComponentActivator
    {
        #region Constructors

        public ComponentActivator( IServiceProvider serviceProvider )
        {
            ServiceProvider = serviceProvider;
        }

        #endregion

        #region Methods

        public IComponent CreateInstance( Type componentType )
        {
            var instance = ServiceProvider.GetService( componentType );

            if ( instance == null )
            {
                instance = Activator.CreateInstance( componentType );
            }

            if ( instance is not IComponent component )
            {
                throw new ArgumentException( $"The type {componentType.FullName} does not implement {nameof( IComponent )}.", nameof( componentType ) );
            }

            return component;
        }

        #endregion

        #region Properties

        public IServiceProvider ServiceProvider { get; }

        #endregion
    }
}
