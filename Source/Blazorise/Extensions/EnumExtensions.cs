﻿namespace Blazorise.Extensions
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class EnumExtensions
    {
        public static string ToButtonTagName( this ButtonType buttonType )
        {
            return buttonType switch
            {
                ButtonType.Link => "a",
                _ => "button",
            };
        }

        public static string ToButtonTypeString( this ButtonType buttonType )
        {
            return buttonType switch
            {
                ButtonType.Button => "button",
                ButtonType.Submit => "submit",
                ButtonType.Reset => "reset",
                _ => null,
            };
        }

        public static string ToTextRoleString( this TextRole textRole )
        {
            return textRole switch
            {
                TextRole.Email => "email",
                TextRole.Password => "password",
                TextRole.Url => "url",
                TextRole.Search => "search",
                _ => "text",
            };
        }


        public static string ToMaskTypeString( this MaskType maskType )
        {
            return maskType switch
            {
                MaskType.Numeric => "numeric",
                MaskType.DateTime => "datetime",
                MaskType.RegEx => "regex",
                _ => null,
            };
        }

        public static string ToTextInputMode( this TextInputMode textInputMode )
        {
            return textInputMode switch
            {
                TextInputMode.Text => "text",
                TextInputMode.Tel => "tel",
                TextInputMode.Url => "url",
                TextInputMode.Email => "email",
                TextInputMode.Numeric => "numeric",
                TextInputMode.Decimal => "decimal",
                TextInputMode.Search => "search",
                _ => null,
            };
        }

        public static string ToDateInputMode( this DateInputMode dateInputMode )
        {
            return dateInputMode switch
            {
                DateInputMode.DateTime => "datetime-local",
                DateInputMode.Month => "month",
                _ => "date",
            };
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

        /// <summary>
        /// Indicates whether the specified enum size is null or a default value.
        /// </summary>
        /// <param name="size">Enum to test.</param>
        /// <returns>True if the value parameter is null or a default value; otherwise, false.</returns>
        public static bool IsNullOrNone( this Size? size )
            => size == null || size == Size.None;
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
