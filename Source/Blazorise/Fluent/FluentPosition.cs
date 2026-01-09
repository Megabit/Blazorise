#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Base interface for all fluent position builders.
/// </summary>
public interface IFluentPosition : IFluentUtilityTarget<IFluentPosition>
{
    /// <summary>
    /// Builds the classnames based on position rules.
    /// </summary>
    /// <param name="classProvider">Currently used class provider.</param>
    /// <returns>List of classnames for the given rules and the class provider.</returns>
    string Class( IClassProvider classProvider );
}

/// <summary>
/// Type of positions allowed for the fluent position builder.
/// </summary>
public interface IFluentPositionType :
    IFluentPosition
{
    /// <summary>
    /// An element is not positioned in any special way; it is always positioned according to the normal flow of the page.
    /// </summary>
    IFluentPositionWithEdgeTypeAndTranslateType Static { get; }

    /// <summary>
    /// An element is positioned relative to its normal position.
    /// </summary>
    IFluentPositionWithEdgeTypeAndTranslateType Relative { get; }

    /// <summary>
    /// An element is positioned relative to the nearest positioned ancestor (instead of positioned relative to the
    /// viewport, like fixed).
    /// </summary>
    IFluentPositionWithEdgeTypeAndTranslateType Absolute { get; }

    /// <summary>
    /// An element is positioned relative to the viewport, which means it always stays in the
    /// same place even if the page is scrolled. The top, right, bottom, and left properties are used to position the element.
    /// </summary>
    IFluentPositionWithEdgeTypeAndTranslateType Fixed { get; }

    /// <summary>
    /// An element is positioned based on the user's scroll position.
    /// </summary>
    IFluentPositionWithEdgeTypeAndTranslateType Sticky { get; }
}

/// <summary>
/// Defines the position of the element relative to its side.
/// </summary>
public interface IFluentPositionEdgeType :
    IFluentPosition
{
    /// <summary>
    /// The top property affects the vertical position of a positioned element.
    /// </summary>
    IFluentPositionEdgeOffset Top { get; }

    /// <summary>
    /// The left property affects the horizontal position of a positioned element.
    /// </summary>
    IFluentPositionEdgeOffset Start { get; }

    /// <summary>
    /// The top property affects the vertical position of a positioned element.
    /// </summary>
    IFluentPositionEdgeOffset Bottom { get; }

    /// <summary>
    /// The right property affects the horizontal position of a positioned element.
    /// </summary>
    IFluentPositionEdgeOffset End { get; }
}

/// <summary>
/// Defines the offset, in percentages, relative to its edge.
/// </summary>
public interface IFluentPositionEdgeOffset :
    IFluentPosition,
    IFluentPositionTranslate
{
    /// <summary>
    /// For 0 edge position.
    /// </summary>
    IFluentPositionWithAll Is0 { get; }

    /// <summary>
    /// For 50% edge position.
    /// </summary>
    IFluentPositionWithAll Is50 { get; }

    /// <summary>
    /// For 100% edge position.
    /// </summary>
    IFluentPositionWithAll Is100 { get; }
}

/// <summary>
/// Starts the translation rules.
/// </summary>
public interface IFluentPositionTranslate :
    IFluentPosition
{
    /// <summary>
    /// Translation rule to start.
    /// </summary>
    IFluentPositionTranslateType Translate { get; }
}

/// <summary>
/// Defines the types of element translations based on its center.
/// </summary>
public interface IFluentPositionTranslateType :
    IFluentPosition
{
    /// <summary>
    /// Translate on both X and Y coordinates.
    /// </summary>
    IFluentPositionWithAll Middle { get; }

    /// <summary>
    /// Translate on X coordinate.
    /// </summary>
    IFluentPositionWithAll MiddleX { get; }

    /// <summary>
    /// Translate on Y coordinate.
    /// </summary>
    IFluentPositionWithAll MiddleY { get; }
}

