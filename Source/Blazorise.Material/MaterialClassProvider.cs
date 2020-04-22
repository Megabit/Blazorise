#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Material
{
    public class MaterialClassProvider : Bootstrap.BootstrapClassProvider
    {
        public override string TabPanel() => "tab-pane fade";

        public override string Bar() => "navbar navbar-full";

        public override string Provider => "Material";
    }
}
