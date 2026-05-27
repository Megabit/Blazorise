#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
#endregion

namespace Blazorise.Utilities;

internal sealed class OnScreenKeyboardDateInputComposer
{
    #region Members

    private readonly List<Segment> segments;

    private int activeSegmentIndex;

    #endregion

    #region Constructors

    public OnScreenKeyboardDateInputComposer( DateInputMode inputMode, bool requireSeconds = false )
    {
        InputMode = inputMode;
        RequireSeconds = requireSeconds;
        segments = CreateSegments();
    }

    #endregion

    #region Methods

    public void Reset( string value )
    {
        ClearSegments();

        if ( string.IsNullOrEmpty( value ) )
            return;

        string valueDigits = OnScreenKeyboardTimeInputComposer.GetDigits( value, MaxDigits );

        SetSegmentDigits( 'y', valueDigits, 0, 4, complete: valueDigits.Length >= 4 );
        SetSegmentDigits( 'M', valueDigits, 4, 2, complete: valueDigits.Length >= 6 );

        if ( InputMode != DateInputMode.Month )
        {
            SetSegmentDigits( 'd', valueDigits, 6, 2, complete: valueDigits.Length >= 8 );
        }

        if ( InputMode == DateInputMode.DateTime )
        {
            SetSegmentDigits( 'H', valueDigits, 8, 2, complete: valueDigits.Length >= 10 );
            SetSegmentDigits( 'm', valueDigits, 10, 2, complete: valueDigits.Length >= 12 );
            SetSegmentDigits( 's', valueDigits, 12, 2, complete: valueDigits.Length >= 14 );
        }

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

        if ( IsComplete && ShouldIgnoreCompletedValueOverflow( text ) )
        {
            return;
        }

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
            else if ( OnScreenKeyboardTimeInputComposer.IsSeparator( character ) )
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
            segment.SetDigits( candidate, IsSegmentComplete( segment, candidate ) );

            if ( segment.IsComplete )
            {
                AdvanceSegment();
            }
        }
        else
        {
            CompleteSegmentForOverflow( segment );

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
            'M' => value > 1,
            'd' => value > 3,
            'H' => value > 2,
            'm' or 's' => value > 5,
            _ => false,
        };
    }

    private bool IsValidSegmentValue( Segment segment, string digits )
    {
        if ( string.IsNullOrEmpty( digits ) )
            return false;

        if ( segment.Kind == 'y' )
            return digits.Length <= segment.MaxDigits;

        if ( digits.Length < segment.MaxDigits )
            return true;

        if ( !int.TryParse( digits, out int value ) )
            return false;

        return value >= segment.MinValue && value <= segment.MaxValue;
    }

    private static bool IsSegmentComplete( Segment segment, string digits )
    {
        return digits.Length == segment.MaxDigits;
    }

    private void CompleteSegmentForOverflow( Segment segment )
    {
        if ( segment.Kind == 'y' )
        {
            segment.IsComplete = segment.Digits.Length is 2 or 4;
            return;
        }

        if ( segment.Digits.Length == 1 )
        {
            segment.PadLeft();
        }

        segment.IsComplete = true;
    }

    private void CompleteActiveSegment()
    {
        Segment segment = ActiveSegment;

        if ( segment is null || string.IsNullOrEmpty( segment.Digits ) )
            return;

        CompleteSegmentForOverflow( segment );
        AdvanceSegment();
    }

    private void CompleteTrailingSegments()
    {
        if ( InputMode != DateInputMode.DateTime || !IsDateComplete )
            return;

        if ( !GetSegment( 'H' ).HasDigits )
        {
            GetSegment( 'H' ).SetDigits( "00", complete: true );
        }

        if ( IsSegmentComplete( 'H' ) && !GetSegment( 'm' ).HasDigits )
        {
            GetSegment( 'm' ).SetDigits( "00", complete: true );
        }

        if ( RequireSeconds
            && IsSegmentComplete( 'H' )
            && IsSegmentComplete( 'm' )
            && !GetSegment( 's' ).HasDigits )
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
        return InputMode switch
        {
            DateInputMode.DateTime => FormatDateTimeValue(),
            DateInputMode.Month => FormatMonthValue(),
            _ => FormatDateValue(),
        };
    }

    private string FormatMonthValue()
    {
        StringBuilder builder = new();

        AppendValueSegment( builder, GetSegment( 'y' ), null );
        AppendValueSegment( builder, GetSegment( 'M' ), "-" );

        return builder.ToString();
    }

    private string FormatDateValue()
    {
        StringBuilder builder = new();

        AppendValueSegment( builder, GetSegment( 'y' ), null );
        AppendValueSegment( builder, GetSegment( 'M' ), "-" );
        AppendValueSegment( builder, GetSegment( 'd' ), "-" );

        return builder.ToString();
    }

    private string FormatDateTimeValue()
    {
        StringBuilder builder = new();

        builder.Append( FormatDateValue() );
        AppendValueSegment( builder, GetSegment( 'H' ), "T" );
        AppendValueSegment( builder, GetSegment( 'm' ), ":" );
        AppendValueSegment( builder, GetSegment( 's' ), ":" );

        return builder.ToString();
    }

    private string FormatPreview()
    {
        if ( !HasDigits )
            return null;

        return InputMode switch
        {
            DateInputMode.DateTime => FormatDateTimePreview(),
            DateInputMode.Month => FormatPatternPreview( CultureInfo.CurrentCulture.DateTimeFormat.YearMonthPattern, GetPreviewSegments() ),
            _ => FormatPatternPreview( CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern, GetPreviewSegments() ),
        };
    }

    private string FormatDateTimePreview()
    {
        string datePreview = FormatPatternPreview( CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern, GetPreviewSegments(), true );
        string timePattern = RequireSeconds || GetSegment( 's' ).HasDigits
            ? CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern
            : CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;

        return $"{datePreview} {FormatPatternPreview( timePattern, GetPreviewSegments() )}";
    }

    private Dictionary<char, string> GetPreviewSegments()
    {
        Dictionary<char, string> values = new();

        foreach ( Segment segment in segments )
        {
            if ( segment.HasDigits )
            {
                values[segment.Kind] = GetPreviewValue( segment );
            }
        }

        return values;
    }

    private string GetPreviewValue( Segment segment )
    {
        if ( segment.Kind == 'y' && segment.IsComplete && segment.Digits.Length < 4 )
            return ExpandYear( segment.Digits );

        return segment.Digits;
    }

    private void SetSegmentDigits( char kind, string digits, int startIndex, int length, bool complete )
    {
        if ( digits.Length <= startIndex )
            return;

        GetSegment( kind ).SetDigits( digits.Substring( startIndex, Math.Min( length, digits.Length - startIndex ) ), complete );
    }

    private List<Segment> CreateSegments()
    {
        List<Segment> result = new();

        foreach ( char segmentKind in GetDatePatternSegmentOrder( InputMode == DateInputMode.Month ) )
        {
            result.Add( CreateDateSegment( segmentKind ) );
        }

        if ( InputMode == DateInputMode.DateTime )
        {
            result.Add( new( 'H', 2, 0, 23 ) );
            result.Add( new( 'm', 2, 0, 59 ) );
            result.Add( new( 's', 2, 0, 59 ) );
        }

        return result;
    }

    private static Segment CreateDateSegment( char kind )
    {
        return kind switch
        {
            'y' => new( 'y', 4, 1, 9999 ),
            'M' => new( 'M', 2, 1, 12 ),
            'd' => new( 'd', 2, 1, 31 ),
            _ => new( kind, 2, 0, 99 ),
        };
    }

    private IReadOnlyList<char> GetDatePatternSegmentOrder( bool monthOnly )
    {
        string pattern = monthOnly
            ? CultureInfo.CurrentCulture.DateTimeFormat.YearMonthPattern
            : CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;

        IReadOnlyList<char> order = GetPatternSegmentOrder( pattern, monthOnly ? "yM" : "yMd" );

        if ( order.Count != 0 )
            return order;

        return monthOnly
            ? new[] { 'y', 'M' }
            : new[] { 'y', 'M', 'd' };
    }

    private static IReadOnlyList<char> GetPatternSegmentOrder( string pattern, string allowedSegments )
    {
        List<char> result = new();
        bool inQuote = false;

        foreach ( char character in pattern )
        {
            if ( character == '\'' )
            {
                inQuote = !inQuote;
                continue;
            }

            if ( !inQuote && allowedSegments.IndexOf( character ) >= 0 && !result.Contains( character ) )
            {
                result.Add( character );
            }
        }

        return result;
    }

    private static string FormatPatternPreview( string pattern, Dictionary<char, string> segments, bool fullDatePlaceholders = false )
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

            if ( !inQuote && IsPreviewPatternSegment( character ) )
            {
                char normalizedSegment = character is 'h' or 'H' ? 'H' : character;
                int startIndex = i;

                while ( i + 1 < pattern.Length && pattern[i + 1] == character )
                {
                    i++;
                }

                string token = pattern.Substring( startIndex, i - startIndex + 1 );

                builder.Append( FormatPreviewSegment( token, normalizedSegment, segments, fullDatePlaceholders ) );
            }
            else if ( character != '\\' && character != 't' )
            {
                builder.Append( character );
            }
        }

        return builder.ToString().Trim();
    }

    private static string FormatPreviewSegment( string token, char segment, Dictionary<char, string> segments, bool fullDatePlaceholders )
    {
        if ( !segments.TryGetValue( segment, out string value ) )
            return fullDatePlaceholders ? GetFullDatePlaceholder( token, segment ) : token;

        if ( segment == 'M' )
            return value;

        return value.Length >= token.Length
            ? value
            : value + token.Substring( value.Length );
    }

    private static string GetFullDatePlaceholder( string token, char segment )
    {
        return segment switch
        {
            'd' when token.Length < 2 => "dd",
            'M' when token.Length < 2 => "MM",
            'y' when token.Length < 4 => "yyyy",
            _ => token,
        };
    }

    private static bool IsPreviewPatternSegment( char character )
    {
        return character is 'y' or 'M' or 'd' or 'H' or 'h' or 'm' or 's';
    }

    private static void AppendValueSegment( StringBuilder builder, Segment segment, string separator )
    {
        if ( segment is null || !segment.HasDigits )
            return;

        if ( separator is not null && builder.Length > 0 )
        {
            builder.Append( separator );
        }

        builder.Append( segment.Kind == 'y' && segment.IsComplete && segment.Digits.Length < 4
            ? ExpandYear( segment.Digits )
            : segment.Digits );
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

        return Math.Max( 0, segments.Count - 1 );
    }

    private void ClearSegments()
    {
        foreach ( Segment segment in segments )
        {
            segment.Clear();
        }

        activeSegmentIndex = 0;
    }

    private static bool ContainsSeparator( string value )
    {
        foreach ( char character in value )
        {
            if ( OnScreenKeyboardTimeInputComposer.IsSeparator( character ) )
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

    private bool ShouldIgnoreCompletedValueOverflow( string value )
    {
        if ( CountDigits( value ) != 1 )
            return false;

        Segment segment = GetLastSegmentWithDigits();

        return segment is not null
            && segment.Digits.Length > 0
            && GetFirstDigit( value ) == segment.Digits[segment.Digits.Length - 1];
    }

    private static char GetFirstDigit( string value )
    {
        foreach ( char character in value )
        {
            if ( char.IsDigit( character ) )
                return character;
        }

        return default;
    }

    private static int CountDigits( string value )
    {
        int count = 0;

        foreach ( char character in value )
        {
            if ( char.IsDigit( character ) )
            {
                count++;
            }
        }

        return count;
    }

    private static string ExpandYear( string digits )
    {
        if ( !int.TryParse( digits, out int year ) )
            return digits;

        return CultureInfo.CurrentCulture.Calendar.ToFourDigitYear( year ).ToString( "0000", CultureInfo.InvariantCulture );
    }

    #endregion

    #region Properties

    public DateInputMode InputMode { get; }

    public bool RequireSeconds { get; }

    public string Value => FormatValue();

    public string PreviewValue => FormatPreview();

    public int MaxDigits => InputMode switch
    {
        DateInputMode.DateTime => 14,
        DateInputMode.Month => 6,
        _ => 8,
    };

    private Segment ActiveSegment => activeSegmentIndex < segments.Count ? segments[activeSegmentIndex] : null;

    private bool HasDigits => segments.Exists( segment => segment.HasDigits );

    private bool HasTimeDigits => GetSegment( 'H' )?.HasDigits == true
        || GetSegment( 'm' )?.HasDigits == true
        || GetSegment( 's' )?.HasDigits == true;

    private bool IsComplete => InputMode switch
    {
        DateInputMode.DateTime => IsDateComplete && IsTimeComplete,
        DateInputMode.Month => IsYearComplete && IsSegmentComplete( 'M' ),
        _ => IsDateComplete,
    };

    private bool IsDateComplete => IsYearComplete && IsSegmentComplete( 'M' ) && IsSegmentComplete( 'd' );

    private bool IsTimeComplete => IsSegmentComplete( 'H' ) && IsSegmentComplete( 'm' )
        && ( ( !RequireSeconds && !GetSegment( 's' ).HasDigits ) || IsSegmentComplete( 's' ) );

    private bool IsYearComplete => GetSegment( 'y' ) is { } year
        && year.IsComplete
        && year.Digits.Length is 2 or 4;

    private bool IsSegmentComplete( char kind )
    {
        Segment segment = GetSegment( kind );

        return segment is not null && segment.IsComplete && IsValidSegmentValue( segment, segment.Digits );
    }

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