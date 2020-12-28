using System;
using System.Collections.Generic;
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
            //ChangeTextOnKeyPress = true;
            SetPositions();
            return base.OnInitializedAsync();
        }

        protected override async Task OnFirstAfterRenderAsync()
        {
            await base.OnFirstAfterRenderAsync();
        }


        protected  async void OnKeyPressHandler( KeyboardEventArgs e )
        {
            caretPosition =  await JSRunner.GetCaret( ElementRef );

            if ( CurrentValue?.Length >= EditMask?.Length )
                return;
            //else if ( caretPosition+1 < CurrentValue.Length )
            //    CurrentValue = CurrentValue.Insert( caretPosition, e.Key );
            
            CurrentValue += e.Key;
            
            CurrentValue = DoMask(CurrentValue);
        }

        private string  DoMask(string value )
        {
            if ( string.IsNullOrEmpty( ( value ) ) )
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
            for ( int i = 0; i <= EditMask.Length - 1; i++ )
                if ( EditMask[i] != 'X' )
                    positions.Add( i, EditMask[i] );
        }

        #endregion        
    }
}