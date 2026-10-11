#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
#endregion

namespace Blazorise.Tailwind.Utilities;

/// <summary>
/// Removes conflicting common Tailwind utilities, preferring important classes and then the last class in each group.
/// Unrecognised classes and different variant scopes are preserved.
/// </summary>
internal static class TailwindClassMerger
{
    #region Members

    private static readonly Dictionary<string, string[]> conflicts = new( StringComparer.Ordinal )
    {
        ["p"] = ["px", "py", "pt", "pr", "pb", "pl", "ps", "pe"],
        ["px"] = ["pr", "pl"],
        ["py"] = ["pt", "pb"],
        ["m"] = ["mx", "my", "mt", "mr", "mb", "ml", "ms", "me"],
        ["mx"] = ["mr", "ml"],
        ["my"] = ["mt", "mb"],
        ["inset"] = ["inset-x", "inset-y", "top", "right", "bottom", "left", "start", "end"],
        ["inset-x"] = ["right", "left"],
        ["inset-y"] = ["top", "bottom"],
        ["size"] = ["w", "h"],
        ["flex"] = ["grow", "shrink", "basis"],
        ["gap"] = ["gap-x", "gap-y"],
        ["overflow"] = ["overflow-x", "overflow-y"],
        ["truncate"] = ["text-overflow", "whitespace", "overflow", "overflow-x", "overflow-y"],
        ["break-normal"] = ["overflow-wrap", "word-break"],
        ["col"] = ["col-start", "col-end"],
        ["row"] = ["row-start", "row-end"],
        ["rounded"] = ["rounded-t", "rounded-r", "rounded-b", "rounded-l", "rounded-s", "rounded-e", "rounded-tl", "rounded-tr", "rounded-bl", "rounded-br", "rounded-ss", "rounded-se", "rounded-es", "rounded-ee"],
        ["rounded-t"] = ["rounded-tl", "rounded-tr"],
        ["rounded-r"] = ["rounded-tr", "rounded-br"],
        ["rounded-b"] = ["rounded-bl", "rounded-br"],
        ["rounded-l"] = ["rounded-tl", "rounded-bl"],
        ["rounded-s"] = ["rounded-ss", "rounded-es"],
        ["rounded-e"] = ["rounded-se", "rounded-ee"],
        ["border-w"] = ["border-x-w", "border-y-w", "border-t-w", "border-r-w", "border-b-w", "border-l-w", "border-s-w", "border-e-w"],
        ["border-x-w"] = ["border-r-w", "border-l-w"],
        ["border-y-w"] = ["border-t-w", "border-b-w"],
        ["border-color"] = ["border-x-color", "border-y-color", "border-t-color", "border-r-color", "border-b-color", "border-l-color", "border-s-color", "border-e-color"],
        ["border-x-color"] = ["border-r-color", "border-l-color"],
        ["border-y-color"] = ["border-t-color", "border-b-color"],
    };

    #endregion

    #region Methods

    /// <summary>
    /// Merges a completed element class list without changing the order of retained classes.
    /// </summary>
    public static string Merge( string classNames )
    {
        if ( string.IsNullOrWhiteSpace( classNames ) )
        {
            return classNames;
        }

        var classes = classNames.Split( (char[])null, StringSplitOptions.RemoveEmptyEntries );
        var claimed = new HashSet<string>( StringComparer.Ordinal );
        var retained = new bool[classes.Length];
        var result = new List<string>( classes.Length );

        // Important utilities take precedence regardless of their position in the class list.
        for ( var pass = 0; pass < 2; pass++ )
        {
            var processImportant = pass == 0;

            for ( var i = classes.Length - 1; i >= 0; i-- )
            {
                ParseClass( classes[i], out var scope, out var utility, out var important );

                if ( important != processImportant )
                {
                    continue;
                }

                var group = ResolveGroup( utility );

                if ( group is null )
                {
                    retained[i] = true;
                    continue;
                }

                if ( !claimed.Add( scope + group ) )
                {
                    continue;
                }

                retained[i] = true;

                if ( conflicts.TryGetValue( group, out var overriddenGroups ) )
                {
                    foreach ( var overriddenGroup in overriddenGroups )
                    {
                        claimed.Add( scope + overriddenGroup );
                    }
                }

                if ( group == "font-size" && FindPostfixSeparator( utility ) >= 0 )
                {
                    claimed.Add( scope + "leading" );
                }
            }
        }

        for ( var i = 0; i < classes.Length; i++ )
        {
            if ( retained[i] )
            {
                result.Add( classes[i] );
            }
        }

        return string.Join( ' ', result );
    }

