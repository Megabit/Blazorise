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

public record OptionChange(bool Changed, bool Value);
public record PaletteChange(bool Changed, string[] Value);

public record ColorPickerUpdateJsOptions(
    PaletteChange PaletteChange,
    OptionChange ShowPalette,
    OptionChange HideAfterPaletteSelect,
    OptionChange Disabled,
    OptionChange ReadOnly
    );


