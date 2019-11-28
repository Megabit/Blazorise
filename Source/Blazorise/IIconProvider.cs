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
        /// Gets the predefined icon name.
        /// </summary>
        /// <param name="name">Icon name.</param>
        /// <returns></returns>
        string Icon( object name );

        /// <summary>
        /// Gets the icon name by predefined icon type.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string Get( IconName name );

        /// <summary>
        /// Overrides the predefined icon name.
        /// </summary>
        /// <param name="name">Icon to override.</param>
        /// <param name="newName">New icon name.</param>
        void Set( IconName name, string newName );

        /// <summary>
        /// Gets the icon name by the custom icon name.
        /// </summary>
        /// <param name="customName"></param>
        /// <returns></returns>
        string Get( string customName );

        /// <summary>
        /// Indicates if the classname will be defined as tag content. 
        /// </summary>
        bool IconNameAsContent { get; }
    }
}
