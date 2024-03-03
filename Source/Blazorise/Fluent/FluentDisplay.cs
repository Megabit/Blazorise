#region Using directives
using System.Collections.Generic;
using System.Linq;
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Base interface for all fluent display builders.
/// </summary>
public interface IFluentDisplay
{
    /// <summary>
    /// Builds the classnames based on display rules.
    /// </summary>
    /// <param name="classProvider">Currently used class provider.</param>
    /// <returns>List of classnames for the given rules and the class provider.</returns>
    string Class( IClassProvider classProvider );
}

/// <summary>
/// Contains all the allowed display rules.
/// </summary>
public interface IFluentDisplayWithDisplayOnBreakpointWithDirection :
    IFluentDisplay,
    IFluentDisplayOnBreakpoint,
    IFluentDisplayWithDisplay,
    IFluentDisplayOnCondition
{
}

/// <summary>
/// Allowed breakpoints for display rules.
/// </summary>
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

    /// <summary>
    /// Breakpoint on large desktops (extra extra large).
    /// </summary>
    IFluentDisplayWithDisplay OnFull2K { get; }
}

/// <summary>
/// Allowed rules for flex display.
/// </summary>
public interface IFluentDisplayWithDisplayFlexWithDirection :
    IFluentDisplay
{
    /// <summary>
    /// Default value. The flexible items are displayed horizontally, as a row.
    /// </summary>
    IFluentDisplayWithDisplayOnBreakpointWithDirection Row { get; }

    /// <summary>
    /// Same as row, but in reverse order.
    /// </summary>
    IFluentDisplayWithDisplayOnBreakpointWithDirection ReverseRow { get; }

    /// <summary>
    /// The flexible items are displayed vertically, as a column.
    /// </summary>
    IFluentDisplayWithDisplayOnBreakpointWithDirection Column { get; }

    /// <summary>
    /// Same as column, but in reverse order.
    /// </summary>
    IFluentDisplayWithDisplayOnBreakpointWithDirection ReverseColumn { get; }
}

/// <summary>
/// All allowed display rules.
/// </summary>
public interface IFluentDisplayWithDisplay :
    IFluentDisplay
{
    /// <summary>
    /// The element is completely removed.
    /// </summary>
    IFluentDisplayWithDisplayOnBreakpointWithDirection None { get; }

    /// <summary>
    /// Displays an element as a block element. It starts on a new line, and takes up the whole width.
    /// </summary>
    IFluentDisplayWithDisplayOnBreakpointWithDirection Block { get; }

    /// <summary>
    /// Displays an element as an inline element. Any height and width properties will have no effect.
    /// </summary>
    IFluentDisplayWithDisplayOnBreakpointWithDirection Inline { get; }

    /// <summary>
    /// Displays an element as an inline-level block container. The element itself is formatted as an inline element, but you can apply height and width values
    /// </summary>
    IFluentDisplayWithDisplayOnBreakpointWithDirection InlineBlock { get; }

    /// <summary>
    /// Let the element behave like a table element.
    /// </summary>
    IFluentDisplayWithDisplayOnBreakpointWithDirection Table { get; }

    /// <summary>
    /// Let the element behave like a table row element.
    /// </summary>
    IFluentDisplayWithDisplayOnBreakpointWithDirection TableRow { get; }

    /// <summary>
    /// Let the element behave like a table cell element.
    /// </summary>
    IFluentDisplayWithDisplayOnBreakpointWithDirection TableCell { get; }

    /// <summary>
    /// Displays an element as a block-level flex container.
    /// </summary>
    IFluentDisplayWithDisplayFlexWithDirection Flex { get; }

    /// <summary>
    /// Displays an element as an inline-level flex container.
    /// </summary>
    IFluentDisplayWithDisplayFlexWithDirection InlineFlex { get; }
}

/// <summary>
/// Conditions for display rules.
/// </summary>
public interface IFluentDisplayOnCondition :
    IFluentDisplay
{
    /// <summary>
    /// Add a condition rule to the display.
    /// </summary>
    /// <param name="condition">Condition result.</param>
    /// <returns>Next rule reference.</returns>
    IFluentDisplayWithDisplayOnBreakpointWithDirection If( bool condition );
}

/// <summary>
/// Holds the build information for current flex rules.
/// </summary>
public record DisplayDefinition
{
    /// <summary>
    /// Defines the flex breakpoint rule.
    /// </summary>
    public Breakpoint Breakpoint { get; set; }

    /// <summary>
    /// Defines the flex direction rule.
    /// </summary>
    public DisplayDirection Direction { get; set; }

    /// <summary>
    /// If condition is true the rule will will be applied.
    /// </summary>
    public bool? Condition { get; set; }
}

