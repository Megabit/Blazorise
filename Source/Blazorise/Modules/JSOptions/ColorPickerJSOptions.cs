using System.Collections.Generic;

namespace Blazorise.Modules.JSOptions;

public class ColorPickerJSOptions
{
    public string Default { get; set; }
    public string[] Palette { get; set; }
    public bool ShowPalette { get; set; }
    public bool HideAfterPaletteSelect { get; set; }
    public bool ShowClearButton { get; set; }
    public bool ShowCancelButton { get; set; }
    public bool ShowOpacitySlider { get; set; }
    public bool ShowHueSlider { get; set; }
    public bool ShowInputField { get; set; }
    public bool Disabled { get; set; }
    public bool ReadOnly { get; set; }
    public IReadOnlyDictionary<string, string> Localization { get; set; }
    public string ColorPreviewElementSelector { get; set; }
    public string ColorValueElementSelector { get; set; }
}

public class ColorPickerUpdateJsOptions
{
    public JSOptionChange<string[]> PaletteChange { get; set; }
    public JSOptionChange<bool> ShowPalette { get; set; }
    public JSOptionChange<bool> HideAfterPaletteSelect { get; set; }
    public JSOptionChange<bool> Disabled { get; set; }
    public JSOptionChange<bool> ReadOnly { get; set; }
}


