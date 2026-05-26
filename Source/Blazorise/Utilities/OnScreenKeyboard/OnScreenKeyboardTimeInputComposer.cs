#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
#endregion

namespace Blazorise.Utilities;

internal sealed class OnScreenKeyboardTimeInputComposer
{
    #region Members

    private readonly List<Segment> segments = new()
    {
        new( 'H', 2, 0, 23 ),
        new( 'm', 2, 0, 59 ),
        new( 's', 2, 0, 59 ),
    };

    private int activeSegmentIndex;

    #endregion

    #region Methods

    public OnScreenKeyboardTimeInputComposer( bool requireSeconds = false )
    {
        RequireSeconds = requireSeconds;
    }

    public void Reset( string value )
    {
        ClearSegments();

        string digits = GetDigits( value, MaxDigits );

        SetSegmentDigits( 'H', digits, 0, 2, complete: digits.Length >= 2 );
        SetSegmentDigits( 'm', digits, 2, 2, complete: digits.Length >= 4 );
        SetSegmentDigits( 's', digits, 4, 2, complete: digits.Length >= 6 );

        activeSegmentIndex = GetNextEditableSegmentIndex();
    }

    public OnScreenKeyboardInputComposition SetValue( string value, Func<string, bool> canParse )
    {
        ClearSegments();
        InsertTextCore( value, finalizeLastSegment: true );

        return CreateComposition( canParse );
    }

    public OnScreenKeyboardInputComposition InsertText( string text, Func<string, bool> canParse )
    {
        InsertTextCore( text, finalizeLastSegment: text?.Length > 1 && ContainsSeparator( text ) );

        return CreateComposition( canParse );
    }

    public OnScreenKeyboardInputComposition Backspace( Func<string, bool> canParse )
    {
        Segment segment = GetLastSegmentWithDigits();

        if ( segment is null )
            return CreateComposition( canParse );

        segment.RemoveLastDigit();
        activeSegmentIndex = segments.IndexOf( segment );

        return CreateComposition( canParse );
    }

    public OnScreenKeyboardInputComposition Complete( Func<string, bool> canParse )
    {
        CompleteActiveSegment();
        CompleteTrailingSegments();

        return CreateComposition( canParse );
    }

    private void InsertTextCore( string text, bool finalizeLastSegment )
    {
        if ( string.IsNullOrEmpty( text ) )
            return;

        if ( IsComplete && StartsWithDigit( text ) )
        {
            ClearSegments();
        }

        foreach ( char character in text )
        {
            if ( char.IsDigit( character ) )
            {
                InsertDigit( character );
            }
            else if ( IsSeparator( character ) )
            {
                CompleteActiveSegment();
            }
        }

        if ( finalizeLastSegment )
        {
            CompleteActiveSegment();
        }
    }

    private void InsertDigit( char digit )
    {
        Segment segment = ActiveSegment;

        if ( segment is null )
            return;

        if ( segment.Digits.Length == 0 )
        {
            segment.AppendDigit( digit );

            if ( ShouldPadAfterFirstDigit( segment, digit ) )
            {
                segment.PadLeft();
                segment.IsComplete = true;
                AdvanceSegment();
            }

            return;
        }

        string candidate = segment.Digits + digit;

        if ( candidate.Length <= segment.MaxDigits && IsValidSegmentValue( segment, candidate ) )
        {
            segment.SetDigits( candidate, complete: candidate.Length == segment.MaxDigits );

            if ( segment.IsComplete )
            {
                AdvanceSegment();
            }
        }
        else
        {
            segment.PadLeft();
            segment.IsComplete = true;

            if ( TryAdvanceSegment() )
            {
                InsertDigit( digit );
            }
        }
    }

    private bool ShouldPadAfterFirstDigit( Segment segment, char digit )
    {
        int value = digit - '0';

        return segment.Kind switch
        {
            'H' => value > 2,
            'm' or 's' => value > 5,
            _ => false,
        };
    }

