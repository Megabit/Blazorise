using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Blazorise.DeepCloner.Helpers;

/// <summary>
/// Safe types are types, which can be copied without real cloning. e.g. simple structs or strings (it is immutable)
/// </summary>
internal static class DeepClonerSafeTypes
{
    internal static readonly ConcurrentDictionary<Type, bool> KnownTypes = new ConcurrentDictionary<Type, bool>();

    static DeepClonerSafeTypes()
    {
        foreach (
            var x in
                new[]
                    {
                            typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong),
                            typeof(float), typeof(double), typeof(decimal), typeof(char), typeof(string), typeof(bool), typeof(DateTime),
                            typeof(nint), typeof(nuint), typeof(Guid),
							// do not clone such native type
							Type.GetType("System.RuntimeType"),
                            Type.GetType("System.RuntimeTypeHandle"),
                            StringComparer.Ordinal.GetType(),
                            StringComparer.CurrentCulture.GetType(), // CultureAwareComparer - can be same
                    } )
            KnownTypes.TryAdd( x, true );
    }

    private static bool CanReturnSameType( Type type, HashSet<Type> processingTypes )
    {
        bool isSafe;
        if ( KnownTypes.TryGetValue( type, out isSafe ) )
            return isSafe;

        // enums are safe
        // pointers (e.g. int*) are unsafe, but we cannot do anything with it except blind copy
        if ( type.IsEnum() || type.IsPointer )
        {
            KnownTypes.TryAdd( type, true );
            return true;
        }


        // do not copy db null
        if ( type.FullName.StartsWith( "System.DBNull" ) )
        {
            KnownTypes.TryAdd( type, true );
            return true;
        }

        if ( type.FullName.StartsWith( "System.RuntimeType" ) )
        {
            KnownTypes.TryAdd( type, true );
            return true;
        }

        if ( type.FullName.StartsWith( "System.Reflection." ) && Equals( type.GetTypeInfo().Assembly, typeof( PropertyInfo ).GetTypeInfo().Assembly ) )
        {
            KnownTypes.TryAdd( type, true );
            return true;
        }

        if ( type.IsSubclassOfTypeByName( "CriticalFinalizerObject" ) )
        {
            KnownTypes.TryAdd( type, true );
            return true;
        }

        // better not to touch ms dependency injection
        if ( type.FullName.StartsWith( "Microsoft.Extensions.DependencyInjection." ) )
        {
            KnownTypes.TryAdd( type, true );
            return true;
        }

        if ( type.FullName == "Microsoft.EntityFrameworkCore.Internal.ConcurrencyDetector" )
        {
            KnownTypes.TryAdd( type, true );
            return true;
        }

        // default comparers should not be cloned due possible comparison EqualityComparer<T>.Default == comparer
        if ( type.FullName.Contains( "EqualityComparer" ) )
        {
            if ( type.FullName.StartsWith( "System.Collections.Generic.GenericEqualityComparer`" )
                || type.FullName.StartsWith( "System.Collections.Generic.ObjectEqualityComparer`" )
                || type.FullName.StartsWith( "System.Collections.Generic.EnumEqualityComparer`" )
                || type.FullName.StartsWith( "System.Collections.Generic.NullableEqualityComparer`" )
                || type.FullName == "System.Collections.Generic.ByteEqualityComparer" )
            {
                KnownTypes.TryAdd( type, true );
                return true;
            }
        }

        // classes are always unsafe (we should copy it fully to count references)
        if ( !type.IsValueType() )
        {
            KnownTypes.TryAdd( type, false );
            return false;
        }

        if ( processingTypes == null )
            processingTypes = new HashSet<Type>();

        // structs cannot have a loops, but check it anyway
        processingTypes.Add( type );

        List<FieldInfo> fi = new List<FieldInfo>();
        var tp = type;
        do
        {
            fi.AddRange( tp.GetAllFields() );
            tp = tp.BaseType();
        }
        while ( tp != null );

        foreach ( var fieldInfo in fi )
        {
            // type loop
            var fieldType = fieldInfo.FieldType;
            if ( processingTypes.Contains( fieldType ) )
                continue;

            // not safe and not not safe. we need to go deeper
            if ( !CanReturnSameType( fieldType, processingTypes ) )
            {
                KnownTypes.TryAdd( type, false );
                return false;
            }
        }

        KnownTypes.TryAdd( type, true );
        return true;
    }

    // not used anymore
    /*/// <summary>
    /// Classes with only safe fields are safe for ShallowClone (if they root objects for copying)
    /// </summary>
    private static bool CanCopyClassInShallow(Type type)
    {
        // do not do this anything for struct and arrays
        if (!type.IsClass() || type.IsArray)
        {
            return false;
        }

        List<FieldInfo> fi = new List<FieldInfo>();
        var tp = type;
        do
        {
            fi.AddRange(tp.GetAllFields());
            tp = tp.BaseType();
        }
        while (tp != null);

        if (fi.Any(fieldInfo => !CanReturnSameType(fieldInfo.FieldType, null)))
        {
            return false;
        }

        return true;
    }*/

    public static bool CanReturnSameObject( Type type )
    {
        return CanReturnSameType( type, null );
    }
}
