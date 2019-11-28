#region Using directives
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Providers
{
    public abstract class BaseIconProvider : IIconProvider
    {
        #region Members

        private readonly ConcurrentDictionary<string, string> customIcons = new ConcurrentDictionary<string, string>();

        #endregion

        #region Constructors

        #endregion

        #region Methods

        public abstract string Icon( object name );

        public string Get( string customName )
        {
            return customIcons.GetOrAdd( customName, customName );
        }

        public abstract string Get( IconName name );

        public abstract void Set( IconName name, string newName );

        #endregion

        #region Properties

        public abstract bool IconNameAsContent { get; }

        #endregion
    }
}