    private static bool IsValidSegmentValue( Segment segment, string digits )
    {
        if ( string.IsNullOrEmpty( digits ) )
            return false;

        if ( digits.Length < segment.MaxDigits )
            return true;

        if ( !int.TryParse( digits, out int value ) )
            return false;

        return value >= segment.MinValue && value <= segment.MaxValue;
    }

    private void CompleteActiveSegment()
    {
        Segment segment = ActiveSegment;

        if ( segment is null || string.IsNullOrEmpty( segment.Digits ) )
            return;

        if ( segment.Digits.Length == 1 )
        {
            segment.PadLeft();
        }

        segment.IsComplete = true;
        AdvanceSegment();
    }

    private void CompleteTrailingSegments()
    {
        if ( RequireSeconds && GetSegment( 'H' ).IsComplete && GetSegment( 'm' ).IsComplete && !GetSegment( 's' ).HasDigits )
        {
            GetSegment( 's' ).SetDigits( "00", complete: true );
        }
    }

    private void AdvanceSegment()
    {
        TryAdvanceSegment();
    }

    private bool TryAdvanceSegment()
    {
        if ( activeSegmentIndex >= segments.Count - 1 )
            return false;

        activeSegmentIndex++;

        return true;
    }

    private OnScreenKeyboardInputComposition CreateComposition( Func<string, bool> canParse )
    {
        string value = Value;
        bool canCommit = string.IsNullOrEmpty( value )
            || ( IsComplete && canParse( value ) );

        return new( value, canCommit );
    }

    private string FormatValue()
    {
        StringBuilder builder = new();

        AppendSegment( builder, GetSegment( 'H' ), null );
        AppendSegment( builder, GetSegment( 'm' ), ":" );
        AppendSegment( builder, GetSegment( 's' ), ":" );

        return builder.ToString();
    }

    private string FormatPreview()
    {
        if ( !HasDigits )
            return null;

        string pattern = RequireSeconds || GetSegment( 's' ).HasDigits
            ? CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern
            : CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;

        return FormatPatternPreview( pattern, GetTimeSegments() );
    }

    private Dictionary<char, string> GetTimeSegments()
    {
        Dictionary<char, string> values = new();

        foreach ( Segment segment in segments )
        {
            if ( segment.HasDigits )
            {
                values[segment.Kind] = segment.Digits;
            }
        }

        return values;
    }

    private Segment GetSegment( char kind )
    {
        return segments.Find( segment => segment.Kind == kind );
    }

    private Segment GetLastSegmentWithDigits()
    {
        for ( int i = segments.Count - 1; i >= 0; i-- )
        {
            if ( segments[i].HasDigits )
                return segments[i];
        }

        return null;
    }

    private int GetNextEditableSegmentIndex()
    {
        for ( int i = 0; i < segments.Count; i++ )
        {
            if ( !segments[i].IsComplete )
                return i;
        }

        return segments.Count - 1;
    }

    private void SetSegmentDigits( char kind, string digits, int startIndex, int length, bool complete )
    {
        if ( digits.Length <= startIndex )
            return;

        GetSegment( kind ).SetDigits( digits.Substring( startIndex, Math.Min( length, digits.Length - startIndex ) ), complete );
    }

    private void ClearSegments()
    {
        foreach ( Segment segment in segments )
        {
            segment.Clear();
        }

        activeSegmentIndex = 0;
    }

