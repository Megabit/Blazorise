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

        #region Methods

        public virtual string Icon( object name, IconStyle style )
        {
            var iconStyle = GetStyleName( style );

            // Some icons must be placed inside of an icon tag element so just return 
            // the style name. The actual icon name will be defined in the Icon.razor file.
            if ( IconNameAsContent || name == null )
                return iconStyle;

            if ( name is IconName iconEnum )
            {
                return $"{iconStyle} {GetIconName( iconEnum )}".Trim();
            }
            else if ( name is string iconName )
            {
                if ( ContainsStyleName( iconName ) )
                    return iconName;

                return $"{iconStyle} {iconName}".Trim();
            }

            return iconStyle;
        }

        public abstract string GetIconName( IconName name );

        public abstract void SetIconName( IconName name, string newName );

        public string GetIconName( string customName )
        {
            return customIcons.GetOrAdd( customName, customName );
        }

        public abstract string GetStyleName( IconStyle iconStyle );

        protected abstract bool ContainsStyleName( string iconName );

        #endregion

        #region Properties

        public abstract bool IconNameAsContent { get; }

        #endregion
    }
}
