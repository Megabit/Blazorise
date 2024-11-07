using System.Collections.Generic;

namespace Blazorise;

public record ColorPickerJsOptions(
    string Default,
    string[] Palette,
    bool ShowPalette,
    bool HideAfterPaletteSelect,
    bool ShowClearButton,
    bool ShowCancelButton,
    bool ShowOpacitySlider,
    bool ShowHueSlider,
    bool ShowInputField,
    bool Disabled,
    bool ReadOnly,
    IReadOnlyDictionary<string, string> Localization,
    string ColorPreviewElementSelector,
    string ColorValueElementSelector
    );


public record ColorPickerUpdateJsOptions(
    JsOptionChange<string[]> PaletteChange,
    JsOptionChange<bool> ShowPalette,
    JsOptionChange<bool> HideAfterPaletteSelect,
    JsOptionChange<bool> Disabled,
    JsOptionChange<bool> ReadOnly
    );


