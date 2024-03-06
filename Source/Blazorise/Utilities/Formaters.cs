#region Using directives
using System;
using System.Globalization;
using System.Text;
#endregion

namespace Blazorise.Utilities;

/// <summary>
/// Helper methods for easier formating of values into strings.
/// </summary>
public static class Formaters
{
    /// <summary>
    /// Formats the supplied value to it's valid string representation.
    /// </summary>
    /// <param name="value">Value to format.</param>
    /// <param name="dateFormat">Date format.</param>
    /// <returns>Returns value formatted as string.</returns>
    public static string FormatDateValueAsString<TValue>( TValue value, string dateFormat )
    {
        return value switch
        {
            null => null,
            DateTime datetime => datetime.ToString( dateFormat ),
            DateTimeOffset datetimeOffset => datetimeOffset.ToString( dateFormat ),
            DateOnly dateOnly => dateOnly.ToString( dateFormat ),
            _ => throw new InvalidOperationException( $"Unsupported type {value.GetType()}" ),
        };
    }


    // Returns the human-readable file size for an arbitrary, 64-bit file size 
    // The default format is "0.### XB", e.g. "4.2 KB" or "1.434 GB"

    /// <summary>
    /// Returns the human-readable file size for an arbitrary, 64-bit file size 
    /// The default format is "0.### XB", e.g. "4.2 KB" or "1.434 GB"
    /// </summary>
    /// <Remarks>Adapted from : https://www.somacon.com/p576.php</Remarks>
    /// <param name="bytes">Bytes to format.</param>
    /// <returns></returns>
    public static string GetBytesReadable( long bytes )
    {
        var absoluteValue = Math.Abs( bytes );

        string suffix;
        double readable;
        if ( absoluteValue >= 0x1000000000000000 ) // Exabyte
        {
            suffix = "EB";
            readable = ( bytes >> 50 );
        }
        else if ( absoluteValue >= 0x4000000000000 ) // Petabyte
        {
            suffix = "PB";
            readable = ( bytes >> 40 );
        }
        else if ( absoluteValue >= 0x10000000000 ) // Terabyte
        {
            suffix = "TB";
            readable = ( bytes >> 30 );
        }
        else if ( absoluteValue >= 0x40000000 ) // Gigabyte
        {
            suffix = "GB";
            readable = ( bytes >> 20 );
        }
        else if ( absoluteValue >= 0x100000 ) // Megabyte
        {
            suffix = "MB";
            readable = ( bytes >> 10 );
        }
        else if ( absoluteValue >= 0x400 ) // Kilobyte
        {
            suffix = "KB";
            readable = bytes;
        }
        else
        {
            return bytes.ToString( "0 B" ); // Byte
        }

        readable = ( readable / 1024 );
        //Should this be Culture sensitive?
        return readable.ToString( "0.### ", CultureInfo.InvariantCulture ) + suffix;
    }


    //TODO : Test GetBytesReadable vs ToFileSize performance.
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string ToFileSize( long source )
    {
        const int byteConversion = 1024;
        double bytes = Convert.ToDouble( source );

        if ( bytes >= Math.Pow( byteConversion, 3 ) ) //GB Range
        {
            return string.Concat( Math.Round( bytes / Math.Pow( byteConversion, 3 ), 2 ), " GB" );
        }
        else if ( bytes >= Math.Pow( byteConversion, 2 ) ) //MB Range
        {
            return string.Concat( Math.Round( bytes / Math.Pow( byteConversion, 2 ), 2 ), " MB" );
        }
        else if ( bytes >= byteConversion ) //KB Range
        {
            return string.Concat( Math.Round( bytes / byteConversion, 2 ), " KB" );
        }
        else //Bytes
        {
            return string.Concat( bytes, " Bytes" );
        }
    }

    /// <summary>
    /// Formats the supplied Pascal Case value to a friendly name with spaces.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string PascalCaseToFriendlyName( string input )
    {
        if ( string.IsNullOrWhiteSpace( input ) )
            return input;

        StringBuilder result = new StringBuilder();
        var firstUpperChar = true;
        foreach ( char c in input )
        {
            if ( char.IsUpper( c ) && result.Length > 0 && !firstUpperChar )
            {
                result.Append( ' ' );
            }

            if ( char.IsUpper( c ) )
            {
                firstUpperChar = false;
            }

            result.Append( c );
        }

        return result.ToString();
    }
}