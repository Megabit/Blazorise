#region Using directives
using System.Collections.Generic;
using System.Linq;
using Blazorise.Utilities;
#endregion

namespace Blazorise
{
    public interface IFluentSpacing
    {
        string Class( IClassProvider classProvider );
    }

    public interface IFluentSpacingOnBreakpointWithSide : IFluentSpacing, IFluentSpacingFromSide, IFluentSpacingOnBreakpoint
    {
    }

    public interface IFluentSpacingOnBreakpointWithSideAndSize : IFluentSpacing, IFluentSpacingFromSide, IFluentSpacingOnBreakpoint, IFluentSpacingWithSize
    {
    }

    public interface IFluentSpacingFromSide : IFluentSpacing
    {
        /// <summary>
        /// For classes that set margin-top or padding-top.
        /// </summary>
        IFluentSpacingOnBreakpointWithSideAndSize FromTop { get; }

        /// <summary>
        /// For classes that set margin-bottom or padding-bottom.
        /// </summary>
        IFluentSpacingOnBreakpointWithSideAndSize FromBottom { get; }

        /// <summary>
        /// For classes that set margin-left or padding-left.
        /// </summary>
        IFluentSpacingOnBreakpointWithSideAndSize FromLeft { get; }

        /// <summary>
        /// For classes that set margin-right or padding-right.
        /// </summary>
        IFluentSpacingOnBreakpointWithSideAndSize FromRight { get; }

        /// <summary>
        /// For classes that set both *-left and *-right.
        /// </summary>
        IFluentSpacingOnBreakpointWithSideAndSize OnX { get; }

        /// <summary>
        /// For classes that set both *-top and *-bottom.
        /// </summary>
        IFluentSpacingOnBreakpointWithSideAndSize OnY { get; }

        /// <summary>
        /// For classes that set a margin or padding on all 4 sides of the element.
        /// </summary>
        IFluentSpacingOnBreakpointWithSideAndSize OnAll { get; }
    }

    public interface IFluentSpacingOnBreakpoint : IFluentSpacing, IFluentSpacingFromSide
    {
        /// <summary>
        /// Valid on all devices. (extra small)
        /// </summary>
        IFluentSpacingOnBreakpointWithSideAndSize OnMobile { get; }

        /// <summary>
        /// Breakpoint on tablets (small).
        /// </summary>
        IFluentSpacingOnBreakpointWithSideAndSize OnTablet { get; }

        /// <summary>
        ///  Breakpoint on desktop (medium).
        /// </summary>
        IFluentSpacingOnBreakpointWithSideAndSize OnDesktop { get; }

        /// <summary>
        /// Breakpoint on widescreen (large).
        /// </summary>
        IFluentSpacingOnBreakpointWithSideAndSize OnWidescreen { get; }

        /// <summary>
        /// Breakpoint on large desktops (extra large).
        /// </summary>
        IFluentSpacingOnBreakpointWithSideAndSize OnFullHD { get; }
    }

    public interface IFluentSpacingWithSize : IFluentSpacing
    {
        /// <summary>
        /// For classes that eliminate the margin or padding by setting it to 0.
        /// </summary>
        IFluentSpacingOnBreakpointWithSideAndSize Is0 { get; }

        /// <summary>
        /// (by default) for classes that set the margin or padding to $spacer * .25
        /// </summary>
        IFluentSpacingOnBreakpointWithSideAndSize Is1 { get; }

        /// <summary>
        /// (by default) for classes that set the margin or padding to $spacer * .5
        /// </summary>
        IFluentSpacingOnBreakpointWithSideAndSize Is2 { get; }

        /// <summary>
        /// (by default) for classes that set the margin or padding to $spacer
        /// </summary>
        IFluentSpacingOnBreakpointWithSideAndSize Is3 { get; }

        /// <summary>
        /// (by default) for classes that set the margin or padding to $spacer * 1.5
        /// </summary>
        IFluentSpacingOnBreakpointWithSideAndSize Is4 { get; }

        /// <summary>
        /// (by default) for classes that set the margin or padding to $spacer * 3
        /// </summary>
        IFluentSpacingOnBreakpointWithSideAndSize Is5 { get; }

        /// <summary>
        /// For classes that set the margin to auto.
        /// </summary>
        IFluentSpacingOnBreakpointWithSideAndSize IsAuto { get; }

        /// <summary>
        /// Used to add custom spacing rule.
        /// </summary>
        /// <param name="value">Custom css classname.</param>
        IFluentSpacingWithSize Is( string value );
    }

    public abstract class FluentSpacing : IFluentSpacing, IFluentSpacingWithSize, IFluentSpacingOnBreakpoint, IFluentSpacingFromSide, IFluentSpacingOnBreakpointWithSide, IFluentSpacingOnBreakpointWithSideAndSize
    {
        #region Members

        private class SpacingDefinition
        {
            public Side Side { get; set; }

            public Breakpoint Breakpoint { get; set; }
        }

        /// <summary>
        /// Spacing type.
        /// </summary>
        protected readonly Spacing spacing;

