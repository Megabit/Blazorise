using System;
using System.Linq;

namespace Blazorise.Docs.Models.ApiDocsDtos;

public static class TypeExtensions
{
    public static bool IsEventType( this Type type )
        => type.Name.Split( '`' ).First()// Get the base name without generic arity (e.g., "Func`1" -> "Func"
            is "Func" or "Action" or "EventCallback";
}