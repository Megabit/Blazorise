﻿#region Using directives
using System.Collections.Generic;
using System.Linq;
using Blazorise.Utilities;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Base interface for all fluent border builders.
    /// </summary>
    public interface IFluentBorder
    {
        /// <summary>
        /// Builds the classnames based on border rules.
        /// </summary>
        /// <param name="classProvider">Currently used class provider.</param>
        /// <returns>List of classnames for the given rules and the class provider.</returns>
        string Class( IClassProvider classProvider );
    }

    /// <summary>
    /// Sizes allowed for fluent border builder.
    /// </summary>
    public interface IFluentBorderSize :
        IFluentBorder
    {
        /// <summary>
        /// Makes the element borderless.
        /// </summary>
        IFluentBorderWithAll Is0 { get; }

        /// <summary>
        /// Borders will be 1px wide.
        /// </summary>
        IFluentBorderWithAll Is1 { get; }
    }

    /// <summary>
    /// Sides allowed for fluent border builder.
    /// </summary>
    public interface IFluentBorderSide :
        IFluentBorder
    {
        /// <summary>
        /// Shows the border on top side of the element.
        /// </summary>
        IFluentBorderWithAll OnTop { get; }

        /// <summary>
        /// Shows the border on right side of the element.
        /// </summary>
        IFluentBorderWithAll OnRight { get; }

        /// <summary>
        /// Shows the border on bottom side of the element.
        /// </summary>
        IFluentBorderWithAll OnBottom { get; }

        /// <summary>
        /// Shows the border on left side of the element.
        /// </summary>
        IFluentBorderWithAll OnLeft { get; }

        /// <summary>
        /// Shows the border on all sides of the element.
        /// </summary>
        IFluentBorderWithAll OnAll { get; }
    }

    /// <summary>
    /// Colors allowed for fluent border builder.
    /// </summary>
    public interface IFluentBorderColor :
        IFluentBorder
    {
        /// <summary>
        /// Defines the border primary color.
        /// </summary>
        IFluentBorderColorWithSide Primary { get; }

        /// <summary>
        /// Defines the border secondary color.
        /// </summary>
        IFluentBorderColorWithSide Secondary { get; }

        /// <summary>
        /// Defines the border success color.
        /// </summary>
        IFluentBorderColorWithSide Success { get; }

        /// <summary>
        /// Defines the border danger color.
        /// </summary>
        IFluentBorderColorWithSide Danger { get; }

        /// <summary>
        /// Defines the border warning color.
        /// </summary>
        IFluentBorderColorWithSide Warning { get; }

        /// <summary>
        /// Defines the border info color.
        /// </summary>
        IFluentBorderColorWithSide Info { get; }

        /// <summary>
        /// Defines the border light color.
        /// </summary>
        IFluentBorderColorWithSide Light { get; }

        /// <summary>
        /// Defines the border dark color.
        /// </summary>
        IFluentBorderColorWithSide Dark { get; }

        /// <summary>
        /// Defines the border white color.
        /// </summary>
        IFluentBorderColorWithSide White { get; }
    }

    /// <summary>
    /// Radius styles allowed for fluent border builder.
    /// </summary>
    public interface IFluentBorderRadius :
        IFluentBorder
    {
        /// <summary>
        /// Makes the element rounded on all sides.
        /// </summary>
        IFluentBorderWithAll Rounded { get; }

        /// <summary>
        /// Makes the element rounded on top side of the element.
        /// </summary>
        IFluentBorderWithAll RoundedTop { get; }

        /// <summary>
        /// Makes the element rounded on right side of the element.
        /// </summary>
        IFluentBorderWithAll RoundedRight { get; }

        /// <summary>
        /// Makes the element rounded on bottom side of the element.
        /// </summary>
        IFluentBorderWithAll RoundedBottom { get; }

        /// <summary>
        /// Makes the element rounded on left side of the element.
        /// </summary>
        IFluentBorderWithAll RoundedLeft { get; }

        /// <summary>
        /// Makes the element as circle shaped.
        /// </summary>
        IFluentBorderWithAll RoundedCircle { get; }

        /// <summary>
        /// Makes the element as pill shaped.
        /// </summary>
        IFluentBorderWithAll RoundedPill { get; }

        /// <summary>
        /// Makes the element without any round corners.
        /// </summary>
        IFluentBorderWithAll RoundedZero { get; }
    }

    /// <summary>
    /// Combination of border, with sizes and sides.
    /// </summary>
    public interface IFluentBorderWithSizeAndSide :
        IFluentBorder,
        IFluentBorderSize,
        IFluentBorderSide
    {
    }

    /// <summary>
    /// Combination of border colors with sides.
    /// </summary>
    public interface IFluentBorderColorWithSide :
        IFluentBorder,
        IFluentBorderSide
    {
    }

    /// <summary>
    /// Combination of all border rules.
    /// </summary>
    public interface IFluentBorderWithAll :
        IFluentBorder,
        IFluentBorderSize,
        IFluentBorderSide,
        IFluentBorderWithSizeAndSide,
        IFluentBorderColor,
        IFluentBorderColorWithSide,
        IFluentBorderRadius
    {
    }

    /// <summary>
    /// Default implementation of fluent border builder.
    /// </summary>
    public class FluentBorder :
        IFluentBorder,
        IFluentBorderSide,
        IFluentBorderSize,
        IFluentBorderWithSizeAndSide,
        IFluentBorderColor,
        IFluentBorderColorWithSide,
        IFluentBorderRadius,
        IFluentBorderWithAll
    {
        #region Members

        /// <summary>
        /// Holds the additions information for current border rules.
        /// </summary>
        private class BorderDefinition
        {
            public BorderSide Side { get; set; }

            public BorderColor Color { get; set; }
        }

        /// <summary>
        /// Currently used border rules.
        /// </summary>
        private BorderDefinition currentBorderDefinition;

        /// <summary>
        /// List of all border rules to build.
        /// </summary>
        private Dictionary<BorderSize, List<BorderDefinition>> rules;

        /// <summary>
        /// Rule for border radius.
        /// </summary>
        private BorderRadius borderRadius = BorderRadius.None;

        /// <summary>
        /// Indicates if the rules have changed.
        /// </summary>
        private bool dirty = true;

        /// <summary>
        /// Holds the built classnames bases on the border rules.
        /// </summary>
        private string classNames;

        #endregion

        #region Methods

        /// <inheritdoc/>
        public string Class( IClassProvider classProvider )
        {
            if ( dirty )
            {
                void BuildClasses( ClassBuilder builder )
                {
                    if ( rules != null )
                    {
                        if ( rules.Count > 0 )
                            builder.Append( rules.Select( r => classProvider.Border( r.Key, r.Value.Select( v => (v.Side, v.Color) ) ) ) );
                    }

                    if ( borderRadius != BorderRadius.None )
                    {
                        builder.Append( classProvider.BorderRadius( borderRadius ) );
                    }
                }

                var classBuilder = new ClassBuilder( BuildClasses );

                classNames = classBuilder.Class;

                dirty = false;
            }

            return classNames;
        }

        /// <summary>
        /// Flags the classnames to be rebuilt.
        /// </summary>
        private void Dirty()
        {
            dirty = true;
        }

        /// <summary>
        /// Starts the new size rule.
        /// </summary>
        /// <param name="borderSize">Border size to be applied.</param>
        /// <returns>Next rule reference.</returns>
        public IFluentBorderWithAll WithSize( BorderSize borderSize )
        {
            rules ??= new();

            var borderDefinition = new BorderDefinition { Side = BorderSide.All };

            if ( !rules.ContainsKey( borderSize ) )
                rules.Add( borderSize, new() { borderDefinition } );
            else
                rules[borderSize].Add( borderDefinition );

            currentBorderDefinition = borderDefinition;
            Dirty();

            return this;
        }

        /// <summary>
        /// Starts the new side rule.
        /// </summary>
        /// <param name="borderSide">Border side to be applied.</param>
        /// <returns>Next rule reference.</returns>
        public IFluentBorderWithAll WithSide( BorderSide borderSide )
        {
            currentBorderDefinition.Side = borderSide;
            Dirty();

            return this;
        }

        /// <summary>
        /// Starts the new color rule.
        /// </summary>
        /// <param name="borderColor">Border color to be applied.</param>
        /// <returns>Next rule reference.</returns>
        public IFluentBorderColorWithSide WithColor( BorderColor borderColor )
        {
            currentBorderDefinition.Color = borderColor;
            Dirty();

            return this;
        }

        /// <summary>
        /// Starts the new radius rule.
        /// </summary>
        /// <param name="borderRadius">Border radius to be applied.</param>
        /// <returns>Next rule reference.</returns>
        public IFluentBorderWithAll WithRadius( BorderRadius borderRadius )
        {
            this.borderRadius = borderRadius;
            Dirty();

            return this;
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public IFluentBorderWithAll Is0 => WithSize( BorderSize.Is0 );

        /// <inheritdoc/>
        public IFluentBorderWithAll Is1 => WithSize( BorderSize.Is1 );

        /// <inheritdoc/>
        public IFluentBorderWithAll OnTop => WithSide( BorderSide.Top );

        /// <inheritdoc/>
        public IFluentBorderWithAll OnRight => WithSide( BorderSide.Right );

        /// <inheritdoc/>
        public IFluentBorderWithAll OnBottom => WithSide( BorderSide.Bottom );

        /// <inheritdoc/>
        public IFluentBorderWithAll OnLeft => WithSide( BorderSide.Left );

        /// <inheritdoc/>
        public IFluentBorderWithAll OnAll => WithSide( BorderSide.All );

        /// <inheritdoc/>
        public IFluentBorderColorWithSide Primary => WithColor( BorderColor.Primary );

        /// <inheritdoc/>
        public IFluentBorderColorWithSide Secondary => WithColor( BorderColor.Secondary );

        /// <inheritdoc/>
        public IFluentBorderColorWithSide Success => WithColor( BorderColor.Success );

        /// <inheritdoc/>
        public IFluentBorderColorWithSide Danger => WithColor( BorderColor.Danger );

        /// <inheritdoc/>
        public IFluentBorderColorWithSide Warning => WithColor( BorderColor.Warning );

        /// <inheritdoc/>
        public IFluentBorderColorWithSide Info => WithColor( BorderColor.Info );

        /// <inheritdoc/>
        public IFluentBorderColorWithSide Light => WithColor( BorderColor.Light );

        /// <inheritdoc/>
        public IFluentBorderColorWithSide Dark => WithColor( BorderColor.Dark );

        /// <inheritdoc/>
        public IFluentBorderColorWithSide White => WithColor( BorderColor.White );

        /// <inheritdoc/>
        public IFluentBorderWithAll Rounded => WithRadius( BorderRadius.Rounded );

        /// <inheritdoc/>
        public IFluentBorderWithAll RoundedTop => WithRadius( BorderRadius.RoundedTop );

        /// <inheritdoc/>
        public IFluentBorderWithAll RoundedRight => WithRadius( BorderRadius.RoundedRight );

        /// <inheritdoc/>
        public IFluentBorderWithAll RoundedBottom => WithRadius( BorderRadius.RoundedBottom );

        /// <inheritdoc/>
        public IFluentBorderWithAll RoundedLeft => WithRadius( BorderRadius.RoundedLeft );

        /// <inheritdoc/>
        public IFluentBorderWithAll RoundedCircle => WithRadius( BorderRadius.RoundedCircle );

        /// <inheritdoc/>
        public IFluentBorderWithAll RoundedPill => WithRadius( BorderRadius.RoundedPill );

        /// <inheritdoc/>
        public IFluentBorderWithAll RoundedZero => WithRadius( BorderRadius.RoundedZero );

        #endregion
    }

    /// <summary>
    /// Set of border rules to start the build process.
    /// </summary>
    public static class Border
    {
        /// <summary>
        /// Makes the element borderless.
        /// </summary>
        public static IFluentBorderWithAll Is0 => new FluentBorder().Is0;

        /// <summary>
        /// Borders will be 1px wide.
        /// </summary>
        public static IFluentBorderWithAll Is1 => new FluentBorder().Is1;

        /// <summary>
        /// Shows the 1px wide border on the top side of the element.
        /// </summary>
        public static IFluentBorderWithAll OnTop => new FluentBorder().Is1.OnTop;

        /// <summary>
        /// Shows the 1px wide border on the right side of the element.
        /// </summary>
        public static IFluentBorderWithAll OnRight => new FluentBorder().Is1.OnRight;

        /// <summary>
        /// Shows the 1px wide border on the bottom side of the element.
        /// </summary>
        public static IFluentBorderWithAll OnBottom => new FluentBorder().Is1.OnBottom;

        /// <summary>
        /// Shows the 1px wide border on the left side of the element.
        /// </summary>
        public static IFluentBorderWithAll OnLeft => new FluentBorder().Is1.OnLeft;

        /// <summary>
        /// Defines the border primary color on all sided of an element.
        /// </summary>
        public static IFluentBorderColorWithSide Primary => new FluentBorder().Is1.Primary;

        /// <summary>
        /// Defines the border primary color on all sided of an element.
        /// </summary>
        public static IFluentBorderColorWithSide Secondary => new FluentBorder().Is1.Secondary;

        /// <summary>
        /// Defines the border success color on all sided of an element.
        /// </summary>
        public static IFluentBorderColorWithSide Success => new FluentBorder().Is1.Success;

        /// <summary>
        /// Defines the border danger color on all sided of an element.
        /// </summary>
        public static IFluentBorderColorWithSide Danger => new FluentBorder().Is1.Danger;

        /// <summary>
        /// Defines the border warning color on all sided of an element.
        /// </summary>
        public static IFluentBorderColorWithSide Warning => new FluentBorder().Is1.Warning;

        /// <summary>
        /// Defines the border info color on all sided of an element.
        /// </summary>
        public static IFluentBorderColorWithSide Info => new FluentBorder().Is1.Info;

        /// <summary>
        /// Defines the border light color on all sided of an element.
        /// </summary>
        public static IFluentBorderColorWithSide Light => new FluentBorder().Is1.Light;

        /// <summary>
        /// Defines the border dark color on all sided of an element.
        /// </summary>
        public static IFluentBorderColorWithSide Dark => new FluentBorder().Is1.Dark;

        /// <summary>
        /// Defines the border white color on all sided of an element.
        /// </summary>
        public static IFluentBorderColorWithSide White => new FluentBorder().Is1.White;

        /// <summary>
        /// Makes the element rounded on all sides.
        /// </summary>
        public static IFluentBorderWithAll Rounded => new FluentBorder().Rounded;

        /// <summary>
        /// Makes the element rounded on top side of the element.
        /// </summary>
        public static IFluentBorderWithAll RoundedTop => new FluentBorder().RoundedTop;

        /// <summary>
        /// Makes the element rounded on right side of the element.
        /// </summary>
        public static IFluentBorderWithAll RoundedRight => new FluentBorder().RoundedRight;

        /// <summary>
        /// Makes the element rounded on bottom side of the element.
        /// </summary>
        public static IFluentBorderWithAll RoundedBottom => new FluentBorder().RoundedBottom;

        /// <summary>
        /// Makes the element rounded on left side of the element.
        /// </summary>
        public static IFluentBorderWithAll RoundedLeft => new FluentBorder().RoundedLeft;

        /// <summary>
        /// Makes the element as circle shaped.
        /// </summary>
        public static IFluentBorderWithAll RoundedCircle => new FluentBorder().RoundedCircle;

        /// <summary>
        /// Makes the element as pill shaped.
        /// </summary>
        public static IFluentBorderWithAll RoundedPill => new FluentBorder().RoundedPill;

        /// <summary>
        /// Makes the element without any round corners.
        /// </summary>
        public static IFluentBorderWithAll RoundedZero => new FluentBorder().RoundedZero;
    }
}
