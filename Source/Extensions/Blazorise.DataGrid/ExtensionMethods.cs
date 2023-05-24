#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using Blazorise.Localization;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Helper extension methods.
/// </summary>
public static class ExtensionMethods
{
    /// <summary>
    /// Gets the next available direction based on the current one.
    /// </summary>
    /// <param name="direction">Current sort direction.</param>
    /// <param name="isReverse">Reverse the next sort direction.</param>
    /// <returns>Returns the next available sort direction.</returns>
    public static SortDirection NextDirection( this SortDirection direction, bool isReverse = false )
    {
        switch ( direction )
        {
            case SortDirection.Default:
                return isReverse ? SortDirection.Descending : SortDirection.Ascending;
            case SortDirection.Ascending:
                return isReverse ? SortDirection.Default : SortDirection.Descending;
            case SortDirection.Descending:
                return isReverse ? SortDirection.Ascending : SortDirection.Default;
            default:
                return SortDirection.Default;
        }
    }

    /// <summary>
    /// Handles the localization of datagrid based on the built-int localizer and a custom localizer handler.
    /// </summary>
    /// <param name="textLocalizer">Default localizer.</param>
    /// <param name="textLocalizerHandler">Custom localizer.</param>
    /// <param name="name">Localization name.</param>
    /// <param name="arguments">Arguments to format the text.</param>
    /// <returns>Returns the localized text.</returns>
    public static string Localize( this ITextLocalizer textLocalizer, TextLocalizerHandler textLocalizerHandler, string name, params object[] arguments )
    {
        if ( textLocalizerHandler != null )
            return textLocalizerHandler.Invoke( name, arguments );

        return textLocalizer[name, arguments];
    }


    /// <summary>
    /// Checks if a type is a collection.
    /// </summary>
    /// <param name="type">Type to check.</param>
    /// <returns>True if <paramref name="type"/> is a collection.</returns>
    public static bool IsCollection( this Type type )
        => typeof( ICollection ).IsAssignableFrom( type )
           || type.IsGenericCollection()
           || type.IsGenericIEnumerable()
           || Array.Find( type.GetInterfaces(), IsGenericCollection ) != null;

    private static bool IsGenericCollection( this Type type )
        => type.IsGenericType
           && type.GetGenericTypeDefinition() == typeof( ICollection<> );

    private static bool IsGenericIEnumerable( this Type type )
        => type.IsGenericType
           && type.GetGenericTypeDefinition() == typeof( IEnumerable<> );
}