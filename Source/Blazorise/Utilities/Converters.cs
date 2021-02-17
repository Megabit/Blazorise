#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using Blazorise.Extensions;

#endregion

namespace Blazorise.Utilities
{
    public static class Converters
    {
        #region Constants

        private static readonly Type[] SimpleTypes =
        {
            typeof(string),
            typeof(decimal),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(Guid)
        };

        #endregion

        #region Methods

        /// <summary>
        /// Converts an object to a dictionary object without the properties which have a null value or a [DataMember( EmitDefaultValue = false )] applied.
        /// This can be used as a workaround for System.Text.Json which will always serialize null values which breaks ChartJS functionality.
        /// </summary>
        /// <param name="source">The source object, can be null.</param>
        /// <param name="addEmptyObjects">Objects which do not have any properties are also added to the dictionary. Default value is true.</param>
        /// <param name="forceCamelCase">Force to use CamelCase, even if a DataMember has another casing defined. Default value is true.</param>
        /// <returns>Dictionary</returns>
        public static IDictionary<string, object> ToDictionary( object source, bool addEmptyObjects = true, bool forceCamelCase = true )
        {
            if ( source == null )
            {
                return null;
            }

            var dictionary = new Dictionary<string, object>();

            object ProcessValue( object value, bool emitDefaultValue )
            {
                if ( value != null && ( emitDefaultValue || !IsEqualToDefaultValue( value ) ) )
                {
                    var type = value.GetType();

                    if ( IsSimpleType( type ) )
                    {
                        return value;
                    }
                    else if ( typeof( IEnumerable ).IsAssignableFrom( type ) )
                    {
                        var list = new List<object>();
                        foreach ( var item in value as IEnumerable )
                        {
                            list.Add( ProcessValue( item, emitDefaultValue ) );
                        }

                        return type.IsArray ? (object)list.ToArray() : list;
                    }
                    else
                    {
                        var dict = ToDictionary( value, addEmptyObjects );

                        if ( addEmptyObjects || dict.Count > 0 )
                        {
                            return dict;
                        }
                    }
                }

                return null;
            }

            foreach ( PropertyDescriptor property in TypeDescriptor.GetProperties( source ) )
            {
                var dataMemberAttribute = property.Attributes.OfType<DataMemberAttribute>().FirstOrDefault();
                var emitDefaultValue = dataMemberAttribute?.EmitDefaultValue ?? true;

                var value = property.GetValue( source );
                var propertyName = dataMemberAttribute?.Name ?? property.Name;
                if ( forceCamelCase )
                {
                    propertyName = propertyName.ToCamelcase();
                }

                if ( value != null && ( emitDefaultValue || !IsEqualToDefaultValue( value ) ) )
                {
                    dictionary.Add( propertyName, ProcessValue( value, emitDefaultValue ) );
                }
            }

            return dictionary;
        }

        // https://stackoverflow.com/a/1107090/833106
        public static TValue ChangeType<TValue>( object value )
        {
            Type conversionType = Nullable.GetUnderlyingType( typeof( TValue ) ) ?? typeof( TValue );

            if ( conversionType.IsEnum && EnumTryParse( value?.ToString(), conversionType, out TValue parsedValue ) )
                return parsedValue;

            try
            {
                return (TValue)Convert.ChangeType( value, conversionType );
            }
            catch
            {
                // If source value or TResult does not implement IConvertible the Convert.ChangeType will fail.
                // (One example is converting [Guid] to [object] type).
                //
                // So, as a fall-back mechanism we can just try casting it. It already failed so we can try this
                // additonal step anyways.
                return (TValue)value;
            }
        }

