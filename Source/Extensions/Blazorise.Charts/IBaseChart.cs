#region Using directives
using System.Threading.Tasks;
#endregion

namespace Blazorise.Charts
{
    /// <summary>
    /// Interface is needed to set the value from javascript because calling generic component directly is not supported by Blazor.
    /// </summary>
    public interface IBaseChart
    {
        Task Event( string eventName, int datasetIndex, int index, string model );
    }
}
