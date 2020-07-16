#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace ThemeApp
{
    public partial class ColorTransparencyEdit : ComponentBase
    {
        #region Members
        private string _color;
        #endregion

        #region Constructors
        #endregion

        #region Methods
        private string GetRGBValue()
        {
            return Color.Substring( 0, 7 );
        }

        private void SetRGBValue( string value )
        {
            Color = value + ( Color.Length == 9 ? Color.Substring( 7, 2 ) : string.Empty );
        }

        private int GetAlphaValue()
        {
            return Color.Length == 9 ? Convert.ToInt32( Color.Substring( 7, 2 ), 16 ) : 255;
        }

        private void SetAlphaValue( int value )
        {
            Color = Color.Substring( 0, 7 ) + value.ToString( "X2" );
        }
        #endregion

        #region Properties
        [Parameter] public string Color { get => _color; set { if ( _color == value ) return; _color = value; ColorChanged.InvokeAsync( value ); } }
        [Parameter] public EventCallback<string> ColorChanged { get; set; }
        [Parameter] public string Label { get; set; }
        #endregion
    }
}