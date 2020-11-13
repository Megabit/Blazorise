#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    public static class Extensions
    {
        public static string ToButtonTagName( this ButtonType buttonType )
        {
            switch ( buttonType )
            {
                case ButtonType.Link:
                    return "a";
                case ButtonType.Button:
                case ButtonType.Submit:
                case ButtonType.Reset:
                default:
                    return "button";
            }
        }

        public static string ToButtonTypeString( this ButtonType buttonType )
        {
            switch ( buttonType )
            {
                case ButtonType.Button:
                    return "button";
                case ButtonType.Submit:
                    return "submit";
                case ButtonType.Reset:
                    return "reset";
                case ButtonType.Link:
                default:
                    return null;
            }
        }

        public static string ToTextRoleString( this TextRole textRole )
        {
            switch ( textRole )
            {
                case TextRole.Email:
                    return "email";
                case TextRole.Password:
                    return "password";
                case TextRole.Url:
                    return "url";
                case TextRole.Search:
                    return "search";
                default:
                    return "text";
            }
        }


        public static string ToMaskTypeString( this MaskType maskType )
        {
            switch ( maskType )
            {
                case MaskType.Numeric:
                    return "numeric";
                case MaskType.DateTime:
                    return "datetime";
                case MaskType.RegEx:
                    return "regex";
                default:
                    return null;
            }
        }

        public static string ToTextInputMode( this TextInputMode textInputMode )
        {
            switch ( textInputMode )
            {
                case TextInputMode.Text:
                    return "text";
                case TextInputMode.Tel:
                    return "tel";
                case TextInputMode.Url:
                    return "url";
                case TextInputMode.Email:
                    return "email";
                case TextInputMode.Numeric:
                    return "numeric";
                case TextInputMode.Decimal:
                    return "decimal";
                case TextInputMode.Search:
                    return "search";
                default:
                    return null;
            }
        }

        public static T GetAndRemove<T>( this IDictionary<string, object> values, string key )
        {
            if ( values.TryGetValue( key, out var value ) )
            {
                values.Remove( key );
                return (T)value;
            }
            else
            {
                return default;
            }
        }

        public static string EmptyToNull( this string value )
        {
            return string.IsNullOrEmpty( value ) ? null : value;
        }
    }
}
