#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Blazorise.Extensions;
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Predefined set of validation functions.
/// </summary>
public static class ValidationRule
{
    #region Basic methods

    /// <summary>
    /// Compares two strings to see if they are equal.
    /// </summary>
    /// <param name="value">First string.</param>
    /// <param name="compare">Second string.</param>
    /// <returns>True if they are equal.</returns>
    public static bool IsEqual( string value, string compare ) => value == compare;

    /// <summary>
    /// Checks if the given string length is in the given range.
    /// </summary>
    /// <param name="value">String to check for the range.</param>
    /// <param name="min">Minimum length allowed.</param>
    /// <param name="max">Maximum length allowed.</param>
    /// <returns>True if string length is in the range.</returns>
    public static bool IsLength( string value, int min, int max ) => value is not null && value.Length >= min && value.Length <= max;

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
    public static bool IsEmail( string value ) => value is not null && Regex.IsMatch( value, @"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,6}$", RegexOptions.IgnoreCase );

    /// <summary>
    /// Check if the string contains only letters (a-zA-Z).
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsAlpha( string value ) => value is not null && Regex.IsMatch( value, @"^[a-zA-Z]+$" );

    /// <summary>
    /// Check if the string contains only letters and numbers.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsAlphanumeric( string value ) => value is not null && Regex.IsMatch( value, @"^[a-zA-Z0-9]+$" );

    /// <summary>
    /// Check if the string contains only letters, numbers and underscore.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsAlphanumericWithUnderscore( string value ) => value is not null && Regex.IsMatch( value, "^[a-zA-Z0-9_]+$" );

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

    /// <summary>
    /// Checks if the boolean based input is checked.
    /// </summary>
    /// <param name="e"></param>
    public static void IsChecked( ValidatorEventArgs e )
    {
        Converters.TryChangeType<bool>( e.Value, out var result );

        e.Status = result ? ValidationStatus.Success : ValidationStatus.Error;
    }

    /// <summary>
    /// Checks if the selection based input has a valid value selected. Valid values are
    /// anything except for <c>null</c>, <c>string.Empty</c>, or <c>0</c>.
    /// </summary>
    /// <param name="e"></param>
    public static void IsSelected( ValidatorEventArgs e )
    {
        var value = e.Value?.ToString();

        e.Status = string.IsNullOrEmpty( value ) || value == "0" ? ValidationStatus.Error : ValidationStatus.Success;
    }

    /// <summary>
    /// Checks if the file is selected.
    /// </summary>
    /// <param name="e"></param>
    public static void IsFileSelected( ValidatorEventArgs e )
    {
        var value = e.Value as IFileEntry[];

        if ( value is not null )
        {
            e.Status = value.Count() == 0 ? ValidationStatus.Error : ValidationStatus.Success;
        }
    }

    /// <summary>
    /// Checks if the date is selected.
    /// </summary>
    /// <typeparam name="TValue">Data-type used be binded by the Date property.</typeparam>
    /// <param name="e"></param>
    public static void IsDateSelected<TValue>( ValidatorEventArgs e )
    {
        var dates = e.Value as IEnumerable<TValue>;

        if ( dates is not null )
        {
            e.Status = dates.Any( x => !x.IsEqual( default ) )
                ? ValidationStatus.Success
                : ValidationStatus.Error;
        }
    }

    /// <summary>
    /// Checks if the date range is selected.
    /// </summary>
    /// <typeparam name="TValue">Data-type used be binded by the Dates property.</typeparam>
    /// <param name="e"></param>
    public static void AreDatesSelected<TValue>( ValidatorEventArgs e )
    {
        var dates = e.Value as IEnumerable<TValue>;

        e.Status = dates?.Count( x => !x.IsEqual( default ) ) >= 2
            ? ValidationStatus.Success
            : ValidationStatus.Error;
    }

    #endregion
}