        private SpacingDefinition currentSpacing;

        private Dictionary<SpacingSize, List<SpacingDefinition>> rules = new Dictionary<SpacingSize, List<SpacingDefinition>>();

        private List<string> customRules;

        private bool dirty = true;

        private string classNames;

        #endregion

        #region Constructors

        public FluentSpacing( Spacing spacing )
        {
            this.spacing = spacing;
        }

        #endregion

        #region Methods

        public string Class( IClassProvider classProvider )
        {
            if ( dirty )
            {
                void BuildClasses( ClassBuilder builder )
                {
                    if ( rules.Count > 0 )
                        builder.Append( rules.Select( r => classProvider.Spacing( spacing, r.Key, r.Value.Select( v => (v.Side, v.Breakpoint) ) ) ) );

                    if ( customRules?.Count > 0 )
                        builder.Append( customRules );
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

        public IFluentSpacingOnBreakpointWithSideAndSize WithSize( SpacingSize spacingSize )
        {
            var spacingDefinition = new SpacingDefinition { Breakpoint = Breakpoint.None, Side = Side.All };

            if ( !rules.ContainsKey( spacingSize ) )
                rules.Add( spacingSize, new List<SpacingDefinition>() { spacingDefinition } );
            else
                rules[spacingSize].Add( spacingDefinition );

            currentSpacing = spacingDefinition;
            Dirty();
            return this;
        }

        public IFluentSpacingOnBreakpointWithSideAndSize WithSide( Side side )
        {
            currentSpacing.Side = side;
            Dirty();

            return this;
        }

        public IFluentSpacingOnBreakpointWithSideAndSize WithBreakpoint( Breakpoint breakpoint )
        {
            currentSpacing.Breakpoint = breakpoint;
            Dirty();

            return this;
        }

        private IFluentSpacingWithSize WithSize( string value )
        {
            if ( customRules == null )
                customRules = new List<string> { value };
            else
                customRules.Add( value );

            Dirty();

            return this;
        }

        /// <summary>
        /// Used to add custom column rule.
        /// </summary>
        /// <param name="value">Custom css classname.</param>
        public IFluentSpacingWithSize Is( string value ) => WithSize( value );

        #endregion

        #region Properties

        /// <summary>
        /// For classes that eliminate the margin or padding by setting it to 0.
        /// </summary>
        public IFluentSpacingOnBreakpointWithSideAndSize Is0 => WithSize( SpacingSize.Is0 );

        /// <summary>
        /// (by default) for classes that set the margin or padding to $spacer * .25
        /// </summary>
        public IFluentSpacingOnBreakpointWithSideAndSize Is1 => WithSize( SpacingSize.Is1 );

        /// <summary>
        /// (by default) for classes that set the margin or padding to $spacer * .5
        /// </summary>
        public IFluentSpacingOnBreakpointWithSideAndSize Is2 => WithSize( SpacingSize.Is2 );

        /// <summary>
        /// (by default) for classes that set the margin or padding to $spacer
        /// </summary>
        public IFluentSpacingOnBreakpointWithSideAndSize Is3 => WithSize( SpacingSize.Is3 );

        /// <summary>
        /// (by default) for classes that set the margin or padding to $spacer * 1.5
        /// </summary>
        public IFluentSpacingOnBreakpointWithSideAndSize Is4 => WithSize( SpacingSize.Is4 );

        /// <summary>
        /// (by default) for classes that set the margin or padding to $spacer * 3
        /// </summary>
        public IFluentSpacingOnBreakpointWithSideAndSize Is5 => WithSize( SpacingSize.Is5 );

        /// <summary>
        /// For classes that set the margin to auto.
        /// </summary>
        public IFluentSpacingOnBreakpointWithSideAndSize IsAuto => WithSize( SpacingSize.IsAuto );

        /// <summary>
        /// For classes that set margin-top or padding-top.
        /// </summary>
        public IFluentSpacingOnBreakpointWithSideAndSize FromTop => WithSide( Side.Top );

        /// <summary>
        /// For classes that set margin-bottom or padding-bottom.
        /// </summary>
        public IFluentSpacingOnBreakpointWithSideAndSize FromBottom => WithSide( Side.Bottom );

        /// <summary>
        /// For classes that set margin-left or padding-left.
        /// </summary>
        public IFluentSpacingOnBreakpointWithSideAndSize FromLeft => WithSide( Side.Left );

        /// <summary>
        /// For classes that set margin-right or padding-right.
        /// </summary>
        public IFluentSpacingOnBreakpointWithSideAndSize FromRight => WithSide( Side.Right );

        /// <summary>
        /// For classes that set both *-left and *-right.
        /// </summary>
        public IFluentSpacingOnBreakpointWithSideAndSize OnX => WithSide( Side.X );

        /// <summary>
        /// For classes that set both *-top and *-bottom.
        /// </summary>
        public IFluentSpacingOnBreakpointWithSideAndSize OnY => WithSide( Side.Y );

        /// <summary>
        /// For classes that set a margin or padding on all 4 sides of the element.
        /// </summary>
        public IFluentSpacingOnBreakpointWithSideAndSize OnAll => WithSide( Side.All );

        /// <summary>
        /// Valid on all devices. (extra small)
        /// </summary>
        public IFluentSpacingOnBreakpointWithSideAndSize OnMobile => WithBreakpoint( Breakpoint.Mobile );

        /// <summary>
        /// Breakpoint on tablets (small).
        /// </summary>
        public IFluentSpacingOnBreakpointWithSideAndSize OnTablet => WithBreakpoint( Breakpoint.Tablet );

        /// <summary>
        ///  Breakpoint on desktop (medium).
        /// </summary>
        public IFluentSpacingOnBreakpointWithSideAndSize OnDesktop => WithBreakpoint( Breakpoint.Desktop );

        /// <summary>
        /// Breakpoint on widescreen (large).
        /// </summary>
        public IFluentSpacingOnBreakpointWithSideAndSize OnWidescreen => WithBreakpoint( Breakpoint.Widescreen );

        /// <summary>
        /// Breakpoint on large desktops (extra large).
        /// </summary>
        public IFluentSpacingOnBreakpointWithSideAndSize OnFullHD => WithBreakpoint( Breakpoint.FullHD );

        #endregion
    }

    public sealed class FluentMargin : FluentSpacing
    {
        public FluentMargin() : base( Spacing.Margin ) { }
    }

    public sealed class FluentPadding : FluentSpacing
    {
        public FluentPadding() : base( Spacing.Padding ) { }
    }

    /// <summary>
    /// Margin builder.
    /// </summary>
    public static class Margin
    {
        /// <summary>
        /// for classes that eliminate the margin by setting it to 0
        /// </summary>
        public static IFluentSpacingOnBreakpointWithSideAndSize Is0 => new FluentMargin().Is0;

        /// <summary>
        /// (by default) for classes that set the margin to $spacer * .25
        /// </summary>
        public static IFluentSpacingOnBreakpointWithSideAndSize Is1 => new FluentMargin().Is1;

        /// <summary>
        /// (by default) for classes that set the margin to $spacer * .5
        /// </summary>
        public static IFluentSpacingOnBreakpointWithSideAndSize Is2 => new FluentMargin().Is2;

        /// <summary>
        /// (by default) for classes that set the margin to $spacer
        /// </summary>
        public static IFluentSpacingOnBreakpointWithSideAndSize Is3 => new FluentMargin().Is3;

        /// <summary>
        /// (by default) for classes that set the margin to $spacer * 1.5
        /// </summary>
        public static IFluentSpacingOnBreakpointWithSideAndSize Is4 => new FluentMargin().Is4;

        /// <summary>
        /// (by default) for classes that set the margin to $spacer * 3
        /// </summary>
        public static IFluentSpacingOnBreakpointWithSideAndSize Is5 => new FluentMargin().Is5;

        /// <summary>
        /// For classes that set the margin to auto.
        /// </summary>
        public static IFluentSpacingOnBreakpointWithSideAndSize IsAuto => new FluentMargin().IsAuto;

        /// <summary>
        /// Add custom margin rule.
        /// </summary>
        /// <param name="value">Custom css classname.</param>
        public static IFluentSpacingWithSize Is( string value ) => new FluentMargin().Is( value );
    }

    /// <summary>
    /// Padding builder.
    /// </summary>
    public static class Padding
    {
        /// <summary>
        /// for classes that eliminate the padding by setting it to 0
        /// </summary>
        public static IFluentSpacingOnBreakpointWithSideAndSize Is0 => new FluentPadding().Is0;

        /// <summary>
        /// (by default) for classes that set the padding to $spacer * .25
        /// </summary>
        public static IFluentSpacingOnBreakpointWithSideAndSize Is1 => new FluentPadding().Is1;

        /// <summary>
        /// (by default) for classes that set the padding to $spacer * .5
        /// </summary>
        public static IFluentSpacingOnBreakpointWithSideAndSize Is2 => new FluentPadding().Is2;

        /// <summary>
        /// (by default) for classes that set the padding to $spacer
        /// </summary>
        public static IFluentSpacingOnBreakpointWithSideAndSize Is3 => new FluentPadding().Is3;

        /// <summary>
        /// (by default) for classes that set the padding to $spacer * 1.5
        /// </summary>
        public static IFluentSpacingOnBreakpointWithSideAndSize Is4 => new FluentPadding().Is4;

        /// <summary>
        /// (by default) for classes that set the padding to $spacer * 3
        /// </summary>
        public static IFluentSpacingOnBreakpointWithSideAndSize Is5 => new FluentPadding().Is5;

        /// <summary>
        /// For classes that set the margin to auto.
        /// </summary>
        public static IFluentSpacingOnBreakpointWithSideAndSize IsAuto => new FluentPadding().IsAuto;

        /// <summary>
        /// Add custom padding rule.
        /// </summary>
        /// <param name="value">Custom css classname.</param>
        public static IFluentSpacingWithSize Is( string value ) => new FluentPadding().Is( value );
    }
}
