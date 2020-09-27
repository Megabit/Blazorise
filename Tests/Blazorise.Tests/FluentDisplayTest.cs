#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
#endregion

namespace Blazorise.Tests
{
    public class FluentDisplayTest
    {
        IClassProvider classProvider;

        public FluentDisplayTest()
        {
            classProvider = new Bootstrap.BootstrapClassProvider();
        }

        [Theory]
        [InlineData( "d-none", DisplayType.None )]
        [InlineData( "d-inline", DisplayType.Inline )]
        [InlineData( "d-inline-block", DisplayType.InlineBlock )]
        [InlineData( "d-block", DisplayType.Block )]
        [InlineData( "d-table", DisplayType.Table )]
        [InlineData( "d-table-cell", DisplayType.TableCell )]
        [InlineData( "d-table-row", DisplayType.TableRow )]
        public void AreDisplay( string expected, DisplayType displayType )
        {
            var display = new FluentDisplay();

            display.WithDisplay( displayType );

            var classname = display.Class( classProvider );

            Assert.Equal( expected, classname );
        }

        [Theory]
        [InlineData( "d-flex", DisplayType.Flex )]
        [InlineData( "d-inline-flex", DisplayType.InlineFlex )]
        public void AreFlex( string expected, DisplayType displayType )
        {
            var display = new FluentDisplay();

            display.WithFlex( displayType );

            var classname = display.Class( classProvider );

            Assert.Equal( expected, classname );
        }