    private static void ParseClass( string className, out string scope, out string utility, out bool important )
    {
        important = className.StartsWith( '!' );

        if ( important )
        {
            className = className[1..];
        }

        var depth = 0;
        var start = 0;

        for ( var i = 0; i < className.Length; i++ )
        {
            switch ( className[i] )
            {
                case '[' or '(':
                    depth++;
                    break;
                case ']' or ')':
                    depth--;
                    break;
                case ':' when depth == 0:
                    start = i + 1;
                    break;
            }
        }

        utility = className[start..];
        important |= utility.StartsWith( '!' ) || utility.EndsWith( '!' );
        scope = className[..start] + "|";

        if ( utility.StartsWith( '!' ) )
        {
            utility = utility[1..];
        }

        if ( utility.EndsWith( '!' ) )
        {
            utility = utility[..^1];
        }

        if ( utility.StartsWith( '-' ) )
        {
            utility = utility[1..];
        }
    }

    private static string ResolveGroup( string utility )
    {
        switch ( utility )
        {
            case "block" or "inline-block" or "inline" or "flex" or "inline-flex" or "grid" or "inline-grid" or "table" or "inline-table" or "table-row" or "table-cell" or "flow-root" or "contents" or "hidden":
                return "display";
            case "static" or "fixed" or "absolute" or "relative" or "sticky":
                return "position";
            case "visible" or "invisible" or "collapse":
                return "visibility";
            case "box-border" or "box-content":
                return "box-sizing";
            case "sr-only" or "not-sr-only":
                return "screen-reader";
            case "italic" or "not-italic":
                return "font-style";
            case "uppercase" or "lowercase" or "capitalize" or "normal-case":
                return "text-transform";
            case "underline" or "overline" or "line-through" or "no-underline":
                return "text-decoration";
            case "truncate":
                return "truncate";
            case "text-ellipsis" or "text-clip":
                return "text-overflow";
            case "break-normal":
                return "break-normal";
            case "break-words":
                return "overflow-wrap";
            case "break-all" or "break-keep":
                return "word-break";
            case "border":
                return "border-w";
            case "border-collapse" or "border-separate":
                return "border-collapse";
            case "divide-x" or "divide-y":
                return utility + "-w";
            case "divide-x-reverse" or "divide-y-reverse" or "space-x-reverse" or "space-y-reverse":
                return utility;
            case "ring":
                return "ring-w";
            case "shadow":
                return "shadow";
            case "rounded":
                return "rounded";
            case "grow" or "shrink":
                return utility;
            case "transition":
                return "transition";
            case "transform" or "transform-none" or "transform-cpu" or "transform-gpu":
                return "transform";
            case "outline" or "outline-solid" or "outline-dashed" or "outline-dotted" or "outline-double" or "outline-none":
                return "outline-style";
            case "outline-hidden":
                return null;
        }

        if ( utility.StartsWith( '[' ) && utility.EndsWith( ']' ) )
        {
            var colon = utility.IndexOf( ':' );
            return colon > 1 ? "property-" + utility[1..colon] : null;
        }

        var separator = utility.IndexOf( '-' );

        if ( separator < 0 )
        {
            return null;
        }

        var prefix = utility[..separator];
        var value = utility[( separator + 1 )..];

        var partSeparator = value.IndexOf( '-' );

        if ( partSeparator > 0
            && ( prefix, value[..partSeparator] ) is ( "min" or "max", "w" or "h" )
                or ( "gap" or "overflow" or "inset" or "translate" or "space" or "divide", "x" or "y" )
                or ( "grid", "cols" or "rows" or "flow" )
                or ( "auto", "cols" or "rows" )
                or ( "col" or "row", "span" or "start" or "end" )
                or ( "ring" or "outline", "offset" )
                or ( "pointer", "events" ) )
        {
            prefix += "-" + value[..partSeparator];
            value = value[( partSeparator + 1 )..];
        }

        switch ( prefix )
        {
            case "p" or "px" or "py" or "pt" or "pr" or "pb" or "pl" or "ps" or "pe":
            case "gap" or "gap-x" or "gap-y":
            case "space-x" or "space-y":
                return IsSpacing( value ) ? prefix : null;
            case "m" or "mx" or "my" or "mt" or "mr" or "mb" or "ml" or "ms" or "me":
            case "inset" or "inset-x" or "inset-y" or "top" or "right" or "bottom" or "left" or "start" or "end":
                return IsSpacing( value ) || value is "auto" or "full" || IsFraction( value ) ? prefix : null;
            case "w" or "h" or "min-w" or "max-w" or "min-h" or "max-h" or "size" or "basis":
                return IsSpacing( value ) || IsFraction( value ) || IsSize( value ) ? prefix : null;
            case "translate-x" or "translate-y":
                return IsSpacing( value ) || IsFraction( value ) || value == "full" ? prefix : null;
            case "grid-cols" or "grid-rows":
                return IsNumber( value ) || IsArbitraryValue( value ) || value is "none" or "subgrid" ? prefix : null;
            case "grid-flow":
                return value is "row" or "col" or "dense" or "row-dense" or "col-dense" ? prefix : null;
            case "auto-cols" or "auto-rows":
                return IsArbitraryValue( value ) || value is "auto" or "min" or "max" or "fr" ? prefix : null;
            case "col" or "row" or "col-start" or "col-end" or "row-start" or "row-end":
                return IsNumber( value ) || IsArbitraryValue( value ) || value == "auto" ? prefix : null;
            case "col-span" or "row-span":
                return IsNumber( value ) || IsArbitraryValue( value ) || value == "full" ? prefix[..3] : null;
            case "align":
                return IsArbitraryValue( value ) || value is "baseline" or "top" or "middle" or "bottom" or "text-top" or "text-bottom" or "sub" or "super" ? "vertical-align" : null;
            case "object":
                return value is "contain" or "cover" or "fill" or "none" or "scale-down" ? "object-fit"
                    : IsArbitraryValue( value ) || value is "center" or "top" or "right" or "bottom" or "left" or "top-left" or "top-right" or "bottom-left" or "bottom-right" ? "object-position" : null;
            case "whitespace":
                return value is "normal" or "nowrap" or "pre" or "pre-line" or "pre-wrap" or "break-spaces" ? prefix : null;
            case "text":
                return ResolveTextGroup( value );
            case "bg":
                return ResolveBackgroundGroup( value );
            case "border":
                return ResolveBorderGroup( value );
            case "divide":
                return value is "solid" or "dashed" or "dotted" or "double" or "none" ? "divide-style" : ResolveColorOrLengthGroup( value, null, "divide-color" );
            case "divide-x" or "divide-y":
                return IsNumber( value ) || IsArbitraryValue( value ) ? prefix + "-w" : null;
            case "ring":
                return IsNumber( value ) ? "ring-w" : ResolveColorOrLengthGroup( value, "ring-w", "ring-color" );
            case "ring-offset":
                return IsNumber( value ) ? "ring-offset-w" : ResolveColorOrLengthGroup( value, "ring-offset-w", "ring-offset-color" );
            case "outline":
                return IsNumber( value ) ? "outline-w" : ResolveColorOrLengthGroup( value, "outline-w", "outline-color" );
            case "outline-offset":
                return IsSpacing( value ) ? prefix : null;
            case "placeholder" or "accent" or "caret":
                return value == "auto" && prefix == "accent" ? prefix + "-color" : ResolveColorOrLengthGroup( value, null, prefix + "-color" );
            case "shadow":
                return value == "none" || IsScale( value )
                    || value.StartsWith( "[inset_", StringComparison.Ordinal )
                    || value.StartsWith( '[' ) && value.Length > 1 && ( char.IsDigit( value[1] ) || value[1] == '-' )
                    ? "shadow"
                    : ResolveColorOrLengthGroup( value, null, "shadow-color" );
            case "rounded":
                var sideSeparator = value.IndexOf( '-' );
                var side = sideSeparator < 0 ? value : value[..sideSeparator];

                if ( side is "t" or "r" or "b" or "l" or "s" or "e" or "tl" or "tr" or "bl" or "br" or "ss" or "se" or "es" or "ee" )
                {
                    return sideSeparator < 0 || IsRadius( value[( sideSeparator + 1 )..] ) ? "rounded-" + side : null;
                }

                return IsRadius( value ) ? "rounded" : null;
            case "font":
                return value is "thin" or "extralight" or "light" or "normal" or "medium" or "semibold" or "bold" or "extrabold" or "black" || IsNumber( value )
                    ? "font-weight"
                    : value is "sans" or "serif" or "mono" ? "font-family" : null;
            case "overflow" or "overflow-x" or "overflow-y":
                return value is "auto" or "hidden" or "clip" or "visible" or "scroll" ? prefix : null;
            case "flex":
                return value is "row" or "row-reverse" or "col" or "col-reverse" ? "flex-direction"
                    : value is "wrap" or "wrap-reverse" or "nowrap" ? "flex-wrap"
                    : IsNumber( value ) || IsFraction( value ) || IsArbitraryValue( value ) || value is "auto" or "initial" or "none" ? "flex" : null;
            case "items" or "justify" or "content" or "self":
                return value is "start" or "end" or "center" or "between" or "around" or "evenly" or "stretch" or "baseline" or "normal" or "auto" ? prefix : null;
            case "cursor":
                return IsArbitraryValue( value ) || value is "auto" or "default" or "pointer" or "wait" or "text" or "move" or "help" or "not-allowed" or "none" or "grab" or "grabbing" or "crosshair" or "col-resize" or "row-resize" ? prefix : null;
            case "pointer-events":
                return value is "auto" or "none" ? prefix : null;
            case "select":
                return value is "auto" or "none" or "text" or "all" ? prefix : null;
            case "appearance":
                return value is "auto" or "none" ? prefix : null;
            case "touch":
                return value is "auto" or "none" or "manipulation" ? prefix : null;
            case "aspect":
                return IsFraction( value ) || IsArbitraryValue( value ) || value is "auto" or "square" or "video" ? prefix : null;
            case "list":
                return value is "none" or "disc" or "decimal" ? "list-style" : value is "inside" or "outside" ? "list-position" : null;
            case "transition":
                return IsArbitraryValue( value ) || value is "all" or "colors" or "opacity" or "shadow" or "transform" or "none" ? prefix : null;
            case "duration" or "delay":
                return IsNumber( value ) || IsArbitraryValue( value ) ? prefix : null;
            case "ease":
                return IsArbitraryValue( value ) || value is "linear" or "in" or "out" or "in-out" ? prefix : null;
            case "opacity" or "grow" or "shrink":
                return IsNumber( value ) || IsArbitraryValue( value ) ? prefix : null;
            case "z" or "order":
                return IsNumber( value ) || IsArbitraryValue( value ) || value == "auto" ? prefix : null;
            case "leading":
                return IsSpacing( value ) || value is "none" or "normal" or "tight" or "snug" or "relaxed" or "loose" ? prefix : null;
            case "tracking":
                return IsNumber( value ) || IsArbitraryValue( value ) || value is "tighter" or "tight" or "normal" or "wide" or "wider" or "widest" ? prefix : null;
        }

        return null;
    }

