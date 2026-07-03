#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders an image source editor inside the report designer properties panel.
/// </summary>
public partial class _ReportDesignerImageSourceProperty
{
    #region Methods

    private Task OpenUpload( MouseEventArgs eventArgs )
    {
        return UploadClicked.InvokeAsync();
    }

    #endregion

    #region Properties

    private bool UploadVisible => UploadImage && UploadClicked.HasDelegate;

    /// <summary>
    /// Property label.
    /// </summary>
    [Parameter] public string Label { get; set; }

    /// <summary>
    /// Current text value.
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <summary>
    /// Prevents editing the value.
    /// </summary>
    [Parameter] public bool ReadOnly { get; set; }

    /// <summary>
    /// Enables the image upload action.
    /// </summary>
    [Parameter] public bool UploadImage { get; set; }

    /// <summary>
    /// Raised when the text value changes.
    /// </summary>
    [Parameter] public EventCallback<string> Changed { get; set; }

    /// <summary>
    /// Raised when the upload action is clicked.
    /// </summary>
    [Parameter] public EventCallback UploadClicked { get; set; }

    #endregion
}