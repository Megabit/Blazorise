#region Using directives
using System.Text;
#endregion

namespace Blazorise
{
    public interface IThemeGenerator
    {
        void GenerateVariables( StringBuilder sb, Theme theme );

        void GenerateStyles( StringBuilder sb, Theme theme );
    }
}
