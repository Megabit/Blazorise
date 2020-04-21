#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise
{
    public interface IFluentDisplay
    {
        string Class( IClassProvider classProvider );
    }

    public interface IFluentDisplayWithDisplayOnBreakpointWithDirection :
        IFluentDisplay,
        IFluentDisplayOnBreakpoint,
        IFluentDisplayWithDisplay
    {
    }

    public interface IFluentDisplayOnBreakpoint :
        IFluentDisplay
    {
        /// <summary>
        /// Valid on all devices. (extra small)
        /// </summary>
        IFluentDisplayWithDisplay OnMobile { get; }

        /// <summary>
        /// Breakpoint on tablets (small).
        /// </summary>
        IFluentDisplayWithDisplay OnTablet { get; }

        /// <summary>
        ///  Breakpoint on desktop (medium).
        /// </summary>
        IFluentDisplayWithDisplay OnDesktop { get; }

        /// <summary>
        /// Breakpoint on widescreen (large).
        /// </summary>
        IFluentDisplayWithDisplay OnWidescreen { get; }

        /// <summary>
        /// Breakpoint on large desktops (extra large).
        /// </summary>
        IFluentDisplayWithDisplay OnFullHD { get; }
    }

    public interface IFluentDisplayWithDisplayFlexWithDirection :
        IFluentDisplay
    {
        IFluentDisplayWithDisplayOnBreakpointWithDirection Row { get; }

        IFluentDisplayWithDisplayOnBreakpointWithDirection ReverseRow { get; }

        IFluentDisplayWithDisplayOnBreakpointWithDirection Column { get; }

        IFluentDisplayWithDisplayOnBreakpointWithDirection ReverseColumn { get; }
    }

    public interface IFluentDisplayWithDisplay :
        IFluentDisplay
    {
        IFluentDisplayWithDisplayOnBreakpointWithDirection Block { get; }

        IFluentDisplayWithDisplayOnBreakpointWithDirection Inline { get; }

        IFluentDisplayWithDisplayOnBreakpointWithDirection InlineBlock { get; }

        IFluentDisplayWithDisplayOnBreakpointWithDirection Table { get; }

        IFluentDisplayWithDisplayOnBreakpointWithDirection TableRow { get; }

        IFluentDisplayWithDisplayOnBreakpointWithDirection TableCell { get; }

        IFluentDisplayWithDisplayFlexWithDirection Flex { get; }

        IFluentDisplayWithDisplayFlexWithDirection InlineFlex { get; }
    }

    public class FluentDisplay :
        IFluentDisplay,
        IFluentDisplayWithDisplayOnBreakpointWithDirection,
        IFluentDisplayOnBreakpoint,
        IFluentDisplayWithDisplayFlexWithDirection,
        IFluentDisplayWithDisplay
    {
        #region Members

        private class DisplayDefinition
        {
            public Breakpoint Breakpoint { get; set; }

            public DisplayDirection Direction { get; set; }
        }

        private DisplayDefinition currentDisplay;

        private readonly Dictionary<DisplayType, List<DisplayDefinition>> rules = new Dictionary<DisplayType, List<DisplayDefinition>>();

        private bool dirty = true;

        private string classNames;

        #endregion

        #region Methods

        public string Class( IClassProvider classProvider )
        {
            if ( dirty )
            {
                void BuildClasses( ClassBuilder builder )
                {
                    if ( rules.Count( x => x.Key != DisplayType.None ) > 0 )
                        builder.Append( rules.Select( r => classProvider.Display( r.Key, r.Value.Select( v => (v.Breakpoint, v.Direction) ) ) ) );
                }

                var classBuilder = new ClassBuilder( BuildClasses );

                classNames = classBuilder.Class;

                dirty = false;
            }

            return classNames;
        }

        private void Dirty()
        {
            dirty = true;
        }

        private IFluentDisplayWithDisplayOnBreakpointWithDirection WithDisplay( DisplayType displayType )
        {
            var columnDefinition = new DisplayDefinition { Breakpoint = Breakpoint.None };

            if ( !rules.ContainsKey( displayType ) )
                rules.Add( displayType, new List<DisplayDefinition> { columnDefinition } );
            else
                rules[displayType].Add( columnDefinition );

            currentDisplay = columnDefinition;
            Dirty();

            return this;
        }

        private IFluentDisplayWithDisplayFlexWithDirection WithFlex( DisplayType displayType )
        {
            var columnDefinition = new DisplayDefinition { Breakpoint = Breakpoint.None };

            if ( !rules.ContainsKey( displayType ) )
                rules.Add( displayType, new List<DisplayDefinition> { columnDefinition } );
            else
                rules[displayType].Add( columnDefinition );

            currentDisplay = columnDefinition;
            Dirty();

            return this;
        }

        private IFluentDisplayWithDisplay WithBreakpoint( Breakpoint breakpoint )
        {
            currentDisplay.Breakpoint = breakpoint;
            Dirty();

            return this;
        }

        public IFluentDisplayWithDisplayOnBreakpointWithDirection WithDirection( DisplayDirection direction )
        {
            currentDisplay.Direction = direction;
            Dirty();

            return this;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Valid on all devices. (extra small)
        /// </summary>
        public IFluentDisplayWithDisplay OnMobile => WithBreakpoint( Breakpoint.Mobile );

        /// <summary>
        /// Breakpoint on tablets (small).
        /// </summary>
        public IFluentDisplayWithDisplay OnTablet => WithBreakpoint( Breakpoint.Tablet );

        /// <summary>
        ///  Breakpoint on desktop (medium).
        /// </summary>
        public IFluentDisplayWithDisplay OnDesktop => WithBreakpoint( Breakpoint.Desktop );

        /// <summary>
        /// Breakpoint on widescreen (large).
        /// </summary>
        public IFluentDisplayWithDisplay OnWidescreen => WithBreakpoint( Breakpoint.Widescreen );

        /// <summary>
        /// Breakpoint on large desktops (extra large).
        /// </summary>
        public IFluentDisplayWithDisplay OnFullHD => WithBreakpoint( Breakpoint.FullHD );

        /// <summary>
        /// Displays an element as a block element.
        /// </summary>
        /// <remarks>
        /// It starts on a new line, and takes up the whole width.
        /// </remarks>
        public IFluentDisplayWithDisplayOnBreakpointWithDirection Block { get { return WithDisplay( DisplayType.Block ); } }

        /// <summary>
        /// Displays an element as an inline element.
        /// </summary>
        /// <remarks>
        /// Any height and width properties will have no effect.
        /// </remarks>
        public IFluentDisplayWithDisplayOnBreakpointWithDirection Inline { get { return WithDisplay( DisplayType.Inline ); } }

        /// <summary>
        /// Displays an element as an inline-level block container.
        /// </summary>
        /// <remarks>
        /// The element itself is formatted as an inline element, but you can apply height and width values
        /// </remarks>
        public IFluentDisplayWithDisplayOnBreakpointWithDirection InlineBlock { get { return WithDisplay( DisplayType.InlineBlock ); } }

        /// <summary>
        /// Let the element behave like a <table> element.
        /// </summary>
        public IFluentDisplayWithDisplayOnBreakpointWithDirection Table { get { return WithDisplay( DisplayType.Table ); } }

        /// <summary>
        /// Let the element behave like a <tr> element.
        /// </summary>
        public IFluentDisplayWithDisplayOnBreakpointWithDirection TableRow { get { return WithDisplay( DisplayType.TableRow ); } }

        /// <summary>
        /// Let the element behave like a <td> element.
        /// </summary>
        public IFluentDisplayWithDisplayOnBreakpointWithDirection TableCell { get { return WithDisplay( DisplayType.TableCell ); } }

        /// <summary>
        /// Displays an element as a block-level flex container.
        /// </summary>
        public IFluentDisplayWithDisplayFlexWithDirection Flex { get { return WithFlex( DisplayType.Flex ); } }

        /// <summary>
        /// Displays an element as an inline-level flex container.
        /// </summary>
        public IFluentDisplayWithDisplayFlexWithDirection InlineFlex { get { return WithFlex( DisplayType.InlineFlex ); } }

        /// <summary>
        /// The flex container's main-axis is defined to be the same as the text direction. The main-start and main-end points are the same as the content direction.
        /// </summary>
        public IFluentDisplayWithDisplayOnBreakpointWithDirection Row { get { return WithDirection( DisplayDirection.Row ); } }

        /// <summary>
        /// The flex container's main-axis is the same as the block-axis. The main-start and main-end points are the same as the before and after points of the writing-mode.
        /// </summary>
        public IFluentDisplayWithDisplayOnBreakpointWithDirection Column { get { return WithDirection( DisplayDirection.Column ); } }

        /// <summary>
        /// Behaves the same as row but the main-start and main-end points are permuted.
        /// </summary>
        public IFluentDisplayWithDisplayOnBreakpointWithDirection ReverseRow { get { return WithDirection( DisplayDirection.ReverseRow ); } }

        /// <summary>
        /// Behaves the same as column but the main-start and main-end are permuted.
        /// </summary>
        public IFluentDisplayWithDisplayOnBreakpointWithDirection ReverseColumn { get { return WithDirection( DisplayDirection.ReverseColumn ); } }

        #endregion
    }

    /// <summary>
    /// Fluent builder for the display utilities.
    /// </summary>
    public static class Display
    {
        /// <summary>
        /// Displays an element as a block element.
        /// </summary>
        /// <remarks>
        /// It starts on a new line, and takes up the whole width.
        /// </remarks>
        public static IFluentDisplayWithDisplayOnBreakpointWithDirection Block { get { return new FluentDisplay().Block; } }

        /// <summary>
        /// Displays an element as an inline element.
        /// </summary>
        /// <remarks>
        /// Any height and width properties will have no effect.
        /// </remarks>
        public static IFluentDisplayWithDisplayOnBreakpointWithDirection Inline { get { return new FluentDisplay().Inline; } }

        /// <summary>
        /// Displays an element as an inline-level block container.
        /// </summary>
        /// <remarks>
        /// The element itself is formatted as an inline element, but you can apply height and width values
        /// </remarks>
        public static IFluentDisplayWithDisplayOnBreakpointWithDirection InlineBlock { get { return new FluentDisplay().InlineBlock; } }

        /// <summary>
        /// Let the element behave like a <table> element.
        /// </summary>
        public static IFluentDisplayWithDisplayOnBreakpointWithDirection Table { get { return new FluentDisplay().Table; } }

        /// <summary>
        /// Let the element behave like a <tr> element.
        /// </summary>
        public static IFluentDisplayWithDisplayOnBreakpointWithDirection TableRow { get { return new FluentDisplay().TableRow; } }

        /// <summary>
        /// Let the element behave like a <td> element.
        /// </summary>
        public static IFluentDisplayWithDisplayOnBreakpointWithDirection TableCell { get { return new FluentDisplay().TableCell; } }

        /// <summary>
        /// Displays an element as a block-level flex container.
        /// </summary>
        public static IFluentDisplayWithDisplayFlexWithDirection Flex { get { return new FluentDisplay().Flex; } }

        /// <summary>
        /// Displays an element as an inline-level flex container.
        /// </summary>
        public static IFluentDisplayWithDisplayFlexWithDirection InlineFlex { get { return new FluentDisplay().InlineFlex; } }
    }
}
