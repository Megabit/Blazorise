#region Using directives
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise
{
    public partial class MaskEdit : TextEdit
    {
        #region Members

        protected const char AnyChar = '*';

        protected const char NumberChar = '9';

        protected const char LetterChar = 'a';

        /// <summary>
        /// The positions of characters to be inserted in the input while is typing.
        /// </summary>
        private Dictionary<int, char> positions = new Dictionary<int, char>();

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override async Task OnInitializedAsync()
        {
            SetPositions();

            if ( !string.IsNullOrEmpty( Currency ) )
            {
                if ( Style?.Length == 0 )
                    Style = "text-align: right";
                else
                    Style += ";text-align: right";

                if ( Text != null )
                    await CurrentValueHandler( Text );

                if ( CurrencyValue > 0 )
                    CurrentValue = CurrencyValue.ToString( "#,###0.00", CultureInfo );
            }
            else
                Text = DoMask( Text );

            await base.OnInitializedAsync();
        }

        protected async Task OnKeyPressHandler( KeyboardEventArgs e )
        {
            var caretPosition = await JSRunner.GetCaret( ElementRef );

            if ( CurrentValue != null )
                if ( CurrentValue?.Length >= EditMask?.Length )
                    return;

            var value = CurrentValue;

            if ( !string.IsNullOrEmpty( Currency ) )
            {

                if ( !char.IsDigit( e.Key[0] ) )
                    return;

                value += e.Key;
                value = ClearCurrencyMask( value );

                if ( value is not null )
                    if ( !IsValidCurrencyNumber( value ) )
                        return;

                value = DoCurrencyMask( value );
            }
            else
            {
                // test if the edition is in the middle of the input
                if ( caretPosition < value?.Length )
                {
                    value = value.Insert( caretPosition, e.Key );
                }
                else
                    value += e.Key;

                value = ClearMask( value );

                value = DoMask( value );

                while ( positions.ContainsKey( caretPosition ) )
                    caretPosition++;

                if ( EditMask[caretPosition] == LetterChar )
                    if ( !char.IsLetter( value[caretPosition] ) )
                        return;

                if ( EditMask[caretPosition] == NumberChar )
                    if ( !char.IsDigit( value[caretPosition] ) )
                        return;

                if ( EditMask[caretPosition] == AnyChar )
                    if ( !char.IsLetterOrDigit( value[caretPosition] ) )
                        return;
            }

            await CurrentValueHandler( value );

            if ( !string.IsNullOrEmpty( Currency ) )
                await JSRunner.SetCaret( ElementRef, value.Length );
            else
                await JSRunner.SetCaret( ElementRef, caretPosition + 1 );
        }

        /// <inheritdoc/>
        protected override Task OnInputHandler( ChangeEventArgs e )
        {
            e.Value = RemovePositionsFromValue( e.Value.ToString() );
            return base.OnInputHandler( e );
        }

        /// <summary>
        /// Do the mask, insert the characters in the mask.
        /// </summary>
        /// <param name="value">value in construction</param>
        /// <returns></returns>
        private string DoMask( string value )
        {
            if ( string.IsNullOrEmpty( value ) )
                return value;

            foreach ( var position in positions )
            {
                if ( value.Length >= position.Key + 1 )
                {
                    var maskValue = position.Value.ToString();
                    if ( value.Substring( position.Key, 1 ) != maskValue )
                        value = value.Insert( position.Key, maskValue );
                }
            }

            return value;
        }

        /// <summary>
        /// Clear mask 
        /// </summary>
        /// <param name="value">value in construction</param>
        /// <returns></returns>
        private string ClearMask( string value )
        {
            foreach ( var position in positions )
            {
                if ( value.Length >= position.Key + 1 )
                {
                    var maskValue = position.Value.ToString();

                    if ( value.Substring( position.Key, 1 ) == maskValue )
                        value = value.Remove( position.Key, 1 );
                }
            }

            return value;
        }

        /// <summary>
        /// When deleting remove positions (characters) that format the value
        /// close to the character that was deleted
        /// </summary>
        /// <param name="value">Value changed</param>
        /// <returns></returns>
        private object RemovePositionsFromValue( string value )
        {
            while ( positions.ContainsKey( value.Length - 1 ) )
            {
                value = value.Remove( value.Length - 1 );
            }

            return value;
        }

        /// <summary>
        /// Do a simple currency format
        /// </summary>
        /// <param name="value">value in construction</param>
        /// <returns></returns>
        private string DoCurrencyMask( string value )
        {
            var decimalDigits = CultureInfo.NumberFormat.CurrencyDecimalDigits;
            var decimalSeparator = CultureInfo.NumberFormat.CurrencyDecimalSeparator;
            var groupSeparator = CultureInfo.NumberFormat.CurrencyGroupSeparator;

            if ( !decimal.TryParse( value, out var parsedValue ) )
                return parsedValue.ToString( CultureInfo.InvariantCulture );

            ///var tempValue = new string( value.Reverse().ToArray() );
            if ( value.Length > decimalDigits )
            {
                var decimalValue = decimal.Parse( value.Substring( value.Length - decimalDigits, decimalDigits ) );
                var integerValue = long.Parse( value.Substring( 0, value.Length - decimalDigits ) );

                return $"{integerValue.ToString( "#,##0" )}{decimalSeparator}{decimalValue.ToString().PadLeft( decimalDigits, '0' )}";
            }
            else
                return $"0{decimalSeparator}{value.PadLeft( decimalDigits, '0' )}";
        }

        /// <summary>
        /// Clear the mask to be rebuild
        /// </summary>
        /// <param name="value">The value in construction</param>
        /// <returns></returns>
        private string ClearCurrencyMask( string value )
        {
            return new string( value.Where( c => char.IsDigit( c ) ).ToArray() );
        }

        /// <summary>
        /// Check if the number is valid to be a currency value
        /// </summary>
        /// <param name="value">value in the construction</param>
        /// <returns></returns>
        private bool IsValidCurrencyNumber( string value )
        {
            return decimal.TryParse( value, out var validCurrencyNumber );
        }

        /// <summary>
        /// Identifie the characters to be insert in value while typping
        /// that is not the domain value, just used to format the value
        /// </summary>
        private void SetPositions()
        {
            if ( string.IsNullOrEmpty( EditMask ) )
                return;

            for ( int i = 0; i <= EditMask.Length - 1; i++ )
            {
                if ( EditMask[i] != AnyChar && EditMask[i] != NumberChar && EditMask[i] != LetterChar )
                    positions.Add( i, EditMask[i] );
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// CultureInfo information for Currency mode MaskEdit
        /// </summary>
        private CultureInfo CultureInfo =>
          CultureInfo.GetCultures( CultureTypes.AllCultures )
              .Where( c => c.NumberFormat.CurrencySymbol == Currency )
              .FirstOrDefault();

        /// <summary>
        /// Define if the MaskEdit is in the Currency mode. 
        /// Only the numeric value will be accepted
        /// For example to Brazil currency we use "R$" 
        /// that means Real, MaskEdit will search for the 
        /// culture that use this currency symbol
        /// </summary>
        [Parameter] public string Currency { get; set; }

        /// <summary>
        /// Return a currentValue as decimal value;
        /// </summary>
        public decimal CurrencyValue
        {
            get
            {
                decimal currencyValue = 0;

                if ( !string.IsNullOrEmpty( Currency ) && CurrentValue != null )
                {
                    var value = new string( CurrentValue
                        .Where( c => c != CultureInfo
                                          .NumberFormat
                                          .CurrencyGroupSeparator[0] ).ToArray() );

                    decimal.TryParse( value, out currencyValue );
                }

                return currencyValue;
            }
        }
        #endregion
    }
}