#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Base interface for all fluent sizing builders.
/// </summary>
public interface IFluentSizing
{
    /// <summary>
    /// Builds the classnames based on sizing rules.
    /// </summary>
    /// <param name="classProvider">Currently used class provider.</param>
    /// <returns>List of classnames for the given rules and the class provider.</returns>
    string Class( IClassProvider classProvider );
}

/// <summary>
/// Contains all the allowed sizing rules.
/// </summary>
public interface IFluentSizingAll :
    IFluentSizing,
    IFluentSizingSize,
    IFluentSizingMinMaxViewport,
    IFluentSizingViewport
{
}

/// <summary>
/// Allowed rules for sizing rule sizes.
/// </summary>
public interface IFluentSizingSize :
    IFluentSizing
{
    /// <summary>
    /// An element will occupy 25% of its parent space.
    /// </summary>
    IFluentSizing Is25 { get; }

    /// <summary>
    /// An element will occupy 50% of its parent space.
    /// </summary>
    IFluentSizing Is50 { get; }

    /// <summary>
    /// An element will occupy 75% of its parent space.
    /// </summary>
    IFluentSizing Is75 { get; }

    /// <summary>
    /// An element will occupy 100% of its parent space.
    /// </summary>
    IFluentSizingMinMaxViewport Is100 { get; }

    /// <summary>
    /// The browser calculates the size.
    /// </summary>
    IFluentSizing Auto { get; }
}

/// <summary>
/// Contains the min, max and viewport rules.
/// </summary>
public interface IFluentSizingMinMaxViewport :
    IFluentSizing,
    IFluentSizingMin,
    IFluentSizingMax,
    IFluentSizingViewport
{
}

/// <summary>
/// Contains the min rule.
/// </summary>
public interface IFluentSizingMin :
    IFluentSizing
{
    /// <summary>
    /// Size will be defined for the min attribute(s) of the element style.
    /// </summary>
    IFluentSizingViewport Min { get; }
}

/// <summary>
/// Contains the viewport rule.
/// </summary>
public interface IFluentSizingViewport :
    IFluentSizing
{
    /// <summary>
    /// Size will be defined for the viewport.
    /// </summary>
    IFluentSizing Viewport { get; }
}

/// <summary>
/// Contains the max rule.
/// </summary>
public interface IFluentSizingMax :
    IFluentSizing
{
    /// <summary>
    /// Size will be defined for the max attribute(s) of the element style.
    /// </summary>
    IFluentSizing Max { get; }
}

/// <summary>
/// Holds the build information for current sizing rules.
/// </summary>
public record SizingDefinition
{
    /// <summary>
    /// Size will be defined for the min attribute(s) of the element style.
    /// </summary>
    public bool IsMin { get; set; }

    /// <summary>
    /// Size will be defined for the max attribute(s) of the element style.
    /// </summary>
    public bool IsMax { get; set; }

    /// <summary>
    /// Size will be defined for the viewport.
    /// </summary>
    public bool IsViewport { get; set; }
}

