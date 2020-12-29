using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Blazorise
{
    public partial class MaskEdit : TextEdit
    {
        #region Members

        private Dictionary<int, char> positions = new Dictionary<int, char>();
        int caretPosition = 0;

        #endregion

        #region Methods

        protected override Task OnInitializedAsync()
        {
            SetPositions();
            if ( IsCurrency )
            {
                if ( Style?.Length == 0 )
                    Style = "text-align: right";
                else
                    Style += ";text-align: right";

            }
            return base.OnInitializedAsync();
        }

        protected override async Task OnFirstAfterRenderAsync()
        {
            await Task.CompletedTask;
        }

        protected async Task OnKeyPressHandler( KeyboardEventArgs e )
        {
            caretPosition = await JSRunner.GetCaret( ElementRef );

            if ( CurrentValue != null )
                if ( CurrentValue?.Length >= EditMask?.Length )
                    return;

            var value = CurrentValue;

            if ( IsCurrency )
            {
                if ( !char.IsDigit( e.Key[0] ) )
                    return;
                value += e.Key;
                value = ClearCurrencyMask( value );
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

                if ( EditMask[caretPosition] == 'a' )
                    if ( !char.IsLetter( value[caretPosition] ) )
                        return; 

                if ( EditMask[caretPosition] == '9' )
                    if ( !char.IsDigit( value[caretPosition] ) )
                        return;
            }

            await CurrentValueHandler( value );
            if ( IsCurrency )
                await JSRunner.SetCaret( ElementRef, value.Length );
            else
                await JSRunner.SetCaret( ElementRef, caretPosition + 1 );
        }

        private string ClearCurrencyMask( string value )
        {
            return new string( value.Where( c => char.IsDigit( c ) ).ToArray() );
        }

        private string ClearMask( string value )
        {
            foreach ( var position in positions )
                if ( value.Length >= position.Key + 1 )
                {
                    var maskValue = position.Value.ToString();
                    if ( value.Substring( position.Key, 1 ) == maskValue )
                        value = value.Remove( position.Key, 1 );
                }
            return value;
        }

        private string DoCurrencyMask( string value )
        {
            var decimalDigits = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalDigits;
            var decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
            var groupSeparator = CultureInfo.CurrentCulture.NumberFormat.CurrencyGroupSeparator;
            var groupSize = 4;

            int theValue;
            if ( !int.TryParse( value, out theValue ) )
                return theValue.ToString( "0.00" );

            ///var tempValue = new string( value.Reverse().ToArray() );
            if ( value.Length > decimalDigits )
            {
                var decimalValue = int.Parse(value.Substring( value.Length - decimalDigits, decimalDigits ));
                var integerValue = int.Parse(value.Substring( 0, value.Length - decimalDigits ));

                return $"{integerValue.ToString( "#,##0" )}{decimalSeparator}{decimalValue.ToString().PadLeft( decimalDigits, '0' )}";
            }
            else
                return $"0{decimalSeparator}{value.PadLeft( decimalDigits, '0' )}";

            //tempValue = tempValue.Remove( 0, decimalDigits );
            //List<string> groups = new();
            //StringBuilder newValue = new();
            //while (tempValue.Length > groupSize )
            //{
            //    newValue.Append(tempValue.Substring( 0, groupSize ) + groupSeparator );
            //    tempValue = tempValue.Remove( 0, groupSize );
            //}


                //if ( tempValue.Length > 0 )
                //    newValue.Append( tempValue );
                //else
                //    newValue.Remove( newValue.Length - 1, 1 );


                //decimal newValue;
                //if ( value.Length > decimalDigits )
                //{
                //    var decimalValue = value.Substring( value.Length - decimalDigits, decimalDigits );
                //    var integerValue = value.Substring( 0, value.Length - decimalDigits );
                //    newValue = Convert.ToDecimal( $"{integerValue}{decimalSeparator}{decimalValue}" );
                //}
                //else
                //    newValue = Convert.ToDecimal( $"0{decimalSeparator}{value}" );

                //if ( newValue == 0 )
                //    return newValue.ToString( "0.00" );

                //return newValue.ToString( "#,###.#0" );

        }

        protected override Task OnInputHandler( ChangeEventArgs e )
        {
            return base.OnInputHandler( e );
        }
        private string DoMask( string value )
        {
            if ( string.IsNullOrEmpty( value ) )
                return value;

            foreach ( var position in positions )
                if ( value.Length >= position.Key + 1 )
                {
                    var maskValue = position.Value.ToString();
                    if ( value.Substring( position.Key, 1 ) != maskValue )
                        value = value.Insert( position.Key, maskValue );
                }

            return value;
        }

        private void SetPositions()
        {
            if ( string.IsNullOrEmpty( EditMask ) )
                return;

            for ( int i = 0; i <= EditMask.Length - 1; i++ )
                if ( EditMask[i] != 'X' && EditMask[i] != '9' && EditMask[i] != 'a')
                    positions.Add( i, EditMask[i] );
        }

        #endregion

        #region properties

        [Parameter] public bool IsCurrency { get; set; }

        #endregion
    }
}