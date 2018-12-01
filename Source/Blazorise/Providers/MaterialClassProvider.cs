#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Providers
{
    /// <summary>
    /// Material design based on the bootstrap.
    /// </summary>
    class MaterialClassProvider : BootstrapClassProvider
    {
        public override string Nav()
        {
            var baseClass = base.Nav();
            var material = "nav-tabs-material";

            return $"{baseClass} {material}";
        }

        public override string TabPanel() => $"{base.TabPanel()} fade";

        public override string Bar() => $"{base.Bar()} navbar-full";

        public override FrameworkProvider Provider { get { return FrameworkProvider.Material; } }
    }
}