/// <summary>
/// Default implementation of <see cref="IFluentDisplay"/>.
/// </summary>
public class FluentDisplay :
    IFluentDisplay,
    IFluentDisplayWithDisplayOnBreakpointWithDirection,
    IFluentDisplayOnBreakpoint,
    IFluentDisplayWithDisplayFlexWithDirection,
    IFluentDisplayWithDisplay
{
    #region Members

    /// <summary>
    /// Currently used display rules.
    /// </summary>
    private DisplayDefinition currentDisplay;

    /// <summary>
    /// List of all display rules to build.
    /// </summary>
    private readonly Dictionary<DisplayType, List<DisplayDefinition>> rules = new();

    /// <summary>
    /// Indicates if the rules have changed.
    /// </summary>
    private bool dirty = true;

    /// <summary>
    /// Holds the built classnames based on the display rules.
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
                if ( rules.Count( x => x.Key != DisplayType.Always ) > 0 )
                    builder.Append( rules.Select( r => classProvider.Display( r.Key, r.Value.Where( x => x.Condition ?? true ).Select( v => v ) ) ) );
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
    /// Appends the new display rule.
    /// </summary>
    /// <param name="displayType">Display type to append.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentDisplayWithDisplayOnBreakpointWithDirection WithDisplay( DisplayType displayType )
    {
        var columnDefinition = new DisplayDefinition { Breakpoint = Breakpoint.None };

        if ( rules.TryGetValue( displayType, out var rule ) )
            rule.Add( columnDefinition );
        else
            rules.Add( displayType, new() { columnDefinition } );

        currentDisplay = columnDefinition;
        Dirty();

        return this;
    }

    /// <summary>
    /// Appends the new flex rule.
    /// </summary>
    /// <param name="displayType">Display type to append.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentDisplayWithDisplayFlexWithDirection WithFlex( DisplayType displayType )
    {
        var columnDefinition = new DisplayDefinition { Breakpoint = Breakpoint.None };

        if ( rules.TryGetValue( displayType, out var rule ) )
            rule.Add( columnDefinition );
        else
            rules.Add( displayType, new() { columnDefinition } );

        currentDisplay = columnDefinition;
        Dirty();

        return this;
    }

    /// <summary>
    /// Appends the new breakpoint rule.
    /// </summary>
    /// <param name="breakpoint">Breakpoint to append</param>
    /// <returns>Next rule reference.</returns>
    public IFluentDisplayWithDisplay WithBreakpoint( Breakpoint breakpoint )
    {
        currentDisplay.Breakpoint = breakpoint;
        Dirty();

        return this;
    }

    /// <summary>
    /// Sets the display direction rule.
    /// </summary>
    /// <param name="direction">Flex direction to set.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentDisplayWithDisplayOnBreakpointWithDirection WithDirection( DisplayDirection direction )
    {
        currentDisplay.Direction = direction;
        Dirty();

        return this;
    }

    /// <inheritdoc/>
    public IFluentDisplayWithDisplayOnBreakpointWithDirection If( bool condition )
    {
        currentDisplay.Condition = condition;
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
    /// Breakpoint on large desktops (extra extra large).
    /// </summary>
    public IFluentDisplayWithDisplay OnFull2K => WithBreakpoint( Breakpoint.Full2K );

    /// <summary>
    /// Display will not be applied, meaning an element will be visible.
    /// </summary>
    public IFluentDisplayWithDisplayOnBreakpointWithDirection Always { get { return WithDisplay( DisplayType.Always ); } }

    /// <summary>
    /// Hides an element.
    /// </summary>
    public IFluentDisplayWithDisplayOnBreakpointWithDirection None { get { return WithDisplay( DisplayType.None ); } }

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
    /// Let the element behave like a table element.
    /// </summary>
    public IFluentDisplayWithDisplayOnBreakpointWithDirection Table { get { return WithDisplay( DisplayType.Table ); } }

    /// <summary>
    /// Let the element behave like a tr element.
    /// </summary>
    public IFluentDisplayWithDisplayOnBreakpointWithDirection TableRow { get { return WithDisplay( DisplayType.TableRow ); } }

    /// <summary>
    /// Let the element behave like a td element.
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
    /// The element is always present.
    /// </summary>
    public static IFluentDisplayWithDisplayOnBreakpointWithDirection Always { get { return new FluentDisplay().Always; } }

    /// <summary>
    /// The element is completely removed.
    /// </summary>
    public static IFluentDisplayWithDisplayOnBreakpointWithDirection None { get { return new FluentDisplay().None; } }

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
    /// Let the element behave like a table element.
    /// </summary>
    public static IFluentDisplayWithDisplayOnBreakpointWithDirection Table { get { return new FluentDisplay().Table; } }

    /// <summary>
    /// Let the element behave like a tr element.
    /// </summary>
    public static IFluentDisplayWithDisplayOnBreakpointWithDirection TableRow { get { return new FluentDisplay().TableRow; } }

    /// <summary>
    /// Let the element behave like a td element.
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