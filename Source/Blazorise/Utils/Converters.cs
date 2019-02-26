#region Using directives
using System;
using System.Collections.Generic;
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

        public static bool TryChangeType<TValue>( object o, out TValue value )
        {
            try
            {
                Type conversionType = Nullable.GetUnderlyingType( typeof( TValue ) ) ?? typeof( TValue );

                if ( conversionType.IsEnum && EnumTryParse( o?.ToString(), conversionType, out TValue theEnum ) )
                    value = theEnum;
                else
                    value = (TValue)Convert.ChangeType( o, conversionType );

                return true;
            }
            catch
            {
                value = default;
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
    }
}
