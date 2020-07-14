﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
#endregion

namespace Blazorise.Tests
{
    public class FluentPaddingTest
    {
        IClassProvider classProvider;

        public FluentPaddingTest()
        {
            classProvider = new Bootstrap.BootstrapClassProvider();
        }

        [Theory]
        [InlineData( "p-0", SpacingSize.Is0 )]
        [InlineData( "p-1", SpacingSize.Is1 )]
        [InlineData( "p-2", SpacingSize.Is2 )]
        [InlineData( "p-3", SpacingSize.Is3 )]
        [InlineData( "p-4", SpacingSize.Is4 )]
        [InlineData( "p-5", SpacingSize.Is5 )]
        [InlineData( "p-auto", SpacingSize.IsAuto )]
        public void AreSizes( string expected, SpacingSize spacingSize )
        {
            var padding = new FluentPadding();

            padding.WithSize( spacingSize );

            var classname = padding.Class( classProvider );

            Assert.Equal( expected, classname );
        }

        [Theory]
        [InlineData( "pt-1", SpacingSize.Is1, Side.Top )]
        [InlineData( "pb-1", SpacingSize.Is1, Side.Bottom )]
        [InlineData( "pl-1", SpacingSize.Is1, Side.Left )]
        [InlineData( "pr-1", SpacingSize.Is1, Side.Right )]
        [InlineData( "px-1", SpacingSize.Is1, Side.X )]
        [InlineData( "py-1", SpacingSize.Is1, Side.Y )]
        public void AreSides( string expected, SpacingSize spacingSize, Side side )
        {
            var padding = new FluentPadding();

            padding.WithSize( spacingSize );

            if ( side != Side.None )
                padding.WithSide( side );

            var classname = padding.Class( classProvider );

            Assert.Equal( expected, classname );
        }

        [Theory]
        [InlineData( "p-1", SpacingSize.Is1, Side.All, Breakpoint.Mobile )]
        [InlineData( "p-sm-1", SpacingSize.Is1, Side.All, Breakpoint.Tablet )]
        [InlineData( "p-md-1", SpacingSize.Is1, Side.All, Breakpoint.Desktop )]
        [InlineData( "p-lg-1", SpacingSize.Is1, Side.All, Breakpoint.Widescreen )]
        [InlineData( "p-xl-1", SpacingSize.Is1, Side.All, Breakpoint.FullHD )]
        [InlineData( "pt-1", SpacingSize.Is1, Side.Top, Breakpoint.Mobile )]
        [InlineData( "pt-sm-1", SpacingSize.Is1, Side.Top, Breakpoint.Tablet )]
        [InlineData( "pt-md-1", SpacingSize.Is1, Side.Top, Breakpoint.Desktop )]
        [InlineData( "pt-lg-1", SpacingSize.Is1, Side.Top, Breakpoint.Widescreen )]
        [InlineData( "pt-xl-1", SpacingSize.Is1, Side.Top, Breakpoint.FullHD )]
        [InlineData( "pb-1", SpacingSize.Is1, Side.Bottom, Breakpoint.Mobile )]
        [InlineData( "pb-sm-1", SpacingSize.Is1, Side.Bottom, Breakpoint.Tablet )]
        [InlineData( "pb-md-1", SpacingSize.Is1, Side.Bottom, Breakpoint.Desktop )]
        [InlineData( "pb-lg-1", SpacingSize.Is1, Side.Bottom, Breakpoint.Widescreen )]
        [InlineData( "pb-xl-1", SpacingSize.Is1, Side.Bottom, Breakpoint.FullHD )]
        [InlineData( "pl-1", SpacingSize.Is1, Side.Left, Breakpoint.Mobile )]
        [InlineData( "pl-sm-1", SpacingSize.Is1, Side.Left, Breakpoint.Tablet )]
        [InlineData( "pl-md-1", SpacingSize.Is1, Side.Left, Breakpoint.Desktop )]
        [InlineData( "pl-lg-1", SpacingSize.Is1, Side.Left, Breakpoint.Widescreen )]
        [InlineData( "pl-xl-1", SpacingSize.Is1, Side.Left, Breakpoint.FullHD )]
        [InlineData( "pr-1", SpacingSize.Is1, Side.Right, Breakpoint.Mobile )]
        [InlineData( "pr-sm-1", SpacingSize.Is1, Side.Right, Breakpoint.Tablet )]
        [InlineData( "pr-md-1", SpacingSize.Is1, Side.Right, Breakpoint.Desktop )]
        [InlineData( "pr-lg-1", SpacingSize.Is1, Side.Right, Breakpoint.Widescreen )]
        [InlineData( "pr-xl-1", SpacingSize.Is1, Side.Right, Breakpoint.FullHD )]
        [InlineData( "px-1", SpacingSize.Is1, Side.X, Breakpoint.Mobile )]
        [InlineData( "px-sm-1", SpacingSize.Is1, Side.X, Breakpoint.Tablet )]
        [InlineData( "px-md-1", SpacingSize.Is1, Side.X, Breakpoint.Desktop )]
        [InlineData( "px-lg-1", SpacingSize.Is1, Side.X, Breakpoint.Widescreen )]
        [InlineData( "px-xl-1", SpacingSize.Is1, Side.X, Breakpoint.FullHD )]
        [InlineData( "py-1", SpacingSize.Is1, Side.Y, Breakpoint.Mobile )]
        [InlineData( "py-sm-1", SpacingSize.Is1, Side.Y, Breakpoint.Tablet )]
        [InlineData( "py-md-1", SpacingSize.Is1, Side.Y, Breakpoint.Desktop )]
        [InlineData( "py-lg-1", SpacingSize.Is1, Side.Y, Breakpoint.Widescreen )]
        [InlineData( "py-xl-1", SpacingSize.Is1, Side.Y, Breakpoint.FullHD )]
        public void AreBreakpoints_OnAll( string expected, SpacingSize spacingSize, Side side, Breakpoint breakpoint )
        {
            var padding = new FluentPadding();

            padding.WithSize( spacingSize );

            if ( side != Side.None )
                padding.WithSide( side );

            if ( breakpoint != Breakpoint.None )
                padding.WithBreakpoint( breakpoint );

            var classname = padding.Class( classProvider );

            Assert.Equal( expected, classname );
        }
    }
}
