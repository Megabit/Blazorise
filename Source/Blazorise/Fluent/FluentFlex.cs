#region Using directives
using System.Collections.Generic;
using System.Linq;
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Base interface for all fluent flex builders.
/// </summary>
public interface IFluentFlex
{
    /// <summary>
    /// Builds the classnames based on flex rules.
    /// </summary>
    /// <param name="classProvider">Currently used class provider.</param>
    /// <returns>List of classnames for the given rules and the class provider.</returns>
    string Class( IClassProvider classProvider );
}

/// <summary>
/// Contains all the allowed flex rules.
/// </summary>
public interface IFluentFlexAll :
    IFluentFlex,
    IFluentFlexBreakpoint,
    IFluentFlexDirection,
    IFluentFlexJustifyContent,
    IFluentFlexAlignItems,
    IFluentFlexAlignSelf,
    IFluentFlexAlignContent,
    IFluentFlexWrap,
    IFluentFlexFill,
    IFluentFlexGrowShrink,
    IFluentFlexOrder,
    IFluentFlexCondition
{
}

/// <summary>
/// Allowed breakpoints for flex rules.
/// </summary>
public interface IFluentFlexBreakpoint :
    IFluentFlex
{
    /// <summary>
    /// Valid on all devices. (extra small)
    /// </summary>
    IFluentFlexAll OnMobile { get; }

    /// <summary>
    /// Breakpoint on tablets (small).
    /// </summary>
    IFluentFlexAll OnTablet { get; }

    /// <summary>
    ///  Breakpoint on desktop (medium).
    /// </summary>
    IFluentFlexAll OnDesktop { get; }

    /// <summary>
    /// Breakpoint on widescreen (large).
    /// </summary>
    IFluentFlexAll OnWidescreen { get; }

    /// <summary>
    /// Breakpoint on large desktops (extra large).
    /// </summary>
    IFluentFlexAll OnFullHD { get; }
}

/// <summary>
/// Allowed rules for flex direction.
/// </summary>
public interface IFluentFlexDirection :
    IFluentFlex
{
    /// <summary>
    /// Default value. The flexible items are displayed horizontally, as a row.
    /// </summary>
    IFluentFlexAll Row { get; }

    /// <summary>
    /// Same as row, but in reverse order.
    /// </summary>
    IFluentFlexAll ReverseRow { get; }

    /// <summary>
    /// The flexible items are displayed vertically, as a column.
    /// </summary>
    IFluentFlexAll Column { get; }

    /// <summary>
    /// Same as column, but in reverse order.
    /// </summary>
    IFluentFlexAll ReverseColumn { get; }
}

/// <summary>
/// Rule for starting the flex justify-content.
/// </summary>
public interface IFluentFlexJustifyContent :
    IFluentFlex
{
    /// <summary>
    /// Defines the alignment along the main axis.
    /// </summary>
    IFluentFlexJustifyContentPositions JustifyContent { get; }
}

/// <summary>
/// Defines all the allowed positions for justify-content rule.
/// </summary>
public interface IFluentFlexJustifyContentPositions :
    IFluentFlex
{
    /// <summary>
    /// Items are packed toward the start of the flex-direction.
    /// </summary>
    IFluentFlexAll Start { get; }

    /// <summary>
    /// Items are packed toward the end of the flex-direction.
    /// </summary>
    IFluentFlexAll End { get; }

    /// <summary>
    /// Items are centered along the line.
    /// </summary>
    IFluentFlexAll Center { get; }

    /// <summary>
    /// Items are evenly distributed in the line; first item is on the start line, last item on the end line.
    /// </summary>
    IFluentFlexAll Between { get; }

    /// <summary>
    /// Items are evenly distributed in the line with equal space around them.
    /// Note that visually the spaces aren't equal, since all the items have equal space on both sides.
    /// </summary>
    /// <remarks>
    /// The first item will have one unit of space against the container edge, but two units of space
    /// between the next item because that next item has its own spacing that applies.
    /// </remarks>
    IFluentFlexAll Around { get; }
}

/// <summary>
/// Rule for starting the flex align-items.
/// </summary>
public interface IFluentFlexAlignItems :
    IFluentFlex
{
    /// <summary>
    /// Defines the default behavior for how flex items are laid out along the cross axis on the current line.
    /// </summary>
    IFluentFlexAlignItemsPosition AlignItems { get; }
}

