#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Predefined set of validation functions.
    /// </summary>
    public static class ValidationRule
    {
        #region Basic methods

        public static bool IsEqual( string value, string compare ) => value == compare;

        public static bool IsLength( string value, int min, int max ) => value != null && value.Length >= min && value.Length <= max;

        /// <summary>
        /// Check if the string is null or empty.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEmpty( string value ) => string.IsNullOrEmpty( value );

        /// <summary>
        /// Check if the string is not null or empty.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNotEmpty( string value ) => !string.IsNullOrEmpty( value );

        /// <summary>
        /// Check if the string is an email.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEmail( string value ) => value != null && Regex.IsMatch( value, @"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,6}$", RegexOptions.IgnoreCase );

        /// <summary>
        /// Check if the string contains only letters (a-zA-Z).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsAlpha( string value ) => value != null && Regex.IsMatch( value, @"^[a-zA-Z]+$" );

        /// <summary>
        /// Check if the string contains only letters and numbers.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsAlphanumeric( string value ) => value != null && Regex.IsMatch( value, @"^[a-zA-Z0-9]+$" );

        /// <summary>
        /// Check if the string contains only letters, numbers and underscore.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsAlphanumericWithUnderscore( string value ) => value != null && Regex.IsMatch( value, "^[a-zA-Z0-9_]+$" );

        /// <summary>
        /// Check if the string is uppercase.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsUppercase( string value ) => value?.All( c => char.IsUpper( c ) ) == true;

        /// <summary>
        /// Check if the string is lowercase.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsLowercase( string value ) => value?.All( c => char.IsLower( c ) ) == true;

        #endregion

        #region Handler methods

        /// <summary>
        /// Check if the string is null or empty.
        /// </summary>
        /// <param name="e"></param>
        public static void IsEmpty( ValidatorEventArgs e ) => e.Status = IsEmpty( e.Value as string ) ? ValidationStatus.Success : ValidationStatus.Error;

        /// <summary>
        /// Check if the string is not null or empty.
        /// </summary>
        /// <param name="e"></param>
        public static void IsNotEmpty( ValidatorEventArgs e ) => e.Status = IsNotEmpty( e.Value as string ) ? ValidationStatus.Success : ValidationStatus.Error;

        /// <summary>
        /// Check if the string is an email.
        /// </summary>
        /// <param name="e"></param>
        public static void IsEmail( ValidatorEventArgs e ) => e.Status = IsEmail( e.Value as string ) ? ValidationStatus.Success : ValidationStatus.Error;

        /// <summary>
        /// Check if the string contains only letters (a-zA-Z).
        /// </summary>
        /// <param name="e"></param>
        public static void IsAlpha( ValidatorEventArgs e ) => e.Status = IsAlpha( e.Value as string ) ? ValidationStatus.Success : ValidationStatus.Error;

        /// <summary>
        /// Check if the string contains only letters and numbers.
        /// </summary>
        /// <param name="e"></param>
        public static void IsAlphanumeric( ValidatorEventArgs e ) => e.Status = IsAlphanumeric( e.Value as string ) ? ValidationStatus.Success : ValidationStatus.Error;

        /// <summary>
        /// Check if the string contains only letters, numbers and underscore.
        /// </summary>
        /// <param name="e"></param>
        public static void IsAlphanumericWithUnderscore( ValidatorEventArgs e ) => e.Status = IsAlphanumericWithUnderscore( e.Value as string ) ? ValidationStatus.Success : ValidationStatus.Error;

        /// <summary>
        /// Check if the string is uppercase.
        /// </summary>
        /// <param name="e"></param>
        public static void IsUppercase( ValidatorEventArgs e ) => e.Status = IsUppercase( e.Value as string ) ? ValidationStatus.Success : ValidationStatus.Error;

        /// <summary>
        /// Check if the string is lowercase.
        /// </summary>
        /// <param name="e"></param>
        public static void IsLowercase( ValidatorEventArgs e ) => e.Status = IsLowercase( e.Value as string ) ? ValidationStatus.Success : ValidationStatus.Error;

        /// <summary>
        /// Empty validator.
        /// </summary>
        /// <param name="e"></param>
        public static void None( ValidatorEventArgs e ) => e.Status = ValidationStatus.None;

        #endregion
    }
}