        public static bool TryChangeType<TValue>( object value, out TValue result, CultureInfo cultureInfo = null )
        {
            try
            {
                Type conversionType = Nullable.GetUnderlyingType( typeof( TValue ) ) ?? typeof( TValue );

                if ( conversionType.IsEnum && EnumTryParse( value?.ToString(), conversionType, out TValue theEnum ) )
                    result = theEnum;
                else if ( conversionType == typeof( Guid ) )
                    result = (TValue)Convert.ChangeType( Guid.Parse( value.ToString() ), conversionType );
                else if ( conversionType == typeof( DateTimeOffset ) )
                    result = (TValue)Convert.ChangeType( DateTimeOffset.Parse( value.ToString() ), conversionType );
                else
                    result = (TValue)Convert.ChangeType( value, conversionType, cultureInfo ?? CultureInfo.InvariantCulture );

                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        // modified version of https://stackoverflow.com/a/11521834/833106
        public static bool EnumTryParse<TValue>( string input, Type conversionType, out TValue theEnum )
        {
            if ( input != null )
            {
                foreach ( string en in Enum.GetNames( conversionType ) )
                {
                    if ( en.Equals( input, StringComparison.InvariantCultureIgnoreCase ) )
                    {
                        theEnum = (TValue)Enum.Parse( conversionType, input, true );
                        return true;
                    }
                }
            }

            theEnum = default;
            return false;
        }

        public static string FormatValue( byte value, CultureInfo culture = null ) => value.ToString( culture ?? CultureInfo.CurrentCulture );

        public static string FormatValue( byte? value, CultureInfo culture = null ) => value?.ToString( culture ?? CultureInfo.CurrentCulture );

        public static string FormatValue( short value, CultureInfo culture = null ) => value.ToString( culture ?? CultureInfo.CurrentCulture );

        public static string FormatValue( short? value, CultureInfo culture = null ) => value?.ToString( culture ?? CultureInfo.CurrentCulture );

        public static string FormatValue( int value, CultureInfo culture = null ) => value.ToString( culture ?? CultureInfo.CurrentCulture );

        public static string FormatValue( int? value, CultureInfo culture = null ) => value?.ToString( culture ?? CultureInfo.CurrentCulture );

        public static string FormatValue( long value, CultureInfo culture = null ) => value.ToString( culture ?? CultureInfo.CurrentCulture );

        public static string FormatValue( long? value, CultureInfo culture = null ) => value?.ToString( culture ?? CultureInfo.CurrentCulture );

        public static string FormatValue( float value, CultureInfo culture = null ) => value.ToString( culture ?? CultureInfo.CurrentCulture );

        public static string FormatValue( float? value, CultureInfo culture = null ) => value?.ToString( culture ?? CultureInfo.CurrentCulture );

        public static string FormatValue( double value, CultureInfo culture = null ) => value.ToString( culture ?? CultureInfo.CurrentCulture );

        public static string FormatValue( double? value, CultureInfo culture = null ) => value?.ToString( culture ?? CultureInfo.CurrentCulture );

        public static string FormatValue( decimal value, CultureInfo culture = null ) => value.ToString( culture ?? CultureInfo.CurrentCulture );

        public static string FormatValue( decimal? value, CultureInfo culture = null ) => value?.ToString( culture ?? CultureInfo.CurrentCulture );

        public static string FormatValue( sbyte value, CultureInfo culture = null ) => value.ToString( culture ?? CultureInfo.CurrentCulture );

        public static string FormatValue( sbyte? value, CultureInfo culture = null ) => value?.ToString( culture ?? CultureInfo.CurrentCulture );

        public static string FormatValue( ushort value, CultureInfo culture = null ) => value.ToString( culture ?? CultureInfo.CurrentCulture );

        public static string FormatValue( ushort? value, CultureInfo culture = null ) => value?.ToString( culture ?? CultureInfo.CurrentCulture );

        public static string FormatValue( uint value, CultureInfo culture = null ) => value.ToString( culture ?? CultureInfo.CurrentCulture );

        public static string FormatValue( uint? value, CultureInfo culture = null ) => value?.ToString( culture ?? CultureInfo.CurrentCulture );

        public static string FormatValue( ulong value, CultureInfo culture = null ) => value.ToString( culture ?? CultureInfo.CurrentCulture );

        public static string FormatValue( ulong? value, CultureInfo culture = null ) => value?.ToString( culture ?? CultureInfo.CurrentCulture );

        public static string FormatValue( DateTime value, CultureInfo culture = null ) => value.ToString( culture ?? CultureInfo.CurrentCulture );

        public static string FormatValue( DateTime? value, CultureInfo culture = null ) => value?.ToString( culture ?? CultureInfo.CurrentCulture );

        public static string FormatValue( DateTimeOffset value, CultureInfo culture = null ) => value.ToString( culture ?? CultureInfo.CurrentCulture );

        public static string FormatValue( DateTimeOffset? value, CultureInfo culture = null ) => value?.ToString( culture ?? CultureInfo.CurrentCulture );

        /// <summary>
        /// Gets the min and max possible value based on the supplied value type
        /// </summary>
        /// <typeparam name="TValue">Value data type.</typeparam>
        /// <returns>Returns the min and max value of supplied value type.</returns>
        /// <exception cref="System.InvalidOperationException">Throws when value type is unknown.</exception>
        public static (object, object) GetMinMaxValueOfType<TValue>()
        {
            var type = typeof( TValue );

            switch ( type )
            {
                case Type byteType when byteType == typeof( byte ) || byteType == typeof( byte? ):
                    return (byte.MinValue, byte.MaxValue);
                case Type shortType when shortType == typeof( short ) || shortType == typeof( short? ):
                    return (short.MinValue, short.MaxValue);
                case Type intType when intType == typeof( int ) || intType == typeof( int? ):
                    return (int.MinValue, int.MaxValue);
                case Type longType when longType == typeof( long ) || longType == typeof( long? ):
                    return (long.MinValue, long.MaxValue);
                case Type floatType when floatType == typeof( float ) || floatType == typeof( float? ):
                    return (float.MinValue, float.MaxValue);
                case Type doubleType when doubleType == typeof( double ) || doubleType == typeof( double? ):
                    return (double.MinValue, double.MaxValue);
                case Type decimalType when decimalType == typeof( decimal ) || decimalType == typeof( decimal? ):
                    return (decimal.MinValue, decimal.MaxValue);
                case Type sbyteType when sbyteType == typeof( sbyte ) || sbyteType == typeof( sbyte? ):
                    return (sbyte.MinValue, sbyte.MaxValue);
                case Type ushortType when ushortType == typeof( ushort ) || ushortType == typeof( ushort? ):
                    return (ushort.MinValue, ushort.MaxValue);
                case Type uintType when uintType == typeof( uint ) || uintType == typeof( uint? ):
                    return (uint.MinValue, uint.MaxValue);
                case Type ulongType when ulongType == typeof( ulong ) || ulongType == typeof( ulong? ):
                    return (ulong.MinValue, ulong.MaxValue);
                default:
                    throw new InvalidOperationException( $"Unsupported type {type}" );
            }
        }

        private static bool IsSimpleType( Type type )
        {
            return
                type.IsPrimitive ||
                type.IsEnum ||
                SimpleTypes.Contains( type ) ||
                Convert.GetTypeCode( type ) != TypeCode.Object ||
                ( type.IsGenericType && type.GetGenericTypeDefinition() == typeof( Nullable<> ) && IsSimpleType( type.GetGenericArguments()[0] ) );
        }

        private static bool IsEqualToDefaultValue<T>( T argument )
        {
            // deal with non-null nullables
            Type methodType = typeof( T );
            if ( Nullable.GetUnderlyingType( methodType ) != null )
            {
                return false;
            }

            // deal with boxed value types
            Type argumentType = argument.GetType();
            if ( argumentType.IsValueType && argumentType != methodType )
            {
                object obj = Activator.CreateInstance( argument.GetType() );
                return obj.Equals( argument );
            }

            return false;
        }

        #endregion
    }
}
