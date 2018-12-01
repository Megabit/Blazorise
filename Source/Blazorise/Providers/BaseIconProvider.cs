#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Providers
{
    public abstract class BaseIconProvider : IIconProvider
    {
        #region Members

        private Dictionary<string, string> customIcons = new Dictionary<string, string>();

        #endregion

        #region Constructors

        #endregion

        #region Methods

        public abstract string Icon();

        public string Get( string customName )
        {
            customIcons.TryGetValue( customName, out var name );

            return name;
        }

        public abstract string Get( IconName name );

        public abstract void Set( IconName name, string newName );

        #endregion

        #region Properties

        public abstract bool IconNameAsContent { get; }

        #endregion
    }
}
