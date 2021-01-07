#region Using directives
using System;
#endregion

namespace Blazorise
{
    public interface IFluentBarCollapseMode
    {
        string Class( IClassProvider classProvider );
    }

    public interface IFluentBarCollapseModeOnBreakpoint :
        IFluentDisplay
    {
        /// <summary>
        /// Valid on all devices. (extra small)
        /// </summary>
        IFluentBarCollapseModeWithMode OnMobile { get; }

        /// <summary>
        /// Breakpoint on tablets (small).
        /// </summary>
        IFluentBarCollapseModeWithMode OnTablet { get; }

        /// <summary>
        ///  Breakpoint on desktop (medium).
        /// </summary>
        IFluentBarCollapseModeWithMode OnDesktop { get; }

        /// <summary>
        /// Breakpoint on widescreen (large).
        /// </summary>
        IFluentBarCollapseModeWithMode OnWidescreen { get; }

        /// <summary>
        /// Breakpoint on large desktops (extra large).
        /// </summary>
        IFluentBarCollapseModeWithMode OnFullHD { get; }
    }

    public interface IFluentBarCollapseModeWithMode
    {
        IFluentBarCollapseModeOnBreakpoint Hide { get; }
        IFluentBarCollapseModeOnBreakpoint Small { get; }
    }

    public class FluentBarCollapseMode :
        IFluentBarCollapseMode,
        IFluentBarCollapseModeOnBreakpoint,
        IFluentBarCollapseModeWithMode
    {
        #region Members

        private class BarCollapseModeDefinition
        {
            public Breakpoint Breakpoint { get; set; }

            public BarCollapseMode Mode { get; set; }
        }

        private BarCollapseModeDefinition currentBarCollapseMode;

        #endregion

        #region Methods

        public IFluentBarCollapseModeOnBreakpoint WithMode( BarCollapseMode mode )
        {
            var barCollapseModeDefinition = new BarCollapseModeDefinition { Breakpoint = Breakpoint.None };

            currentBarCollapseMode = barCollapseModeDefinition;

            return this;
        }

        public IFluentBarCollapseModeWithMode WithBreakpoint( Breakpoint breakpoint )
        {
            currentBarCollapseMode.Breakpoint = breakpoint;
            //Dirty();

            return this;
        }

        public string Class( IClassProvider classProvider )
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Valid on all devices. (extra small)
        /// </summary>
        public IFluentBarCollapseModeWithMode OnMobile => WithBreakpoint( Breakpoint.Mobile );

        /// <summary>
        /// Breakpoint on tablets (small).
        /// </summary>
        public IFluentBarCollapseModeWithMode OnTablet => WithBreakpoint( Breakpoint.Tablet );

        /// <summary>
        ///  Breakpoint on desktop (medium).
        /// </summary>
        public IFluentBarCollapseModeWithMode OnDesktop => WithBreakpoint( Breakpoint.Desktop );

        /// <summary>
        /// Breakpoint on widescreen (large).
        /// </summary>
        public IFluentBarCollapseModeWithMode OnWidescreen => WithBreakpoint( Breakpoint.Widescreen );

        /// <summary>
        /// Breakpoint on large desktops (extra large).
        /// </summary>
        public IFluentBarCollapseModeWithMode OnFullHD => WithBreakpoint( Breakpoint.FullHD );

        /// <summary>
        /// 
        /// </summary>
        public IFluentBarCollapseModeOnBreakpoint Hide { get { return WithMode( BarCollapseMode.Hide ); } }

        /// <summary>
        /// 
        /// </summary>
        public IFluentBarCollapseModeOnBreakpoint Small { get { return WithMode( BarCollapseMode.Small ); } }

        #endregion
    }

    public static class BarCollapseModee
    {
        public static IFluentBarCollapseModeOnBreakpoint Hide { get { return new FluentBarCollapseMode().Hide; } }
        public static IFluentBarCollapseModeOnBreakpoint Small { get { return new FluentBarCollapseMode().Small; } }
    }
}
