﻿using System.Threading.Tasks;
using Blazorise.Utilities;

namespace Blazorise.AntDesign
{
    public partial class Table
    {
        protected override void BuildClasses( ClassBuilder builder )
        {
            base.BuildClasses( builder );

            builder.Append( ClassProvider.TableFixedHeader(), FixedHeader );
        }

    }
}
