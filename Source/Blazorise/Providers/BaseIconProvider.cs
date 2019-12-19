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

        public virtual string Icon( object name, IconStyle style )
        {
            var iconStyle = GetStyleName( style );

            // Some icons must be placed inside of an icon tag element so just return 
            // the style name. The actual icon name will be defined in the Icon.razor file.
            if ( IconNameAsContent || name == null )
                return iconStyle;

            // Icon name can be either IconName(enum) or string type.
            var iconName = name is IconName ? GetIconName( (IconName)name ) : (string)name;

            // Sometimes icon style can be defined with the icon name. In those cases we need to remove it.
            if ( iconName.StartsWith( iconStyle ) )
            {
                iconName = iconName.Remove( 0, iconStyle.Length + 1 );
            }

            return $"{iconStyle} {iconName}";
        }

        public abstract string GetIconName( IconName name );

        public abstract void SetIconName( IconName name, string newName );

        public string GetIconName( string customName )
        {
            return customIcons.GetOrAdd( customName, customName );
        }

        public abstract string GetStyleName( IconStyle iconStyle );

        #endregion

        #region Properties

        public abstract bool IconNameAsContent { get; }

        #endregion
    }
}
