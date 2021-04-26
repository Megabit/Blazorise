#region Using directives
using System.Text;
#endregion

namespace Blazorise
{
    public interface IThemeGenerator
    {
        string GenerateVariables( Theme theme );

        string GenerateStyles( Theme theme );
    }
}