/// <summary>
/// Defines all the allowed positions for align-items rule.
/// </summary>
public interface IFluentFlexAlignItemsPosition :
    IFluentFlex
{
    /// <summary>
    /// Items are placed at the start of the cross axis. The difference between these is subtle,
    /// and is about respecting the flex-direction rules or the writing-mode rules.
    /// </summary>
    IFluentFlexAll Start { get; }

    /// <summary>
    /// Items are placed at the end of the cross axis. The difference again is subtle and is about
    /// respecting flex-direction rules vs. writing-mode rules.
    /// </summary>
    IFluentFlexAll End { get; }

    /// <summary>
    /// Items are centered in the cross-axis.
    /// </summary>
    IFluentFlexAll Center { get; }

    /// <summary>
    /// Items are aligned such as their baselines align.
    /// </summary>
    IFluentFlexAll Baseline { get; }

    /// <summary>
    /// Stretch to fill the container (still respect min-width/max-width).
    /// </summary>
    IFluentFlexAll Stretch { get; }
}

/// <summary>
/// Rule for starting the flex align-self.
/// </summary>
public interface IFluentFlexAlignSelf :
    IFluentFlex
{
    /// <summary>
    /// Allows the default alignment (or the one specified by align-items) to be overridden for individual flex items.
    /// </summary>
    IFluentFlexAlignSelfPosition AlignSelf { get; }
}

/// <summary>
/// Defines all the allowed positions for align-self rule.
/// </summary>
public interface IFluentFlexAlignSelfPosition :
    IFluentFlex
{
    /// <summary>
    /// Equals to the value specified in the align-items property for the flex container (it’s the default value).
    /// </summary>
    IFluentFlexAll Auto { get; }

    /// <summary>
    /// Items are placed at the start of the cross axis. The difference between these is subtle,
    /// and is about respecting the flex-direction rules or the writing-mode rules.
    /// </summary>
    IFluentFlexAll Start { get; }

    /// <summary>
    /// Items are placed at the end of the cross axis. The difference again is subtle and is about
    /// respecting flex-direction rules vs. writing-mode rules.
    /// </summary>
    IFluentFlexAll End { get; }

    /// <summary>
    /// Items are centered in the cross-axis.
    /// </summary>
    IFluentFlexAll Center { get; }

    /// <summary>
    /// Items are aligned such as their baselines align.
    /// </summary>
    IFluentFlexAll Baseline { get; }

    /// <summary>
    /// Stretch to fill the container (still respect min-width/max-width).
    /// </summary>
    IFluentFlexAll Stretch { get; }
}

/// <summary>
/// Rule for starting the flex align-content.
/// </summary>
public interface IFluentFlexAlignContent :
    IFluentFlex
{
    /// <summary>
    /// The align-content property aligns a flex container’s lines within the
    /// flex container when there is extra space in the cross-axis.
    /// </summary>
    IFluentFlexAlignContentPosition AlignContent { get; }
}

/// <summary>
/// Defines all the allowed positions for align-content rule.
/// </summary>
public interface IFluentFlexAlignContentPosition :
    IFluentFlex
{
    /// <summary>
    /// Lines are packed toward the start of the flex container.
    /// </summary>
    /// <remarks>
    /// The cross-start edge of the first line in the flex container is placed flush
    /// with the cross-start edge of the flex container, and each subsequent line
    /// is placed flush with the preceding line.
    /// </remarks>
    IFluentFlexAll Start { get; }

    /// <summary>
    /// Lines are packed toward the end of the flex container.
    /// </summary>
    /// <remarks>
    /// The cross-end edge of the last line is placed flush with the cross-end edge
    /// of the flex container, and each preceding line is placed flush with the subsequent line.
    /// </remarks>
    IFluentFlexAll End { get; }

    /// <summary>
    /// Lines are packed toward the center of the flex container.
    /// </summary>
    /// <remarks>
    /// The lines in the flex container are placed flush with each other and aligned in the center
    /// of the flex container, with equal amounts of space between the cross-start content edge of
    /// the flex container and the first line in the flex container, and between the cross-end
    /// content edge of the flex container and the last line in the flex container.
    /// (If the leftover free-space is negative, the lines will overflow equally in both directions.)
    /// </remarks>
    IFluentFlexAll Center { get; }

    /// <summary>
    /// Lines are evenly distributed in the flex container.
    /// </summary>
    /// <remarks>
    /// If the leftover free-space is negative or there is only a single flex line in the flex container,
    /// this value is identical to flex-start. Otherwise, the cross-start edge of the first line in the flex
    /// container is placed flush with the cross-start content edge of the flex container, the cross-end edge
    /// of the last line in the flex container is placed flush with the cross-end content edge of the flex
    /// container, and the remaining lines in the flex container are distributed so that the spacing between
    /// any two adjacent lines is the same.
    /// </remarks>
    IFluentFlexAll Between { get; }

    /// <summary>
    /// Lines are evenly distributed in the flex container, with half-size spaces on either end.
    /// </summary>
    /// <remarks>
    /// If the leftover free-space is negative this value is identical to center.
    /// Otherwise, the lines in the flex container are distributed such that the spacing between
    /// any two adjacent lines is the same, and the spacing between the first/last lines and the
    /// flex container edges is half the size of the spacing between flex lines.
    /// </remarks>
    IFluentFlexAll Around { get; }

    /// <summary>
    /// Lines stretch to take up the remaining space.
    /// </summary>
    /// <remarks>
    /// If the leftover free-space is negative, this value is identical to flex-start.
    /// Otherwise, the free-space is split equally between all of the lines, increasing their cross size.
    /// </remarks>
    IFluentFlexAll Stretch { get; }
}

