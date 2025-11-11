#region Using directives
using System.Collections.Generic;
using System.Linq;
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Base interface for all fluent gutter builders.
/// </summary>
public interface IFluentGutter
{
    /// <summary>
    /// Builds the classnames based on gutter rules.
    /// </summary>
    /// <param name="classProvider">Currently used class provider.</param>
    /// <returns>List of classnames for the given rules and the class provider.</returns>
    string Class( IClassProvider classProvider );
}

/// <summary>
/// Contains all the allowed gutter rules, except sizes.
/// </summary>
public interface IFluentGutterWithSide :
    IFluentGutter,
    IFluentGutterFromSide
{
}

/// <summary>
/// Contains all the allowed gutter rules.
/// </summary>
public interface IFluentGutterWithSideAndSize :
    IFluentGutter,
    IFluentGutterFromSide,
    IFluentGutterWithSize
{
}

/// <summary>
/// Allowed sides for gutter rules.
/// </summary>
public interface IFluentGutterFromSide :
    IFluentGutter
{
    /// <summary>
    /// For classes that set both *-left and *-right.
    /// </summary>
    IFluentGutterWithSideAndSize OnX { get; }

    /// <summary>
    /// For classes that set both *-top and *-bottom.
    /// </summary>
    IFluentGutterWithSideAndSize OnY { get; }

    /// <summary>
    /// For classes that set a margin or padding on all 4 sides of the element.
    /// </summary>
    IFluentGutterWithSideAndSize OnAll { get; }
}

/// <summary>
/// Allowed sizes for gutter rules.
/// </summary>
public interface IFluentGutterWithSize :
    IFluentGutter
{
    /// <summary>
    /// For classes that eliminate the margin or padding by setting it to 0.
    /// </summary>
    IFluentGutterWithSideAndSize Is0 { get; }

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * .25
    /// </summary>
    IFluentGutterWithSideAndSize Is1 { get; }

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * .5
    /// </summary>
    IFluentGutterWithSideAndSize Is2 { get; }

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer
    /// </summary>
    IFluentGutterWithSideAndSize Is3 { get; }

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * 1.5
    /// </summary>
    IFluentGutterWithSideAndSize Is4 { get; }

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * 3
    /// </summary>
    IFluentGutterWithSideAndSize Is5 { get; }

    /// <summary>
    /// Used to add custom gutter rule.
    /// </summary>
    /// <param name="value">Custom css classname.</param>
    IFluentGutterWithSize Is( string value );
}

/// <summary>
/// Default implementation of <see cref="IFluentGutter"/>.
/// </summary>
public class FluentGutter :
    IFluentGutter,
    IFluentGutterWithSize,
    IFluentGutterFromSide,
    IFluentGutterWithSide,
    IFluentGutterWithSideAndSize
{
    #region Members

    private class GutterDefinition
    {
        public GutterSide Side { get; set; }
    }

    private GutterDefinition currentGutter;

    private readonly Dictionary<GutterSize, List<GutterDefinition>> rules = new();

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
                    builder.Append( rules.Select( r => classProvider.Gutter( r.Key, r.Value.Select( v => v.Side ) ) ) );

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
    /// Appends the new gutter size rule.
    /// </summary>
    /// <param name="gutterSize">Gutter size to append.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentGutterWithSideAndSize WithSize( GutterSize gutterSize )
    {
        var gutterDefinition = new GutterDefinition { Side = GutterSide.All };

        if ( rules.TryGetValue( gutterSize, out var rule ) )
            rule.Add( gutterDefinition );
        else
            rules.Add( gutterSize, new() { gutterDefinition } );

        currentGutter = gutterDefinition;
        Dirty();

        return this;
    }

    /// <summary>
    /// Appends the new side rule.
    /// </summary>
    /// <param name="side">Side to append.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentGutterWithSideAndSize WithSide( GutterSide side )
    {
        currentGutter.Side = side;
        Dirty();

        return this;
    }

    private IFluentGutterWithSize WithSize( string value )
    {
        if ( customRules is null )
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
    public IFluentGutterWithSize Is( string value ) => WithSize( value );

    #endregion

    #region Properties

    /// <summary>
    /// For classes that eliminate the margin or padding by setting it to 0.
    /// </summary>
    public IFluentGutterWithSideAndSize Is0 => WithSize( GutterSize.Is0 );

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * .25
    /// </summary>
    public IFluentGutterWithSideAndSize Is1 => WithSize( GutterSize.Is1 );

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * .5
    /// </summary>
    public IFluentGutterWithSideAndSize Is2 => WithSize( GutterSize.Is2 );

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer
    /// </summary>
    public IFluentGutterWithSideAndSize Is3 => WithSize( GutterSize.Is3 );

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * 1.5
    /// </summary>
    public IFluentGutterWithSideAndSize Is4 => WithSize( GutterSize.Is4 );

    /// <summary>
    /// (by default) for classes that set the margin or padding to $spacer * 3
    /// </summary>
    public IFluentGutterWithSideAndSize Is5 => WithSize( GutterSize.Is5 );

    /// <summary>
    /// For classes that set both *-left and *-right.
    /// </summary>
    public IFluentGutterWithSideAndSize OnX => WithSide( GutterSide.X );

    /// <summary>
    /// For classes that set both *-top and *-bottom.
    /// </summary>
    public IFluentGutterWithSideAndSize OnY => WithSide( GutterSide.Y );

    /// <summary>
    /// For classes that set a margin or padding on all 4 sides of the element.
    /// </summary>
    public IFluentGutterWithSideAndSize OnAll => WithSide( GutterSide.All );

    #endregion
}

/// <summary>
/// Gutter builder.
/// </summary>
public static class Gutter
{
    /// <summary>
    /// for classes that eliminate the margin by setting it to 0
    /// </summary>
    public static IFluentGutterWithSideAndSize Is0 => new FluentGutter().Is0;

    /// <summary>
    /// (by default) for classes that set the margin to $spacer * .25
    /// </summary>
    public static IFluentGutterWithSideAndSize Is1 => new FluentGutter().Is1;

    /// <summary>
    /// (by default) for classes that set the margin to $spacer * .5
    /// </summary>
    public static IFluentGutterWithSideAndSize Is2 => new FluentGutter().Is2;

    /// <summary>
    /// (by default) for classes that set the margin to $spacer
    /// </summary>
    public static IFluentGutterWithSideAndSize Is3 => new FluentGutter().Is3;

    /// <summary>
    /// (by default) for classes that set the margin to $spacer * 1.5
    /// </summary>
    public static IFluentGutterWithSideAndSize Is4 => new FluentGutter().Is4;

    /// <summary>
    /// (by default) for classes that set the margin to $spacer * 3
    /// </summary>
    public static IFluentGutterWithSideAndSize Is5 => new FluentGutter().Is5;

    /// <summary>
    /// Add custom margin rule.
    /// </summary>
    /// <param name="value">Custom css classname.</param>
    public static IFluentGutterWithSize Is( string value ) => new FluentGutter().Is( value );
}