    private static string ResolveTextGroup( string value )
    {
        if ( value is "left" or "center" or "right" or "justify" or "start" or "end" )
        {
            return "text-align";
        }

        if ( value is "wrap" or "nowrap" or "balance" or "pretty" )
        {
            return "text-wrap";
        }

        var slash = FindPostfixSeparator( value );
        var size = slash < 0 ? value : value[..slash];

        if ( size == "base" || IsScale( size ) )
        {
            return "font-size";
        }

        return ResolveColorOrLengthGroup( size, "font-size", "text-color" );
    }

    private static string ResolveBackgroundGroup( string value )
    {
        if ( value is "auto" or "cover" or "contain" )
        {
            return "bg-size";
        }

        if ( value is "center" or "top" or "right" or "bottom" or "left" or "top-left" or "top-right" or "bottom-left" or "bottom-right" )
        {
            return "bg-position";
        }

        if ( value is "fixed" or "local" or "scroll" )
        {
            return "bg-attachment";
        }

        if ( value is "repeat" or "no-repeat" or "repeat-x" or "repeat-y" or "repeat-round" or "repeat-space" )
        {
            return "bg-repeat";
        }

        if ( value == "none"
            || value.StartsWith( "linear-", StringComparison.Ordinal )
            || value.StartsWith( "radial", StringComparison.Ordinal )
            || value.StartsWith( "conic", StringComparison.Ordinal )
            || value.StartsWith( "[url(", StringComparison.Ordinal )
            || value.StartsWith( "[image:", StringComparison.Ordinal )
            || value.StartsWith( "[linear-gradient(", StringComparison.Ordinal )
            || value.StartsWith( "[radial-gradient(", StringComparison.Ordinal )
            || value.StartsWith( "[conic-gradient(", StringComparison.Ordinal ) )
        {
            return "bg-image";
        }

        if ( value.StartsWith( "clip-", StringComparison.Ordinal )
            || value.StartsWith( "origin-", StringComparison.Ordinal )
            || value.StartsWith( "blend-", StringComparison.Ordinal ) )
        {
            return null;
        }

        return ResolveColorOrLengthGroup( value, null, "bg-color" );
    }

