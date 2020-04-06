using System.Text;
using Microsoft.AspNetCore.Components;

namespace Blazorise.AntDesign
{
    public partial class Row : Blazorise.Row
    {
        #region Members

        #endregion

        #region Methods

        private string BuildGutterStyles()
        {
            var sb = new StringBuilder();
            sb.Append( Gutter.Horizontal > 0 ? $"margin-left: -{Gutter.Horizontal / 2}px; margin-right: -{Gutter.Horizontal / 2}px;" : string.Empty );
            sb.Append( Gutter.Vertical > 0 ? $"margin-top: -{Gutter.Vertical / 2}px" : string.Empty );
            return sb.ToString();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Row grid spacing - we recommend setting Horizontal and/or Vertical it to (16 + 8n). (n stands for natural number.)
        /// </summary>
        [Parameter] public ( int Horizontal, int Vertical ) Gutter { get; set; }

        string GutterStyles => BuildGutterStyles();

        #endregion
    }
}
