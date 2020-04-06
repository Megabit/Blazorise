using System.Text;
using Microsoft.AspNetCore.Components;

namespace Blazorise.AntDesign
{
    public partial class Column : Blazorise.Column
    {
        #region Members

        #endregion

        #region Methods

        private string BuildGutterStyles()
        {
            var sb = new StringBuilder();
            sb.Append( Gutter.Horizontal > 0 ? $"padding-left: {Gutter.Horizontal / 2}px; padding-right: {Gutter.Horizontal / 2}px;" : string.Empty );
            sb.Append( Gutter.Vertical > 0 ? $"padding-top: {Gutter.Vertical / 2}px; padding-bottom: {Gutter.Vertical / 2}px" : string.Empty );
            return sb.ToString();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Coulmn grid spacing, we recommend setting it to (16 + 8n). (n stands for natural number.)
        /// </summary>
        [CascadingParameter] public ( int Horizontal, int Vertical ) Gutter { get; set; }

        string GutterStyles => BuildGutterStyles();

        #endregion
    }
}
