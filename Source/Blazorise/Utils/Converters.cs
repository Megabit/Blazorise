#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Utils
{
    public static class Converters
    {
        // https://stackoverflow.com/a/1107090/833106
        public static TValue ChangeType<TValue>( object o )
        {
            Type conversionType = Nullable.GetUnderlyingType( typeof( TValue ) ) ?? typeof( TValue );

            if ( conversionType.IsEnum && EnumTryParse( o?.ToString(), conversionType, out TValue value ) )
                return value;

            return (TValue)Convert.ChangeType( o, conversionType );
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
    }
}