        [Theory]
        [InlineData( "d-none", DisplayType.None, DisplayDirection.None, Breakpoint.Mobile )]
        [InlineData( "d-sm-none", DisplayType.None, DisplayDirection.None, Breakpoint.Tablet )]
        [InlineData( "d-md-none", DisplayType.None, DisplayDirection.None, Breakpoint.Desktop )]
        [InlineData( "d-lg-none", DisplayType.None, DisplayDirection.None, Breakpoint.Widescreen )]
        [InlineData( "d-xl-none", DisplayType.None, DisplayDirection.None, Breakpoint.FullHD )]
        [InlineData( "d-inline", DisplayType.Inline, DisplayDirection.None, Breakpoint.Mobile )]
        [InlineData( "d-sm-inline", DisplayType.Inline, DisplayDirection.None, Breakpoint.Tablet )]
        [InlineData( "d-md-inline", DisplayType.Inline, DisplayDirection.None, Breakpoint.Desktop )]
        [InlineData( "d-lg-inline", DisplayType.Inline, DisplayDirection.None, Breakpoint.Widescreen )]
        [InlineData( "d-xl-inline", DisplayType.Inline, DisplayDirection.None, Breakpoint.FullHD )]
        [InlineData( "d-inline-block", DisplayType.InlineBlock, DisplayDirection.None, Breakpoint.Mobile )]
        [InlineData( "d-sm-inline-block", DisplayType.InlineBlock, DisplayDirection.None, Breakpoint.Tablet )]
        [InlineData( "d-md-inline-block", DisplayType.InlineBlock, DisplayDirection.None, Breakpoint.Desktop )]
        [InlineData( "d-lg-inline-block", DisplayType.InlineBlock, DisplayDirection.None, Breakpoint.Widescreen )]
        [InlineData( "d-xl-inline-block", DisplayType.InlineBlock, DisplayDirection.None, Breakpoint.FullHD )]
        [InlineData( "d-block", DisplayType.Block, DisplayDirection.None, Breakpoint.Mobile )]
        [InlineData( "d-sm-block", DisplayType.Block, DisplayDirection.None, Breakpoint.Tablet )]
        [InlineData( "d-md-block", DisplayType.Block, DisplayDirection.None, Breakpoint.Desktop )]
        [InlineData( "d-lg-block", DisplayType.Block, DisplayDirection.None, Breakpoint.Widescreen )]
        [InlineData( "d-xl-block", DisplayType.Block, DisplayDirection.None, Breakpoint.FullHD )]
        [InlineData( "d-table", DisplayType.Table, DisplayDirection.None, Breakpoint.Mobile )]
        [InlineData( "d-sm-table", DisplayType.Table, DisplayDirection.None, Breakpoint.Tablet )]
        [InlineData( "d-md-table", DisplayType.Table, DisplayDirection.None, Breakpoint.Desktop )]
        [InlineData( "d-lg-table", DisplayType.Table, DisplayDirection.None, Breakpoint.Widescreen )]
        [InlineData( "d-xl-table", DisplayType.Table, DisplayDirection.None, Breakpoint.FullHD )]
        [InlineData( "d-table-row", DisplayType.TableRow, DisplayDirection.None, Breakpoint.Mobile )]
        [InlineData( "d-sm-table-row", DisplayType.TableRow, DisplayDirection.None, Breakpoint.Tablet )]
        [InlineData( "d-md-table-row", DisplayType.TableRow, DisplayDirection.None, Breakpoint.Desktop )]
        [InlineData( "d-lg-table-row", DisplayType.TableRow, DisplayDirection.None, Breakpoint.Widescreen )]
        [InlineData( "d-xl-table-row", DisplayType.TableRow, DisplayDirection.None, Breakpoint.FullHD )]
        [InlineData( "d-table-cell", DisplayType.TableCell, DisplayDirection.None, Breakpoint.Mobile )]
        [InlineData( "d-sm-table-cell", DisplayType.TableCell, DisplayDirection.None, Breakpoint.Tablet )]
        [InlineData( "d-md-table-cell", DisplayType.TableCell, DisplayDirection.None, Breakpoint.Desktop )]
        [InlineData( "d-lg-table-cell", DisplayType.TableCell, DisplayDirection.None, Breakpoint.Widescreen )]
        [InlineData( "d-xl-table-cell", DisplayType.TableCell, DisplayDirection.None, Breakpoint.FullHD )]
        [InlineData( "d-flex flex-row", DisplayType.Flex, DisplayDirection.Row, Breakpoint.Mobile )]
        [InlineData( "d-sm-flex flex-row", DisplayType.Flex, DisplayDirection.Row, Breakpoint.Tablet )]
        [InlineData( "d-md-flex flex-row", DisplayType.Flex, DisplayDirection.Row, Breakpoint.Desktop )]
        [InlineData( "d-lg-flex flex-row", DisplayType.Flex, DisplayDirection.Row, Breakpoint.Widescreen )]
        [InlineData( "d-xl-flex flex-row", DisplayType.Flex, DisplayDirection.Row, Breakpoint.FullHD )]
        [InlineData( "d-flex flex-column", DisplayType.Flex, DisplayDirection.Column, Breakpoint.Mobile )]
        [InlineData( "d-sm-flex flex-column", DisplayType.Flex, DisplayDirection.Column, Breakpoint.Tablet )]
        [InlineData( "d-md-flex flex-column", DisplayType.Flex, DisplayDirection.Column, Breakpoint.Desktop )]
        [InlineData( "d-lg-flex flex-column", DisplayType.Flex, DisplayDirection.Column, Breakpoint.Widescreen )]
        [InlineData( "d-xl-flex flex-column", DisplayType.Flex, DisplayDirection.Column, Breakpoint.FullHD )]
        [InlineData( "d-inline-flex flex-row", DisplayType.InlineFlex, DisplayDirection.Row, Breakpoint.Mobile )]
        [InlineData( "d-sm-inline-flex flex-row", DisplayType.InlineFlex, DisplayDirection.Row, Breakpoint.Tablet )]
        [InlineData( "d-md-inline-flex flex-row", DisplayType.InlineFlex, DisplayDirection.Row, Breakpoint.Desktop )]
        [InlineData( "d-lg-inline-flex flex-row", DisplayType.InlineFlex, DisplayDirection.Row, Breakpoint.Widescreen )]
        [InlineData( "d-xl-inline-flex flex-row", DisplayType.InlineFlex, DisplayDirection.Row, Breakpoint.FullHD )]
        [InlineData( "d-inline-flex flex-column", DisplayType.InlineFlex, DisplayDirection.Column, Breakpoint.Mobile )]
        [InlineData( "d-sm-inline-flex flex-column", DisplayType.InlineFlex, DisplayDirection.Column, Breakpoint.Tablet )]
        [InlineData( "d-md-inline-flex flex-column", DisplayType.InlineFlex, DisplayDirection.Column, Breakpoint.Desktop )]
        [InlineData( "d-lg-inline-flex flex-column", DisplayType.InlineFlex, DisplayDirection.Column, Breakpoint.Widescreen )]
        [InlineData( "d-xl-inline-flex flex-column", DisplayType.InlineFlex, DisplayDirection.Column, Breakpoint.FullHD )]
        public void AreBreakpoints_OnAll( string expected, DisplayType displayType, DisplayDirection direction, Breakpoint breakpoint )
        {
            var display = new FluentDisplay();

            display.WithDisplay( displayType );

            if ( direction != DisplayDirection.None )
                display.WithDirection( direction );

            if ( breakpoint != Breakpoint.None )
                display.WithBreakpoint( breakpoint );

            var classname = display.Class( classProvider );

            Assert.Equal( expected, classname );
        }
    }
}
