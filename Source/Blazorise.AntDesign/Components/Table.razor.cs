﻿using Blazorise.Utilities;

namespace Blazorise.AntDesign.Components;

public partial class Table
{
    protected override void BuildClasses( ClassBuilder builder )
    {
        base.BuildClasses( builder );

        builder.Append( ClassProvider.TableFixedHeader( FixedHeader ) );
        builder.Append( ClassProvider.TableFixedColumns( FixedColumns ) );
    }
}