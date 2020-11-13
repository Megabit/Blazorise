#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Blazorise.Utils
{
    public static class LinkHelpers
    {
        /// <summary>
        /// Gets the link target name.
        /// </summary>
        public static string TargetName( Target target ) => target switch
        {
            Target.Blank => "_blank",
            Target.Parent => "_parent",
            Target.Top => "_top",
            Target.Self => "_self",
            _ => null,
        };
    }
}
