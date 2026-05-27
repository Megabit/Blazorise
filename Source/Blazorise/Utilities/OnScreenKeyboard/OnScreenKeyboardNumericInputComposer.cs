#region Using directives
using System;
using System.Globalization;
using System.Text;
#endregion

namespace Blazorise.Utilities;

internal sealed class OnScreenKeyboardNumericInputComposer
{
    #region Members

    private string value;

    #endregion

    #region Constructors

    public OnScreenKeyboardNumericInputComposer( CultureInfo cultureInfo )
    {
        CultureInfo = cultureInfo ?? CultureInfo.InvariantCulture;
    }

    #endregion

    #region Methods

    public void Reset( string value )
    {
        this.value = value ?? string.Empty;
        Caret = Value.Length;
    }

    public void SetCaret( int caret )
    {
        if ( caret < 0 )
            return;

        Caret = Math.Min( caret, Value.Length );
    }

    public OnScreenKeyboardInputComposition SetValue( string value, Func<string, bool> canParse )
    {
        this.value = NormalizeValue( value );
        Caret = Value.Length;

        return CreateComposition( canParse );
    }

    public OnScreenKeyboardInputComposition InsertText( string text, Func<string, bool> canParse )
    {
        if ( string.IsNullOrEmpty( text ) )
            return CreateComposition( canParse );

        StringBuilder builder = new( Value );

        foreach ( char character in text )
        {
            InsertCharacter( builder, character );
        }

        value = builder.ToString();

        return CreateComposition( canParse );
    }

    public OnScreenKeyboardInputComposition Backspace( Func<string, bool> canParse )
    {
        if ( Value.Length == 0 )
            return CreateComposition( canParse );

        if ( Caret == 0 )
            return CreateComposition( canParse );

        value = Value.Remove( Caret - 1, 1 );
        Caret--;

        return CreateComposition( canParse );
    }

    private void InsertCharacter( StringBuilder builder, char character )
    {
        if ( char.IsDigit( character ) )
        {
            InsertTextAtCaret( builder, character.ToString() );
            return;
        }

        if ( IsDecimalSeparator( character ) )
        {
            if ( !HasDecimalSeparator( builder ) )
            {
                InsertTextAtCaret( builder, DecimalSeparator );
            }

            return;
        }

        if ( character is '-' or '+' && Caret == 0 && !HasSign( builder ) )
        {
            InsertTextAtCaret( builder, character.ToString() );
        }
    }

    private void InsertTextAtCaret( StringBuilder builder, string text )
    {
        builder.Insert( Caret, text );
        Caret += text.Length;
    }

    private OnScreenKeyboardInputComposition CreateComposition( Func<string, bool> canParse )
    {
        string currentValue = Value;
        bool canCommit = string.IsNullOrEmpty( currentValue )
            || ( !IsIncompleteNumber( currentValue ) && canParse( currentValue ) );

        return new( currentValue, canCommit );
    }

    private string NormalizeValue( string value )
    {
        if ( string.IsNullOrEmpty( value ) )
            return string.Empty;

        StringBuilder builder = new( value.Length );
        bool hasDecimalSeparator = false;

        foreach ( char character in value )
        {
            if ( char.IsDigit( character ) )
            {
                builder.Append( character );
            }
            else if ( IsDecimalSeparator( character ) && !hasDecimalSeparator )
            {
                builder.Append( DecimalSeparator );
                hasDecimalSeparator = true;
            }
            else if ( character is '-' or '+' && builder.Length == 0 )
            {
                builder.Append( character );
            }
        }

        return builder.ToString();
    }

    private bool IsIncompleteNumber( string value )
    {
        if ( string.IsNullOrEmpty( value ) )
            return false;

        return value is "-" or "+"
            || value == DecimalSeparator
            || value == "-" + DecimalSeparator
            || value == "+" + DecimalSeparator
            || value.EndsWith( DecimalSeparator, StringComparison.Ordinal );
    }

    private bool HasDecimalSeparator( StringBuilder builder )
    {
        return builder.ToString().Contains( DecimalSeparator, StringComparison.Ordinal );
    }

    private static bool HasSign( StringBuilder builder )
    {
        return builder.Length > 0 && builder[0] is '-' or '+';
    }

    private bool IsDecimalSeparator( char character )
    {
        return character.ToString() == DecimalSeparator || character is '.' or ',';
    }

    #endregion

    #region Properties

    public CultureInfo CultureInfo { get; }

    public string Value => value ?? string.Empty;

    public string PreviewValue => Value;

    public int Caret { get; private set; }

    private string DecimalSeparator => CultureInfo.NumberFormat.NumberDecimalSeparator;

    #endregion
}