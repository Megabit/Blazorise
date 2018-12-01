#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise
{
    public interface IFluentColumn
    {
        string Class( IClassProvider classProvider );
    }

    public interface IFluentColumnOnBreakpointWithOffsetAndSize : IFluentColumn, IFluentColumnOnBreakpoint, IFluentColumnWithOffset, IFluentColumnWithSize
    {
    }

    public interface IFluentColumnOnBreakpoint : IFluentColumn
    {
        /// <summary>
        /// Valid on all devices.
        /// </summary>
        IFluentColumnWithSize OnExtraSmall { get; }

        /// <summary>
        /// Breakpoint on small devices (landscape phones).
        /// </summary>
        IFluentColumnWithSize OnSmall { get; }

        /// <summary>
        ///  Breakpoint on medium devices (tablets).
        /// </summary>
        IFluentColumnWithSize OnMedium { get; }

        /// <summary>
        /// Breakpoint on large devices.
        /// </summary>
        IFluentColumnWithSize OnLarge { get; }

        /// <summary>
        /// Breakpoint on extra large devices (large desktops).
        /// </summary>
        IFluentColumnWithSize OnExtraLarge { get; }
    }

    public interface IFluentColumnWithOffset : IFluentColumn
    {
        IFluentColumnOnBreakpoint WithOffset { get; }
    }

    public interface IFluentColumnWithSize : IFluentColumn
    {
        /// <summary>
        /// One column width.
        /// </summary>
        IFluentColumnOnBreakpointWithOffsetAndSize Is1 { get; }

        /// <summary>
        /// Two columns width.
        /// </summary>
        IFluentColumnOnBreakpointWithOffsetAndSize Is2 { get; }

        /// <summary>
        /// Three columns width.
        /// </summary>
        IFluentColumnOnBreakpointWithOffsetAndSize Is3 { get; }

        /// <summary>
        /// Four columns width.
        /// </summary>
        IFluentColumnOnBreakpointWithOffsetAndSize Is4 { get; }

        /// <summary>
        /// Five columns width.
        /// </summary>
        IFluentColumnOnBreakpointWithOffsetAndSize Is5 { get; }

        /// <summary>
        /// Six columns width.
        /// </summary>
        IFluentColumnOnBreakpointWithOffsetAndSize Is6 { get; }

        /// <summary>
        /// Seven columns width.
        /// </summary>
        IFluentColumnOnBreakpointWithOffsetAndSize Is7 { get; }

        /// <summary>
        /// Eight columns width.
        /// </summary>
        IFluentColumnOnBreakpointWithOffsetAndSize Is8 { get; }

        /// <summary>
        /// Nine columns width.
        /// </summary>
        IFluentColumnOnBreakpointWithOffsetAndSize Is9 { get; }

        /// <summary>
        /// Ten columns width.
        /// </summary>
        IFluentColumnOnBreakpointWithOffsetAndSize Is10 { get; }

        /// <summary>
        /// Eleven columns width.
        /// </summary>
        IFluentColumnOnBreakpointWithOffsetAndSize Is11 { get; }

        /// <summary>
        /// Twelve columns width.
        /// </summary>
        IFluentColumnOnBreakpointWithOffsetAndSize Is12 { get; }

        /// <summary>
        /// Twelve columns width.
        /// </summary>
        IFluentColumnOnBreakpointWithOffsetAndSize IsFull { get; }

        /// <summary>
        /// Six columns width.
        /// </summary>
        IFluentColumnOnBreakpointWithOffsetAndSize IsHalf { get; }

        /// <summary>
        /// Four columns width.
        /// </summary>
        IFluentColumnOnBreakpointWithOffsetAndSize IsThird { get; }

        /// <summary>
        /// Three columns width.
        /// </summary>
        IFluentColumnOnBreakpointWithOffsetAndSize IsQuarter { get; }

        /// <summary>
        /// Fill all available space.
        /// </summary>
        IFluentColumnOnBreakpointWithOffsetAndSize IsAuto { get; }
    }

    public class FluentColumn : IFluentColumn, IFluentColumnOnBreakpointWithOffsetAndSize, IFluentColumnOnBreakpoint, IFluentColumnWithSize, IFluentColumnWithOffset
    {
        #region Members

        private class ColumnDefinition
        {
            public Breakpoint Breakpoint { get; set; }

            public bool Offset { get; set; }
        }

        private ColumnDefinition currentColumn;

        private Dictionary<ColumnWidth, List<ColumnDefinition>> rules = new Dictionary<ColumnWidth, List<ColumnDefinition>>();

        private bool built = false;

        #endregion

        #region Methods

        public string Class( IClassProvider classProvider )
        {
            if ( !built )
            {
                ClassMapper
                    .If( () => classProvider.Col( ColumnWidth.Is1, rules[ColumnWidth.Is1].Select( x => (x.Breakpoint, x.Offset) ) ), () => rules.ContainsKey( ColumnWidth.Is1 ) )
                    .If( () => classProvider.Col( ColumnWidth.Is2, rules[ColumnWidth.Is2].Select( x => (x.Breakpoint, x.Offset) ) ), () => rules.ContainsKey( ColumnWidth.Is2 ) )
                    .If( () => classProvider.Col( ColumnWidth.Is3, rules[ColumnWidth.Is3].Select( x => (x.Breakpoint, x.Offset) ) ), () => rules.ContainsKey( ColumnWidth.Is3 ) )
                    .If( () => classProvider.Col( ColumnWidth.Is4, rules[ColumnWidth.Is4].Select( x => (x.Breakpoint, x.Offset) ) ), () => rules.ContainsKey( ColumnWidth.Is4 ) )
                    .If( () => classProvider.Col( ColumnWidth.Is5, rules[ColumnWidth.Is5].Select( x => (x.Breakpoint, x.Offset) ) ), () => rules.ContainsKey( ColumnWidth.Is5 ) )
                    .If( () => classProvider.Col( ColumnWidth.Is6, rules[ColumnWidth.Is6].Select( x => (x.Breakpoint, x.Offset) ) ), () => rules.ContainsKey( ColumnWidth.Is6 ) )
                    .If( () => classProvider.Col( ColumnWidth.Is7, rules[ColumnWidth.Is7].Select( x => (x.Breakpoint, x.Offset) ) ), () => rules.ContainsKey( ColumnWidth.Is7 ) )
                    .If( () => classProvider.Col( ColumnWidth.Is8, rules[ColumnWidth.Is8].Select( x => (x.Breakpoint, x.Offset) ) ), () => rules.ContainsKey( ColumnWidth.Is8 ) )
                    .If( () => classProvider.Col( ColumnWidth.Is9, rules[ColumnWidth.Is9].Select( x => (x.Breakpoint, x.Offset) ) ), () => rules.ContainsKey( ColumnWidth.Is9 ) )
                    .If( () => classProvider.Col( ColumnWidth.Is10, rules[ColumnWidth.Is10].Select( x => (x.Breakpoint, x.Offset) ) ), () => rules.ContainsKey( ColumnWidth.Is10 ) )
                    .If( () => classProvider.Col( ColumnWidth.Is11, rules[ColumnWidth.Is11].Select( x => (x.Breakpoint, x.Offset) ) ), () => rules.ContainsKey( ColumnWidth.Is11 ) )
                    .If( () => classProvider.Col( ColumnWidth.Is12, rules[ColumnWidth.Is12].Select( x => (x.Breakpoint, x.Offset) ) ), () => rules.ContainsKey( ColumnWidth.Is12 ) )
                    .If( () => classProvider.Col( ColumnWidth.Auto, rules[ColumnWidth.Auto].Select( x => (x.Breakpoint, x.Offset) ) ), () => rules.ContainsKey( ColumnWidth.Auto ) );

                built = true;
            }

            return ClassMapper.Class;
        }

        private IFluentColumnOnBreakpointWithOffsetAndSize WithColumnSize( ColumnWidth columnSize )
        {
            var columnDefinition = new ColumnDefinition { Breakpoint = Breakpoint.None };

            if ( !rules.ContainsKey( columnSize ) )
                rules.Add( columnSize, new List<ColumnDefinition> { columnDefinition } );
            else
                rules[columnSize].Add( columnDefinition );

            currentColumn = columnDefinition;
            ClassMapper.Dirty();
            return this;
        }

        private IFluentColumnWithSize WithBreakpoint( Breakpoint breakpoint )
        {
            currentColumn.Breakpoint = breakpoint;
            ClassMapper.Dirty();
            return this;
        }

        #endregion

        #region Properties

        internal ClassMapper ClassMapper { get; } = new ClassMapper();

        /// <summary>
        /// Valid on all devices.
        /// </summary>
        public IFluentColumnWithSize OnExtraSmall => WithBreakpoint( Breakpoint.ExtraSmall );

        /// <summary>
        /// Breakpoint on small devices (landscape phones).
        /// </summary>
        public IFluentColumnWithSize OnSmall => WithBreakpoint( Breakpoint.Small );

        /// <summary>
        ///  Breakpoint on medium devices (tablets).
        /// </summary>
        public IFluentColumnWithSize OnMedium => WithBreakpoint( Breakpoint.Medium );

        /// <summary>
        /// Breakpoint on large devices.
        /// </summary>
        public IFluentColumnWithSize OnLarge => WithBreakpoint( Breakpoint.Large );

        /// <summary>
        /// Breakpoint on extra large devices (large desktops).
        /// </summary>
        public IFluentColumnWithSize OnExtraLarge => WithBreakpoint( Breakpoint.ExtraLarge );

        /// <summary>
        /// Move columns to the right.
        /// </summary>
        public IFluentColumnOnBreakpoint WithOffset { get { currentColumn.Offset = true; ClassMapper.Dirty(); return this; } }

        /// <summary>
        /// One column width.
        /// </summary>
        public IFluentColumnOnBreakpointWithOffsetAndSize Is1 { get { return WithColumnSize( ColumnWidth.Is1 ); } }

        /// <summary>
        /// Two columns width.
        /// </summary>
        public IFluentColumnOnBreakpointWithOffsetAndSize Is2 { get { return WithColumnSize( ColumnWidth.Is2 ); } }

        /// <summary>
        /// Three columns width.
        /// </summary>
        public IFluentColumnOnBreakpointWithOffsetAndSize Is3 { get { return WithColumnSize( ColumnWidth.Is3 ); } }

        /// <summary>
        /// Four columns width.
        /// </summary>
        public IFluentColumnOnBreakpointWithOffsetAndSize Is4 { get { return WithColumnSize( ColumnWidth.Is4 ); } }

        /// <summary>
        /// Five columns width.
        /// </summary>
        public IFluentColumnOnBreakpointWithOffsetAndSize Is5 { get { return WithColumnSize( ColumnWidth.Is5 ); } }

        /// <summary>
        /// Six columns width.
        /// </summary>
        public IFluentColumnOnBreakpointWithOffsetAndSize Is6 { get { return WithColumnSize( ColumnWidth.Is6 ); } }

        /// <summary>
        /// Seven columns width.
        /// </summary>
        public IFluentColumnOnBreakpointWithOffsetAndSize Is7 { get { return WithColumnSize( ColumnWidth.Is7 ); } }

        /// <summary>
        /// Eight columns width.
        /// </summary>
        public IFluentColumnOnBreakpointWithOffsetAndSize Is8 { get { return WithColumnSize( ColumnWidth.Is8 ); } }

        /// <summary>
        /// Nine columns width.
        /// </summary>
        public IFluentColumnOnBreakpointWithOffsetAndSize Is9 { get { return WithColumnSize( ColumnWidth.Is9 ); } }

        /// <summary>
        /// Ten columns width.
        /// </summary>
        public IFluentColumnOnBreakpointWithOffsetAndSize Is10 { get { return WithColumnSize( ColumnWidth.Is10 ); } }

        /// <summary>
        /// Eleven columns width.
        /// </summary>
        public IFluentColumnOnBreakpointWithOffsetAndSize Is11 { get { return WithColumnSize( ColumnWidth.Is11 ); } }

        /// <summary>
        /// Twelve columns width.
        /// </summary>
        public IFluentColumnOnBreakpointWithOffsetAndSize Is12 { get { return WithColumnSize( ColumnWidth.Is12 ); } }

        /// <summary>
        /// Twelve columns width.
        /// </summary>
        public IFluentColumnOnBreakpointWithOffsetAndSize IsFull { get { return WithColumnSize( ColumnWidth.Is12 ); } }

        /// <summary>
        /// Six columns width.
        /// </summary>
        public IFluentColumnOnBreakpointWithOffsetAndSize IsHalf { get { return WithColumnSize( ColumnWidth.Is6 ); } }

        /// <summary>
        /// Four columns width.
        /// </summary>
        public IFluentColumnOnBreakpointWithOffsetAndSize IsThird { get { return WithColumnSize( ColumnWidth.Is4 ); } }

        /// <summary>
        /// Three columns width.
        /// </summary>
        public IFluentColumnOnBreakpointWithOffsetAndSize IsQuarter { get { return WithColumnSize( ColumnWidth.Is3 ); } }

        /// <summary>
        /// Fill all available space.
        /// </summary>
        public IFluentColumnOnBreakpointWithOffsetAndSize IsAuto { get { return WithColumnSize( ColumnWidth.Auto ); } }

        #endregion
    }

    /// <summary>
    /// Fluent builder for the column sizes.
    /// </summary>
    public static class ColumnSize
    {
        /// <summary>
        /// One column width.
        /// </summary>
        public static IFluentColumnOnBreakpointWithOffsetAndSize Is1 { get { return new FluentColumn().Is1; } }

        /// <summary>
        /// Two columns width.
        /// </summary>
        public static IFluentColumnOnBreakpointWithOffsetAndSize Is2 { get { return new FluentColumn().Is2; } }

        /// <summary>
        /// Three columns width.
        /// </summary>
        public static IFluentColumnOnBreakpointWithOffsetAndSize Is3 { get { return new FluentColumn().Is3; } }

        /// <summary>
        /// Four columns width.
        /// </summary>
        public static IFluentColumnOnBreakpointWithOffsetAndSize Is4 { get { return new FluentColumn().Is4; } }

        /// <summary>
        /// Five columns width.
        /// </summary>
        public static IFluentColumnOnBreakpointWithOffsetAndSize Is5 { get { return new FluentColumn().Is5; } }

        /// <summary>
        /// Six columns width.
        /// </summary>
        public static IFluentColumnOnBreakpointWithOffsetAndSize Is6 { get { return new FluentColumn().Is6; } }

        /// <summary>
        /// Seven columns width.
        /// </summary>
        public static IFluentColumnOnBreakpointWithOffsetAndSize Is7 { get { return new FluentColumn().Is7; } }

        /// <summary>
        /// Eight columns width.
        /// </summary>
        public static IFluentColumnOnBreakpointWithOffsetAndSize Is8 { get { return new FluentColumn().Is8; } }

        /// <summary>
        /// Nine columns width.
        /// </summary>
        public static IFluentColumnOnBreakpointWithOffsetAndSize Is9 { get { return new FluentColumn().Is9; } }

        /// <summary>
        /// Ten columns width.
        /// </summary>
        public static IFluentColumnOnBreakpointWithOffsetAndSize Is10 { get { return new FluentColumn().Is10; } }

        /// <summary>
        /// Eleven columns width.
        /// </summary>
        public static IFluentColumnOnBreakpointWithOffsetAndSize Is11 { get { return new FluentColumn().Is11; } }

        /// <summary>
        /// Twelve columns width.
        /// </summary>
        public static IFluentColumnOnBreakpointWithOffsetAndSize Is12 { get { return new FluentColumn().Is12; } }

        /// <summary>
        /// Twelve columns width.
        /// </summary>
        public static IFluentColumnOnBreakpointWithOffsetAndSize IsFull { get { return new FluentColumn().IsFull; } }

        /// <summary>
        /// Six columns width.
        /// </summary>
        public static IFluentColumnOnBreakpointWithOffsetAndSize IsHalf { get { return new FluentColumn().IsHalf; } }

        /// <summary>
        /// Four columns width.
        /// </summary>
        public static IFluentColumnOnBreakpointWithOffsetAndSize IsThird { get { return new FluentColumn().IsThird; } }

        /// <summary>
        /// Three columns width.
        /// </summary>
        public static IFluentColumnOnBreakpointWithOffsetAndSize IsQuarter { get { return new FluentColumn().IsQuarter; } }

        /// <summary>
        /// Fill all available space.
        /// </summary>
        public static IFluentColumnOnBreakpointWithOffsetAndSize IsAuto { get { return new FluentColumn().IsAuto; } }
    }
}
