#region Using directives
using System.Text;
#endregion

namespace Blazorise.Utilities;

/// <summary>
/// Converts the public picker format syntax into an equivalent .NET custom format.
/// </summary>
/// <remarks>
/// DatePicker and TimePicker historically converted their public .NET-like formats to Flatpickr tokens.
/// Some of those tokens differ subtly from direct <c>DateTime.ToString</c> behavior. This converter
/// preserves the established picker behavior without depending on Flatpickr.
/// </remarks>
internal static class PickerDateTimeFormat
{
    /// <summary>
    /// Converts a public picker format into a .NET custom format with the same output semantics as the
    /// former picker format converter.
    /// </summary>
    public static string Normalize( string format )
    {
        if ( string.IsNullOrEmpty( format ) )
            return format;

        StringBuilder builder = new();

        for ( int index = 0; index < format.Length; )
        {
            char character = format[index];

            if ( character == '\\' && index + 1 < format.Length )
            {
                builder.Append( character );
                builder.Append( format[index + 1] );
                index += 2;
                continue;
            }

            if ( character is '\'' or '"' )
            {
                char quote = character;
                builder.Append( character );
                index++;

                while ( index < format.Length )
                {
                    char literalCharacter = format[index++];
                    builder.Append( literalCharacter );

                    if ( literalCharacter == quote )
                        break;
                }

                continue;
            }

            if ( !char.IsLetter( character ) )
            {
                builder.Append( character );
                index++;
                continue;
            }

            int tokenStart = index;

            while ( index < format.Length && format[index] == character )
            {
                index++;
            }

            int tokenLength = index - tokenStart;
            builder.Append( NormalizeToken( character, tokenLength ) );
        }

        return builder.ToString();
    }

    private static string NormalizeToken( char character, int length )
    {
        return character switch
        {
            'd' => length switch
            {
                1 => "%d",
                2 => "dd",
                3 => "ddd",
                _ => "dddd",
            },
            'M' => length switch
            {
                1 => "%M",
                2 => "MM",
                3 => "MMM",
                _ => "MMMM",
            },
            'y' => length <= 2 ? "yy" : "yyyy",
            'H' => "HH",
            'h' => length == 1 ? "%h" : "hh",
            'm' => "mm",
            's' => length == 1 ? "%s" : "ss",
            't' => "tt",
            _ => new string( character, length ),
        };
    }
}