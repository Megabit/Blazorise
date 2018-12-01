#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Blazor.Components;
#endregion

namespace Blazorise
{
    public class BaseContainerComponent : BaseComponent
    {
        protected override string Build( params string[] classnames )
        {
            var xs = XS != ColumnSize.None ? ClassProvider.Col( Size.ExtraSmall, XS ) : null;
            var sm = SM != ColumnSize.None ? ClassProvider.Col( Size.Small, SM ) : null;
            var md = MD != ColumnSize.None ? ClassProvider.Col( Size.Medium, MD ) : null;
            var lg = LG != ColumnSize.None ? ClassProvider.Col( Size.Large, LG ) : null;
            var xl = XL != ColumnSize.None ? ClassProvider.Col( Size.ExtraLarge, XL ) : null;

            Append( sb, xs );
            Append( sb, sm );
            Append( sb, md );
            Append( sb, lg );
            Append( sb, xl );

            //DeviceSize?.Build( ClassProvider, Append );

            return base.Build( classnames );
        }

        [Parameter] private ColumnSize XS { get; set; } = ColumnSize.None;

        [Parameter] private ColumnSize SM { get; set; } = ColumnSize.None;

        [Parameter] private ColumnSize MD { get; set; } = ColumnSize.None;

        [Parameter] private ColumnSize LG { get; set; } = ColumnSize.None;

        [Parameter] private ColumnSize XL { get; set; } = ColumnSize.None;

        //[Parameter] private FluentColumnSize DeviceSize { get; set; }
    }

    //public sealed class FluentColumnSize
    //{
    //    #region Members

    //    protected Dictionary<Size, ColumnSize> sizes = new Dictionary<Size, ColumnSize>();

    //    #endregion

    //    #region Constructors

    //    #endregion

    //    #region Methods

    //    public void Build( IClassProvider classProvider, AppendClassname append )
    //    {
    //        foreach ( var kv in sizes )
    //        {
    //            var classname = kv.Value != ColumnSize.None ? classProvider.Col( kv.Key, kv.Value ) : null;

    //            append?.Invoke( classname );
    //        }
    //    }

    //    private FluentColumnSize Add( Size size, ColumnSize columnSize )
    //    {
    //        if ( columnSize != ColumnSize.None )
    //            sizes[size] = columnSize;

    //        return this;
    //    }

    //    public FluentColumnSize XS( ColumnSize columnSize ) => Add( Size.ExtraSmall, columnSize );

    //    public FluentColumnSize SM( ColumnSize columnSize ) => Add( Size.Small, columnSize );

    //    public FluentColumnSize MD( ColumnSize columnSize ) => Add( Size.Medium, columnSize );

    //    public FluentColumnSize LG( ColumnSize columnSize ) => Add( Size.Large, columnSize );

    //    public FluentColumnSize XL( ColumnSize columnSize ) => Add( Size.ExtraLarge, columnSize );

    //    #endregion

    //    #region Properties

    //    #endregion
    //}

    //public static class DeviceSize
    //{
    //    public static FluentColumnSize XS( ColumnSize columnSize ) => new FluentColumnSize().XS( columnSize );

    //    public static FluentColumnSize SM( ColumnSize columnSize ) => new FluentColumnSize().SM( columnSize );

    //    public static FluentColumnSize MD( ColumnSize columnSize ) => new FluentColumnSize().MD( columnSize );

    //    public static FluentColumnSize LG( ColumnSize columnSize ) => new FluentColumnSize().LG( columnSize );

    //    public static FluentColumnSize XL( ColumnSize columnSize ) => new FluentColumnSize().XL( columnSize );
    //}
}
