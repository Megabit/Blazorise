namespace Blazorise.Material.Components;

public partial class ColorPicker : Blazorise.ColorPicker
{
    #region Properties

    protected override string ColorPreviewElementSelector => ":scope > .mui-color-preview > .mui-color-preview-color";

    protected override string ColorValueElementSelector => ":scope > .mui-color-value";

    #endregion
}