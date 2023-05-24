#region Using directives
using System.Collections.Generic;
using System.Linq;
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Base interface for all fluent gap builders.
/// </summary>
public interface IFluentGap
{
    /// <summary>
    /// Builds the classnames based on gap rules.
    /// </summary>
    /// <param name="classProvider">Currently used class provider.</param>
    /// <returns>List of classnames for the given rules and the class provider.</returns>
    string Class( IClassProvider classProvider );
}

/// <summary>
/// Contains all the allowed gap rules, except sizes.
/// </summary>
public interface IFluentGapWithSide :
    IFluentGap,
    IFluentGapFromSide
{
}

/// <summary>
/// Contains all the allowed gap rules.
/// </summary>
public interface IFluentGapWithSideAndSize :
    IFluentGap,
    IFluentGapFromSide,
    IFluentGapWithSize
{
}

/// <summary>
/// Allowed sides for gap rules.
/// </summary>
public interface IFluentGapFromSide :
    IFluentGap
{
    /// <summary>
    /// For classes that set both *-left and *-right.
    /// </summary>
    IFluentGapWithSideAndSize OnX { get; }

    /// <summary>
    /// For classes that set both *-top and *-bottom.
    /// </summary>
    IFluentGapWithSideAndSize OnY { get; }

    /// <summary>
    /// For classes that set a margin or padding on all 4 sides of the element.
    /// </summary>
    IFluentGapWithSideAndSize OnAll { get; }
}

/// <summary>
/// Allowed sizes for gap rules.
/// </summary>
public interface IFluentGapWithSize :
    IFluentGap
{
    /// <summary>
    /// For classes that eliminate the margin or padding by setting it to 0.
    /// </summary>
    IFluentGapWithSideAndSize Is0 { get; }

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * .25
    /// </summary>
    IFluentGapWithSideAndSize Is1 { get; }

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * .5
    /// </summary>
    IFluentGapWithSideAndSize Is2 { get; }

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer
    /// </summary>
    IFluentGapWithSideAndSize Is3 { get; }

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * 1.5
    /// </summary>
    IFluentGapWithSideAndSize Is4 { get; }

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * 3
    /// </summary>
    IFluentGapWithSideAndSize Is5 { get; }

    /// <summary>
    /// Used to add custom gap rule.
    /// </summary>
    /// <param name="value">Custom css classname.</param>
    IFluentGapWithSize Is( string value );
}

/// <summary>
/// Default implementation of <see cref="IFluentGap"/>.
/// </summary>
public class FluentGap :
    IFluentGap,
    IFluentGapWithSize,
    IFluentGapFromSide,
    IFluentGapWithSide,
    IFluentGapWithSideAndSize
{
    #region Members

    private class GapDefinition
    {
        public GapSide Side { get; set; }
    }

    private GapDefinition currentGap;

    private readonly Dictionary<GapSize, List<GapDefinition>> rules = new();

    private List<string> customRules;

    private bool dirty = true;

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
                if ( rules.Count > 0 )
                    builder.Append( rules.Select( r => classProvider.Gap( r.Key, r.Value.Select( v => v.Side ) ) ) );

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

    /// <summary>
    /// Appends the new gap size rule.
    /// </summary>
    /// <param name="gapSize">Gap size to append.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentGapWithSideAndSize WithSize( GapSize gapSize )
    {
        var gapDefinition = new GapDefinition { Side = GapSide.All };

        if ( rules.TryGetValue( gapSize, out var rule ) )
            rule.Add( gapDefinition );
        else
            rules.Add( gapSize, new() { gapDefinition } );

        currentGap = gapDefinition;
        Dirty();

        return this;
    }

    /// <summary>
    /// Appends the new side rule.
    /// </summary>
    /// <param name="side">Side to append.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentGapWithSideAndSize WithSide( GapSide side )
    {
        currentGap.Side = side;
        Dirty();

        return this;
    }

    private IFluentGapWithSize WithSize( string value )
    {
        if ( customRules == null )
            customRules = new() { value };
        else
            customRules.Add( value );

        Dirty();

        return this;
    }

    /// <summary>
    /// Used to add custom column rule.
    /// </summary>
    /// <param name="value">Custom css classname.</param>
    public IFluentGapWithSize Is( string value ) => WithSize( value );

    #endregion

    #region Properties

    /// <summary>
    /// For classes that eliminate the margin or padding by setting it to 0.
    /// </summary>
    public IFluentGapWithSideAndSize Is0 => WithSize( GapSize.Is0 );

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * .25
    /// </summary>
    public IFluentGapWithSideAndSize Is1 => WithSize( GapSize.Is1 );

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * .5
    /// </summary>
    public IFluentGapWithSideAndSize Is2 => WithSize( GapSize.Is2 );

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer
    /// </summary>
    public IFluentGapWithSideAndSize Is3 => WithSize( GapSize.Is3 );

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * 1.5
    /// </summary>
    public IFluentGapWithSideAndSize Is4 => WithSize( GapSize.Is4 );

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * 3
    /// </summary>
    public IFluentGapWithSideAndSize Is5 => WithSize( GapSize.Is5 );

    /// <summary>
    /// For classes that set both *-left and *-right.
    /// </summary>
    public IFluentGapWithSideAndSize OnX => WithSide( GapSide.X );

    /// <summary>
    /// For classes that set both *-top and *-bottom.
    /// </summary>
    public IFluentGapWithSideAndSize OnY => WithSide( GapSide.Y );

    /// <summary>
    /// For classes that set a margin or padding on all 4 sides of the element.
    /// </summary>
    public IFluentGapWithSideAndSize OnAll => WithSide( GapSide.All );

    #endregion
}

/// <summary>
/// Gap builder.
/// </summary>
public static class Gap
{
    /// <summary>
    /// for classes that eliminate the margin by setting it to 0
    /// </summary>
    public static IFluentGapWithSideAndSize Is0 => new FluentGap().Is0;

    /// <summary>
    /// (by default) for classes that set the margin to $spacer * .25
    /// </summary>
    public static IFluentGapWithSideAndSize Is1 => new FluentGap().Is1;

    /// <summary>
    /// (by default) for classes that set the margin to $spacer * .5
    /// </summary>
    public static IFluentGapWithSideAndSize Is2 => new FluentGap().Is2;

    /// <summary>
    /// (by default) for classes that set the margin to $spacer
    /// </summary>
    public static IFluentGapWithSideAndSize Is3 => new FluentGap().Is3;

    /// <summary>
    /// (by default) for classes that set the margin to $spacer * 1.5
    /// </summary>
    public static IFluentGapWithSideAndSize Is4 => new FluentGap().Is4;

    /// <summary>
    /// (by default) for classes that set the margin to $spacer * 3
    /// </summary>
    public static IFluentGapWithSideAndSize Is5 => new FluentGap().Is5;

    /// <summary>
    /// Add custom margin rule.
    /// </summary>
    /// <param name="value">Custom css classname.</param>
    public static IFluentGapWithSize Is( string value ) => new FluentGap().Is( value );
}