    private static string FormatPatternPreview( string pattern, Dictionary<char, string> segments )
    {
        if ( string.IsNullOrEmpty( pattern ) )
            return null;

        StringBuilder builder = new();
        bool inQuote = false;

        for ( int i = 0; i < pattern.Length; i++ )
        {
            char character = pattern[i];

            if ( character == '\'' )
            {
                inQuote = !inQuote;
                continue;
            }

            if ( !inQuote && character is 'H' or 'h' or 'm' or 's' )
            {
                char normalizedSegment = character is 'h' or 'H' ? 'H' : character;
                int startIndex = i;

                while ( i + 1 < pattern.Length && pattern[i + 1] == character )
                {
                    i++;
                }

                string token = pattern.Substring( startIndex, i - startIndex + 1 );

                builder.Append( FormatPreviewSegment( token, normalizedSegment, segments ) );
            }
            else if ( character != '\\' && character != 't' )
            {
                builder.Append( character );
            }
        }

        return builder.ToString().Trim();
    }

    private static string FormatPreviewSegment( string token, char segment, Dictionary<char, string> segments )
    {
        if ( !segments.TryGetValue( segment, out string value ) )
            return token;

        return value.Length >= token.Length
            ? value
            : value + token.Substring( value.Length );
    }

    private static void AppendSegment( StringBuilder builder, Segment segment, string separator )
    {
        if ( segment is null || !segment.HasDigits )
            return;

        if ( separator is not null && builder.Length > 0 )
        {
            builder.Append( separator );
        }

        builder.Append( segment.Digits );
    }

    private static bool ContainsSeparator( string value )
    {
        foreach ( char character in value )
        {
            if ( IsSeparator( character ) )
                return true;
        }

        return false;
    }

    private static bool StartsWithDigit( string value )
    {
        foreach ( char character in value )
        {
            if ( char.IsWhiteSpace( character ) )
                continue;

            return char.IsDigit( character );
        }

        return false;
    }

    internal static bool IsSeparator( char character )
    {
        return !char.IsDigit( character ) && !char.IsWhiteSpace( character );
    }

    internal static string GetDigits( string value, int maxLength )
    {
        if ( string.IsNullOrEmpty( value ) || maxLength <= 0 )
            return string.Empty;

        StringBuilder builder = new( maxLength );

        foreach ( char character in value )
        {
            if ( char.IsDigit( character ) )
            {
                builder.Append( character );

                if ( builder.Length == maxLength )
                    break;
            }
        }

        return builder.ToString();
    }

    #endregion

    #region Properties

    public string Value => FormatValue();

    public string PreviewValue => FormatPreview();

    public int MaxDigits => 6;

    public bool RequireSeconds { get; }

    private Segment ActiveSegment => activeSegmentIndex < segments.Count ? segments[activeSegmentIndex] : null;

    private bool HasDigits => segments.Exists( segment => segment.HasDigits );

    private bool IsComplete => GetSegment( 'H' ).IsComplete && GetSegment( 'm' ).IsComplete
        && ( ( !RequireSeconds && !GetSegment( 's' ).HasDigits ) || GetSegment( 's' ).IsComplete );

    #endregion

    #region Classes

    private sealed class Segment
    {
        public Segment( char kind, int maxDigits, int minValue, int maxValue )
        {
            Kind = kind;
            MaxDigits = maxDigits;
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public void AppendDigit( char digit )
        {
            if ( Digits.Length < MaxDigits )
            {
                Digits += digit;
            }
        }

        public void SetDigits( string digits, bool complete )
        {
            Digits = digits ?? string.Empty;
            IsComplete = complete;
        }

        public void RemoveLastDigit()
        {
            if ( Digits.Length == 0 )
                return;

            Digits = Digits.Substring( 0, Digits.Length - 1 );
            IsComplete = false;
        }

        public void PadLeft()
        {
            if ( Digits.Length == 1 )
            {
                Digits = "0" + Digits;
            }
        }

        public void Clear()
        {
            Digits = string.Empty;
            IsComplete = false;
        }

        public char Kind { get; }

        public int MaxDigits { get; }

        public int MinValue { get; }

        public int MaxValue { get; }

        public string Digits { get; private set; } = string.Empty;

        public bool IsComplete { get; set; }

        public bool HasDigits => !string.IsNullOrEmpty( Digits );
    }

    #endregion
}
