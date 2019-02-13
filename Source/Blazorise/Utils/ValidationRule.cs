using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Blazorise
{
    /// <summary>
    /// Predefined set of validation functions.
    /// </summary>
    public static class ValidationRule
    {
        /// <summary>
        /// Check if the string is null or empty.
        /// </summary>
        /// <param name="e"></param>
        public static void IsEmpty( ValidatorEventArgs e )
        {
            e.Status = string.IsNullOrEmpty( Convert.ToString( e.Value ) ) ? ValidationStatus.Success : ValidationStatus.Error;
        }

        /// <summary>
        /// Check if the string is not null or empty.
        /// </summary>
        /// <param name="e"></param>
        public static void IsNotEmpty( ValidatorEventArgs e )
        {
            e.Status = !string.IsNullOrEmpty( Convert.ToString( e.Value ) ) ? ValidationStatus.Success : ValidationStatus.Error;
        }

        /// <summary>
        /// Check if the string is an email.
        /// </summary>
        /// <param name="e"></param>
        public static void IsEmail( ValidatorEventArgs e )
        {
            e.Status = Regex.IsMatch( Convert.ToString( e.Value ), @"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,6}$", RegexOptions.IgnoreCase ) ? ValidationStatus.Success : ValidationStatus.Error;
        }

        /// <summary>
        /// Check if the string contains only letters (a-zA-Z).
        /// </summary>
        /// <param name="e"></param>
        public static void IsAlpha( ValidatorEventArgs e )
        {
            e.Status = Regex.IsMatch( Convert.ToString( e.Value ), @"^[a-zA-Z]+$" ) ? ValidationStatus.Success : ValidationStatus.Error;
        }

        /// <summary>
        /// Check if the string contains only letters and numbers.
        /// </summary>
        /// <param name="e"></param>
        public static void IsAlphanumeric( ValidatorEventArgs e )
        {
            e.Status = Regex.IsMatch( Convert.ToString( e.Value ), @"^[a-zA-Z0-9]+$" ) ? ValidationStatus.Success : ValidationStatus.Error;
        }

        /// <summary>
        /// Check if the string contains only letters, numbers and underscore.
        /// </summary>
        /// <param name="e"></param>
        public static void IsAlphanumericWithUnderscore( ValidatorEventArgs e )
        {
            e.Status = Regex.IsMatch( Convert.ToString( e.Value ), "^[a-zA-Z0-9_]+$" ) ? ValidationStatus.Success : ValidationStatus.Error;
        }

        /// <summary>
        /// Check if the string is uppercase.
        /// </summary>
        /// <param name="e"></param>
        public static void IsUppercase( ValidatorEventArgs e )
        {
            e.Status = Convert.ToString( e.Value )?.All( c => char.IsUpper( c ) ) == true ? ValidationStatus.Success : ValidationStatus.Error;
        }

        /// <summary>
        /// Check if the string is lowercase.
        /// </summary>
        /// <param name="e"></param>
        public static void IsLowercase( ValidatorEventArgs e )
        {
            e.Status = Convert.ToString( e.Value )?.All( c => char.IsLower( c ) ) == true ? ValidationStatus.Success : ValidationStatus.Error;
        }
    }
}
