#region Using directives
using System.Threading.Tasks;
#endregion

namespace Blazorise;

/// <summary>
/// This is needed to set the value from javascript because calling generic component directly is not supported by Blazor.
/// </summary>
public interface IFileInput
{
    /// <summary>
    /// Notify us that one or more files has changed.
    /// </summary>
    /// <param name="files">List of changed files.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task NotifyChange( FileEntry[] files );
}