/// <summary>
/// Rule for starting the flex grow and shrink.
/// </summary>
public interface IFluentFlexGrowShrink :
    IFluentFlex
{
    /// <summary>
    /// Defines the ability for a flex item to grow if necessary. It accepts a unitless value that serves
    /// as a proportion. It dictates what amount of the available space inside the flex container the item should take up.
    /// </summary>
    IFluentFlexGrowShrinkSize Grow { get; }

    /// <summary>
    /// This defines the ability for a flex item to shrink if necessary.
    /// </summary>
    IFluentFlexGrowShrinkSize Shrink { get; }
}

/// <summary>
/// Defines all the allowed sizes for grow and shrink rule.
/// </summary>
public interface IFluentFlexGrowShrinkSize :
    IFluentFlex
{
    /// <summary>
    /// Element uses a default space.
    /// </summary>
    IFluentFlexAll Is0 { get; }

    /// <summary>
    /// Element uses all available space it can.
    /// </summary>
    IFluentFlexAll Is1 { get; }
}

/// <summary>
/// Rule for starting the flex order.
/// </summary>
public interface IFluentFlexOrder :
    IFluentFlex
{
    /// <summary>
    /// Controls the order in which items appear in the flex container.
    /// </summary>
    IFluentFlexOrderNumber Order { get; }
}

/// <summary>
/// Controls the order in which items appear in the flex container.
/// </summary>
public interface IFluentFlexOrderNumber :
    IFluentFlex
{
    /// <summary>
    /// A default order.
    /// </summary>
    IFluentFlexAll Is0 { get; }

    /// <summary>
    /// An element will be shown as first item.
    /// </summary>
    IFluentFlexAll Is1 { get; }

    /// <summary>
    /// An element will be shown as second item.
    /// </summary>
    IFluentFlexAll Is2 { get; }

    /// <summary>
    /// An element will be shown as third item.
    /// </summary>
    IFluentFlexAll Is3 { get; }

    /// <summary>
    /// An element will be shown as fourth item.
    /// </summary>
    IFluentFlexAll Is4 { get; }

    /// <summary>
    /// An element will be shown as fifth item.
    /// </summary>
    IFluentFlexAll Is5 { get; }

    /// <summary>
    /// An element will be shown as sixth item.
    /// </summary>
    IFluentFlexAll Is6 { get; }

    /// <summary>
    /// An element will be shown as seventh item.
    /// </summary>
    IFluentFlexAll Is7 { get; }

    /// <summary>
    /// An element will be shown as eight item.
    /// </summary>
    IFluentFlexAll Is8 { get; }

    /// <summary>
    /// An element will be shown as ninth item.
    /// </summary>
    IFluentFlexAll Is9 { get; }

    /// <summary>
    /// An element will be shown as tenth item.
    /// </summary>
    IFluentFlexAll Is10 { get; }

    /// <summary>
    /// An element will be shown as eleventh item.
    /// </summary>
    IFluentFlexAll Is11 { get; }

    /// <summary>
    /// An element will be shown as twelvth item.
    /// </summary>
    IFluentFlexAll Is12 { get; }
}

