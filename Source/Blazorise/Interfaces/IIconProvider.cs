#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    public interface IIconProvider
    {
        /// <summary>
        /// Gets the predefined icon style.
        /// </summary>
        /// <param name="name">Icon name.</param>
        /// <param name="iconStyle">Icon style.</param>
        /// <returns></returns>
        string Icon( object name, IconStyle iconStyle );

        /// <summary>
        /// Gets the icon name by predefined icon type.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetIconName( IconName name );

        /// <summary>
        /// Overrides the predefined icon name.
        /// </summary>
        /// <param name="name">Icon to override.</param>
        /// <param name="newName">New icon name.</param>
        void SetIconName( IconName name, string newName );

        /// <summary>
        /// Gets the icon name by the custom icon name.
        /// </summary>
        /// <param name="customName"></param>
        /// <returns></returns>
        string GetIconName( string customName );

        /// <summary>
        /// Indicates if the classname will be defined as tag content. 
        /// </summary>
        bool IconNameAsContent { get; }
    }
}
