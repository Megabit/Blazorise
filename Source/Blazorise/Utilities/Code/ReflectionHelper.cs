using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Blazorise.Utilities;

/// <summary>
/// An helper for reflection based apis.
/// </summary>
public static class ReflectionHelper
{
    /// <summary>
    /// Gets the public instance properties of the specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static PropertyInfo[] GetPublicProperties<T>()
    {
        var properties = typeof( T ).GetProperties( BindingFlags.Public | BindingFlags.Instance );
        return properties;
    }

    /// <summary>
    /// Gets a method by name.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public static MethodInfo GetStaticMethod( Type type, string methodName )
    {
        return type.GetMethod( methodName, BindingFlags.Public | BindingFlags.Static );
    }

    /// <summary>
    /// Gets a method by name.
    /// </summary>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public static MethodInfo GetStaticMethod<T>( string methodName )
    {
        return GetStaticMethod( typeof( T ), methodName );
    }

    /// <summary>
    /// Gets a method by name.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public static MethodInfo GetMethod( Type type, string methodName )
    {
        return type.GetMethod( methodName, BindingFlags.Public | BindingFlags.Instance );
    }

    /// <summary>
    /// Gets a method by name.
    /// </summary>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public static MethodInfo GetMethod<T>( string methodName )
    {
        return GetMethod( typeof( T ), methodName );
    }

    /// <summary>
    /// Based on this particular property, tries to resolve a caption out of the attributes.
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <returns></returns>
    public static string ResolveCaption( PropertyInfo propertyInfo )
    {
        var displayAttribute = propertyInfo.GetCustomAttribute<DisplayAttribute>();
        if ( displayAttribute is not null )
        {
            return displayAttribute.GetName();
        }

        var displayNameAttribute = propertyInfo.GetCustomAttribute<DisplayNameAttribute>();
        if ( displayNameAttribute is not null )
        {
            return displayNameAttribute.DisplayName;
        }

        return Formaters.PascalCaseToFriendlyName( propertyInfo.Name );
    }

    /// <summary>
    /// Based on this particular property, resolves whether this property should be ignored.
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <returns></returns>
    public static bool ResolveIsIgnore( PropertyInfo propertyInfo )
    {
        var ignoreAttribute = propertyInfo.GetCustomAttribute<IgnoreFieldAttribute>();
        return ignoreAttribute is not null;
    }

    /// <summary>
    /// Based on this particular property, resolves the display order.
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <returns></returns>
    public static int ResolveDisplayOrder( PropertyInfo propertyInfo )
    {
        var orderAttribute = propertyInfo.GetCustomAttribute<OrderAttribute>();
        return orderAttribute?.DisplayOrder ?? default;
    }

    /// <summary>
    /// Based on this particular property, resolves the edit order.
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <returns></returns>
    public static int? ResolveEditOrder( PropertyInfo propertyInfo )
    {
        var orderAttribute = propertyInfo.GetCustomAttribute<OrderAttribute>();
        return orderAttribute?.EditOrder;
    }

    /// <summary>
    /// Based on this particular property, resolves a numeric attribute.
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <returns></returns>
    public static NumericAttribute ResolveNumericAttribute( PropertyInfo propertyInfo )
    {
        var attribute = propertyInfo.GetCustomAttribute<NumericAttribute>();
        return attribute;
    }

    /// <summary>
    /// Based on this particular property, resolves a select attribute.
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <returns></returns>
    public static SelectAttribute ResolveSelectAttribute( PropertyInfo propertyInfo )
    {
        var attribute = propertyInfo.GetCustomAttribute<SelectAttribute>();
        return attribute;
    }

    /// <summary>
    /// Based on this particular property, resolves a date attribute.
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <returns></returns>
    public static DateAttribute ResolveDateAttribute( PropertyInfo propertyInfo )
    {
        var attribute = propertyInfo.GetCustomAttribute<DateAttribute>();
        return attribute;
    }

}