/// <summary>
/// Rule for starting the flex wrap.
/// </summary>
public interface IFluentFlexWrap :
    IFluentFlex
{
    /// <summary>
    /// Flex items will wrap onto multiple lines, from top to bottom.
    /// </summary>
    IFluentFlexAll Wrap { get; }

    /// <summary>
    /// Flex items will wrap onto multiple lines from bottom to top.
    /// </summary>
    IFluentFlexAll ReverseWrap { get; }

    /// <summary>
    /// (default): all flex items will be on one line.
    /// </summary>
    IFluentFlexAll NoWrap { get; }
}

/// <summary>
/// Rule for starting the flex fill.
/// </summary>
public interface IFluentFlexFill :
    IFluentFlex
{
    /// <summary>
    /// Force all child items to be equal widths.
    /// </summary>
    IFluentFlexAll Fill { get; }
}

/// <summary>
/// Conditions for display rules.
/// </summary>
public interface IFluentFlexCondition :
    IFluentFlex
{
    /// <summary>
    /// Add a condition rule to the display.
    /// </summary>
    /// <param name="condition">Condition result.</param>
    /// <returns>Next rule reference.</returns>
    IFluentFlexAll If( bool condition );
}

/// <summary>
/// Holds the build information for current flex rules.
/// </summary>
public record FlexDefinition
{
    /// <summary>
    /// Gets the empty flex definition.
    /// </summary>
    public static readonly FlexDefinition Empty = new();

    /// <summary>
    /// Defines the flex breakpoint rule.
    /// </summary>
    public Breakpoint Breakpoint { get; set; }

    /// <summary>
    /// Defines the flex direction rule.
    /// </summary>
    public FlexDirection Direction { get; set; }

    /// <summary>
    /// Defines the flex justify-content rule.
    /// </summary>
    public FlexJustifyContent JustifyContent { get; set; }

    /// <summary>
    /// Defines the flex align-items rule.
    /// </summary>
    public FlexAlignItems AlignItems { get; set; }

    /// <summary>
    /// Defines the flex align-self rule.
    /// </summary>
    public FlexAlignSelf AlignSelf { get; set; }

    /// <summary>
    /// Defines the flex align-content rule.
    /// </summary>
    public FlexAlignContent AlignContent { get; set; }

    /// <summary>
    /// Defines the flex grow or shrink rule.
    /// </summary>
    public FlexGrowShrink GrowShrink { get; set; }

    /// <summary>
    /// Defines the flex grow or shrink size.
    /// </summary>
    public FlexGrowShrinkSize GrowShrinkSize { get; set; }

    /// <summary>
    /// Defines the flex wrap rule.
    /// </summary>
    public FlexWrap Wrap { get; set; }

    /// <summary>
    /// Defines the flex order rule.
    /// </summary>
    public FlexOrder Order { get; set; }

    /// <summary>
    /// Defines the flex fill rule.
    /// </summary>
    public bool Fill { get; set; }

    /// <summary>
    /// If condition is true the rule will will be applied.
    /// </summary>
    public bool? Condition { get; set; }
}

