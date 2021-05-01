using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazorise.Utilities;

namespace Blazorise.AntDesign
{
    public partial class Table : Blazorise.Table
    {
        /// <summary>
        /// Builds a list of classnames for the responsive or the fixed table container element if applicable.
        /// </summary>
        /// <param name="builder">Class builder used to append the classnames.</param>
        protected override void BuildTableDivClasses( ClassBuilder builder )
        {
            base.BuildTableDivClasses( builder );
            builder.Append( " ant-table-container" );
        }
    }
}