/// <summary>
/// Set of rules for edges and translations.
/// </summary>
public interface IFluentPositionWithEdgeTypeAndTranslateType :
    IFluentPosition,
    IFluentPositionEdgeType,
    IFluentPositionTranslate
{
}

/// <summary>
/// Combination of all position rules.
/// </summary>
public interface IFluentPositionWithAll :
    IFluentPosition,
    IFluentPositionType,
    IFluentPositionEdgeType,
    IFluentPositionEdgeOffset,
    IFluentPositionTranslate,
    IFluentPositionWithEdgeTypeAndTranslateType
{
}

/// <summary>
/// Default implementation of fluent position builder.
/// </summary>
public class FluentPosition :
    IFluentPosition,
    IFluentPositionType,
    IFluentPositionEdgeType,
    IFluentPositionEdgeOffset,
    IFluentPositionTranslate,
    IFluentPositionTranslateType,
    IFluentPositionWithAll,
    IFluentPositionWithEdgeTypeAndTranslateType,
    IUtilityTargeted
{
    #region Members

    /// <summary>
    /// Holds the additions information for the current position rules.
    /// </summary>
    private class PositionEdgeDefinition
    {
        public int EdgeOffset { get; set; }
    }

    /// <summary>
    /// Holds the type of position for the current element. Only one can be applied by the element.
    /// </summary>
    private PositionType positionType;

    /// <summary>
    /// Currently used edge rules.
    /// </summary>
    private PositionEdgeDefinition currentPositionEdgeDefinition;

    /// <summary>
    /// List of all position edge rules to build.
    /// </summary>
    private Dictionary<PositionEdgeType, PositionEdgeDefinition> edgeRules;

    /// <summary>
    /// Holds the type of translation for the current element.
    /// </summary>
    private PositionTranslateType translateType;

    /// <summary>
    /// Indicates if the rules have changed.
    /// </summary>
    private bool dirty = true;

    /// <summary>
    /// Holds the built classnames bases on the position rules.
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
                builder.Append( classProvider.Position( positionType, edgeRules?.Select( x => (x.Key, x.Value.EdgeOffset) ), translateType ) );
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

    private IFluentPosition WithUtilityTarget( UtilityTarget target )
    {
        UtilityTarget = target;
        return this;
    }

    /// <summary>
    /// Starts the new position rule.
    /// </summary>
    /// <param name="positionType">Position type to be applied.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentPositionWithAll WithPosition( PositionType positionType )
    {
        this.positionType = positionType;
        Dirty();

        return this;
    }

    /// <summary>
    /// Starts the new edge rule.
    /// </summary>
    /// <param name="edgeType">Edge side to be applied.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentPositionWithAll WithEdge( PositionEdgeType edgeType )
    {
        edgeRules ??= new();

        var positionEdgeDefinition = new PositionEdgeDefinition();

        if ( edgeRules.TryGetValue( edgeType, out var rule ) )
            rule = positionEdgeDefinition;
        else
            edgeRules.Add( edgeType, positionEdgeDefinition );

        currentPositionEdgeDefinition = positionEdgeDefinition;
        Dirty();

        return this;
    }

    /// <summary>
    /// Starts the new edge offset.
    /// </summary>
    /// <param name="offset">Edge offset to be applied.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentPositionWithAll WithEdgeOffset( int offset )
    {
        currentPositionEdgeDefinition.EdgeOffset = offset;
        Dirty();

        return this;
    }

    /// <summary>
    /// Starts the new translate rule.
    /// </summary>
    /// <param name="translateType">Translate type to be applied.</param>
    /// <returns>Next rule reference.</returns>
    public IFluentPositionWithAll WithTranslate( PositionTranslateType translateType )
    {
        this.translateType = translateType;
        Dirty();

        return this;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the utility target override.
    /// </summary>
    public UtilityTarget? UtilityTarget { get; set; }

    /// <summary>
    /// Targets the utility output to the component element.
    /// </summary>
    public IFluentPosition OnSelf => WithUtilityTarget( global::Blazorise.UtilityTarget.Self );

    /// <summary>
    /// Targets the utility output to a wrapper element.
    /// </summary>
    public IFluentPosition OnWrapper => WithUtilityTarget( global::Blazorise.UtilityTarget.Wrapper );

    /// <inheritdoc/>
    public IFluentPositionWithEdgeTypeAndTranslateType Static => WithPosition( PositionType.Static );

    /// <inheritdoc/>
    public IFluentPositionWithEdgeTypeAndTranslateType Relative => WithPosition( PositionType.Relative );

    /// <inheritdoc/>
    public IFluentPositionWithEdgeTypeAndTranslateType Absolute => WithPosition( PositionType.Absolute );

    /// <inheritdoc/>
    public IFluentPositionWithEdgeTypeAndTranslateType Fixed => WithPosition( PositionType.Fixed );

    /// <inheritdoc/>
    public IFluentPositionWithEdgeTypeAndTranslateType Sticky => WithPosition( PositionType.Sticky );

    /// <inheritdoc/>
    public IFluentPositionEdgeOffset Top => WithEdge( PositionEdgeType.Top );

    /// <inheritdoc/>
    public IFluentPositionEdgeOffset Start => WithEdge( PositionEdgeType.Start );

    /// <inheritdoc/>
    public IFluentPositionEdgeOffset Bottom => WithEdge( PositionEdgeType.Bottom );

    /// <inheritdoc/>
    public IFluentPositionEdgeOffset End => WithEdge( PositionEdgeType.End );

    /// <inheritdoc/>
    public IFluentPositionWithAll Is0 => WithEdgeOffset( 0 );

    /// <inheritdoc/>
    public IFluentPositionWithAll Is50 => WithEdgeOffset( 50 );

    /// <inheritdoc/>
    public IFluentPositionWithAll Is100 => WithEdgeOffset( 100 );

    /// <inheritdoc/>
    public IFluentPositionTranslateType Translate => this;

    /// <inheritdoc/>
    public IFluentPositionWithAll Middle => WithTranslate( PositionTranslateType.Middle );

    /// <inheritdoc/>
    public IFluentPositionWithAll MiddleX => WithTranslate( PositionTranslateType.MiddleX );

    /// <inheritdoc/>
    public IFluentPositionWithAll MiddleY => WithTranslate( PositionTranslateType.MiddleY );

    #endregion
}

/// <summary>
/// Set of position rules to start the build process.
/// </summary>
public static class Position
{
    /// <summary>
    /// An element is not positioned in any special way; it is always positioned according to the normal flow of the page.
    /// </summary>
    /// <remarks>
    /// Static positioned elements are not affected by the top, bottom, left, and right properties.
    /// </remarks>
    public static IFluentPositionWithEdgeTypeAndTranslateType Static => new FluentPosition().Static;

    /// <summary>
    /// An element is positioned relative to its normal position.
    /// </summary>
    public static IFluentPositionWithEdgeTypeAndTranslateType Relative => new FluentPosition().Relative;

    /// <summary>
    /// An element is positioned relative to the nearest positioned ancestor (instead of positioned relative to the
    /// viewport, like fixed).
    /// </summary>
    public static IFluentPositionWithEdgeTypeAndTranslateType Absolute => new FluentPosition().Absolute;

    /// <summary>
    /// An element is positioned relative to the viewport, which means it always stays in the
    /// same place even if the page is scrolled. The top, right, bottom, and left properties are used to position the element.
    /// </summary>
    public static IFluentPositionWithEdgeTypeAndTranslateType Fixed => new FluentPosition().Fixed;

    /// <summary>
    /// An element is positioned based on the user's scroll position.
    /// </summary>
    public static IFluentPositionWithEdgeTypeAndTranslateType Sticky => new FluentPosition().Sticky;
}