/// <summary>
/// Default builder implementation of <see cref="IFluentFlex"/>.
/// </summary>
public class FluentFlex :
    IFluentFlex,
    IFluentFlexBreakpoint,
    IFluentFlexDirection,
    IFluentFlexJustifyContent,
    IFluentFlexJustifyContentPositions,
    IFluentFlexAlignItems,
    IFluentFlexAlignItemsPosition,
    IFluentFlexAlignSelf,
    IFluentFlexAlignSelfPosition,
    IFluentFlexAlignContent,
    IFluentFlexAlignContentPosition,
    IFluentFlexWrap,
    IFluentFlexFill,
    IFluentFlexGrowShrink,
    IFluentFlexGrowShrinkSize,
    IFluentFlexOrder,
    IFluentFlexOrderNumber,
    IFluentFlexCondition,
    IFluentFlexAll
{
    #region Members

    /// <summary>
    /// Currently used display type.
    /// </summary>
    private FlexType currentFlexType /*= DisplayType.Flex*/;

    /// <summary>
    /// Currently used flex rules.
    /// </summary>
    private FlexDefinition currentFlexDefinition;

    /// <summary>
    /// List of all flex rules to build.
    /// </summary>
    private Dictionary<FlexType, List<FlexDefinition>> rules;

    /// <summary>
    /// Indicates if the rules have changed.
    /// </summary>
    private bool dirty = true;

    /// <summary>
    /// Holds the built classnames based on the flex rules.
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
                if ( rules != null && rules.Count > 0 )
                {
                    builder.Append( rules.Select( r => classProvider.Flex( r.Key, r.Value.Where( x => x.Condition ?? true ).Select( v => v ) ) ) );
                }
                else if ( currentFlexDefinition != null && currentFlexDefinition != FlexDefinition.Empty && ( currentFlexDefinition.Condition ?? true ) )
                {
                    builder.Append( classProvider.Flex( currentFlexDefinition ) );
                }
                else if ( currentFlexType != FlexType.Default )
                {
                    // In some cases we will have no definitions but flex type can still be defined.
                    // We need to also cover those situations.
                    builder.Append( classProvider.Flex( currentFlexType ) );
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
    /// Starts the new flex type rule.
    /// </summary>
    /// <param name="flexType">Flex type to start.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentFlexAll WithFlexType( FlexType flexType )
    {
        currentFlexType = flexType;
        Dirty();

        return this;
    }

    /// <summary>
    /// Gets the current flex definition.
    /// </summary>
    /// <returns>Current definition or new if none was found.</returns>
    private FlexDefinition GetDefinition()
    {
        if ( currentFlexDefinition is null )
            currentFlexDefinition = CreateDefinition();

        return currentFlexDefinition;
    }

    /// <summary>
    /// Creates the new flex definition.
    /// </summary>
    /// <returns>The newly created flex definition.</returns>
    private FlexDefinition CreateDefinition()
    {
        rules ??= new();

        var flexDefinition = new FlexDefinition();

        if ( rules.TryGetValue( currentFlexType, out var rule ) )
            rule.Add( flexDefinition );
        else
            rules.Add( currentFlexType, new() { flexDefinition } );

        return flexDefinition;
    }

    /// <summary>
    /// Applies the breakpoint rule to the current flex definition.
    /// </summary>
    /// <param name="breakpoint">Breakpoint to be apply.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentFlexAll WithBreakpoint( Breakpoint breakpoint )
    {
        currentFlexDefinition = GetDefinition();
        currentFlexDefinition.Breakpoint = breakpoint;
        Dirty();

        return this;
    }

    /// <summary>
    /// Applies the direction rule to the current flex definition.
    /// </summary>
    /// <param name="direction">Direction to be apply.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentFlexAll WithDirection( FlexDirection direction )
    {
        currentFlexDefinition = CreateDefinition();
        currentFlexDefinition.Direction = direction;
        Dirty();

        return this;
    }

    /// <summary>
    /// Creates the new justify-content rule on the current flex definition.
    /// </summary>
    /// <returns>Next rule reference.</returns>
    public IFluentFlexJustifyContentPositions WithJustifyContent()
    {
        currentFlexDefinition = CreateDefinition();
        currentFlexDefinition.JustifyContent = FlexJustifyContent.Default;
        Dirty();

        return this;
    }

    /// <summary>
    /// Applies the justify-content rule to the current flex definition.
    /// </summary>
    /// <param name="justifyContent">Justify-content to be apply.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentFlexAll WithJustifyContent( FlexJustifyContent justifyContent )
    {
        currentFlexDefinition = GetDefinition();
        currentFlexDefinition.JustifyContent = justifyContent;
        Dirty();

        return this;
    }

    /// <summary>
    /// Creates the new align-items rule on the current flex definition.
    /// </summary>
    /// <returns>Next rule reference.</returns>
    public IFluentFlexAlignItemsPosition WithAlignItems()
    {
        currentFlexDefinition = CreateDefinition();
        currentFlexDefinition.AlignItems = FlexAlignItems.Default;
        Dirty();

        return this;
    }

    /// <summary>
    /// Applies the align-items rule to the current flex definition.
    /// </summary>
    /// <param name="alignItems">Align-items to be apply.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentFlexAll WithAlignItems( FlexAlignItems alignItems )
    {
        currentFlexDefinition = GetDefinition();
        currentFlexDefinition.AlignItems = alignItems;
        Dirty();

        return this;
    }

    /// <summary>
    /// Creates the new align-self rule on the current flex definition.
    /// </summary>
    /// <returns>Next rule reference.</returns>
    public IFluentFlexAlignSelfPosition WithAlignSelf()
    {
        currentFlexDefinition = CreateDefinition();
        currentFlexDefinition.AlignSelf = FlexAlignSelf.Default;
        Dirty();

        return this;
    }

    /// <summary>
    /// Applies the align-self rule to the current flex definition.
    /// </summary>
    /// <param name="alignSelf">Align-self to be apply.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentFlexAll WithAlignSelf( FlexAlignSelf alignSelf )
    {
        currentFlexDefinition = GetDefinition();
        currentFlexDefinition.AlignSelf = alignSelf;
        Dirty();

        return this;
    }

    /// <summary>
    /// Creates the new align-content rule on the current flex definition.
    /// </summary>
    /// <returns>Next rule reference.</returns>
    public IFluentFlexAlignContentPosition WithAlignContent()
    {
        currentFlexDefinition = CreateDefinition();
        currentFlexDefinition.AlignContent = FlexAlignContent.Default;
        Dirty();

        return this;
    }

    /// <summary>
    /// Applies the align-content rule to the current flex definition.
    /// </summary>
    /// <param name="alignContent">Align-content to be apply.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentFlexAll WithAlignContent( FlexAlignContent alignContent )
    {
        currentFlexDefinition = GetDefinition();
        currentFlexDefinition.AlignContent = alignContent;
        Dirty();

        return this;
    }

    /// <summary>
    /// Creates the new grow or shrink rule on the current flex definition.
    /// </summary>
    /// <returns>Next rule reference.</returns>
    public IFluentFlexGrowShrinkSize WithGrowShrink( FlexGrowShrink growShrink )
    {
        currentFlexDefinition = CreateDefinition();
        currentFlexDefinition.GrowShrink = growShrink;
        Dirty();

        return this;
    }

    /// <summary>
    /// Applies the grow or shrink size rule to the current flex definition.
    /// </summary>
    /// <param name="growShrinkSize">Grow or shrink size to be apply.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentFlexAll WithGrowShrinkSize( FlexGrowShrinkSize growShrinkSize )
    {
        currentFlexDefinition = GetDefinition();
        currentFlexDefinition.GrowShrinkSize = growShrinkSize;
        Dirty();

        return this;
    }

    /// <summary>
    /// Creates the new wrap rule on the current flex definition.
    /// </summary>
    /// <returns>Next rule reference.</returns>
    public IFluentFlexAll WithWrap( FlexWrap wrap )
    {
        currentFlexDefinition = CreateDefinition();
        currentFlexDefinition.Wrap = wrap;
        Dirty();

        return this;
    }

    /// <summary>
    /// Creates the new order rule on the current flex definition.
    /// </summary>
    /// <returns>Next rule reference.</returns>
    public IFluentFlexOrderNumber WithOrder()
    {
        currentFlexDefinition = CreateDefinition();
        currentFlexDefinition.Order = FlexOrder.Default;
        Dirty();

        return this;
    }

    /// <summary>
    /// Applies the order rule to the current flex definition.
    /// </summary>
    /// <param name="order">Order to be apply.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentFlexAll WithOrder( FlexOrder order )
    {
        currentFlexDefinition = GetDefinition();
        currentFlexDefinition.Order = order;
        Dirty();

        return this;
    }

    /// <summary>
    /// Creates the new fill rule on the current flex definition.
    /// </summary>
    /// <returns>Next rule reference.</returns>
    public IFluentFlexAll WithFill()
    {
        currentFlexDefinition = CreateDefinition();
        currentFlexDefinition.Fill = true;
        Dirty();

        return this;
    }

    /// <inheritdoc/>
    public IFluentFlexAll If( bool condition )
    {
        currentFlexDefinition = GetDefinition();
        currentFlexDefinition.Condition = condition;
        Dirty();

        return this;
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public IFluentFlexAll OnMobile => WithBreakpoint( Breakpoint.Mobile );

    /// <inheritdoc/>
    public IFluentFlexAll OnTablet => WithBreakpoint( Breakpoint.Tablet );

    /// <inheritdoc/>
    public IFluentFlexAll OnDesktop => WithBreakpoint( Breakpoint.Desktop );

    /// <inheritdoc/>
    public IFluentFlexAll OnWidescreen => WithBreakpoint( Breakpoint.Widescreen );

    /// <inheritdoc/>
    public IFluentFlexAll OnFullHD => WithBreakpoint( Breakpoint.FullHD );

    /// <inheritdoc/>
    public IFluentFlexAll Row => WithDirection( FlexDirection.Row );

    /// <inheritdoc/>
    public IFluentFlexAll ReverseRow => WithDirection( FlexDirection.ReverseRow );

    /// <inheritdoc/>
    public IFluentFlexAll Column => WithDirection( FlexDirection.Column );

    /// <inheritdoc/>
    public IFluentFlexAll ReverseColumn => WithDirection( FlexDirection.ReverseColumn );

    /// <inheritdoc/>
    public IFluentFlexAll Wrap => WithWrap( FlexWrap.Wrap );

    /// <inheritdoc/>
    public IFluentFlexAll ReverseWrap => WithWrap( FlexWrap.ReverseWrap );

    /// <inheritdoc/>
    public IFluentFlexAll NoWrap => WithWrap( FlexWrap.NoWrap );

    /// <inheritdoc/>
    public IFluentFlexJustifyContentPositions JustifyContent => WithJustifyContent();

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexJustifyContentPositions.Start => WithJustifyContent( FlexJustifyContent.Start );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexJustifyContentPositions.End => WithJustifyContent( FlexJustifyContent.End );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexJustifyContentPositions.Center => WithJustifyContent( FlexJustifyContent.Center );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexJustifyContentPositions.Between => WithJustifyContent( FlexJustifyContent.Between );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexJustifyContentPositions.Around => WithJustifyContent( FlexJustifyContent.Around );

    /// <inheritdoc/>
    IFluentFlexAlignItemsPosition IFluentFlexAlignItems.AlignItems => WithAlignItems();

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexAlignItemsPosition.Start => WithAlignItems( FlexAlignItems.Start );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexAlignItemsPosition.End => WithAlignItems( FlexAlignItems.End );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexAlignItemsPosition.Center => WithAlignItems( FlexAlignItems.Center );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexAlignItemsPosition.Baseline => WithAlignItems( FlexAlignItems.Baseline );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexAlignItemsPosition.Stretch => WithAlignItems( FlexAlignItems.Stretch );

    /// <inheritdoc/>
    IFluentFlexAlignSelfPosition IFluentFlexAlignSelf.AlignSelf => WithAlignSelf();

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexAlignSelfPosition.Auto => WithAlignSelf( FlexAlignSelf.Auto );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexAlignSelfPosition.Start => WithAlignSelf( FlexAlignSelf.Start );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexAlignSelfPosition.End => WithAlignSelf( FlexAlignSelf.End );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexAlignSelfPosition.Center => WithAlignSelf( FlexAlignSelf.Center );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexAlignSelfPosition.Baseline => WithAlignSelf( FlexAlignSelf.Baseline );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexAlignSelfPosition.Stretch => WithAlignSelf( FlexAlignSelf.Stretch );

    /// <inheritdoc/>
    IFluentFlexAlignContentPosition IFluentFlexAlignContent.AlignContent => WithAlignContent();

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexAlignContentPosition.Start => WithAlignContent( FlexAlignContent.Start );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexAlignContentPosition.End => WithAlignContent( FlexAlignContent.End );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexAlignContentPosition.Center => WithAlignContent( FlexAlignContent.Center );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexAlignContentPosition.Between => WithAlignContent( FlexAlignContent.Between );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexAlignContentPosition.Around => WithAlignContent( FlexAlignContent.Around );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexAlignContentPosition.Stretch => WithAlignContent( FlexAlignContent.Stretch );

    /// <inheritdoc/>
    IFluentFlexGrowShrinkSize IFluentFlexGrowShrink.Grow => WithGrowShrink( FlexGrowShrink.Grow );

    /// <inheritdoc/>
    IFluentFlexGrowShrinkSize IFluentFlexGrowShrink.Shrink => WithGrowShrink( FlexGrowShrink.Shrink );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexGrowShrinkSize.Is0 => WithGrowShrinkSize( FlexGrowShrinkSize.Is0 );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexGrowShrinkSize.Is1 => WithGrowShrinkSize( FlexGrowShrinkSize.Is1 );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexFill.Fill => WithFill();

    /// <inheritdoc/>
    IFluentFlexOrderNumber IFluentFlexOrder.Order => WithOrder();

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexOrderNumber.Is0 => WithOrder( FlexOrder.Is0 );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexOrderNumber.Is1 => WithOrder( FlexOrder.Is1 );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexOrderNumber.Is2 => WithOrder( FlexOrder.Is2 );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexOrderNumber.Is3 => WithOrder( FlexOrder.Is3 );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexOrderNumber.Is4 => WithOrder( FlexOrder.Is4 );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexOrderNumber.Is5 => WithOrder( FlexOrder.Is5 );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexOrderNumber.Is6 => WithOrder( FlexOrder.Is6 );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexOrderNumber.Is7 => WithOrder( FlexOrder.Is7 );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexOrderNumber.Is8 => WithOrder( FlexOrder.Is8 );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexOrderNumber.Is9 => WithOrder( FlexOrder.Is9 );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexOrderNumber.Is10 => WithOrder( FlexOrder.Is10 );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexOrderNumber.Is11 => WithOrder( FlexOrder.Is11 );

    /// <inheritdoc/>
    IFluentFlexAll IFluentFlexOrderNumber.Is12 => WithOrder( FlexOrder.Is12 );

    #endregion
}

/// <summary>
/// Set of flex rules to start the build process.
/// </summary>
public static class Flex
{
    /// <summary>
    /// Explicitly starts the new set of rules for flex display.
    /// </summary>
    /// <remarks>
    /// By default all rules use "flex" type but in case someone wants to define it explicitly
    /// I have added this option. It is named as "_" because "Flex" cannot be used(Compiler Error CS0542).
    /// </remarks>
    public static IFluentFlexAll _ => new FluentFlex().WithFlexType( FlexType.Flex );

    /// <summary>
    /// Starts the new set of rules for inline-flex display.
    /// </summary>
    public static IFluentFlexAll InlineFlex => new FluentFlex().WithFlexType( FlexType.InlineFlex );

    /// <summary>
    /// Default value. The flexible items are displayed horizontally, as a row.
    /// </summary>
    public static IFluentFlexAll Row => new FluentFlex().WithFlexType( FlexType.Flex ).Row;

    /// <summary>
    /// Same as row, but in reverse order.
    /// </summary>
    public static IFluentFlexAll ReverseRow => new FluentFlex().WithFlexType( FlexType.Flex ).ReverseRow;

    /// <summary>
    /// The flexible items are displayed vertically, as a column.
    /// </summary>
    public static IFluentFlexAll Column => new FluentFlex().WithFlexType( FlexType.Flex ).Column;

    /// <summary>
    /// Same as column, but in reverse order.
    /// </summary>
    public static IFluentFlexAll ReverseColumn => new FluentFlex().WithFlexType( FlexType.Flex ).ReverseColumn;

    /// <summary>
    /// Defines the alignment along the main axis.
    /// </summary>
    public static IFluentFlexJustifyContentPositions JustifyContent => new FluentFlex().WithFlexType( FlexType.Flex ).JustifyContent;

    /// <summary>
    /// Defines the default behavior for how flex items are laid out along the cross axis on the current line.
    /// </summary>
    public static IFluentFlexAlignItemsPosition AlignItems => new FluentFlex().WithFlexType( FlexType.Flex ).AlignItems;

    /// <summary>
    /// Allows the default alignment (or the one specified by align-items) to be overridden for individual flex items.
    /// </summary>
    public static IFluentFlexAlignSelfPosition AlignSelf => new FluentFlex().WithAlignSelf();

    /// <summary>
    /// The align-content property aligns a flex container’s lines within the
    /// flex container when there is extra space in the cross-axis.
    /// </summary>
    public static IFluentFlexAlignContentPosition AlignContent => new FluentFlex().WithFlexType( FlexType.Flex ).AlignContent;

    /// <summary>
    /// Defines the ability for a flex item to grow if necessary. It accepts a unitless value that serves
    /// as a proportion. It dictates what amount of the available space inside the flex container the item should take up.
    /// </summary>
    public static IFluentFlexGrowShrinkSize Grow => new FluentFlex().WithGrowShrink( FlexGrowShrink.Grow );

    /// <summary>
    /// This defines the ability for a flex item to shrink if necessary.
    /// </summary>
    public static IFluentFlexGrowShrinkSize Shrink => new FluentFlex().WithGrowShrink( FlexGrowShrink.Shrink );

    /// <summary>
    /// Controls the order in which items appear in the flex container.
    /// </summary>
    public static IFluentFlexOrderNumber Order => new FluentFlex().WithOrder();

    /// <summary>
    /// Flex items will wrap onto multiple lines, from top to bottom.
    /// </summary>
    public static IFluentFlexAll Wrap => new FluentFlex().WithFlexType( FlexType.Flex ).Wrap;

    /// <summary>
    /// Flex items will wrap onto multiple lines from bottom to top.
    /// </summary>
    public static IFluentFlexAll ReverseWrap => new FluentFlex().WithFlexType( FlexType.Flex ).ReverseWrap;

    /// <summary>
    /// All flex items will be on one line.
    /// </summary>
    public static IFluentFlexAll NoWrap => new FluentFlex().WithFlexType( FlexType.Flex ).NoWrap;

    /// <summary>
    /// Force all child items to be equal widths.
    /// </summary>
    public static IFluentFlexAll Fill => new FluentFlex().WithFill();
}