#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Providers
{
    internal class VersionProvider : IVersionProvider
    {
        Lazy<Version> version;

        public VersionProvider()
        {
            version = new Lazy<Version>( () => Assembly.GetExecutingAssembly().GetName().Version );
        }

        public string Version => version.Value.ToString();

        public string MilestoneVersion => version.Value.ToString( 3 );
    }
}
