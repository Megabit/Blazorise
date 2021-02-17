namespace Blazorise.Extensions
{
    public static class EnumExtensions
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

        public static string ToDateInputMode( this DateInputMode dateInputMode )
        {
            switch ( dateInputMode )
            {
                case DateInputMode.DateTime:
                    return "datetime-local";
                case DateInputMode.Date:
                default:
                    return "date";
            }
        }

        /// <summary>
        /// Gets the link target name.
        /// </summary>
        public static string ToTargetString( this Target target ) => target switch
        {
            Target.Blank => "_blank",
            Target.Parent => "_parent",
            Target.Top => "_top",
            Target.Self => "_self",
            _ => null,
        };
    }
}