/// <summary>
/// Default builder implementation of <see cref="IFluentSizing"/>.
/// </summary>
public class FluentSizing :
    IFluentSizing,
    IFluentSizingSize,
    IFluentSizingMin,
    IFluentSizingViewport,
    IFluentSizingMax,
    IFluentSizingMinMaxViewport,
    IFluentSizingAll
{

    #region Members

    /// <summary>
    /// Currently used sizing type.
    /// </summary>
    private SizingType sizingType;

    /// <summary>
    /// Currently used sizing size.
    /// </summary>
    private SizingSize currentSizingSize;

    /// <summary>
    /// Currently used sizing rules.
    /// </summary>
    private SizingDefinition currentSizingDefinition;

    /// <summary>
    /// Indicates if the rules have changed.
    /// </summary>
    private bool dirty = true;

    /// <summary>
    /// Holds the built classnames based on the sizing rules.
    /// </summary>
    private string classNames;

    #endregion

    #region Constructors

    /// <summary>
    /// A default <see cref="FluentSizing"/> constructor.
    /// </summary>
    /// <param name="sizingType">Sizing builder type.</param>
    public FluentSizing( SizingType sizingType )
    {
        this.sizingType = sizingType;
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public string Class( IClassProvider classProvider )
    {
        if ( dirty )
        {
            void BuildClasses( ClassBuilder builder )
            {
                if ( currentSizingSize != SizingSize.Default && currentSizingDefinition is not null )
                {
                    builder.Append( classProvider.Sizing( sizingType, currentSizingSize, currentSizingDefinition ) );
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
    /// Starts the new sizing rule.
    /// </summary>
    /// <param name="sizingSize">Size of the element.</param>
    /// <returns>Next rule reference.</returns>returns>
    public IFluentSizingAll WithSize( SizingSize sizingSize )
    {
        currentSizingSize = sizingSize;
        currentSizingDefinition = new();
        Dirty();

        return this;
    }

    /// <summary>
    /// Sets the min rule for the current definition.
    /// </summary>
    /// <returns>Next rule reference.</returns>
    public IFluentSizingViewport WithMin()
    {
        currentSizingDefinition.IsMin = true;
        Dirty();

        return this;
    }

    /// <summary>
    /// Sets the max rule for the current definition.
    /// </summary>
    /// <returns>Next rule reference.</returns>
    public IFluentSizing WithMax()
    {
        currentSizingDefinition.IsMax = true;
        Dirty();

        return this;
    }

    /// <summary>
    /// Sets the viewport rule for the current definition.
    /// </summary>
    /// <returns>Next rule reference.</returns>
    public IFluentSizing WithViewport()
    {
        currentSizingDefinition.IsViewport = true;
        Dirty();

        return this;
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    IFluentSizing IFluentSizingSize.Is25 => WithSize( SizingSize.Is25 );

    /// <inheritdoc/>
    IFluentSizing IFluentSizingSize.Is50 => WithSize( SizingSize.Is50 );

    /// <inheritdoc/>
    IFluentSizing IFluentSizingSize.Is75 => WithSize( SizingSize.Is75 );

    /// <inheritdoc/>
    IFluentSizingMinMaxViewport IFluentSizingSize.Is100 => WithSize( SizingSize.Is100 );

    /// <inheritdoc/>
    IFluentSizing IFluentSizingSize.Auto => WithSize( SizingSize.Auto );

    /// <inheritdoc/>
    IFluentSizingViewport IFluentSizingMin.Min => WithMin();

    /// <inheritdoc/>
    IFluentSizing IFluentSizingMax.Max => WithMax();

    /// <inheritdoc/>
    IFluentSizing IFluentSizingViewport.Viewport => WithViewport();

    #endregion
}

/// <summary>
/// Set of width sizing rules to start the build process.
/// </summary>
public static class Width
{
    /// <summary>
    /// An element will occupy 25% of its parent space.
    /// </summary>
    public static IFluentSizing Is25 => new FluentSizing( SizingType.Width ).WithSize( SizingSize.Is25 );

    /// <summary>
    /// An element will occupy 50% of its parent space.
    /// </summary>
    public static IFluentSizing Is50 => new FluentSizing( SizingType.Width ).WithSize( SizingSize.Is50 );

    /// <summary>
    /// An element will occupy 75% of its parent space.
    /// </summary>
    public static IFluentSizing Is75 => new FluentSizing( SizingType.Width ).WithSize( SizingSize.Is75 );

    /// <summary>
    /// An element will occupy 100% of its parent space.
    /// </summary>
    public static IFluentSizingMinMaxViewport Is100 => new FluentSizing( SizingType.Width ).WithSize( SizingSize.Is100 );

    /// <summary>
    /// The browser calculates the size.
    /// </summary>
    public static IFluentSizing Auto => new FluentSizing( SizingType.Width ).WithSize( SizingSize.Auto );

    /// <summary>
    /// Defines the maximum allowed element width. Shorthand for "Width.Is100.Max".
    /// </summary>
    public static IFluentSizing Max100 => new FluentSizing( SizingType.Width ).WithSize( SizingSize.Is100 ).Max;
}

/// <summary>
/// Set of height sizing rules to start the build process.
/// </summary>
public static class Height
{
    /// <summary>
    /// An element will occupy 25% of its parent space.
    /// </summary>
    public static IFluentSizing Is25 => new FluentSizing( SizingType.Height ).WithSize( SizingSize.Is25 );

    /// <summary>
    /// An element will occupy 50% of its parent space.
    /// </summary>
    public static IFluentSizing Is50 => new FluentSizing( SizingType.Height ).WithSize( SizingSize.Is50 );

    /// <summary>
    /// An element will occupy 75% of its parent space.
    /// </summary>
    public static IFluentSizing Is75 => new FluentSizing( SizingType.Height ).WithSize( SizingSize.Is75 );

    /// <summary>
    /// An element will occupy 100% of its parent space.
    /// </summary>
    public static IFluentSizingMinMaxViewport Is100 => new FluentSizing( SizingType.Height ).WithSize( SizingSize.Is100 );

    /// <summary>
    /// The browser calculates the size.
    /// </summary>
    public static IFluentSizing Auto => new FluentSizing( SizingType.Height ).WithSize( SizingSize.Auto );

    /// <summary>
    /// Defines the maximum allowed element height. Shorthand for "Height.Is100.Max".
    /// </summary>
    public static IFluentSizing Max100 => new FluentSizing( SizingType.Height ).WithSize( SizingSize.Is100 ).Max;
}