    private static string ResolveBorderGroup( string value )
    {
        if ( value is "solid" or "dashed" or "dotted" or "double" or "hidden" or "none" )
        {
            return "border-style";
        }

        var separator = value.IndexOf( '-' );
        var side = separator < 0 ? value : value[..separator];
        var prefix = "border";

        if ( side is "x" or "y" or "t" or "r" or "b" or "l" or "s" or "e" )
        {
            prefix += "-" + side;

            if ( separator < 0 )
            {
                return prefix + "-w";
            }

            value = value[( separator + 1 )..];
        }

        return IsNumber( value ) ? prefix + "-w" : ResolveColorOrLengthGroup( value, prefix + "-w", prefix + "-color" );
    }

    private static string ResolveColorOrLengthGroup( string value, string lengthGroup, string colorGroup )
    {
        var slash = FindPostfixSeparator( value );

        if ( slash >= 0 )
        {
            value = value[..slash];
        }

        if ( !value.StartsWith( '[' ) && !value.StartsWith( '(' ) )
        {
            return IsColor( value ) ? colorGroup : null;
        }

        if ( !value.EndsWith( ']' ) && !value.EndsWith( ')' ) )
        {
            return null;
        }

        var content = value[1..^1];

        if ( content.StartsWith( "color:", StringComparison.Ordinal )
            || content.StartsWith( '#' )
            || content.StartsWith( "rgb", StringComparison.Ordinal )
            || content.StartsWith( "hsl", StringComparison.Ordinal )
            || content.StartsWith( "okl", StringComparison.Ordinal )
            || content.StartsWith( "hwb(", StringComparison.Ordinal )
            || content.StartsWith( "lab(", StringComparison.Ordinal )
            || content.StartsWith( "lch(", StringComparison.Ordinal )
            || content.StartsWith( "color(", StringComparison.Ordinal )
            || content.StartsWith( "color-mix(", StringComparison.Ordinal )
            || content.StartsWith( "contrast-color(", StringComparison.Ordinal ) )
        {
            return colorGroup;
        }

        if ( content.StartsWith( "length:", StringComparison.Ordinal )
            || content.StartsWith( "calc(", StringComparison.Ordinal )
            || content.StartsWith( "clamp(", StringComparison.Ordinal )
            || content.StartsWith( "min(", StringComparison.Ordinal )
            || content.StartsWith( "max(", StringComparison.Ordinal )
            || content.EndsWith( "px", StringComparison.Ordinal )
            || content.EndsWith( "rem", StringComparison.Ordinal )
            || content.EndsWith( "em", StringComparison.Ordinal )
            || content.EndsWith( "vh", StringComparison.Ordinal )
            || content.EndsWith( "vw", StringComparison.Ordinal )
            || content.EndsWith( "ch", StringComparison.Ordinal )
            || content.EndsWith( '%' ) )
        {
            return lengthGroup;
        }

        if ( lengthGroup is null
            && ( content.StartsWith( "--", StringComparison.Ordinal ) || content.StartsWith( "var(", StringComparison.Ordinal ) ) )
        {
            return colorGroup;
        }

        // Ambiguous arbitrary values are preserved rather than assigned to the wrong property.
        return null;
    }

    private static bool IsColor( string value )
    {
        var slash = value.IndexOf( '/' );

        if ( slash >= 0 )
        {
            value = value[..slash];
        }

        var separator = value.IndexOf( '-' );
        var name = separator < 0 ? value : value[..separator];

        if ( separator >= 0 && !IsNumber( value[( separator + 1 )..] ) )
        {
            return false;
        }

        return name is "inherit" or "current" or "transparent" or "black" or "white"
            or "slate" or "gray" or "zinc" or "neutral" or "stone" or "red" or "orange" or "amber" or "yellow" or "lime" or "green" or "emerald" or "teal" or "cyan" or "sky" or "blue" or "indigo" or "violet" or "purple" or "fuchsia" or "pink" or "rose"
            or "primary" or "secondary" or "success" or "danger" or "warning" or "info" or "light" or "dark" or "link" or "midnight";
    }

    private static bool IsNumber( string value ) => double.TryParse( value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out _ );

    private static int FindPostfixSeparator( string value )
    {
        var depth = 0;

        for ( var i = 0; i < value.Length; i++ )
        {
            switch ( value[i] )
            {
                case '[' or '(':
                    depth++;
                    break;
                case ']' or ')':
                    depth--;
                    break;
                case '/' when depth == 0:
                    return i;
            }
        }

        return -1;
    }

    private static bool IsSpacing( string value ) => IsNumber( value ) || value == "px" || IsArbitraryValue( value );

    private static bool IsArbitraryValue( string value )
        => value.StartsWith( '[' ) && value.EndsWith( ']' )
            || value.StartsWith( '(' ) && value.EndsWith( ')' );

    private static bool IsFraction( string value )
    {
        var slash = value.IndexOf( '/' );
        return slash > 0 && IsNumber( value[..slash] ) && IsNumber( value[( slash + 1 )..] );
    }

    private static bool IsSize( string value )
        => value is "auto" or "full" or "screen" or "min" or "max" or "fit" or "none" or "svh" or "lvh" or "dvh" or "svw" or "lvw" or "dvw" or "lh"
            || IsRadius( value );

    private static bool IsRadius( string value )
        => value is "none" or "full" || IsScale( value ) || IsSpacing( value );

    private static bool IsScale( string value )
        => value is "3xs" or "2xs" or "xs" or "sm" or "md" or "lg" or "xl"
            || value.EndsWith( "xl", StringComparison.Ordinal ) && IsNumber( value[..^2] );

    #endregion
}